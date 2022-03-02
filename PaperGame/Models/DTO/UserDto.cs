using PaperGame.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperGame.Models.DTO
{
    public class UserDto : User
    {
        public string ConnectionId { get; set; }
    }
}
