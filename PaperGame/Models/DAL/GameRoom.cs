using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperGame.Models.DAL
{
    public class GameRoom
    {
        public int Id { get; set; }
        public RoomStatusEnum Status { get; set; }
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
        public string WinnerId { get; set; }
        public bool IsTournament { get; set; }
    }

    public enum RoomStatusEnum
    {
        Ongoing = 1,
        Finished = 2,
        FinishedWithError = 3
    }

    public enum GameChoiceEnum
    {
        Scissors = 1,
        Rock = 2,
        Paper = 3
    }
}
