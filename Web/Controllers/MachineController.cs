using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using Orbital.Web.Models;
using System.Dynamic;

namespace Orbital.Web.Controllers
{
    public class MachineController : Controller
    {
        private IDbContext DbContext { get; set; }

        public MachineController(IDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public ActionResult Index()
        {
            return View();
        }



        public ActionResult Get(int id)
        {
            ViewBag.Name = "Phoenix";
            ViewBag.Description = "Windows 2008 R2 Web Server (Xen Virtual Machine) in Chicago, IL.";
            ViewBag.HostName = "phoenix.lightviper.net";

            var sql = @"
SELECT CounterID, SUBSTRING(MachineName, 3, LEN(MachineName) - 2) AS MachineName, ObjectName, CounterName, InstanceName
FROM CounterDetails
ORDER BY MachineName, ObjectName, CounterName, InstanceName";
            var query = DbContext.GetConnection().Query(sql);

            ViewBag.Machines = from item in query
                               group item by item.MachineName into machines
                               select new Machine()
                               {
                                   Name = machines.Key,
                                   Objects = from machine in machines.ToList()
                                             group machine by machine.ObjectName into objects
                                             select new Object()
                                             {
                                                 Name = objects.Key,
                                                 Counters = from obj in objects.ToList()
                                                            select new Counter()
                                                            {
                                                                Name = obj.CounterName,
                                                                Instance = obj.InstanceName
                                                            }
                                             }
                               };
            sql = @"
SELECT
    CounterId,
	CAST(SUBSTRING(CounterDateTime, 0, 20) AS DATETIME) AS Date, 
	CounterValue AS Value
FROM CounterData 
WHERE CounterId in (29, 30, 31, 32, 33, 34) AND CAST(SUBSTRING(CounterDateTime, 0, 20) AS DATETIME) > DATEADD(mi, -30, GETDATE())";
            query = DbContext.GetConnection().Query(sql);

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

            
            //ViewBag.Categories.Add
             
            return View();
        }
    }

    public class Machine
    {
        public string Name { get; set; }

        public IEnumerable<Object> Objects { get; set; }
    }

    public class Object
    {
        public string Name { get; set; }

        public IEnumerable<Counter> Counters { get; set; }
    }

    public class Counter
    {
        public string Name { get; set; }

        public string Instance { get; set; }
    }
}
