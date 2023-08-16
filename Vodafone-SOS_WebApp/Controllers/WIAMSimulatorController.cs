using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.ViewModels;
using Vodafone_SOS_WebApp.Helper;

namespace Vodafone_SOS_WebApp.Controllers
{
    public class WIAMSimulatorController : Controller
    {
        IWIAMSimulatorRestClient RestClient = new WIAMSimulatorRestClient();
        IGCompaniesRestClient GRC = new GCompaniesRestClient();
        string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
        public ActionResult Index()
        {
            var Companies = GRC.GetAll();
            ViewBag.CompanyCode = new SelectList(Companies, "GcCode", "GcCode",CompanyCode);
            ViewBag.UserGroup = GetUserGroup(null);
            ViewBag.UserType = GetUserType(null);
            ViewBag.IsVFADUser = IsADUser();

            ViewBag.OperationType = GetOperation("");
            return View();
        }

        public ActionResult Create()
        {
            var Companies = GRC.GetAll();
            ViewBag.CompanyCode = new SelectList(Companies, "GcCode", "GcCode", CompanyCode);
            ViewBag.UserGroup = GetUserGroup(null);
            ViewBag.UserType = GetUserType(null);
            ViewBag.IsVFADUser = IsADUser();
            return View();
        }
        [HttpPost]
        public ActionResult Create(CreateViewModel data)
        {
            IndexViewModel model = new IndexViewModel
            {
                OperationType = "Create",
                FirstName = data.FirstName,
                LastName = data.LastName,
                Email = data.Email,
                RequestorEmail = data.RequesterEmail,
                ManagerEmail = data.ManagerEmail,
                CompanyCode = data.CompanyCode,
                PayeeCode = data.PayeeCode,
                Phone = data.Phone,
                UserGroup = data.UserGroup,
                UserType = data.UserType,
                VFADUser = data.IsVFADUser
            };
            var Companies = GRC.GetAll();
            ViewBag.CompanyCode = new SelectList(Companies, "GcCode", "GcCode",model.CompanyCode);
            ViewBag.UserGroup = GetUserGroup(model.UserGroup);
            ViewBag.UserType = GetUserType(model.UserType);
            ViewBag.IsVFADUser = IsADUser();
            try
            {
                var result = RestClient.webservice(model);
                return Json (new { SuccessMessage = result, ErrorMessage = data.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                data.ErrorMessage = ex.Data["ErrorMessage"].ToString();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        //public ActionResult Operation(string OperationType)
        //{
        //    var Companies = GRC.GetAll();
        //    ViewBag.CompanyCode = new SelectList(Companies, "GcCode", "GcCode",null);
        //    //ViewBag.OperationType = OperationType;
        //    return View(new IndexViewModel { OperationType = OperationType});
        //}
        //[HttpPost]
        //public ActionResult Operation(IndexViewModel model)
        //{
        //    var Companies = GRC.GetAll();
        //    ViewBag.CompanyCode = new SelectList(Companies, "GcCode", "GcCode", model.CompanyCode);
        //    //ViewBag.OperationType = model.OperationType;
        //    //ViewBag.UserGroup = GetUserGroup(null);
        //    //ViewBag.UserType = GetUserType(null);
        //    //ViewBag.IsVFADUser = IsADUser();
            
        //    try
        //    {
        //        var result = RestClient.webservice(model);
        //        return Json(new { SuccessMessage = result, ErrorMessage = "" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
        //        model.ErrorMessage = ex.Data["ErrorMessage"].ToString();
        //        return Json(model, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //[HttpGet]
        public ActionResult Enable()
        {
            var Companies = GRC.GetAll();
            ViewBag.CompanyCode = new SelectList(Companies, "GcCode", "GcCode", null);
            //ViewBag.OperationType = OperationType;
            return View(new EnableViewModel());
        }
        [HttpPost]
        public ActionResult Enable(EnableViewModel data)
        {
            IndexViewModel model = new IndexViewModel { CompanyCode = data.CompanyCode, Email = data.Email, RequestorEmail = data.RequestorEmail,OperationType= "Enable"};
            var Companies = GRC.GetAll();
            ViewBag.CompanyCode = new SelectList(Companies, "GcCode", "GcCode", model.CompanyCode);
            try
            {
                var result = RestClient.webservice(model);
                return Json(new { SuccessMessage = result, ErrorMessage = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                model.ErrorMessage = ex.Data["ErrorMessage"].ToString();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Disable()
        {
            var Companies = GRC.GetAll();
            ViewBag.CompanyCode = new SelectList(Companies, "GcCode", "GcCode", null);
            //ViewBag.OperationType = OperationType;
            return View(new DisableViewModel());
        }
        [HttpPost]
        public ActionResult Disable(DisableViewModel data)
        {
            IndexViewModel model = new IndexViewModel { CompanyCode = data.CompanyCode, Email = data.Email, RequestorEmail = data.RequestorEmail,OperationType = "Disable" };
            var Companies = GRC.GetAll();
            ViewBag.CompanyCode = new SelectList(Companies, "GcCode", "GcCode", model.CompanyCode);
            try
            {
                var result = RestClient.webservice(model);
                return Json(new { SuccessMessage = result, ErrorMessage = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                model.ErrorMessage = ex.Data["ErrorMessage"].ToString();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpGet]
        public ActionResult Revoke()
        {
            var Companies = GRC.GetAll();
            ViewBag.CompanyCode = new SelectList(Companies, "GcCode", "GcCode", null);
            return View(new RevokeViewModel());
        }
        [HttpPost]
        public ActionResult Revoke(RevokeViewModel data)
        {
            IndexViewModel model = new IndexViewModel { Email = data.Email, RequestorEmail = data.RequestorEmail, OperationType = "Revoke" };
            var Companies = GRC.GetAll();
            ViewBag.CompanyCode = new SelectList(Companies, "GcCode", "GcCode", model.CompanyCode);
            try
            {
                var result = RestClient.webservice(model);
                return Json(new { SuccessMessage = result, ErrorMessage = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                model.ErrorMessage = ex.Data["ErrorMessage"].ToString();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult Update()
        {
            var Companies = GRC.GetAll();
            ViewBag.CompanyCode = new SelectList(Companies, "GcCode", "GcCode", null);
            return View(new UpdateViewModel());
        }
        [HttpPost]
        public ActionResult Update(UpdateViewModel data)
        {
            IndexViewModel model = new IndexViewModel { OldEmail = data.OldEmail, NewEmail = data.NewEmail, RequestorEmail = data.RequestorEmail, OperationType = "Update" };
            var Companies = GRC.GetAll();
            ViewBag.CompanyCode = new SelectList(Companies, "GcCode", "GcCode", model.CompanyCode);
            try
            {
                var result = RestClient.webservice(model);
                return Json(new { SuccessMessage = result, ErrorMessage = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                model.ErrorMessage = ex.Data["ErrorMessage"].ToString();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult SetUserGroup()
        {
            var Companies = GRC.GetAll();
            ViewBag.CompanyCode = new SelectList(Companies, "GcCode", "GcCode", null);
            ViewBag.OldUserGroup = GetUserGroup(null);
            ViewBag.NewUserGroup = GetUserGroup(null);
            return View(new SetUserGroupViewModel());
        }
        [HttpPost]
        public ActionResult SetUserGroup(SetUserGroupViewModel data)
        {
            IndexViewModel model = new IndexViewModel { Email = data.Email, OldUserGroup = data.OldUserGroup,
                NewUserGroup= data.NewUserGroup, RequestorEmail = data.RequestorEmail, OperationType = "SetUserGroup"
            };
            var Companies = GRC.GetAll();
            ViewBag.CompanyCode = new SelectList(Companies, "GcCode", "GcCode", model.CompanyCode);
            ViewBag.OldUserGroup = GetUserGroup(null);
            ViewBag.NewUserGroup = GetUserGroup(null);
            try
            {
                var result = RestClient.webservice(model);
                return Json(new { SuccessMessage = result, ErrorMessage = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                model.ErrorMessage = ex.Data["ErrorMessage"].ToString();
                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        private SelectList GetOperation(string selected)
        {
            IList<SelectListItem> UserGroup = new List<SelectListItem>
            {
                new SelectListItem{Text = "Create", Value = "Create"},
                new SelectListItem{Text = "Enable", Value = "Enable"},
                new SelectListItem{Text = "Disable", Value = "Disable"},
                new SelectListItem{Text = "Revoke", Value = "Revoke"},
                new SelectListItem{Text = "Update UserGroup", Value = "UpdateUserGroup"},
            };
            var x = new SelectList(UserGroup, "Value", "Text", selected);
            return x;
        }

        private SelectList GetUserGroup(string selected)
        {
            IList<SelectListItem> UserGroup = new List<SelectListItem>
            {
                new SelectListItem{Text = "Global Qlik Admin", Value = "Global Qlik Admin"},
                new SelectListItem{Text = "OpCo Qlik Admin", Value = "OpCo Qlik Admin"},
                new SelectListItem{Text = "OpCo Qlik Dev", Value = "OpCo Qlik Dev"},
                new SelectListItem{Text = "OpCo Qlik Users Edit", Value = "OpCo Qlik Users Edit"},
                new SelectListItem{Text = "OpCo Qlik Users View", Value = "OpCo Qlik Users View"},
            };
            var x = new SelectList(UserGroup, "Value", "Text", selected);
            return x;
        }
        private SelectList GetUserType(string selected)
        {
            IList<SelectListItem> UserType = new List<SelectListItem>
            {
                new SelectListItem{Text = "FinOps", Value = "FinOps"},
                new SelectListItem{Text = "Payee", Value = "Payee"},
            };
            var x = new SelectList(UserType, "Value", "Text", selected);
            return x;
        }
        private SelectList IsADUser()
        {
            IList<SelectListItem> user = new List<SelectListItem>
            {
                //new SelectListItem{Text = "Yes", Value = "Yes"},
                new SelectListItem{Text = "No", Value = "No"},
            };
            var x = new SelectList(user, "Value", "Text", "No");
            return x;
        }


    }
}