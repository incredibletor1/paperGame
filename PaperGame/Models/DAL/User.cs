using FluentValidation;
using Microsoft.AspNetCore.Identity;
using PaperGame.Models.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PaperGame.Models.DAL
{
    public class User 
    {
        [Key]
        public string Id { get; set; }
        public string Email { get; set; }
        public int CurrentTournamentStage { get; set; }
        public int TotalMatchesWon { get; set; }
        public int TournamentsWon { get; set; }
        public int RatingMatchesWon { get; set; }
        public int TotalMatches { get; set; }
        public int WinsInARow { get; set; }
    }
}
