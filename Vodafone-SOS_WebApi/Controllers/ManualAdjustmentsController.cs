using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.SqlClient; //being used in catch statement for identifying exception only.
using Vodafone_SOS_WebApi.Models;
using Vodafone_SOS_WebApi.Utilities;
using System.Globalization;

namespace Vodafone_SOS_WebApi.Controllers
{
    [CustomExceptionFilter]
    public class ManualAdjustmentsController : ApiController
    {
        private SOSEDMV10Entities db = new SOSEDMV10Entities();

        [HttpPost]
        public IHttpActionResult PostUploadManualAdjustMent(List<PostUploadManualAdjustMentViewModel> LCalc, string FileName, string CommissionPeriod, string UpdatedBy, string AtachmentFiles,string LoggedInRoleId,string WorkflowName,string FilePath,string BatchName,int CompanyId)
        {
            string PortfolioList = string.Empty;
            if (LCalc.Count > 0)
            {
                PortfolioList = LCalc[0].ParameterCarrier;
            }
            
            //Globals.SendEmail("ssharma@megacube.com.au", null, "Manual Adj Api Error","Entered method");
            string XPayeeName = string.Empty;
            string XParentPayeeName = string.Empty;
            string XParentPayee = string.Empty;
            var BatchModel = new LBatch();
            var RawQuery = db.Database.SqlQuery<Int32>("SELECT NEXT VALUE FOR dbo.SQ_BatchNumber");
            var Task = RawQuery.SingleAsync();
            var BatchNumber = Task.Result;
            Task.Dispose();
            // var CompanyCode = LCalc.FirstOrDefault().LcOpCoCode;
            // var CompanyId = db.GCompanies.Where(p => p.GcCode == CompanyCode).FirstOrDefault().Id;
            var WFDetails = db.RWorkFlows.Where(p => p.RwfName.Equals(WorkflowName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            var CommissionPeriodDetails = db.LCommissionPeriods.Where(p => p.LcpPeriodName == CommissionPeriod & p.LcpCompanyId == CompanyId).FirstOrDefault();
            //db.SpLogError("Vodafone-SOS_WebApi", "ManualAdjustment", "PostUploadManualAdjustment", "Batch Number Created-" + BatchNumber, "", "Testing", "", "", "L2 Admin", null, null, "New");
            // Globals.SendEmail("ssharma@megacube.com.au", null, "Manual Adj Api Error", "Batch Number Obtained");
            //steps to add multiple records under a transaction
            using (var transaction = db.Database.BeginTransaction())
            {
                int ExceptionCount = 0;
                try
                {
                    //                    //WFStatus = Saved
                    //                    WFOrdinal = 1
                    //WFRequesterID = Login User
                    //WFAnalystID = (Look for ActingAs = Analyst in the WFConfig for that WorkflowID/ OpCo combo)
                    //                            WFManagerID = (Look for ActingAs = Manager in the WFConfig for that WorkflowID/ OpCo combo)
                    //                            CurrentOwner = Login User
                    //Note: Calculate Ordinal based on the Current Role who has created Manual Adjustment based on RoleId and Opco and WorkflowName
                    var Ordinal = db.LWorkFlowConfigs.Where(p => p.LwfcCompanyId == CompanyId).Where(p => p.LwfcRoleId == LoggedInRoleId).Where(p => p.RWorkFlow.RwfName == WorkflowName).FirstOrDefault().LwfcOrdinalNumber;
                    var RecordCount = LCalc.Count();
                    // Globals.SendEmail("ssharma@megacube.com.au", null, "Manual Adj Api Error", "Ordinal Obtained");
                    BatchModel = new LBatch { LbBatchName = BatchName, WFCompanyId = CompanyId, WFRequesterRoleId = LoggedInRoleId, WFType = WFDetails.RwfWFType, WFOrdinal = Ordinal, WFCurrentOwnerId = UpdatedBy, WFStatus = "Saved", LbRecordCount = RecordCount, WFRequesterId = UpdatedBy, LbCommissionPeriod = CommissionPeriod, LbBatchNumber = BatchNumber, LbBatchType = "ManualAdjustment", LbCompanyId = CompanyId, LbStatus = "ManualAdjustmentBatchAccepted", LbUpdatedBy = UpdatedBy, LbUploadStartDateTime = DateTime.UtcNow, LbPeriodCode = (CommissionPeriodDetails.LcpPeriodCode.HasValue) ? CommissionPeriodDetails.LcpPeriodCode : null };
                    db.LBatches.Add(BatchModel);
                    db.SaveChanges();
                    // Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "ManualAdjustment", "PostUploadManualAdjustment", "Batch Created-" + BatchModel.Id, "", "Testing", "", "", "L2 Admin", null, null, "New");
                    // Globals.SendEmail("ssharma@megacube.com.au", null, "Manual Adj Api Error", "Batch Saved");
                    var LBatchFiles = new LBatchFile { LbfBatchId = BatchModel.Id, LbfFileName = FileName, LbfFileTimeStamp = DateTime.UtcNow };
                    db.LBatchFiles.Add(LBatchFiles);
                    db.SaveChanges();
                    //Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "ManualAdjustment", "PostUploadManualAdjustment","BatchFiles Created-"+BatchModel.Id, "", "Testing", "", "", "L2 Admin", null, null, "New");

                    if (!string.IsNullOrEmpty(PortfolioList))
                    {
                        //add data in entity portfolios
                        var ListPortfolio = PortfolioList.Split(',').ToList();
                        //loop through portfolio list and assign portfolio to batch
                        foreach (var portfolio in ListPortfolio)
                        {
                            var PortfolioId = Convert.ToInt32(portfolio);
                            var EntityPortfolio = new MEntityPortfolio { MepPortfolioId = PortfolioId, MepEntityId = BatchModel.Id, MepEntityType = "LBatches" };
                            db.MEntityPortfolios.Add(EntityPortfolio);
                            db.SaveChanges();
                        }
                    }
                    // Globals.SendEmail("ssharma@megacube.com.au", null, "Manual Adj Api Error", "Porttfolio Saved");
                    //add attachments
                    if (!string.IsNullOrEmpty(AtachmentFiles))
                    {
                        var AtachmentsList = AtachmentFiles.Split(',').ToList();
                        //loop through atachments list to add atachments in LSupporting Documents
                        foreach (var Atachment in AtachmentsList)
                        {
                            //var AtachedFileName = Atachment.Split('/').LastOrDefault();
                            // var xx = Atachment.Split('/').ToArray();
                            //var FilePath = string.Join("/",xx.Take(xx.Length-1));
                            var SupportingDocuments = new LSupportingDocument { LsdCreatedById = UpdatedBy, LsdCreatedDateTime = DateTime.UtcNow, LsdUpdatedById = UpdatedBy, LsdUpdatedDateTime = DateTime.UtcNow, LsdFileName = Atachment, LsdFilePath = FilePath, LsdEntityId = BatchModel.Id, LsdEntityType = "LBatches" };
                            db.LSupportingDocuments.Add(SupportingDocuments);
                            db.SaveChanges();
                        }
                    }
                    // Globals.SendEmail("ssharma@megacube.com.au", null, "Manual Adj Api Error", "Supporting Document Added","QA");

                    int LoopCount = 0;
                    var ActivityType = db.RActivityTypes.Where(p => p.RatCompanyId == CompanyId).Where(p => p.RatIsActive == true).Select(p => p.RatName).ToList();
                    var ProductCode = db.RProductCodes.Where(p => p.RpcCompanyId == CompanyId).Where(p => p.RpcIsActive == true).Select(p => p.RpcProductCode).ToList();
                    var DeviceType = db.RDeviceTypes.Where(p => p.RdtCompanyId == CompanyId).Where(p => p.RdtIsActive == true).Select(p => p.RdtName).ToList();
                    var CommissionType = db.RCommissionTypes.Where(p => p.RctCompanyId == CompanyId).Where(p => p.RctIsActive == true).Select(p => p.RctName).ToList();
                    var Payees = db.LPayees.Where(p => p.LpCompanyId == CompanyId).Select(p => p.LpPayeeCode).ToList();
                    List<string> ErrorList = new List<string>();
                    var Counter = 1;//used for AlteryxTransactionNumber
                    foreach (var CalcModel in LCalc)
                    {
                        var ExceptionString = "";
                        try
                        {
                            // var Payee = db.LPayees.Where(p => p.LpPayeeCode == CalcModel.XPayee).FirstOrDefault(); --Commented for R2.4
                            var Payee = db.LPayees.Where(p => p.LpPayeeCode == CalcModel.XPayee && p.LpCompanyId == CompanyId).FirstOrDefault();
                            //check if payee code entered exist in this opco
                            if (string.IsNullOrEmpty(CalcModel.XPayee))
                            {
                                ExceptionCount += 1;
                                ExceptionString += "Payee Code is mandatory | ";
                            }
                            if (CalcModel.XCommAmtExTax == null)
                            {
                                ExceptionCount += 1;
                                ExceptionString += "CommAmtExTax is mandatory | ";
                            }
                            if (CalcModel.XCommAmtExTax != null)
                            {
                                decimal value;
                                if (Decimal.TryParse(CalcModel.XCommAmtExTax.ToString(), out value))
                                {
                                }
                                else
                                {
                                    ExceptionCount += 1;
                                    ExceptionString += "Invalid CommAmtExTax | ";
                                }
                            }
                            if (CalcModel.XTax != null)
                            {
                                decimal XTaxvalue;
                                if (Decimal.TryParse(CalcModel.XTax.ToString(), out XTaxvalue))
                                {
                                }
                                else
                                {
                                    ExceptionCount += 1;
                                    ExceptionString += "Invalid Tax | ";
                                }


                            }
                            if (CalcModel.XCommAmtIncTax != null)
                            {
                                decimal XCommAmtIncTaxvalue;
                                if (Decimal.TryParse(CalcModel.XCommAmtIncTax.ToString(), out XCommAmtIncTaxvalue))
                                {
                                }
                                else
                                {
                                    ExceptionCount += 1;
                                    ExceptionString += "Invalid CommAmtIncTax | ";
                                }


                            }
                            if (!string.IsNullOrEmpty(CalcModel.XPayee))
                            {
                                if (!Payees.Contains(CalcModel.XPayee.Trim(), StringComparer.OrdinalIgnoreCase))
                                // if (Payees.XPayee == CalcModel.XPayee && !string.IsNullOrEmpty(CalcModel.XPayee))
                                {
                                    ExceptionCount += 1;
                                    ExceptionString += "Invalid Payee Code | ";
                                }
                                else if (Payees.Contains(CalcModel.XPayee.Trim(), StringComparer.OrdinalIgnoreCase) && !string.IsNullOrEmpty(CalcModel.XPayee))//payee exist in opco
                                {
                                    //Find Porfolios associated with PayeeCode
                                    var PayeePortfolios = db.MEntityPortfolios.Where(p => p.MepEntityId == Payee.Id).Where(p => p.MepEntityType == "LPayees").Select(p => p.MepPortfolioId).ToList();
                                    var SelectedPortfolioList = PortfolioList.Split(',').ToList();
                                    var IntersectionResult = PayeePortfolios.Select(i => i.ToString()).Intersect(SelectedPortfolioList).ToList();
                                    if (IntersectionResult.Count() == 0)
                                    {
                                        ExceptionCount += 1;
                                        ExceptionString += "Payees (" + CalcModel.XPayee + ") portfolio does not match with selected portfolios | ";
                                    }
                                }
                            }


                            if (CalcModel.XOrderDate != null)
                            {

                                try
                                {
                                    DateTime dDate = DateTime.ParseExact(CalcModel.XOrderDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                }
                                catch
                                {
                                    ExceptionCount += 1;
                                    ExceptionString += "Invalid OrderDate | ";
                                }


                            }
                            if (CalcModel.XConnectionDate != null)
                            {
                                try
                                {
                                    DateTime dDate = DateTime.ParseExact(CalcModel.XConnectionDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                }
                                catch
                                {
                                    ExceptionCount += 1;
                                    ExceptionString += "Invalid ConnectionDate | ";
                                }
                            }
                            if (CalcModel.XTerminationDate != null)
                            {
                                try
                                {
                                    DateTime dDate = DateTime.ParseExact(CalcModel.XTerminationDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                                }
                                catch
                                {
                                    ExceptionCount += 1;
                                    ExceptionString += "Invalid TerminationDate | ";
                                }
                            }
                            //if (!Payees.Contains(CalcModel.XParentPayee) && !string.IsNullOrEmpty(CalcModel.XParentPayee))
                            //{
                            //    ExceptionCount += 1;
                            //    ExceptionString += "Incorrect Input for Parent Payee (" + CalcModel.XParentPayee + ") | ";
                            //}
                            if (!string.IsNullOrEmpty(CalcModel.XActivityType))
                            {
                                if (!ActivityType.Contains(CalcModel.XActivityType.Trim(), StringComparer.OrdinalIgnoreCase))
                                {
                                    ExceptionCount += 1;
                                    ExceptionString += "Invalid Activity type (" + CalcModel.XActivityType + ") | ";
                                }
                            }
                            if (!string.IsNullOrEmpty(CalcModel.XProductCode))
                            {
                                if (!ProductCode.Contains(CalcModel.XProductCode.Trim(), StringComparer.OrdinalIgnoreCase))
                                {
                                    ExceptionCount += 1;
                                    ExceptionString += "Invalid Product Code (" + CalcModel.XProductCode + ") | ";
                                }
                            }
                            if (!string.IsNullOrEmpty(CalcModel.XDeviceType))
                            {
                                if (!DeviceType.Contains(CalcModel.XDeviceType.Trim(), StringComparer.OrdinalIgnoreCase))
                                {
                                    ExceptionCount += 1;
                                    ExceptionString += "Invalid Device Type (" + CalcModel.XDeviceType + ") | ";
                                }
                            }
                            if (!string.IsNullOrEmpty(CalcModel.XCommType))
                            {
                                if (!CommissionType.Contains(CalcModel.XCommType.Trim(), StringComparer.OrdinalIgnoreCase))
                                {
                                    ExceptionCount += 1;
                                    ExceptionString += "Invalid Commission Type (" + CalcModel.XCommType + ") | ";
                                }
                            }
                            if (!string.IsNullOrEmpty(CalcModel.XPayee))
                            {
                                if (Payees.Contains(CalcModel.XPayee.Trim(), StringComparer.OrdinalIgnoreCase))
                                {
                                    //RKENCR Change
                                    //if (Payee.LpFirstName != null)
                                    //{
                                    //    XPayeeName = Payee.LpFirstName;
                                    //}
                                    //if (Payee.LpLastName != null)
                                    //{
                                    //    XPayeeName += ' ' + Payee.LpLastName;
                                    //}

                                    XPayeeName = Globals.GetPayeeName(Payee.Id, false);
                                    var ParentPayeID = db.LPayeeParents.Where(p => p.LppPayeeId == Payee.Id).FirstOrDefault();
                                    XParentPayee = string.Empty;
                                    XParentPayeeName = string.Empty;
                                    if (ParentPayeID != null)
                                    {
                                        var ParentPayeeData = db.LPayees.Where(p => p.Id == ParentPayeID.LppParentPayeeId).FirstOrDefault();

                                        if (ParentPayeeData != null)
                                        {
                                            XParentPayee = ParentPayeeData.LpPayeeCode;
                                            //RKENCR Change
                                            //if (ParentPayeeData.LpFirstName != null)
                                            //{
                                            //    XParentPayeeName = ParentPayeeData.LpFirstName;
                                            //}
                                            //if (ParentPayeeData.LpLastName != null)
                                            //{
                                            //    XParentPayeeName += ' ' + ParentPayeeData.LpLastName;
                                            //}
                                            XParentPayeeName = Globals.GetPayeeName(ParentPayeeData.Id, false);
                                        }
                                    }

                                }
                            }

                            if (string.IsNullOrEmpty(ExceptionString))
                            {
                                //XCalc Insert Query
                                if (CalcModel.XActivityType != null) { CalcModel.XActivityType = CalcModel.XActivityType.Trim(); }
                                if (CalcModel.XProductCode != null) { CalcModel.XProductCode = CalcModel.XProductCode.Trim(); }
                                if (CalcModel.XDeviceType != null) { CalcModel.XDeviceType = CalcModel.XDeviceType.Trim(); }
                                if (CalcModel.XCommType != null) { CalcModel.XCommType = CalcModel.XCommType.Trim(); }
                                if (Payee.LpPayeeCode != null) { Payee.LpPayeeCode = Payee.LpPayeeCode.Trim(); }
                                if (Payee.LpPrimaryChannel != null) { Payee.LpPrimaryChannel = Payee.LpPrimaryChannel.Trim(); }
                                if (CalcModel.XSubscriberNumber != null) { CalcModel.XSubscriberNumber = CalcModel.XSubscriberNumber.Trim(); }
                                if (CalcModel.XBAN != null) { CalcModel.XBAN = CalcModel.XBAN.Trim(); }
                                if (CalcModel.XActivityType != null) { CalcModel.XActivityType = CalcModel.XActivityType.Trim(); }
                                if (CalcModel.XPlanDescrition != null) { CalcModel.XPlanDescrition = CalcModel.XPlanDescrition.Trim(); }
                                if (CalcModel.XUpgradeCode != null) { CalcModel.XUpgradeCode = CalcModel.XUpgradeCode.Trim(); }
                                if (CalcModel.XDevieCode != null) { CalcModel.XDevieCode = CalcModel.XDevieCode.Trim(); }
                                if (CalcModel.XIMEI != null) { CalcModel.XIMEI = CalcModel.XIMEI.Trim(); }
                                if (CalcModel.XCommAmtExTax != null) { CalcModel.XCommAmtExTax = CalcModel.XCommAmtExTax.Trim(); }
                                if (CalcModel.XTax != null) { CalcModel.XTax = CalcModel.XTax.Trim(); }
                                if (CalcModel.XCommAmtIncTax != null) { CalcModel.XCommAmtIncTax = CalcModel.XCommAmtIncTax.Trim(); }
                                if (CalcModel.XChannel != null) { CalcModel.XChannel = CalcModel.XChannel.Trim(); }
                                if (CalcModel.XSource != null) { CalcModel.XSource = CalcModel.XSource.Trim(); }
                                if (CalcModel.ManualAdjustmentCode != null) { CalcModel.ManualAdjustmentCode = CalcModel.ManualAdjustmentCode.Trim(); }

                                var CalcQuery = "insert into {Schema}.XCalc([XBatchNumber],[XAlteryxTransactionNumber],[XPrimaryChannel],[XPayee],[XParentPayee],[XOrderDate],[XConnectionDate],[XTerminationDate],[XSubscriberNumber],[XBAN],";
                                CalcQuery += "[XActivityType],[XPlanDescrition],[XProductCode],[XUpgradeCode],[XIMEI],[XDevieCode],[XDeviceType],[XCommType],[XContractDuration],[XContractId],[XCommAmtExTax],[XTax],[XCommAmtIncTax],[XComments],[XChannel],";
                                CalcQuery += "[XPayeeName],[XParentPayeeName],[XPeriodCode],[XSource],[ManualAdjustmentCode],IsPayeeAccepted,XCommissionPeriod,XBatchName) values(";
                                CalcQuery += "" + BatchNumber + "," + Counter + ",'" + Payee.LpPrimaryChannel + "','" + Payee.LpPayeeCode + "','" + XParentPayee + "'," + ((!string.IsNullOrEmpty(CalcModel.XOrderDate)) ? "'" + DateTime.ParseExact(CalcModel.XOrderDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") + "'" : "null") + "," + (!string.IsNullOrEmpty(CalcModel.XConnectionDate) ? "'" + DateTime.ParseExact(CalcModel.XConnectionDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") + "'" : "null") + "," + (!string.IsNullOrEmpty(CalcModel.XTerminationDate) ? "'" + DateTime.ParseExact(CalcModel.XTerminationDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") + "'" : "null") + ",'" + CalcModel.XSubscriberNumber + "','" + CalcModel.XBAN + "',";
                                CalcQuery += "'" + CalcModel.XActivityType + "','" + CalcModel.XPlanDescrition + "','" + CalcModel.XProductCode + "','" + CalcModel.XUpgradeCode + "','" + CalcModel.XIMEI + "','" + CalcModel.XDevieCode + "','" + CalcModel.XDeviceType + "','" + CalcModel.XCommType + "'," + ((CalcModel.XContractDuration.HasValue) ? CalcModel.XContractDuration.Value.ToString() : "null") + "," + ((!string.IsNullOrEmpty(CalcModel.XContractId)) ? "'" + CalcModel.XContractId + "'" : "null") + "," + (!string.IsNullOrEmpty(CalcModel.XCommAmtExTax) ? CalcModel.XCommAmtExTax.ToString() : "null") + "," + (!string.IsNullOrEmpty(CalcModel.XTax) ? CalcModel.XTax.ToString() : "null") + "," + (!string.IsNullOrEmpty(CalcModel.XCommAmtIncTax) ? CalcModel.XCommAmtIncTax.ToString() : "null") + ",'" + CalcModel.XComments + "','" + CalcModel.XChannel + "',";
                                CalcQuery += "N'" + XPayeeName + "',N'" + XParentPayeeName + "'," + ((CommissionPeriodDetails.LcpPeriodCode.HasValue) ? CommissionPeriodDetails.LcpPeriodCode.ToString() : "null") + ",'" + CalcModel.XSource + "','" + CalcModel.ManualAdjustmentCode + "',0,'" + CommissionPeriod + "', '" + BatchName + "')";
                                //Globals.SendEmail("ssharma@megacube.com.au", null, "MA Api Error", CalcQuery, "QA");
                                Globals.RunUpdateQueryInOpcoDB(CompanyId, CalcQuery, CommissionPeriod);
                                Counter++;
                                //var LCalccModel = new  {
                                //    LcPrimaryChannel=Payee.LpPrimaryChannel,
                                //    LcActivityType = CalcModel.XActivityType,
                                //    LcAdjustmenCode=CalcModel.XAdjustmenCode,
                                //    LcBAN=CalcModel.XBAN,
                                //    LcCommAmtExTax=CalcModel.XCommAmtExTax,
                                //    LcCommAmtIncTax=CalcModel.XCommAmtIncTax,
                                //    LcComments=CalcModel.XComments,
                                //    LcCommType=CalcModel.XCommType,
                                //    LcConnectionDate=CalcModel.XConnectionDate,
                                //    LcContractDuration=CalcModel.XContractDuration,
                                //    LcDeviceType=CalcModel.XDeviceType,
                                //    LcDevieCode=CalcModel.XDevieCode,
                                //    LcIMEI=CalcModel.XIMEI,
                                //    LcOpCoCode=CalcModel.XOpCoCode,
                                //    LcOrderDate=CalcModel.LcOrderDate,
                                //    LcParentPayee=CalcModel.LcParentPayee,
                                //    LcPayee=CalcModel.LcPayee,
                                //    LcProductCode=CalcModel.LcProductCode,
                                //    LcSOSBatchNumber=BatchModel.LbBatchNumber,
                                //    LcSource=CalcModel.LcSource,
                                //    LcSubscriberNumber=CalcModel.LcSubscriberNumber,
                                //    LcTax=CalcModel.LcTax,
                                //    LcTerminationDate=CalcModel.LcTerminationDate,
                                //    LcUpgradeCode=CalcModel.LcUpgradeCode
                                //};

                                //db.LCalcs.Add(LCalccModel);
                                //db.SaveChanges();

                            }
                        }
                        catch (Exception ex)
                        {
                            //Globals.SendEmail("ssharma@megacube.com.au", null, "Api Error", ex.Message + ex.StackTrace, "QA");
                            ExceptionCount += 1;
                            ExceptionString += ex.Message + " | ";//Need to enhance this exception string with specific values regarding exception. Need to disscuss with JS and ammend this line of code
                            ErrorList.Add(ExceptionString);
                            //Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "ManualAdjustment", "PostUploadManualAdjustment", ex.StackTrace+ex.InnerException, "", "Type1", "", "", "L2 Admin", null, null, "New");
                            continue;
                        }

                        ErrorList.Add(ExceptionString);
                        LoopCount++;
                    }
                    if (ExceptionCount == 0)//data is not saved if a single exception is found
                    {
                        transaction.Commit();

                        //Add Entry in Audit Log
                        // string LaPeriodid = Convert.ToString(db.LCommissionPeriods.Where(p => p.LcpPeriodName == BatchModel.LbCommissionPeriod).Where(p => p.LcpCompanyId == CompanyId).Select(p => p.Id).FirstOrDefault());
                        string LaPeriodid = BatchModel.LbCommissionPeriod;
                        Globals.ExecuteSPAuditLog(WorkflowName, "Audit", null, "Create",
                                   "Create", BatchModel.WFRequesterId, DateTime.UtcNow, BatchModel.WFStatus, BatchModel.WFStatus,
                                  WFDetails.RwfBaseTableName, BatchModel.Id, Convert.ToString(BatchModel.LbBatchName + '(' + BatchModel.LbBatchNumber + ')'), WFDetails.Id, BatchModel.WFCompanyId, string.Empty, BatchModel.WFRequesterRoleId, LaPeriodid);

                    }
                    else
                    {
                        //Globals.ExecuteSPLogError("Vodafone-SOS_WebApi", "ManualAdjustment", "PostUploadManualAdjustment","Test MA ERRORList-"+string.Join(",",ErrorList), "", "Type1", "", "", "L2 Admin", null, null, "New");
                        return Ok(ErrorList); //Faulty records are resent//throw new Exception();//This exception will again be caught in the outer catch  block
                    }
                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            // var BatchList = new List<string>();
            //BatchList.Add();
            //db.SpLogError("Vodafone-SOS_WebApi", "ManualAdjustment", "PostUploadManualAdjustment", "Outside Using Block", "", "Testing", "", "", "L2 Admin", null, null, "New");
            return Ok(BatchModel.Id);
        }

        private string GetCustomizedErrorMessage(Exception ex)
        {
            //First check whether exception is sqlException type 
            //if (ex.GetType().IsAssignableFrom(typeof(SqlException))|| ex.InnerException.GetType().IsAssignableFrom(typeof(SqlException)))
            //{
            //Convert the exception to SqlException to get the error message returned by database.
            var SqEx = ex.GetBaseException() as SqlException;
            return SqEx.Message;
        }
    }
}
