using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaperGame.Helpers;
using PaperGame.Models;
using PaperGame.Models.VM;
using PaperGame.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperGame.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize(Policy = "Administrators")]
    //[SwaggerResponse(statusCode: 401, type: typeof(string), description: "unauthorized")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Defines the services.
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// Defines the logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">The formsRepository<see cref="IUserService"/>.</param>
        public UserController(IServiceProvider serviceProvider, ILogger<UserController> logger)
        {
            userService = serviceProvider.UserService();
            _logger = logger;
        }

        [HttpPost]
        [Route("addUser")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async Task<IActionResult> CreateUserAsync(string email)
        {
            await userService.CreateUserAsync(email);

            _logger.LogInformation($"{nameof(CreateUserAsync)}: user with email {email} has been created.");
            return Ok();
        }

        [HttpGet]
        [Route("login")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async Task<IActionResult> LoginAsync(string email)
        {
            await userService.LoginAsync(email);

            _logger.LogInformation($"{nameof(LoginAsync)}: user with email {email} has been authenticated.");
            return Ok();
        }

        [HttpPost]
        [Route("signin-google")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async Task<IActionResult> LoginOrCreateUserWithGoogleAsync(string tokenId)
        {
            var user = await userService.LoginOrCreateUserWithGoogleAsync(tokenId);

            _logger.LogInformation($"{nameof(LoginOrCreateUserWithGoogleAsync)}: user with tokenId {tokenId} has been authenticated.");
            return Ok(user.Email);
        }

        [HttpGet]
        [Route("all-statistics")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async Task<IActionResult> GetAllStatisticsAsync(int PageSize, int PageIndex, string searchedString, string orderByPropertyName, SortDirection orderByDirection = SortDirection.OrderByDescending)
        {
            var usersStatisticsPageDto = await userService.GetAllStatisticsFilteredPaginatedAsync(PageSize, PageIndex, searchedString, orderByPropertyName, (SortDirection)Convert.ToInt32(orderByDirection));

            // Convert DTOs to VMs
            var page = new List<AllUsersStatisticsVM>();
            foreach (var usersStatisticsDto in usersStatisticsPageDto.Page)
                page.Add(usersStatisticsDto.ToVM());

            _logger.LogInformation($"Successfully called {nameof(GetAllStatisticsAsync)}: returns page {usersStatisticsPageDto.PageIndex} from {usersStatisticsPageDto.TotalItemsCount} items.");
            return Ok(new AllUsersStatisticsPageVM
            {
                PageIndex = usersStatisticsPageDto.PageIndex,
                TotalItemsCount = usersStatisticsPageDto.TotalItemsCount,
                Page = page
            });
        }

        [HttpGet]
        [Route("{email}/statistics")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async Task<IActionResult> GetUserStatisticsByEmailAsync(string email)
        {
            var userStatistics = await userService.GetUserStatisticsByEmailAsync(email);

            _logger.LogInformation($"Successfully called {nameof(GetUserStatisticsByEmailAsync)} for user with email {email}.");
            return Ok(userStatistics.ToVM());
        }
    }
}

