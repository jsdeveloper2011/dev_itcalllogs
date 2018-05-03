using PagedList;
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

    public class SyncViewMoodel
    {
        public Sync newSync { get; set; }
        public IEnumerable<Sync> sync { get; set; }
    }

    public class SyncErrorViewModel
    {
        public ref_syncerrors newSyncError { get; set; }
        public IEnumerable<ref_syncerrors> SyncError { get; set; }
    }

    public class sysOfflineViewModel
    {
        public sysOfflines newSysOffline { get; set; }
        public IEnumerable<sysOfflines> sysoffline { get; set; }
    }

    public class ref_sysOfflineViewModel
    {
        public ref_sysOfflines newrefSysOffline { get; set; }
        public IEnumerable<ref_sysOfflines> ref_sysOffline { get; set; }
    }

}