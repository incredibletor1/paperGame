using PaperGame.Models;
using PaperGame.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperGame.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUserByEmailAsync(string userEmail);
        Task<UserDto> GetUserByIdAsync(string id);
        Task<UserDto> UpdateUserAsync(UserDto userWithUpdatedInfos);
        Task<UserDto> CreateUserAsync(string userEmail);
        Task LoginAsync(string email);
        Task<UserDto> LoginOrCreateUserWithGoogleAsync(string tokenId);
        Task<AllUsersStatisticsPageDto> GetAllStatisticsFilteredPaginatedAsync(int PageSize, int PageIndex, string searchedString = null, string orderByPropertyName = "TournamentsWon", SortDirection orderByDirection = SortDirection.OrderByDescending);
        Task<UserStatisticsDto> GetUserStatisticsByEmailAsync(string email);
    }
}
