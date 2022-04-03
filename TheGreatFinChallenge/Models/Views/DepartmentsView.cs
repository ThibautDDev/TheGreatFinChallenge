using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TheGreatFinChallenge.Models.Data;
using TheGreatFinChallenge.Xtra;

namespace TheGreatFinChallenge.Models.Views
{
    public class DepartmentsView
    {
        public DateTime ChallengeStartDate { get; set; }
        public DateTime ChallengeEndDate { get; set; }

        public User CurrentUser { get; set; }
        public List<User> UsersOfDirectorate = new List<User>();
        public List<Discipline> Disciplines = new List<Discipline>();
        public List<Department> Departments = new List<Department>();
        public List<ActivityType> ActivityTypes = new List<ActivityType>();
        public Dictionary<Department, List<Activity>> ActivitiesByDepartments = new Dictionary<Department, List<Activity>>();


        public List<string> ChartHeaders = new List<string>();
        public List<string> LineChartLabels = new List<string>();
        public Dictionary<int, Dictionary<int, List<double>>> LineChartData = new Dictionary<int, Dictionary<int, List<double>>>();

        public List<string> RankingChartHeaders = new List<String>();
        public Dictionary<int, List<User>> UserRankingData = new Dictionary<int, List<User>>();
        public Dictionary<int, List<Department>> GroupRankingData = new Dictionary<int, List<Department>>();
        public Dictionary<int, List<LineChartDataSet>> UserRankingChartData = new Dictionary<int, List<LineChartDataSet>>();
        public Dictionary<int, List<LineChartDataSet>> GroupRankingChartData = new Dictionary<int, List<LineChartDataSet>>();

        public DepartmentsView(TGFCContext _ctx, IEnumerable<Claim> _claims)
        {
            CurrentUser = Queries.GetUserByClaims(_ctx, _claims);
            Disciplines = Queries.GetAllDisciplines(_ctx);
            Departments = Queries.GetAllDepartmentsOfDirectorate(_ctx, CurrentUser.Department.Directorate);
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
            foreach (var dep in Departments) ActivitiesByDepartments[dep] = dep.Activities.Where(a => a.StartTime >= ChallengeStartDate && a.EndTime <= ChallengeEndDate).ToList();


            List<DateTime> dates = GetDatesBetween(ChallengeStartDate, ChallengeEndDate);
            foreach (var date in dates) LineChartLabels.Add($"{date.ToString("dd/MM/yy")}");

            ChartHeaders.Add("Total Activities");
            ChartHeaders.Add("Total Calories");
            ChartHeaders.Add("Total Distance");
            ChartHeaders.Add("Total Duration");
            foreach (var dscpl in Disciplines) ChartHeaders.Add($"{dscpl.Name} - Activities Overview");

            foreach (var department in Departments)
            {
                Dictionary<int, List<double>> tempDepartment;
                if (!LineChartData.TryGetValue(department.DepartmentId, out tempDepartment)) tempDepartment = new Dictionary<int, List<double>>();
                for(int i=0; i<ChartHeaders.Count; i++)
                {
                    List<double> tempDataList;
                    if (!tempDepartment.TryGetValue(i, out tempDataList)) tempDataList = new List<double>();
                    foreach (var date in dates) tempDataList.Add(0);
                    tempDepartment[i] = tempDataList;
                }
                LineChartData[department.DepartmentId] = tempDepartment;
            }
            SetDataTotalActivities();
            SetDataTotalCalories();
            SetDataTotalDistance();
            SetDataTotalDuration();
            SetDataDisciplines();

            RankingChartHeaders.Add("Total Activities");
            RankingChartHeaders.Add("Total Calories");
            RankingChartHeaders.Add("Total Distance");
            RankingChartHeaders.Add("Total Duration");

            UsersOfDirectorate = Queries.GetAllUsersFromDirectorate(_ctx, CurrentUser.Department.Directorate);

            for (int i = 0; i < 4; i++) UserRankingData[i] = new List<User>();
            for (int i = 0; i < 4; i++) GroupRankingData[i] = new List<Department>();
            for (int i = 0; i < 4; i++) UserRankingChartData[i] = new List<LineChartDataSet>();
            for (int i = 0; i < 4; i++) GroupRankingChartData[i] = new List<LineChartDataSet>();
            foreach (var user in UsersOfDirectorate) if (user.TotalActivities > 0) UserRankingData[0].Add(user);
            foreach (var user in UsersOfDirectorate) if (user.TotalCalories > 0) UserRankingData[1].Add(user);
            foreach (var user in UsersOfDirectorate) if (user.TotalDistance > 0) UserRankingData[2].Add(user);
            foreach (var user in UsersOfDirectorate) if (user.TotalDuration.TotalMinutes > 0) UserRankingData[3].Add(user);

            foreach (var dep in Departments) if (dep.TotalActivities > 0) GroupRankingData[0].Add(dep);
            foreach (var dep in Departments) if (dep.TotalCalories > 0) GroupRankingData[1].Add(dep);
            foreach (var dep in Departments) if (dep.TotalDistance > 0) GroupRankingData[2].Add(dep);
            foreach (var dep in Departments) if (dep.TotalDuration.TotalMinutes > 0) GroupRankingData[3].Add(dep);

            for (int i = 0; i < 4; i++) foreach (var user in UserRankingData[i]) UserRankingChartData[i].Add(new LineChartDataSet(CurrentUser, user, i));
            for (int i = 0; i < 4; i++) foreach (var dep in GroupRankingData[i]) GroupRankingChartData[i].Add(new LineChartDataSet(CurrentUser, dep, i));
            
        }

        public void SetDataTotalActivities()
        {
            foreach(var department in Departments)
            {
                var values = LineChartData[department.DepartmentId][0];
                foreach (var activity in department.Activities.Where(a => a.StartTime >= ChallengeStartDate && a.EndTime <= ChallengeEndDate)) {
                    var day = (activity.StartTime.Day - ChallengeStartDate.Day);
                    values[day] = values[day] + 1;
                }
                LineChartData[department.DepartmentId][0] = values;
            }
        }

        public void SetDataTotalCalories()
        {
            foreach (var department in Departments)
            {
                var values = LineChartData[department.DepartmentId][1];
                foreach (var activity in department.Activities.Where(a => a.StartTime >= ChallengeStartDate && a.EndTime <= ChallengeEndDate))
                {
                    var day = (activity.StartTime.Day - ChallengeStartDate.Day);
                    values[day] = values[day] + activity.CalculatedCalories;
                }
                LineChartData[department.DepartmentId][1] = values;
            }
        }

        public void SetDataTotalDistance()
        {
            foreach (var department in Departments)
            {
                var values = LineChartData[department.DepartmentId][2];
                foreach (var activity in department.Activities.Where(a => a.StartTime >= ChallengeStartDate && a.EndTime <= ChallengeEndDate))
                {
                    var day = (activity.StartTime.Day - ChallengeStartDate.Day);
                    values[day] = Math.Round(values[day] + activity.Distance, 2);
                }
                LineChartData[department.DepartmentId][2] = values;
            }
        }

        public void SetDataTotalDuration()
        {
            foreach (var department in Departments)
            {
                var values = LineChartData[department.DepartmentId][3];
                foreach (var activity in department.Activities.Where(a => a.StartTime >= ChallengeStartDate && a.EndTime <= ChallengeEndDate))
                {
                    var day = (activity.StartTime.Day - ChallengeStartDate.Day);
                    values[day] = values[day] + activity.Duration.TotalMinutes;
                }
                LineChartData[department.DepartmentId][3] = values;
            }
        }

        public void SetDataDisciplines()
        {
            foreach (var department in Departments)
            {
                foreach (var activity in department.Activities.Where(a => a.StartTime >= ChallengeStartDate && a.EndTime <= ChallengeEndDate))
                {
                    var index = Disciplines.IndexOf(activity.ActivityType.Discipline);
                    var values = LineChartData[department.DepartmentId][4+index];

                    var day = (activity.StartTime.Day - ChallengeStartDate.Day);
                    values[day] = values[day] + 1;
                    LineChartData[department.DepartmentId][4 + index] = values;
                }
            }
        }


        public int NumberOfDaysInMonth(DateTime date) => DateTime.DaysInMonth(date.Year, date.Month);
        public List<DateTime> GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);
            return allDates;
        }

    }
}