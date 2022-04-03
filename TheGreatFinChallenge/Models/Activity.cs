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
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        [NotMapped]
        public int CalculatedCalories { get { return Calories.CalculateCalories(this); } }

        [NotMapped]
        public TimeSpan Duration { get { return (EndTime - StartTime); } }



        public static DateTime WithDate(DateTime datetime, DateTime newDate) => newDate.Date + datetime.TimeOfDay;

        public Activity(int userId, int activityTypeId, double distance, DateTime startTime, DateTime endTime)
        {
            UserId = userId;
            ActivityTypeId = activityTypeId;
            Distance = distance;
            StartTime = startTime;
            EndTime = WithDate(endTime, startTime);
        }
    }
}
