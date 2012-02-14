using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orbital.Web.Models
{
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