using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Fall_Of_The_Sun.Models
{
    public class HallOfFame
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int TotalScore { get; set; }

    }
}
