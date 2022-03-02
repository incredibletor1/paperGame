using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PaperGame.Helpers;
using PaperGame.Models.DAL;
using PaperGame.Models.DTO;
using PaperGame.Models.VM;
using PaperGame.Services.Interfaces;
using PaperGame.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperGame.SignalR
{
    public class PaperGameHub : Hub<IPaperGameHub>
    {
        /// <summary>
        /// Defines the services.
        /// </summary>
        private readonly IUserService userService;
        private readonly IGameRoomService gameRoomService;

        /// <summary>
        /// Defines the logger
        /// </summary>
        private readonly ILogger _logger;

        private static List<UserDto> usersInGame = new List<UserDto>();
        private static List<UserDto> usersInTournamentStage1Game = new List<UserDto>();
        private static List<UserDto> usersInTournamentStage2Game = new List<UserDto>();
        private static List<UserDto> usersInTournamentStage3Game = new List<UserDto>();

        private static string roomName = "roomName";
        private static string tournamentStage1RoomName = "tournamentStage1RoomName";
        private static string tournamentStage2RoomName = "tournamentStage2RoomName";
        private static string tournamentStage3RoomName = "tournamentStage3RoomName";

        public PaperGameHub(IServiceProvider serviceProvider, ILogger<PaperGameHub> logger)
        {
            userService = serviceProvider.UserService();
            gameRoomService = serviceProvider.GameRoomService();
            _logger = logger;
        }

        public async Task LeaveRoomAsync(string email)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            usersInGame.Remove(usersInGame.FirstOrDefault(uig => uig.Email == email));
            _logger.LogInformation($"{nameof(LeaveRoomAsync)}: [NonTournamentGame] user with email {email} left waiting room.");
        }

        // Method that add users to group for game session or create game if it's full
        public async Task AddUserInGameRoomOrCreateGameRoomAsync(string email) 
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("empty email");

            // Clears list if there are already more than 1 players in it 
            if (usersInGame.Count() is 2)
                usersInGame.Clear();

            // Check if user with this email really exist
            var user = await userService.GetUserByEmailAsync(email);
            if (user is null)
                throw new Exception($"No user with email {email}");
            
            // Set connection id for current user
            user.ConnectionId = Context.ConnectionId;

            await Groups.AddToGroupAsync(user.ConnectionId, roomName);
            usersInGame.Add(user);

            _logger.LogInformation($"{nameof(AddUserInGameRoomOrCreateGameRoomAsync)}: user with email {email} has been added to {usersInGame}.");
            _logger.LogInformation($"{nameof(AddUserInGameRoomOrCreateGameRoomAsync)}: there are {usersInGame.Count} users in {usersInGame}.");
            if (usersInGame.Count() is 2)
            {
                var newGameRoom = await gameRoomService.CreateGameRoomAsync(usersInGame);
                _logger.LogInformation($"{nameof(AddUserInGameRoomOrCreateGameRoomAsync)}: new game room with id {newGameRoom.Id} was created.");
                
                await Clients.Group(roomName).StartGame(newGameRoom.Id);
                _logger.LogInformation($"{nameof(AddUserInGameRoomOrCreateGameRoomAsync)}: start game in gameRoom with id {newGameRoom.Id} for userGroup with name {roomName}. Called client method : StartGame");

                foreach (var userInGame in usersInGame)
                {
                    await Groups.RemoveFromGroupAsync(userInGame.ConnectionId, roomName);
                }
                _logger.LogInformation($"{nameof(AddUserInGameRoomOrCreateGameRoomAsync)}: clear game room {nameof(roomName)}");
            }
        }

        public async Task SetPlayerGameChoiceAsync(int gameRoomId, string email, GameChoiceEnum result)
        {
            var gameRoom = await gameRoomService.GetGameRoomByIdAsync(gameRoomId);
            if (gameRoom is null)
                throw new ArgumentNullException($"No gameRoom with id {gameRoomId}");

            // Check if user with this email really exist
            var user = await userService.GetUserByEmailAsync(email);
            if (user is null)
                throw new Exception($"No user with email {email}");

            // Check if user related to current gameRoom
            if (gameRoom.Player1Email != user.Email && gameRoom.Player2Email != user.Email/* && gameRoom.Player3Email != user.Email &&
                gameRoom.Player4Email != user.Email && gameRoom.Player5Email != user.Email*/)
                throw new Exception($"user with email {email} doesn't exist in gameRoom with id {gameRoom.Id}");

            if (user.Email == gameRoom.Player1Email)
                gameRoom.Player1Result = result;
            if (user.Email == gameRoom.Player2Email)
                gameRoom.Player2Result = result;
            /*if (user.Email == gameRoom.Player3Email)
                gameRoom.Player3Result = result;
            if (user.Email == gameRoom.Player4Email)
                gameRoom.Player4Result = result;
            if (user.Email == gameRoom.Player5Email)
                gameRoom.Player5Result = result;*/

            await gameRoomService.UpdateGameRoomAsync(gameRoom);
            _logger.LogInformation($"{nameof(SetPlayerGameChoiceAsync)}: result {result} set for user with email {email} in gameRoom with id {gameRoomId}.");
        }

        public async Task GetPlayersGameChoicesAsync(int gameRoomId)
        {
            var gameRoom = await gameRoomService.GetGameRoomByIdAsync(gameRoomId);
            if (gameRoom is null)
                throw new ArgumentNullException($"No gameRoom with id {gameRoomId}");

            var gameChoicesJson = JsonConvert.SerializeObject(gameRoom.ToPlayersGameChoicesVM());
            await Clients.Caller.GetPlayersGameChoices(gameChoicesJson);
            _logger.LogInformation($"{nameof(GetPlayersGameChoicesAsync)}: success call for results for gameRoom with id {gameRoomId}");
        }

        public async Task FinishGameWithWinnerAsync(int gameRoomId, string winnerEmail)
        {
            if (string.IsNullOrWhiteSpace(winnerEmail) || gameRoomId <= 0)
                throw new Exception("empty email or negative(0) gameRoomId");

            // Check if gameRoom with this id really exist
            var gameRoom = await gameRoomService.GetGameRoomByIdAsync(gameRoomId);
            if (gameRoom is null)
                throw new Exception($"no gameRoom with id {gameRoomId}");
            if (gameRoom.Status != RoomStatusEnum.Ongoing)
                throw new Exception($"this game already ended");
            if (gameRoom.WinnerId != null)
                throw new Exception($"this game already has winner with id {gameRoom.WinnerId}");

            // Check if user with this email really exist
            var winnerUser = await userService.GetUserByEmailAsync(winnerEmail);
            if (winnerUser is null)
                throw new Exception($"No user with email {winnerEmail}");

            // Check if WinnerUser related to current gameRoom
            if (gameRoom.Player1Email != winnerUser.Email && gameRoom.Player2Email != winnerUser.Email/* && gameRoom.Player3Email != winnerUser.Email &&
                gameRoom.Player4Email != winnerUser.Email && gameRoom.Player5Email != winnerUser.Email*/)
                throw new Exception($"user with email {winnerEmail} doesn't exist in gameRoom with id {gameRoom.Id}");

            gameRoom.WinnerId = winnerUser.Id;
            gameRoom.Status = RoomStatusEnum.Finished;

            // Finish game and set a gameRoom winner  
            await gameRoomService.UpdateGameRoomAsync(gameRoom);
            _logger.LogInformation($"{nameof(FinishGameWithWinnerAsync)}: successfully end game in gameRoom with id {gameRoomId} with status {gameRoom.Status} and WinnerId: {winnerUser.Id}");

            // Update statistics and CurrentTournamentStage for users in the current gameRoom
            if (gameRoom.IsTournament is true)
            {
                // Update losers
                await GetAndUpdateTournamentLoserAsync(winnerUser.Email, gameRoom.Player1Email);
                await GetAndUpdateTournamentLoserAsync(winnerUser.Email, gameRoom.Player2Email);
                /*await GetAndUpdateTournamentLoserAsync(winnerUser.Email, gameRoom.Player3Email);
                await GetAndUpdateTournamentLoserAsync(winnerUser.Email, gameRoom.Player4Email);
                await GetAndUpdateTournamentLoserAsync(winnerUser.Email, gameRoom.Player5Email);*/

                // Update winner
                if (winnerUser.CurrentTournamentStage < 3)
                {
                    winnerUser.WinsInARow += 1;
                    winnerUser.CurrentTournamentStage += 1;
                    winnerUser.TotalMatches += 1;
                    winnerUser.TotalMatchesWon += 1;
                }
                else
                {
                    await Clients.Caller.TournamentWinner(winnerUser.Email);
                    _logger.LogInformation($"{nameof(FinishGameWithWinnerAsync)}: [TournamentGame] user with email {winnerEmail} won tournament! Called client method : {nameof(Clients.Caller.TournamentWinner)}");
                    winnerUser.WinsInARow += 1;
                    winnerUser.TotalMatches += 1;
                    winnerUser.TotalMatchesWon += 1;
                    winnerUser.TournamentsWon += 1;
                    winnerUser.CurrentTournamentStage = 1;
                }

                await userService.UpdateUserAsync(winnerUser);
                _logger.LogInformation($"{nameof(FinishGameWithWinnerAsync)}: [TournamentGame] successfully update user statistics for players in gameRoom with id {gameRoomId}");
            }
            else
            {
                // Update losers
                await GetAndUpdateLoserAsync(winnerUser.Email, gameRoom.Player1Email);
                await GetAndUpdateLoserAsync(winnerUser.Email, gameRoom.Player2Email);
                /*await GetAndUpdateLoserAsync(winnerUser.Email, gameRoom.Player3Email);
                await GetAndUpdateLoserAsync(winnerUser.Email, gameRoom.Player4Email);
                await GetAndUpdateLoserAsync(winnerUser.Email, gameRoom.Player5Email);*/

                // Update winner
                winnerUser.WinsInARow += 1;
                winnerUser.TotalMatches += 1;
                winnerUser.TotalMatchesWon += 1;
                winnerUser.RatingMatchesWon += 1;

                await userService.UpdateUserAsync(winnerUser);
                _logger.LogInformation($"{nameof(FinishGameWithWinnerAsync)}: [NonTournamentGame] successfully update user statistics for players in gameRoom with id {gameRoomId}");
            }
        }

        private async Task GetAndUpdateLoserAsync(string winnerEmail, string loserEmail)
        {
            if (winnerEmail != loserEmail)
            {
                var user = await userService.GetUserByEmailAsync(loserEmail);
                if (user != null)
                {
                    user.WinsInARow = 0;
                    user.TotalMatches += 1;
                    await userService.UpdateUserAsync(user);
                }
            }
        }

        private async Task GetAndUpdateTournamentLoserAsync(string winnerEmail, string loserEmail)
        {
            if (winnerEmail != loserEmail)
            {
                var user = await userService.GetUserByEmailAsync(loserEmail);
                if (user != null)
                {
                    user.WinsInARow = 0;
                    user.TotalMatches += 1;
                    user.CurrentTournamentStage = 1;
                    await userService.UpdateUserAsync(user);
                }
            }
        }

        // TOURNAMENT 

        public async Task AddUserInTournamentGameRoomOrCreateGameRoomAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("empty email");

            // Clears list if there are already more than 4 players in it 
            if (usersInTournamentStage1Game.Count() is 2)
                usersInTournamentStage1Game.Clear();
            if (usersInTournamentStage2Game.Count() is 2)
                usersInTournamentStage2Game.Clear();
            if (usersInTournamentStage3Game.Count() is 2)
                usersInTournamentStage3Game.Clear();

            // Check if user with this email really exist
            var user = await userService.GetUserByEmailAsync(email);
            if (user is null)
                throw new Exception($"No user with email {email}");

            // Set connection id for current user
            user.ConnectionId = Context.ConnectionId;

            if (user.CurrentTournamentStage < 1 || user.CurrentTournamentStage > 3)
                user.CurrentTournamentStage = 1;

            switch (user.CurrentTournamentStage)
            {
                case 1:
                    {
                        await AddUserToTournamentGameGroupAndStartGameIfGroupFullAsync(user, usersInTournamentStage1Game, tournamentStage1RoomName);

                        break;
                    }

                case 2:
                    {
                        await AddUserToTournamentGameGroupAndStartGameIfGroupFullAsync(user, usersInTournamentStage2Game, tournamentStage2RoomName);

                        break;
                    }

                case 3:
                    {
                        await AddUserToTournamentGameGroupAndStartGameIfGroupFullAsync(user, usersInTournamentStage3Game, tournamentStage3RoomName);

                        break;
                    }

                default:
                    throw new Exception($"{user.CurrentTournamentStage} stage of the tournament is unsupported");
            }
        }

        private async Task AddUserToTournamentGameGroupAndStartGameIfGroupFullAsync(UserDto user, List<UserDto> usersInTournamentStageGame, string tournamentStageRoomName)
        {
            await Groups.AddToGroupAsync(user.ConnectionId, tournamentStageRoomName);
            usersInTournamentStageGame.Add(user);
            _logger.LogInformation($"{nameof(AddUserInTournamentGameRoomOrCreateGameRoomAsync)}: user with email {user.Email} has been added to {usersInTournamentStageGame}.");
            _logger.LogInformation($"{nameof(AddUserInTournamentGameRoomOrCreateGameRoomAsync)}: there are {usersInTournamentStageGame.Count} users in {usersInTournamentStageGame}.");

            if (usersInTournamentStageGame.Count() is 2)
            {
                var newGameRoom = await gameRoomService.CreateGameRoomAsync(usersInTournamentStageGame, true);
                _logger.LogInformation($"{nameof(AddUserInTournamentGameRoomOrCreateGameRoomAsync)}: new tournament game room with id {newGameRoom.Id} for tournament stage {user.CurrentTournamentStage} was created .");
                
                await Clients.Group(tournamentStageRoomName).StartGame(newGameRoom.Id);
                _logger.LogInformation($"{nameof(AddUserInTournamentGameRoomOrCreateGameRoomAsync)}: start game in gameRoom with id {newGameRoom.Id} for userGroup with name {tournamentStageRoomName}. Called client method : StartGame");

                foreach (var userInGame in usersInTournamentStageGame)
                {
                    await Groups.RemoveFromGroupAsync(userInGame.ConnectionId, tournamentStageRoomName);
                }
                _logger.LogInformation($"{nameof(AddUserInTournamentGameRoomOrCreateGameRoomAsync)}: clear game room {nameof(tournamentStageRoomName)}");
            }
        }

        public async Task LeaveTournamentRoomAsync(string email)
        {
            // Check if user with this email really exist
            var user = await userService.GetUserByEmailAsync(email);
            if (user is null)
                throw new Exception($"No user with email {email}");

            if (user.CurrentTournamentStage < 1 || user.CurrentTournamentStage > 3)
                user.CurrentTournamentStage = 1;

            switch (user.CurrentTournamentStage)
            {
                case 1:
                    {
                        await RemoveUserFromGameGroupAsync(user, usersInTournamentStage1Game, tournamentStage1RoomName);

                        break;
                    }

                case 2:
                    {
                        await RemoveUserFromGameGroupAsync(user, usersInTournamentStage2Game, tournamentStage2RoomName);

                        break;
                    }

                case 3:
                    {
                        await RemoveUserFromGameGroupAsync(user, usersInTournamentStage3Game, tournamentStage3RoomName);

                        break;
                    }

                default:
                    throw new Exception($"{user.CurrentTournamentStage} stage of the tournament is unsupported");
            }
        }

        private async Task RemoveUserFromGameGroupAsync(UserDto user, List<UserDto> usersInTournamentStageGame, string tournamentStageRoomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, tournamentStageRoomName);
            usersInTournamentStageGame.Remove(usersInTournamentStageGame.FirstOrDefault(uig => uig.Email == user.Email));
            _logger.LogInformation($"{nameof(LeaveTournamentRoomAsync)}: [TournamentGame] user with email {user.Email} left waiting room for tournament stage {user.CurrentTournamentStage}.");
        }
    }
}
