using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperGame.Models.VM
{
    public class UserStatisticsVM
    {
        public int TotalMatches { get; set; }
        public int Wins { get; set; }
        public int Loses { get; set; }
        public double WinPercentage { get; set; }
        public int WinsInARow { get; set; }
    }
}
