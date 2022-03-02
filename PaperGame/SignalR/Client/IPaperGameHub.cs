using PaperGame.Models.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperGame.SignalR.Client
{
    public interface IPaperGameHub
    {
        Task StartGame(int gameRoomId);
        Task TournamentWinner(string winnerEmail);
        Task GetPlayersGameChoices(string playersGameChoicesVM);
    }
}
