using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Vodafone_SOS_WebApi.Models
{
    public partial class LSupportTicketContextModel
    {
        [Key]
        public int Id { get; set; }
        public string LstTicketNumber { get; set; }
        public string LstPhone { get; set; }
        public string LstType { get; set; }
        public int? LstStageID { get; set; }
        public string LstClosureCode { get; set; }
        public string LstSeverity { get; set; }
        public string LstImpact { get; set; }
        public string LstPriority { get; set; }
        public string LstSummary { get; set; }
        public DateTime? LstCreatedDateTime { get; set; }
        //public string LstCC { get; set; }
        public string LstStatus { get; set; }
        public DateTime? LstLastUpdatedDateTime { get; set; }
        public Nullable<int> LstTeamId { get; set; }
        public string LstCreatedById { get; set; }
        public int? LstCompanyId { get; set; }
        public string LstCreatedOnBehalfOfId { get; set; }
        public string LstLastUpdatedById { get; set; }
        public string LstL1Id { get; set; }
        public string LstL2Id { get; set; }
        public string LstL3Id { get; set; }
        public string LstCurrentOwnerId { get; set; }
        public int? LstCategoryID { get; set; }
        public string LstLastUpdatedUserName { get; set; }
        public string LstCreatedByUserName { get; set; }
        public int? Ordinal { get; set; }
        //For Other Linked tables





        public string LstRequestor { get; set; }
        public string LstEmail { get; set; }
       // public string LstDescription { get; set; }
        public string LstExDescription { get; set; }
        //Not used in data saving, created to refer only
        public int LstQuickTicketID { get; set; }
        public string LstQuickTicketName { get; set; }

        //public int Id { get; set; }
        public DateTime LsrResponseDateTime { get; set; }
        public string LsrDescription { get; set; }
        public string LsrUploadedFileNames { get; set; }
        public string LsrTicketStatus { get; set; }
        public int LsrSupportTicketId { get; set; }
        public string LsrResponseById { get; set; }
        public string LsrResponseByName { get; set; }


        //TicketAssignment
        public DateTime LstaCreatedDateTime { get; set; }
        public int LstaSupportTicketId { get; set; }
        public string LstaAssignedToId { get; set; }
        public string LstaAssignedById { get; set; }
        public int LstaSupportTeamId { get; set; }
        //Other Values
        public string OpCoName { get; set; }
        public int Counts { get; set; }

        public string GcCode { get; set; }

        public int Requester { get; set; }
        public int L1 { get; set; }
        public int L2 { get; set; }
        public int Closed { get; set; }
    }
    

}