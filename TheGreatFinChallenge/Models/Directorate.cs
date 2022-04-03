using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TheGreatFinChallenge.Models
{
    public class Directorate
    {
        [Key]
        public int DirectorateId { get; set; }

        public string Name { get; set; }
        public DateTime? ChallengeStartDate { get; set; }
        public DateTime? ChallengeEndDate { get; set; }

        public ICollection<Department> Departments { get; set; }

    }
}
