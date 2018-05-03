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

        [Required]
        public string Personnel { get; set; }

        [Display(Name ="Dept./Branch")]
        [Required]
        public string Department { get; set; }
        
        [Required]
        [Display(Name ="Person Concern")]
        public string PersonConcern { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Concern { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Details { get; set; }

        [DataType(DataType.MultilineText)]
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
        public DateTime DateIN { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy - hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime DateCreated { get; set; }

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

    public class Sync
    {
        [Key]
        public int SynchId { get; set; }

        [Required]
        public string Branches { get; set; }

        [Required]
        public string Personnel { get; set; }

        [Required]
        public string Reason { get; set; }

        public string Status { get; set; }

        [Display(Name = "DATE/TIME CREATED")]
        public DateTime? DateIn { get; set; }

        [Display(Name = "DATE/TIME STARTED")]
        public DateTime? TimeStarted { get; set; }

        [Display(Name = "DATE/TIME ENDED")]
        public DateTime? TimeEnded { get; set; }

        public string Downloading { get; set; }
        public string Uploading { get; set; }

        public bool Others { get; set; }

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

    public class ref_syncerrors
    {
        [Key]
        public int SyncErrorId { get; set; }

        public int SyncId { get; set; }

        [Required]
        public string ProblemEncountered { get; set; }

        public string Solution { get; set; }

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

        public virtual Sync sync { get; set; }
    }

}