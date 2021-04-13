using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace MarketUpdateHelper
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            Console.WriteLine("<Darkshore Capital> Market Update Processing Algorithm");
            //var responseString = client.GetStringAsync("https://theunderminejournal.com/api/item.php?house=103&item=72092").Result;

            var gioObject = GetResponseForItem(itemMap[GhostIronOre]);

            var avg = gioObject.globalmonthly.FirstOrDefault().Average(x => x.silver);

            var globalMonthlies = gioObject.globalmonthly.FirstOrDefault();

            var lastMonthly = globalMonthlies.LastOrDefault();


            var datesOfInterest = GetDatesOfInterest(DateTime.Now.Date);

            var todaysPrice = globalMonthlies.OrderByDescending(x => x.date).Select(x => x.silver).FirstOrDefault();

            Console.WriteLine("== Ghost Iron Ore ==");
            Console.WriteLine($"Current Global Price: {todaysPrice / 100:F2}g");
            foreach(var date in datesOfInterest)
            {
                var thisPrice = globalMonthlies.Where(x => x.date == date.Value.ToString("yyyy-MM-dd")).FirstOrDefault();
                Console.WriteLine($"{date.Key}: {(todaysPrice/thisPrice.silver - 1)/100:P} - {thisPrice.silver/100:F2}g");
            }

            Console.WriteLine($"Average Price of Ghost Iron ore between {gioObject.globalmonthly.FirstOrDefault().Select(x => x.date).FirstOrDefault()} and {lastMonthly.date} is {avg / 100:F2}g");
            return;
        }

        public static Dictionary<string, DateTime> GetDatesOfInterest(DateTime ReferenceDate)
        {
            var resultMap = new Dictionary<string, DateTime>();
            var weekToDate = ReferenceDate;
            while (weekToDate.DayOfWeek != DayOfWeek.Tuesday)
            {
                weekToDate = weekToDate.AddDays(-1);
            }

            resultMap.Add(WeekToDate, weekToDate.Date);

            var monthToDate = new DateTime(ReferenceDate.Year, ReferenceDate.Month, 1);
            resultMap.Add(MonthToDate, monthToDate);

            var yearToDate = new DateTime(ReferenceDate.Year, 1, 1);
            resultMap.Add(YearToDate, yearToDate.Date);

            var expansionToDate = new DateTime(2020, 11, 23);
            resultMap.Add(ExpansionToDate, expansionToDate);

            return resultMap;
        }

        public static TujItemResponse GetResponseForItem(int itemID)
        {
            var responseString = client.GetStringAsync($"https://theunderminejournal.com/api/item.php?house=103&item={itemID}").Result;

            return JsonConvert.DeserializeObject<TujItemResponse>(responseString);
        }

        public static readonly Dictionary<string, int> itemMap = new Dictionary<string, int>()
        {
            { GhostIronOre, 72092 },
        };

        public static readonly Dictionary<int, int> expansionToDateOffset = new Dictionary<int, int>()
        {

        };


        public const string GhostIronOre = "Ghost Iron Ore";

        public const string WeekToDate = "Week to Date";
        public const string MonthToDate = "Month to Date";
        public const string YearToDate = "Year to Date";
        public const string ExpansionToDate = "Expansion to Date";
    }

    

    public class TujStats
    {
        public int id
        { get; set; }

        public string name_enus
        { get; set; }

        public int buyfromvendor
        { get; set; }

        public int selltovendor
        { get; set; }

        public int price
        { get; set; }

        public int quantity
        { get; set; }
    }

    public class TujGlobalMonthlyCollection
    {
        public TujGlobalMonthly[] GlobalMonthlyArray
        { get; set; }
    }

    public class TujGlobalMonthly
    {
        public string date
        { get; set; }

        public decimal silver
        { get; set; }

        public int quantity
        { get; set; }
    }

    public class TujItemResponse
    {
        public List<TujStats> stats
        { get; set; }

        public List<List<TujGlobalMonthly>> globalmonthly
        { get; set; }
    }
}
