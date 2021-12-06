using Launcher.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Extensions
{
    public static class TimeWordagelizetionExtensions
    {
        public static string TimeWordagelizition(this DateTime date, DateTime? date1 = null)
        {
            var diff = DateTimeSpan.CompareDates(date, date1 ?? DateTime.UtcNow);

            if (diff.Years > 0) return diff.Years.ToString() + " " + $"year{IsPlural(diff.Years)}";

            if (diff.Months > 0) return diff.Months.ToString() + " " + $"month{IsPlural(diff.Months)}";

            if (diff.Days > 0) return diff.Days.ToString() + " " + $"day{IsPlural(diff.Days)}";

            if (diff.Hours > 0) return diff.Hours.ToString() + " " + $"hour{IsPlural(diff.Hours)}";

            if (diff.Minutes > 0) return diff.Minutes.ToString() + " " + $"minute{IsPlural(diff.Minutes)}";

            return "Just Now";

           
        }
        static string IsPlural(int num) => (num > 1) ? "s" : "";

    }
}
