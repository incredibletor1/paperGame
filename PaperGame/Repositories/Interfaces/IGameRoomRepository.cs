using PaperGame.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperGame.Repositories.Interfaces
{
    public interface IGameRoomRepository
    {
        Task<GameRoomDto> CreateGameRoomAsync(List<string> userEmails, bool isTournament = false);
        Task<GameRoomDto> GetGameRoomByIdAsync(int gameRoomId);
        Task<GameRoomDto> UpdateGameRoomAsync(GameRoomDto gameRoomWithUpdatedInfos);
    }
}
