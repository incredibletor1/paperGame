using AutoMapper;
using PaperGame.Models.DAL;
using PaperGame.Models.DTO;
using PaperGame.Models.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperGame.Helpers
{
    public class DtoAndEntityConversionProfile : Profile
    {
        private readonly IMapper _mapper = null;

        public DtoAndEntityConversionProfile()
        {
            // Create mapping for all entities, DTOs and VMs
            CreateMap<User, UserDto>();

            CreateMap<GameRoomVM, GameRoomDto>();
            CreateMap<GameRoom, GameRoomDto>();

            CreateMap<AllUsersStatisticsDto, AllUsersStatisticsVM>();

            CreateMap<UserStatisticsDto, UserStatisticsVM>();
        }
    }

    public static class ConversionExtension
    {
        private static IMapper _mapper = null;

        public static void InitMapper(IMapper mapper)
        {
            if (_mapper is null)
                _mapper = mapper;
        }

        public static UserDto ToDto(this User userEntity)
        {
            if (userEntity is null)
                return null;
            else
                return _mapper.Map<UserDto>(userEntity);
        }

        public static GameRoomDto ToDto(this GameRoomVM gameRoomVM)
        {
            if (gameRoomVM is null)
                return null;
            else
                return _mapper.Map<GameRoomDto>(gameRoomVM);
        }

        public static GameRoomDto ToDto(this GameRoom gameRoomEntity)
        {
            if (gameRoomEntity is null)
                return null;
            else
                return _mapper.Map<GameRoomDto>(gameRoomEntity);
        }

        public static PlayersGameChoicesVM ToPlayersGameChoicesVM(this GameRoomDto gameRoomDto)
        {
            if (gameRoomDto is null)
                return null;
            else
                return new PlayersGameChoicesVM
                {
                    Player1Email = gameRoomDto.Player1Email,
                    Player2Email = gameRoomDto.Player2Email,
                    Player1Result = gameRoomDto.Player1Result,
                    Player2Result = gameRoomDto.Player2Result
                };
        }

        public static AllUsersStatisticsVM ToVM(this AllUsersStatisticsDto allUsersStatisticsDto)
        {
            if (allUsersStatisticsDto is null)
                return null;
            else
                return _mapper.Map<AllUsersStatisticsVM>(allUsersStatisticsDto);
        }

        public static UserStatisticsVM ToVM(this UserStatisticsDto userStatisticsDto)
        {
            if (userStatisticsDto is null)
                return null;
            else
                return _mapper.Map<UserStatisticsVM>(userStatisticsDto);
        }
    }
}
