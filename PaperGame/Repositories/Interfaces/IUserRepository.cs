using PaperGame.Models;
using PaperGame.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperGame.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<UserDto> GetUserByEmailAsync(string userEmail);
        Task<UserDto> GetUserByIdAsync(string id);
        Task<UserDto> UpdateUserAsync(UserDto userWithUpdatedInfos);
        Task<UserDto> CreateUserAsync(string userEmail);
        Task<int> GetUsersStatisticsCountFilteredAsync(string searchedString = null);
        Task<IEnumerable<AllUsersStatisticsDto>> GetUsersStatisticsFilteredPaginatedAsync(int PageSize, int PageIndex, string searchedString = null, string orderByPropertyName = "TournamentsWon", SortDirection orderByDirection = SortDirection.OrderByDescending);
    }
}
