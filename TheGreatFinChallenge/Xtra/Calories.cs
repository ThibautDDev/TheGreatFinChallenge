using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheGreatFinChallenge.Models;
using TheGreatFinChallenge.Models.Data;

namespace TheGreatFinChallenge.Xtra
{
    public class Calories
    {
        private static readonly int Weight = 72;
        private static readonly double Constant1 = 3.5;
        private static readonly double Constant2 = 200;

        private static int CalculateCalories(double met, double minutes) => Convert.ToInt32((met * Weight * Constant1 * minutes) / Constant2);

        public static int CalculateCalories(Activity activity)
        {
            double met = activity.ActivityType.MET;
            double minutes = (activity.EndTime - activity.StartTime).TotalMinutes;
            return CalculateCalories(met, minutes);
        }

        public static Dictionary<Activity, int> CalculateCalories(List<Activity> activities)
        {
            var result = new Dictionary<Activity, int>();
            foreach (var a in activities) result[a] = CalculateCalories(a);
            return result;
        }

        public static int CalculateTotalCalories(List<Activity> activities)
        {
            int result = 0;
            foreach (var a in activities) result += CalculateCalories(a);
            return result;
        }
    }
}
