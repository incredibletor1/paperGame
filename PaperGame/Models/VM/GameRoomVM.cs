using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperGame.Models.VM
{
    public class GameRoomVM 
    {
        public string Player1Email { get; set; }
        public string Player2Email { get; set; }
        public string Player3Email { get; set; }
        public string Player4Email { get; set; }
        public string Player5Email { get; set; }
        public bool isTournament { get; set; }
    }
}
