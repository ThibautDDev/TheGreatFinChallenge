using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TheGreatFinChallenge.Xtra;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheGreatFinChallenge.Models
{
    public class Activity
    {
        [Key]
        public int ActivityId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int ActivityTypeId { get; set; }
        public ActivityType ActivityType { get; set; }

        public double Distance { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }

        [NotMapped]
        public int CalculatedCalories { get { return Calories.CalculateCalories(this); } }



        public static DateTime WithDate(DateTime datetime, DateTime newDate) => newDate.Date + datetime.TimeOfDay;

        public Activity(int userId, int activityTypeId, double distance, DateTime date, TimeSpan duration)
        {
            UserId = userId;
            ActivityTypeId = activityTypeId;
            Distance = distance;
            Date = date;
            Duration = duration;
        }
    }
}
