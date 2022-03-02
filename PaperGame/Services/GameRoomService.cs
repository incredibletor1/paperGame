using PaperGame.Helpers;
using PaperGame.Models.DTO;
using PaperGame.Repositories.Interfaces;
using PaperGame.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperGame.Services
{
    public class GameRoomService : IGameRoomService
    {
        private readonly IUserService userService;
        private readonly IGameRoomRepository gameRoomRepository;

        public GameRoomService(IServiceProvider serviceProvider)
        {
            userService = serviceProvider.UserService();
            gameRoomRepository = serviceProvider.GameRoomRepository();
        }

        public async Task<GameRoomDto> CreateGameRoomAsync(List<UserDto> userDtos, bool isTournament = false)
        {
            return await gameRoomRepository.CreateGameRoomAsync(userDtos.Select(u => u.Email).ToList(), isTournament);
        }

        public async Task<GameRoomDto> GetGameRoomByIdAsync(int gameRoomId)
        {
            return await gameRoomRepository.GetGameRoomByIdAsync(gameRoomId);
        }

        public async Task<GameRoomDto> UpdateGameRoomAsync(GameRoomDto gameRoomWithUpdatedInfos)
        {
            return await gameRoomRepository.UpdateGameRoomAsync(gameRoomWithUpdatedInfos);
        }
    }
}
