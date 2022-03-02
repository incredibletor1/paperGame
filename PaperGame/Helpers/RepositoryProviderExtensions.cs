using System;
using Microsoft.Extensions.DependencyInjection;
using PaperGame.Repositories.Interfaces;

namespace PaperGame.Helpers
{
    // Get all Repositories from service provider (methods extensions)
    public static class RepositoryProviderExtensions
    {
        /// <summary>
        /// GetUserRepository.
        /// </summary>
        /// <returns><see cref="IUserRepository"/>.</returns>
        public static IUserRepository UserRepository(this IServiceProvider services)
        {
            return services.GetService<IUserRepository>();
        }

        /// <summary>
        /// GetGameRoomRepository.
        /// </summary>
        /// <returns><see cref="IGameRoomRepository"/>.</returns>
        public static IGameRoomRepository GameRoomRepository(this IServiceProvider services)
        {
            return services.GetService<IGameRoomRepository>();
        }
    }
}
