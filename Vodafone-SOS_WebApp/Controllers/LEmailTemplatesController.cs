//Code Review for this file (from security perspective) done

using System;
using System.Net;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LEmailTemplatesController : PrimaryController
    {
        ILEmailTemplateRestClient RestClient = new LEmailTemplateRestClient();
        int CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);

        [ControllerActionFilter]
        public JsonResult GetTemplates()
        {
            var ApiData = RestClient.GetAll();
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        // GET: LEmailTemplates
       [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Templates";
            return View();
        }
        
       [ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Template";
            return View();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        [ValidateInput(false)]
        public ActionResult Create(LEmailTemplateViewModel model,string EditorBody, string EditorSignature)
        {
            try
            { 
                    model.LetCompanyId = CompanyId;
                    model.LetEmailBody = EditorBody;
                    model.LetSignature = EditorSignature;
                    RestClient.Add(model);
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(model);
            }
        }

        // GET: LEmailTemplates/Edit/5
        [ControllerActionFilter]
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Web.HttpContext.Current.Session["Title"] = "Edit Template";
            LEmailTemplateViewModel model = RestClient.GetById(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: LEmailTemplates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        [ValidateInput(false)]
        public ActionResult Edit(LEmailTemplateViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RestClient.Update(model);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View(model);
            }
        }

        [ControllerActionFilter]
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Web.HttpContext.Current.Session["Title"] = "Delete Template";
            LEmailTemplateViewModel model = RestClient.GetById(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: GCompanies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(LEmailTemplateViewModel model)
        {
            try
            {
                RestClient.Delete(model.Id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View(model);
            }
        }

    }
}