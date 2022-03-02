using System;
using Microsoft.Extensions.DependencyInjection;
using PaperGame.Services.Interfaces;

namespace PaperGame.Helpers
{
    // Get all Services from service provider (methods extensions)
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// GetUserService.
        /// </summary>
        /// <returns><see cref="IUserService"/>.</returns>
        public static IUserService UserService(this IServiceProvider services)
        {
            return services.GetService<IUserService>();
        }

        /// <summary>
        /// GetGameRoomService.
        /// </summary>
        /// <returns><see cref="IGameRoomService"/>.</returns>
        public static IGameRoomService GameRoomService(this IServiceProvider services)
        {
            return services.GetService<IGameRoomService>();
        }
    }
}
