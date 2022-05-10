using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheGreatFinChallenge.Models;

namespace TheGreatFinChallenge.Xtra
{

    public class LineChartDataSet
    {
        public string label { get; set; }
        public double lineTension { get; set; }
        public int borderWidth { get; set; }
        public string backgroundColor { get; set; }
        public string borderColor { get; set; }
        public int pointRadius { get; set; }
        public string pointBackgroundColor { get; set; }
        public string pointBorderColor { get; set; }
        public int pointHoverRadius { get; set; }
        public string pointHoverBackgroundColor { get; set; }
        public string pointHoverBorderColor { get; set; }
        public int pointHitRadius { get; set; }
        public int pointBorderWidth { get; set; }
        public List<double> data { get; set; }

        public LineChartDataSet(User _user, User user, int calcType)
        {
            label = $"{user.FirstName} {user.LastName[0]}.";
            lineTension = 0.1;
            borderWidth = (_user == user) ? 4 : 2;
            backgroundColor = "rgba(78, 115, 223, 0.05)";
            borderColor = (_user == user) ? "#FF0000" : "rgba(78, 115, 223, 1)";
            pointRadius = (_user == user) ? 4 : 3;
            pointBackgroundColor = (_user == user) ? "#FF0000" : "rgba(78, 115, 223, 1)";
            pointBorderColor = (_user == user) ? "#FF0000" : "rgba(78, 115, 223, 1)";
            pointHoverRadius = (_user == user) ? 4 : 3;
            pointHoverBackgroundColor = (_user == user) ? "#FF0000" : "rgba(78, 115, 223, 1)";
            pointHoverBorderColor = (_user == user) ? "#FF0000" : "rgba(78, 115, 223, 1)";
            pointHitRadius = 10;
            pointBorderWidth = 2;
            if (calcType == 0) data = generateActData(user);
            else if (calcType == 1) data = generateCalData(user);
            else if (calcType == 2) data = generateDisData(user);
            else if (calcType == 3) data = generateDurData(user);
        }

        public LineChartDataSet(User _user, Department dep, int calcType)
        {
            label = $"{dep.Name}";
            lineTension = 0.1;
            borderWidth = (_user.Department == dep) ? 4 : 2;
            backgroundColor = "rgba(78, 115, 223, 0.05)";
            borderColor = (_user.Department == dep) ? "#FF0000" : "rgba(78, 115, 223, 1)";
            pointRadius = (_user.Department == dep) ? 4 : 3;
            pointBackgroundColor = (_user.Department == dep) ? "#FF0000" : "rgba(78, 115, 223, 1)";
            pointBorderColor = (_user.Department == dep) ? "#FF0000" : "rgba(78, 115, 223, 1)";
            pointHoverRadius = (_user.Department == dep) ? 4 : 3;
            pointHoverBackgroundColor = (_user.Department == dep) ? "#FF0000" : "rgba(78, 115, 223, 1)";
            pointHoverBorderColor = (_user.Department == dep) ? "#FF0000" : "rgba(78, 115, 223, 1)";
            pointHitRadius = 10;
            pointBorderWidth = 2;
            if (calcType == 0) data = generateActData(dep);
            else if (calcType == 1) data = generateCalData(dep);
            else if (calcType == 2) data = generateDisData(dep);
            else if (calcType == 3) data = generateDurData(dep);
        }


        public List<DateTime> GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddMinutes(1))
                allDates.Add(date);
            return allDates;
        }

        public List<double> generateActData(User _user)
        {
            DateTime _start;
            DateTime _end;
            if (_user.Department.Directorate.ChallengeStartDate == null || _user.Department.Directorate.ChallengeEndDate == null)
            {
                _start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                _end = _start.AddMonths(1).AddMinutes(-1);
            }
            else
            {
                _start = (DateTime)_user.Department.Directorate.ChallengeStartDate;
                _end = (DateTime)_user.Department.Directorate.ChallengeEndDate;
            }
            List<DateTime> dates = GetDatesBetween(_start, _end);

            List<double> res = new List<double>();
            foreach (var date in dates) res.Add(0);

            foreach (var activity in _user.Activities.Where(a => a.Date >= _start && a.Date <= _end))
            {
                var day = (activity.Date.Day - _start.Day);
                res[day] = res[day] + 1;
            }

            for (int i = 1; i < res.Count; i++)
            {
                var value = res[i - 1] + res[i];
                res[i] = value;
            }
            return res;
        }

        public List<double> generateActData(Department _dep)
        {
            DateTime _start;
            DateTime _end;
            if (_dep.Directorate.ChallengeStartDate == null || _dep.Directorate.ChallengeEndDate == null)
            {
                _start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                _end = _start.AddMonths(1).AddMinutes(-1);
            }
            else
            {
                _start = (DateTime)_dep.Directorate.ChallengeStartDate;
                _end = (DateTime)_dep.Directorate.ChallengeEndDate;
            }
            List<DateTime> dates = GetDatesBetween(_start, _end);

            List<double> res = new List<double>();
            foreach (var date in dates) res.Add(0);

            foreach (var activity in _dep.Activities.Where(a => a.Date >= _start && a.Date <= _end))
            {
                var day = (activity.Date.Day - _start.Day);
                res[day] = res[day] + (1 / _dep.Users.Count);
            }

            for (int i = 1; i < res.Count; i++)
            {
                var value = res[i - 1] + res[i];
                res[i] = value;
            }
            return res;
        }

        public List<double> generateCalData(User _user)
        {
            DateTime _start;
            DateTime _end;
            if (_user.Department.Directorate.ChallengeStartDate == null || _user.Department.Directorate.ChallengeEndDate == null)
            {
                _start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                _end = _start.AddMonths(1).AddMinutes(-1);
            }
            else
            {
                _start = (DateTime)_user.Department.Directorate.ChallengeStartDate;
                _end = (DateTime)_user.Department.Directorate.ChallengeEndDate;
            }
            List<DateTime> dates = GetDatesBetween(_start, _end);

            List<double> res = new List<double>();
            foreach (var date in dates) res.Add(0);

            foreach(var activity in _user.Activities.Where(a => a.Date >= _start && a.Date <= _end))
            {
                var day = (activity.Date.Day - _start.Day);
                res[day] = res[day] + activity.CalculatedCalories;
            }
            
            for(int i=1; i<res.Count; i++)
            {
                var value = res[i - 1] + res[i];
                res[i] = value;
            }
            return res;
        }

        public List<double> generateCalData(Department _dep)
        {
            DateTime _start;
            DateTime _end;
            if (_dep.Directorate.ChallengeStartDate == null || _dep.Directorate.ChallengeEndDate == null)
            {
                _start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                _end = _start.AddMonths(1).AddMinutes(-1);
            }
            else
            {
                _start = (DateTime)_dep.Directorate.ChallengeStartDate;
                _end = (DateTime)_dep.Directorate.ChallengeEndDate;
            }
            List<DateTime> dates = GetDatesBetween(_start, _end);

            List<double> res = new List<double>();
            foreach (var date in dates) res.Add(0);

            foreach (var activity in _dep.Activities.Where(a => a.Date >= _start && a.Date <= _end))
            {
                var day = (activity.Date.Day - _start.Day);
                res[day] = res[day] + (activity.CalculatedCalories / _dep.Users.Count);
            }

            for (int i = 1; i < res.Count; i++)
            {
                var value = res[i - 1] + res[i];
                res[i] = value;
            }
            return res;
        }

        public List<double> generateDisData(User _user)
        {
            DateTime _start;
            DateTime _end;
            if (_user.Department.Directorate.ChallengeStartDate == null || _user.Department.Directorate.ChallengeEndDate == null)
            {
                _start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                _end = _start.AddMonths(1).AddMinutes(-1);
            }
            else
            {
                _start = (DateTime)_user.Department.Directorate.ChallengeStartDate;
                _end = (DateTime)_user.Department.Directorate.ChallengeEndDate;
            }
            List<DateTime> dates = GetDatesBetween(_start, _end);

            List<double> res = new List<double>();
            foreach (var date in dates) res.Add(0);

            foreach (var activity in _user.Activities.Where(a => a.Date >= _start && a.Date <= _end))
            {
                var day = (activity.Date.Day - _start.Day);
                res[day] = Math.Round(res[day] + activity.Distance, 2);
            }

            for (int i = 1; i < res.Count; i++)
            {
                var value = res[i - 1] + res[i];
                res[i] = Math.Round(value, 2);
            }
            return res;
        }

        public List<double> generateDisData(Department _dep)
        {
            DateTime _start;
            DateTime _end;
            if (_dep.Directorate.ChallengeStartDate == null || _dep.Directorate.ChallengeEndDate == null)
            {
                _start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                _end = _start.AddMonths(1).AddMinutes(-1);
            }
            else
            {
                _start = (DateTime)_dep.Directorate.ChallengeStartDate;
                _end = (DateTime)_dep.Directorate.ChallengeEndDate;
            }
            List<DateTime> dates = GetDatesBetween(_start, _end);

            List<double> res = new List<double>();
            foreach (var date in dates) res.Add(0);

            foreach (var activity in _dep.Activities.Where(a => a.Date >= _start && a.Date <= _end))
            {
                var day = (activity.Date.Day - _start.Day);
                res[day] = res[day] + (activity.Distance / _dep.Users.Count);
            }

            for (int i = 1; i < res.Count; i++)
            {
                var value = res[i - 1] + res[i];
                res[i] = Math.Round(value, 2);
            }
            return res;
        }

        public List<double> generateDurData(User _user)
        {
            DateTime _start;
            DateTime _end;
            if (_user.Department.Directorate.ChallengeStartDate == null || _user.Department.Directorate.ChallengeEndDate == null)
            {
                _start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                _end = _start.AddMonths(1).AddMinutes(-1);
            }
            else
            {
                _start = (DateTime)_user.Department.Directorate.ChallengeStartDate;
                _end = (DateTime)_user.Department.Directorate.ChallengeEndDate;
            }
            List<DateTime> dates = GetDatesBetween(_start, _end);

            List<double> res = new List<double>();
            foreach (var date in dates) res.Add(0);

            foreach (var activity in _user.Activities.Where(a => a.Date >= _start && a.Date <= _end))
            {
                var day = (activity.Date.Day - _start.Day);
                res[day] = res[day] + activity.Duration.TotalMinutes;
            }

            for (int i = 1; i < res.Count; i++)
            {
                var value = res[i - 1] + res[i];
                res[i] = value;
            }
            return res;
        }

        public List<double> generateDurData(Department _dep)
        {
            DateTime _start;
            DateTime _end;
            if (_dep.Directorate.ChallengeStartDate == null || _dep.Directorate.ChallengeEndDate == null)
            {
                _start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                _end = _start.AddMonths(1).AddMinutes(-1);
            }
            else
            {
                _start = (DateTime)_dep.Directorate.ChallengeStartDate;
                _end = (DateTime)_dep.Directorate.ChallengeEndDate;
            }
            List<DateTime> dates = GetDatesBetween(_start, _end);

            List<double> res = new List<double>();
            foreach (var date in dates) res.Add(0);

            foreach (var activity in _dep.Activities.Where(a => a.Date >= _start && a.Date <= _end))
            {
                var day = (activity.Date.Day - _start.Day);
                res[day] = res[day] + (activity.Duration.TotalMinutes / _dep.Users.Count);
            }

            for (int i = 1; i < res.Count; i++)
            {
                var value = res[i - 1] + res[i];
                res[i] = value;
            }
            return res;
        }

    }
}
