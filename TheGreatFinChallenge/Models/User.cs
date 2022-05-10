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
    public class User
    {
        public User(string firstName, string lastName, bool admin, string email, string password, string salt, bool gdpr, int departmentId)
        {
            FirstName = firstName;
            LastName = lastName;
            Admin = admin;
            Email = email;
            Password = password;
            Salt = salt;
            Gdpr = gdpr;

            DepartmentId = departmentId;
        }

        [Key]
        public int UserId { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Admin { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public bool Gdpr { get; set; }

        public List<Activity> Activities = new List<Activity>();

        public List<Image> Images = new List<Image>();


        [NotMapped]
        public int TotalActivities { 
            get
            {
                if (Department.Directorate.ChallengeStartDate == null || Department.Directorate.ChallengeEndDate == null)
                {
                    var _start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    var _end = _start.AddMonths(1).AddMinutes(-1);
                    return Activities.Where(a => a.Date >= _start && a.Date <= _end).ToList().Count;
                }
                return Activities.Where(a => a.Date >= Department.Directorate.ChallengeStartDate && a.Date <= Department.Directorate.ChallengeEndDate).ToList().Count;
            } 
        }

        [NotMapped]
        public int TotalCalories
        {
            get
            {
                if (Department.Directorate.ChallengeStartDate == null || Department.Directorate.ChallengeEndDate == null)
                {
                    var _start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    var _end = _start.AddMonths(1).AddMinutes(-1);
                    return Activities.Where(a => a.Date >= _start && a.Date <= _end).Sum(a => a.CalculatedCalories);
                }
                return Activities.Where(a => a.Date >= Department.Directorate.ChallengeStartDate && a.Date <= Department.Directorate.ChallengeEndDate).Sum(a => a.CalculatedCalories);
            }
        }

        [NotMapped]
        public double TotalDistance
        {
            get
            {
                if (Department.Directorate.ChallengeStartDate == null || Department.Directorate.ChallengeEndDate == null)
                {
                    var _start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    var _end = _start.AddMonths(1).AddMinutes(-1);
                    return Math.Round(Activities.Where(a => a.Date >= _start && a.Date <= _end).Sum(a => a.Distance), 2);
                }
                return Math.Round(Activities.Where(a => a.Date >= Department.Directorate.ChallengeStartDate && a.Date <= Department.Directorate.ChallengeEndDate).Sum(a => a.Distance), 2);
            }
        }

        [NotMapped]
        public TimeSpan TotalDuration
        {
            get
            {
                if (Department.Directorate.ChallengeStartDate == null || Department.Directorate.ChallengeEndDate == null)
                {
                    var _start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    var _end = _start.AddMonths(1).AddMinutes(-1);
                    return new TimeSpan(Activities.Where(a => a.Date >= _start && a.Date <= _end).Sum(a => a.Duration.Ticks));
                }
                return new TimeSpan(Activities.Where(a => a.Date >= Department.Directorate.ChallengeStartDate && a.Date <= Department.Directorate.ChallengeEndDate).Sum(a => a.Duration.Ticks));
            }
        }
    }
}
