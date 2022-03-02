using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PaperGame.Helpers;
using PaperGame.Models;
using PaperGame.Models.Context.Interface;
using PaperGame.Models.DAL;
using PaperGame.Models.DTO;
using PaperGame.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PaperGame.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IPaperGameContext _paperGameContext;

        public UserRepository(IPaperGameContext paperGameContext, IMapper mapper)
        {
            this._paperGameContext = paperGameContext;
            ConversionExtension.InitMapper(mapper);
        }

        public async Task<UserDto> GetUserByEmailAsync(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
                throw new ArgumentException("userEmail is null");

            var user = await _paperGameContext.Users.AsNoTracking()
                .FirstOrDefaultAsync(user => user.Email == userEmail);

            return user.ToDto();
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("id is null");

            var user = await _paperGameContext.Users.AsNoTracking()
                .FirstOrDefaultAsync(user => user.Id == id);

            return user.ToDto();
        }

        public async Task<UserDto> UpdateUserAsync(UserDto userWithUpdatedInfos)
        {
            var originalUser = await _paperGameContext.Users.FirstOrDefaultAsync(user => user.Id == userWithUpdatedInfos.Id);

            originalUser.Email = userWithUpdatedInfos.Email;
            originalUser.CurrentTournamentStage = userWithUpdatedInfos.CurrentTournamentStage;
            originalUser.RatingMatchesWon = userWithUpdatedInfos.RatingMatchesWon;
            originalUser.TotalMatches = userWithUpdatedInfos.TotalMatches;
            originalUser.TotalMatchesWon = userWithUpdatedInfos.TotalMatchesWon;
            originalUser.TournamentsWon = userWithUpdatedInfos.TournamentsWon;

            await _paperGameContext.SaveChangesAsync();
            return originalUser.ToDto();
        }

        public async Task<UserDto> CreateUserAsync(string userEmail)
        {
            var newUser = await _paperGameContext.Users
                .AddAsync(new User
                {
                    Email = userEmail
                });

            await _paperGameContext.SaveChangesAsync();
            return newUser.Entity.ToDto();
        }

        public async Task<int> GetUsersStatisticsCountFilteredAsync(string searchedString = null)
        {
            var usersQuery = GetUsersFilteredQuery(searchedString);
            return await usersQuery.CountAsync();
        }

        public async Task<IEnumerable<AllUsersStatisticsDto>> GetUsersStatisticsFilteredPaginatedAsync(int PageSize, int PageIndex, string searchedString = null, string orderByPropertyName = "TournamentsWon", SortDirection orderByDirection = SortDirection.OrderByDescending)
        {
            orderByPropertyName ??= "TournamentsWon";
            // Check that the provided property name to use to order results belongs to User entity (using reflexion)
            if (typeof(AllUsersStatisticsDto).GetProperty(orderByPropertyName) is null)
            {
                // Try to uppercase first letter to handle json formated properties
                orderByPropertyName = orderByPropertyName.Substring(0, 1).ToUpper() + orderByPropertyName.Substring(1);
                if (typeof(AllUsersStatisticsDto).GetProperty(orderByPropertyName) is null)
                    throw new ArgumentException($"Provided property with name: '{orderByPropertyName}' on which sorting should be performed doesn't belong to {nameof(AllUsersStatisticsDto)} entity nor table");
            }

            // Check page size and page index
            if (PageIndex <= 0) PageIndex = 1;
            if (PageSize <= 0) PageSize = 10000;

            var usersQuery = GetUsersFilteredQuery(searchedString);
            var userStatisticsPage = await usersQuery
                .Select(user => new AllUsersStatisticsDto
                {
                    Email = user.Email,
                    TotalMatches = user.TotalMatches,
                    TotalMatchesWon = user.TotalMatchesWon,
                    TournamentsWon = user.TournamentsWon,
                    RatingMatchesWon = user.RatingMatchesWon
                })
                .OrderByDynamic<AllUsersStatisticsDto>(orderByPropertyName, orderByDirection)
                .ThenByDescending(user => user.TournamentsWon)
                .Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return userStatisticsPage;
        }

        private IQueryable<User> GetUsersFilteredQuery(string searchedString = null)
        {
            Expression<Func<User, bool>> SearchForString = (user) => string.IsNullOrWhiteSpace(searchedString) ? true :
                             (user.Email.Contains(searchedString));

            var usersQuery = _paperGameContext.Users
                           .AsNoTracking()
                           .Where(SearchForString);

            return usersQuery;
        }
    }
}
