using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PaperGame.Helpers;
using PaperGame.Models.Context.Interface;
using PaperGame.Models.DAL;
using PaperGame.Models.DTO;
using PaperGame.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperGame.Repositories
{
    public class GameRoomRepository : IGameRoomRepository
    {
        private readonly IPaperGameContext _paperGameContext;

        public GameRoomRepository(IPaperGameContext paperGameContext, IMapper mapper)
        {
            this._paperGameContext = paperGameContext;
            ConversionExtension.InitMapper(mapper);
        }

        public async Task<GameRoomDto> CreateGameRoomAsync(List<string> userEmails, bool isTournament = false)
        {
            var newGameRoom = new GameRoom
            {
                Player1Email = userEmails[0],
                Player2Email = userEmails[1],
                /*Player3Email = userEmails[2],
                Player4Email = userEmails[3],
                Player5Email = userEmails[4],*/
                Status = RoomStatusEnum.Ongoing,
                WinnerId = null,
                IsTournament = isTournament
            };

            var gameRoom = await _paperGameContext.GameRooms.AddAsync(newGameRoom);
            await _paperGameContext.SaveChangesAsync();

            return gameRoom.Entity.ToDto();
        }

        public async Task<GameRoomDto> GetGameRoomByIdAsync(int gameRoomId)
        {
            var gameRoom = await _paperGameContext.GameRooms.FirstOrDefaultAsync(gr => gr.Id == gameRoomId);

            return gameRoom.ToDto();
        }

        public async Task<GameRoomDto> UpdateGameRoomAsync(GameRoomDto gameRoomWithUpdatedInfos)
        {
            var originalGameRoom = await _paperGameContext.GameRooms.FirstOrDefaultAsync(gr => gr.Id == gameRoomWithUpdatedInfos.Id);

            originalGameRoom.Player1Email = gameRoomWithUpdatedInfos.Player1Email;
            originalGameRoom.Player2Email = gameRoomWithUpdatedInfos.Player2Email;
            /*originalGameRoom.Player3Email = gameRoomWithUpdatedInfos.Player3Email;
            originalGameRoom.Player4Email = gameRoomWithUpdatedInfos.Player4Email;
            originalGameRoom.Player5Email = gameRoomWithUpdatedInfos.Player5Email;*/
            originalGameRoom.WinnerId = gameRoomWithUpdatedInfos.WinnerId;
            originalGameRoom.Status = gameRoomWithUpdatedInfos.Status;
            originalGameRoom.Player1Result = gameRoomWithUpdatedInfos.Player1Result;
            originalGameRoom.Player2Result = gameRoomWithUpdatedInfos.Player2Result;
            /*originalGameRoom.Player3Result = gameRoomWithUpdatedInfos.Player3Result;
            originalGameRoom.Player4Result = gameRoomWithUpdatedInfos.Player4Result;
            originalGameRoom.Player5Result = gameRoomWithUpdatedInfos.Player5Result;*/

            await _paperGameContext.SaveChangesAsync();
            return originalGameRoom.ToDto();
        }          
    }
}
