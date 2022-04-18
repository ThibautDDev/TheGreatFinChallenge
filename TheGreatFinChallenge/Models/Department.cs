using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheGreatFinChallenge.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        public string Name { get; set; }
        public string NameNormalized { get; set; }

        public int DirectorateId { get; set; }
        public Directorate Directorate { get; set; }


        public List<User> Users { get; set; }

        [NotMapped]
        public List<Activity> Activities
        {
            get
            {
                List<Activity> res = new List<Activity>();
                foreach (var user in Users) res.AddRange(user.Activities);
                return res;
            }
        }

        [NotMapped]
        public int TotalActivities
        {
            get
            {
                if (Directorate.ChallengeStartDate == null || Directorate.ChallengeEndDate == null)
                {
                    var _start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    var _end = _start.AddMonths(1).AddMinutes(-1);
                    return Activities.Where(a => a.StartTime >= _start && a.EndTime <= _end).ToList().Count;
                }
                return Activities.Where(a => a.StartTime >= Directorate.ChallengeStartDate && a.EndTime <= Directorate.ChallengeEndDate).ToList().Count;
            }
        }

        [NotMapped]
        public int TotalCalories
        {
            get
            {
                if (Directorate.ChallengeStartDate == null || Directorate.ChallengeEndDate == null)
                {
                    var _start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    var _end = _start.AddMonths(1).AddMinutes(-1);
                    return Activities.Where(a => a.StartTime >= _start && a.EndTime <= _end).Sum(a => a.CalculatedCalories);
                }
                return Activities.Where(a => a.StartTime >= Directorate.ChallengeStartDate && a.EndTime <= Directorate.ChallengeEndDate).Sum(a => a.CalculatedCalories);
            }
        }

        [NotMapped]
        public double TotalDistance
        {
            get
            {
                if (Directorate.ChallengeStartDate == null || Directorate.ChallengeEndDate == null)
                {
                    var _start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    var _end = _start.AddMonths(1).AddMinutes(-1);
                    return Math.Round(Activities.Where(a => a.StartTime >= _start && a.EndTime <= _end).Sum(a => a.Distance), 2);
                }
                return Math.Round(Activities.Where(a => a.StartTime >= Directorate.ChallengeStartDate && a.EndTime <= Directorate.ChallengeEndDate).Sum(a => a.Distance), 2);
            }
        }

        [NotMapped]
        public TimeSpan TotalDuration
        {
            get
            {
                if (Directorate.ChallengeStartDate == null || Directorate.ChallengeEndDate == null)
                {
                    var _start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    var _end = _start.AddMonths(1).AddMinutes(-1);
                    return new TimeSpan(Activities.Where(a => a.StartTime >= _start && a.EndTime <= _end).Sum(a => a.Duration.Ticks));
                }
                return new TimeSpan(Activities.Where(a => a.StartTime >= Directorate.ChallengeStartDate && a.EndTime <= Directorate.ChallengeEndDate).Sum(a => a.Duration.Ticks));
            }
        }
    }
}
