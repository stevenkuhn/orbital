using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;

namespace Web.Controllers
{
	public class HomeController : Controller
	{
        // EARNING: THIS IS JUST CONCEPT CODE.

		private IDbContext DbContext { get; set; }

		public HomeController(IDbContext dbContext)
		{
			DbContext = dbContext;
		}

		public ActionResult Index()
		{
			var sql = @"
SELECT
	DisplayString, 
	SUBSTRING(MachineName, 3, LEN(MachineName) - 2) AS MachineName, 
	ObjectName, 
	CounterName, 
	InstanceName, 
	CAST(SUBSTRING(CounterDateTime, 0, 20) AS DATETIME) AS CounterDateTime, 
	CounterValue 
FROM CounterData 
JOIN CounterDetails ON CounterData.CounterID = CounterDetails.CounterID 
JOIN DisplayToID ON CounterData.GUID = DisplayToID.GUID
WHERE CAST(SUBSTRING(CounterDateTime, 0, 20) AS DATETIME) > DATEADD(mi, -15, GETDATE())";
			/*var groups = from t in DataContext.Tags
             group t by t.Tag into g
             select new { Tag = g.Key, Frequency = g.Count() };
return groups.OrderByDescending(g => g.Frequency).Take(25);
			 
			 var basketBalls = 
   from Ball ball in Balls 
   group ball by ball.IsBasketBall() into g
   select new {
     IsBasketBall = g.Key,
     Elements = from b in g 
                group b by new { 
                  Color = ball.Color, IsBasketBall= g.Key, Size = ball.Size, 
                  Material = g.Key ? DefaultMaterial : ball.Material }
   }*/
			var query = DbContext.GetConnection().Query<CounterDataPoint>(sql);

			/*ViewBag.TotalRunTime = from dataPoint in query
										  where dataPoint.CounterName == "% Total Run Time"
										  group dataPoint by dataPoint.ObjectName into g
										  select new Group
										  {
											  Name = g.Key,
											  Elements = g.ToList()
										  };

			ViewBag.GuestRunTime = from dataPoint in query
										  where dataPoint.CounterName == "% Guest Run Time"
										  group dataPoint by dataPoint.InstanceName into g
										  select new Group
										  {
											  Name = g.Key,
											  Elements = g.ToList()
										  };*/

			ViewBag.TotalProcessorTime = from dataPoint in query
												  where
														dataPoint.ObjectName == "Processor" &&
														dataPoint.CounterName == "% Processor Time" &&
														dataPoint.InstanceName == "_Total"
												  group dataPoint by dataPoint.MachineName into g
												  select new Group
												  {
													  Name = g.Key,
													  Elements = g.ToList()
												  };

			ViewBag.AvailableMemory = from dataPoint in query
											  where dataPoint.ObjectName == "Memory" && dataPoint.CounterName == "Available MBytes"
											  group dataPoint by dataPoint.MachineName into g
											  select new Group
											  {
												  Name = g.Key,
												  Elements = g.ToList()
											  };

			return View();
		}

		public ActionResult About()
		{
			return View();
		}
	}

	public class Group
	{
		public string Name { get; set; }
		public IEnumerable<CounterDataPoint> Elements { get; set; }
	}

	public class CounterDataPoint
	{
		public string DisplayString { get; set; }
		public string MachineName { get; set; }
		public string ObjectName { get; set; }
		public string CounterName { get; set; }
		public string InstanceName { get; set; } 
		public DateTime CounterDateTime { get; set; }
		public double CounterValue { get; set; }
	}
}
