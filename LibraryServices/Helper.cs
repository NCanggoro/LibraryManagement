using LibraryData.Models;
using System;
using System.Collections.Generic;

namespace LibraryServices
{
    public class Helper
    {
        // readable time
        public static List<string> ReadableLibHours (IEnumerable<BranchHours> branchHours)
        {
            var hours = new List<string>();
            foreach (var time in branchHours)
            {
                var day = ReadableDay(time.DayOfWeek);
                var openTime = ReadableTime(time.OpenTime);
                var closeTime = ReadableTime(time.CloseTime);

                var timeEntry = $"{day} {openTime} until {closeTime} ";
                hours.Add(timeEntry);
            }
            return hours;
        }

        public static string ReadableDay(int number)
        {
            return Enum.GetName(typeof(DayOfWeek), number -1);
        }

        public static string ReadableTime(int time)
        {
            return TimeSpan.FromHours(time).ToString("hh': 'mm");
        }
    }
}
