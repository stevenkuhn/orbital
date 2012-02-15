using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using System.Dynamic;

namespace Orbital.Web.Controllers
{
    public class HomeController : Controller
    {
        private IDbContext DbContext { get; set; }

        public HomeController(IDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public ActionResult Index()
        {
            /*var sql = @"
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

            var query = DbContext.GetConnection().Query<CounterDataPoint>(sql);

            ViewBag.GuestRunTime = from dataPoint in query
                                   where dataPoint.CounterName == "% Total Run Time"
                                   group dataPoint by dataPoint.ObjectName into g
                                   select new Group()
                                   {
                                       Name = g.Key,
                                       Elements = g.ToList()
                                   };

            ViewBag.TotalRunTime = from dataPoint in query
                                   where dataPoint.CounterName == "% Guest Run Time"
                                   group dataPoint by dataPoint.InstanceName into g
                                   select new Group()
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
            */
            return View();
        }
    }

    /*public class Group
    {
        public string Name { get; set; }
        public IEnumerable<CounterDataPoint> Elements { get; set; }
    }*/
}
