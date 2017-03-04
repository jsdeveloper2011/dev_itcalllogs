using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITWorkLogs.Models
{
    public class WorkLogs
    {
        [Key]
        public int WorkID { get; set; }

        [Display(Name ="Control No.")]
        public string ControlID { get; set; }

        public string Personnel { get; set; }

        [Display(Name ="Dept./Branch")]
        [Required]
        public string Department { get; set; }
        
        [Display(Name ="Person Concern")]
        public string PersonConcern { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Concern { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Details { get; set; }
        public string Remarks { get; set; }

        //Status List : ONGOING,PENDING,TRANSFERED, DONE
        [Required]
        public string Status { get; set; }

        //Status = DONE - Include DoneBy, DateDone.
        [Display(Name = "DONE BY")]
        public string DoneBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy - hh:mm tt}", ApplyFormatInEditMode = true)]
        [Display(Name ="DATE/TIME DONE")]
        public DateTime? DateDone { get; set; }

        //Status = CN - Include CancelBy,DateCanceled.
        [Display(Name = "CANCEL BY")]
        public string CancelBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy - hh:mm tt}", ApplyFormatInEditMode = true)]
        [Display(Name = "DATE/TIME CANCELED")]
        public DateTime? DateCanceled { get; set; }

        //Status = TRANSFERED - Include CancelBy,DateCanceled.
        [Display(Name = "TRANSFER BY")]
        public string TransferBy { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy - hh:mm tt}", ApplyFormatInEditMode = true)]
        [Display(Name = "DATE/TIME TRANSFERED")]
        public DateTime? DateTransfered { get; set; }

        //always include in tables
        [Display(Name = "CREATED BY")]
        public string CreatedBy { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy - hh:mm tt}", ApplyFormatInEditMode = true)]
        [Display(Name = "DATE/TIME IN")]
        public DateTime? DateCreated { get; set; }
        [Display(Name = "MODIFIED BY")]
        public string ModifiedBy { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy - hh:mm tt}", ApplyFormatInEditMode = true)]

        [Display(Name = "DATE/TIME MODIFIED")]
        public DateTime? DateModified { get; set; }

        public virtual Branches Branches { get; set; }

    }

    public class Branches
    {
        [Key]
        public int BranchID { get; set; }
        [Display(Name ="Department/Branch")]
        public string BRANCHCODE { get; set; }
        public string BRANCHNAME { get; set; }
    }
}