using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TheGreatFinChallenge.Models
{
    public class Discipline
    {
        [Key]
        public int DisciplineId { get; set; }

        public string Name { get; set; }
        public string NameNormalized { get; set; }
        public string Color { get; set; }
        public string ImageData { get; set; }
        public string IconData { get; set; }

        public List<ActivityType> ActivityTypes { get; set; }
    }
}
