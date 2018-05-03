using System;
using System.ComponentModel.DataAnnotations;

namespace ITWorkLogs.Models
{

    public class sysOfflines
    {
        [Key]
        public int sysOfflineId { get; set; }

        public string Branches { get; set; }
        public string personConcern { get; set; }
        public string Details { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateConnected { get; set; }

        //required in creating table..
        [Display(Name = "CREATED BY")]
        public string CreatedBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy - hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime DateCreated { get; set; }

        [Display(Name = "MODIFIED BY")]
        public string ModifiedBy { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy - hh:mm tt}", ApplyFormatInEditMode = true)]

        [Display(Name = "DATE/TIME MODIFIED")]
        public DateTime? DateModified { get; set; }
    }

    public class ref_sysOfflines
    {
        [Key]
        public int  ref_sysOfflineId { get; set; }
        public int? sysOfflineId { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy }", ApplyFormatInEditMode = true)]
        public DateTime? DateIn { get; set; }

        public string Remarks { get; set; }

        public string Status { get; set; }

        //required in creating table..
        [Display(Name = "CREATED BY")]
        public string CreatedBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy - hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime? DateCreated { get; set; }

        [Display(Name = "MODIFIED BY")]
        public string ModifiedBy { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy - hh:mm tt}", ApplyFormatInEditMode = true)]

        [Display(Name = "DATE/TIME MODIFIED")]
        public DateTime? DateModified { get; set; }
    }

}