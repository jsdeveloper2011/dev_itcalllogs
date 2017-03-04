using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITWorkLogs.Models
{
    public class WorkLogsViewModel
    {
        
        public  WorkLogs newWorkLogs { get; set; }
        public IEnumerable<WorkLogs> worklogs { get; set; }
    }
}