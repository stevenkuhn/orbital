using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using System.Dynamic;
using Orbital.Web.Models;
using Raven.Client;

namespace Orbital.Web.Controllers
{
    public class MachineController : Controller
    {
        private IDbContext DbContext { get; set; }
        private IDocumentSession RavenSession { get; set; }

        public MachineController(IDbContext dbContext, IDocumentSession ravenSession)
        {
            DbContext = dbContext;
            RavenSession = ravenSession;
        }

        public ActionResult Index()
        {
            ViewBag.Machines = from machine in RavenSession.Query<Machine>()
                               orderby machine.Name
                               select machine;

            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Machine machine)
        {
            RavenSession.Store(machine);
            return RedirectToRoute("machines");
        }

        public ActionResult Get(string name)
        {
            ViewBag.Machine = (from machine in RavenSession.Query<Machine>().ToList()
                               where machine != null && machine.Name.ToUpperInvariant() == name.ToUpperInvariant()
                               select machine).SingleOrDefault();

            ViewBag.Machines = Enumerable.Empty<dynamic>();
            ViewBag.Categories = Enumerable.Empty<dynamic>();

            ViewBag.CounterDetails = DbContext.GetConnection().Query(@"
SELECT CounterID, SUBSTRING(MachineName, 3, LEN(MachineName) - 2) AS MachineName, ObjectName, CounterName, InstanceName
FROM CounterDetails
ORDER BY MachineName, ObjectName, CounterName, InstanceName");

            var query = DbContext.GetConnection().Query(@"
SELECT
    CounterId,
	CAST(SUBSTRING(CounterDateTime, 0, 20) AS DATETIME) AS Date, 
	CounterValue AS Value
FROM CounterData 
WHERE CounterId in (29, 30, 31, 32, 33, 34) AND CAST(SUBSTRING(CounterDateTime, 0, 20) AS DATETIME) > DATEADD(mi, -30, GETDATE())");

            ViewBag.Categories = new List<ExpandoObject>();
            
            dynamic category = new ExpandoObject();
            category.Name = "CPU";
            category.Charts = new List<dynamic>();
            category.Charts.Add(new ExpandoObject());
            category.Charts[0].Id = "usage";
            category.Charts[0].YAxisLabel = "Usage (%)";
            category.Charts[0].Series = new List<dynamic>();
            category.Charts[0].Series.Add(new ExpandoObject());
            category.Charts[0].Series[0].Name = "0";
            category.Charts[0].Series[0].Data = from item in query where item.CounterId == 33 select item;
            category.Charts[0].Series.Add(new ExpandoObject());
            category.Charts[0].Series[1].Name = "1";
            category.Charts[0].Series[1].Data = from item in query where item.CounterId == 34 select item;
            ViewBag.Categories.Add(category);

            category = new ExpandoObject();
            category.Name = "Memory";
            category.Charts = new List<dynamic>();
            category.Charts.Add(new ExpandoObject());
            category.Charts[0].Id = "available";
            category.Charts[0].YAxisLabel = "Available (MB)";
            category.Charts[0].Series = new List<dynamic>();
            category.Charts[0].Series.Add(new ExpandoObject());
            category.Charts[0].Series[0].Name = "Phoenix";
            category.Charts[0].Series[0].Data = from item in query where item.CounterId == 31 select item;
            ViewBag.Categories.Add(category);

            category = new ExpandoObject();
            category.Name = "Disk";
            category.Charts = new List<dynamic>();
            category.Charts.Add(new ExpandoObject());
            category.Charts[0].Id = "disk_read_write_sec";
            category.Charts[0].YAxisLabel = "Avg. Seconds per Read/Write";
            category.Charts[0].Series = new List<dynamic>();
            category.Charts[0].Series.Add(new ExpandoObject());
            category.Charts[0].Series[0].Name = "Read (C:)";
            category.Charts[0].Series[0].Data = from item in query where item.CounterId == 29 select item;
            category.Charts[0].Series.Add(new ExpandoObject());
            category.Charts[0].Series[1].Name = "Write (C:)";
            category.Charts[0].Series[1].Data = from item in query where item.CounterId == 30 select item;
            ViewBag.Categories.Add(category);

            return View();
        }
    }
}
