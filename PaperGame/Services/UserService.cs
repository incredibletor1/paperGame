using Google.Apis.Auth;
using PaperGame.Helpers;
using PaperGame.Models;
using PaperGame.Models.DTO;
using PaperGame.Repositories.Interfaces;
using PaperGame.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperGame.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IServiceProvider serviceProvider)
        {
            userRepository = serviceProvider.UserRepository();
        }

        public async Task<UserDto> GetUserByEmailAsync(string userEmail)
        {
            return await userRepository.GetUserByEmailAsync(userEmail);
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            return await userRepository.GetUserByIdAsync(id);
        }

        public async Task<UserDto> UpdateUserAsync(UserDto userWithUpdatedInfos)
        {
            return await userRepository.UpdateUserAsync(userWithUpdatedInfos);
        }

        public async Task<UserDto> CreateUserAsync(string userEmail)
        {
            var user = await GetUserByEmailAsync(userEmail);
            if (user != null)
                throw new Exception("User with this email already exist");

            return await userRepository.CreateUserAsync(userEmail);
        }

        public async Task LoginAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);
            if (user is null)
                throw new NullReferenceException($"user with email {email} doesn't exist");
        }

        public async Task<UserDto> LoginOrCreateUserWithGoogleAsync(string tokenId)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(tokenId, new GoogleJsonWebSignature.ValidationSettings());
            var user = await userRepository.GetUserByEmailAsync(payload.Email);
            if (user is null)
            {
                var newUser = await userRepository.CreateUserAsync(payload.Email);
                return newUser;
            }

            return user;
        }

        public async Task<AllUsersStatisticsPageDto> GetAllStatisticsFilteredPaginatedAsync(int PageSize, int PageIndex, string searchedString = null, string orderByPropertyName = "TournamentsWon", SortDirection orderByDirection = SortDirection.OrderByDescending)
        {
            var usersStatisticsCount = await userRepository.GetUsersStatisticsCountFilteredAsync(searchedString);
            var usersStatisticsPage = await userRepository.GetUsersStatisticsFilteredPaginatedAsync(PageSize, PageIndex, searchedString, orderByPropertyName, orderByDirection);

            return new AllUsersStatisticsPageDto
            {
                PageIndex = PageIndex,
                TotalItemsCount = usersStatisticsCount,
                Page = usersStatisticsPage
            };
        }

        public async Task<UserStatisticsDto> GetUserStatisticsByEmailAsync(string email)
        {
            var user = await userRepository.GetUserByEmailAsync(email);
            if (user is null)
                throw new NullReferenceException($"no user with email {email}");
            
            return new UserStatisticsDto
            {
                TotalMatches = user.TotalMatches,
                Wins = user.TotalMatchesWon,
                Loses = user.TotalMatches - user.TotalMatchesWon,
                WinPercentage = (double)(100*user.TotalMatchesWon)/(double)user.TotalMatches, 
                WinsInARow = user.WinsInARow
            };
        }
    }
}
