using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaperGame.Models.DTO
{
    public class AllUsersStatisticsDto
    {
        public string Email { get; set; }
        public int TotalMatchesWon { get; set; }
        public int TournamentsWon { get; set; }
        public int RatingMatchesWon { get; set; }
        public int TotalMatches { get; set; }
    }

    public class AllUsersStatisticsPageDto
    {
        public int PageIndex { get; set; }
        public int TotalItemsCount { get; set; }
        public IEnumerable<AllUsersStatisticsDto> Page { get; set; }
    }
}
