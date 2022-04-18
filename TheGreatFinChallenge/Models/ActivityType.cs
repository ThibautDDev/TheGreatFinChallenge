using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TheGreatFinChallenge.Models
{
    public class ActivityType
    {
        [Key]
        public int ActivityTypeId { get; set; }

        public string Name { get; set; }
        public double MET { get; set; }
        public string ImageData { get; set; }

        public int DisciplineId { get; set; }
        public Discipline Discipline { get; set; }

        public List<Activity> Activities { get; set; }
    }
}
