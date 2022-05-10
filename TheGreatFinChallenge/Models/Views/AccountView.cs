using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TheGreatFinChallenge.Models.Data;
using TheGreatFinChallenge.Xtra;

namespace TheGreatFinChallenge.Models.Views
{
    public class AccountView
    {
        public DateTime ChallengeStartDate { get; set; }
        public DateTime ChallengeEndDate { get; set; }
        public User CurrentUser { get; set; }
        public List<Discipline> Disciplines = new List<Discipline>();
        public List<ActivityType> ActivityTypes = new List<ActivityType>();
        public int TotalCalories = 0;

        public Dictionary<string, List<int>> LineChartData = new Dictionary<string, List<int>>();
        public List<int> PieChartData = new List<int>();
        public List<string> LineChartLabels = new List<string>();
        public List<string> PieChartLabels = new List<string>();
        public List<string> PieChartColors = new List<string>();
        public List<string> DisciplineNames = new List<string>();

        public AccountView(TGFCContext _ctx, IEnumerable<Claim> _claims)
        {
            CurrentUser = Queries.GetUserByClaims(_ctx, _claims);
            Disciplines = Queries.GetAllDisciplines(_ctx);
            ActivityTypes = Queries.GetAllActivityTypes(_ctx);
            if (CurrentUser.Department.Directorate.ChallengeStartDate == null || CurrentUser.Department.Directorate.ChallengeEndDate == null)
            {
                ChallengeStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                ChallengeEndDate = ChallengeStartDate.AddMonths(1).AddMinutes(-1);
            }
            else
            {
                ChallengeStartDate = (DateTime)CurrentUser.Department.Directorate.ChallengeStartDate;
                ChallengeEndDate = (DateTime)CurrentUser.Department.Directorate.ChallengeEndDate;
            }

            List<DateTime> dates = GetDatesBetween(ChallengeStartDate, ChallengeEndDate);
            foreach (var date in dates) LineChartLabels.Add($"{date.ToString("dd/MM/yy")}");

            TotalCalories = Calories.CalculateTotalCalories(CurrentUser.Activities.Where(a => a.Date >= ChallengeStartDate && a.Date <= ChallengeEndDate).ToList());

            for (int i = 0; i < Disciplines.Count; i++) PieChartData.Add(0);

            List<int> allCounter = new List<int>();
            for (int j = 0; j < dates.Count; j++) allCounter.Add(0);

            foreach (var dscpln in Disciplines)
            {
                List<int> temp;
                if (!LineChartData.TryGetValue(dscpln.NameNormalized, out temp))
                {
                    temp = new List<int>();
                    for (int j = 0; j < dates.Count; j++) temp.Add(0);
                }
                LineChartData[dscpln.NameNormalized] = temp;
            }

            foreach (var ac in CurrentUser.Activities.Where(a => a.Date >= ChallengeStartDate && a.Date <= ChallengeEndDate))
            {
                List<int> temp;
                if (!LineChartData.TryGetValue(ac.ActivityType.Discipline.NameNormalized, out temp))
                {
                    temp = new List<int>();
                    for (int j = 0; j < dates.Count; j++) temp.Add(0);
                }
                int day = (ac.Date.Day - ChallengeStartDate.Day);

                int value = temp[day];
                temp[day] = ++value;

                int value2 = allCounter[day];
                allCounter[day] = ++value2;

                LineChartData[ac.ActivityType.Discipline.NameNormalized] = temp;

                int index = Disciplines.IndexOf(ac.ActivityType.Discipline);
                PieChartData[index] = PieChartData[index] + 1;
            }
            LineChartData["all"] = allCounter;

            foreach (var dscpln in Disciplines)
            {
                DisciplineNames.Add(dscpln.NameNormalized);
                PieChartLabels.Add(dscpln.Name);
                PieChartColors.Add(dscpln.Color);
            }
            DisciplineNames.Add("all");
        }

        public List<DateTime> GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);
            return allDates;
        }

    }
}
