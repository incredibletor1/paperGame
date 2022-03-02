using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaperGame.Helpers;
using PaperGame.Models.DTO;
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
    public class GameRoomController : ControllerBase
    {
        /// <summary>
        /// Defines the services.
        /// </summary>
        private readonly IGameRoomService gameRoomService;
        private readonly IUserService userService;

        /*/// <summary>
        /// Defines the logger
        /// </summary>
        private readonly ILogger _logger;*/

        /// <summary>
        /// Initializes a new instance of the <see cref="GameRoomController"/> class.
        /// </summary>
        /// <param name="gameSessionService">The formsRepository<see cref="IServiceProvider"/>.</param>
        public GameRoomController(IServiceProvider serviceProvider)
        {
            gameRoomService = serviceProvider.GameRoomService();
            userService = serviceProvider.UserService();
        }


        // For Test only! 

        /// <summary>
        /// AddUserToGameRoomAndStartGameSession
        /// </summary>
        /// <returns>AdminUsersVM</returns>
        [HttpPost]
        [Route("addUser")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "successful operation")]
        [SwaggerResponse(statusCode: 400, type: typeof(string), description: "bad request")]
        public async virtual Task<IActionResult> AddUserToGameRoomAndStartGameSessionAsync(GameRoomVM gameRoomVM)
        {
            List<UserDto> userDtos = new List<UserDto>();
            var user = await userService.GetUserByEmailAsync(gameRoomVM.Player1Email);
            userDtos.Add(user);
            user = await userService.GetUserByEmailAsync(gameRoomVM.Player2Email);
            userDtos.Add(user);
            user = await userService.GetUserByEmailAsync(gameRoomVM.Player3Email);
            userDtos.Add(user);
            user = await userService.GetUserByEmailAsync(gameRoomVM.Player4Email);
            userDtos.Add(user);
            user = await userService.GetUserByEmailAsync(gameRoomVM.Player5Email);
            userDtos.Add(user);

            await gameRoomService.CreateGameRoomAsync(userDtos);

            return Ok();
        }
    }
}
