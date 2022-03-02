using PaperGame.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperGame.Services.Interfaces
{
    public interface IGameRoomService
    {
        Task<GameRoomDto> CreateGameRoomAsync(List<UserDto> userDtos, bool isTournament = false);
        Task<GameRoomDto> GetGameRoomByIdAsync(int gameRoomId);
        Task<GameRoomDto> UpdateGameRoomAsync(GameRoomDto gameRoomWithUpdatedInfos);
    }
}
