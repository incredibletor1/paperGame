using PaperGame.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperGame.Models.VM
{
    public class PlayersGameChoicesVM
    {
        public string Player1Email { get; set; }
        public string Player2Email { get; set; }
        /*public string Player3Email { get; set; }
        public string Player4Email { get; set; }
        public string Player5Email { get; set; }*/
        public GameChoiceEnum? Player1Result { get; set; }
        public GameChoiceEnum? Player2Result { get; set; }
        /*public GameChoiceEnum? Player3Result { get; set; }
        public GameChoiceEnum? Player4Result { get; set; }
        public GameChoiceEnum? Player5Result { get; set; }*/
    }
}
