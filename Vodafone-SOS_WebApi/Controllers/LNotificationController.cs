
using System;

//using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.SqlClient; //being used in catch statement for identifying exception only.
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;
namespace Vodafone_SOS_WebApi.Controllers
{
 //   [CustomExceptionFilter]
    public class LNotificationController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        public IHttpActionResult GetNotificationByCompanyId(int CompanyId)
        {
            var NotificationQueryQry = "select Lb.id , R.RwfName As WorkFlowName, AR.Name As StepName, CASE  WHEN Lb.Recipient is null  THEN AR1.Name ELSE Lb.Recipient END as  Recipient , lb.Description, Lb.IsActive , E.LetEmailSubject,e.LetEmailBody from LNotifications lb  join LEmailTemplates E on lb.EmailTemplateId = E.id  left join LWorkFlowConfig Wf on lb.LandingConfigID = Wf.id join RWorkFlows R on wf.lwfcworkflowid = r.id left join AspNetRoles AR on wf.LwfcRoleId = AR.ID left join AspNetRoles AR1 on lb.RecipientRoleID = AR1.ID  where lb.CompanyID = " + CompanyId + " order by R.RwfName ,wf.LwfcOrdinalNumber";
            // var BatchQry = "select lb.LbBatchNumber from LBatches lb where lb.LbCommissionPeriod in ('" + CommissionPeriod + "') AND lb.LbBatchType = 'CALC' AND WFStatus in ('" + WFStatus + "')";
            var NotificationList = db.Database.SqlQuery<LNotificationModel>(NotificationQueryQry).ToList();
            return Ok(NotificationList);

        }
        public IHttpActionResult UpdateNotificationId(LNotificationModel serverData)
        {
            //LNotificationModel serverData1 = new LNotificationModel();
            //serverData1.id = 1;
            //serverData1.IsActive = false;
            var NotificationQueryQry = "";
            if (serverData.IsActive == true)
            {
                 NotificationQueryQry = "update LNotifications set IsActive = 1   where id = " + serverData.id + " ";
            }else
            {
                 NotificationQueryQry = "update LNotifications set IsActive = 0   where id = " + serverData.id + " ";
            }
            // var BatchQry = "select lb.LbBatchNumber from LBatches lb where lb.LbCommissionPeriod in ('" + CommissionPeriod + "') AND lb.LbBatchType = 'CALC' AND WFStatus in ('" + WFStatus + "')";
            var NotificationID = db.Database.ExecuteSqlCommand(NotificationQueryQry);
            return Ok(true);

        }
    }
}