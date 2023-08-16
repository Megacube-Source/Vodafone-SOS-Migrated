using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.Models;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    public class AspnetRolesController : PrimaryController
    {
        IAspnetRolesRestClient RestClient = new AspnetRolesRestClient();
        string CompanyCode = System.Web.HttpContext.Current.Session["CompanyCode"] as string;
        public JsonResult GetSelectedRoles(int MenuId)
        {
            var ApiData = RestClient.GetByCompanyCode(CompanyCode).Select(p => new {p.Id,p.Name,select=0 });
            IMGMenusAspnetRolesRestClient MGARC = new MGMenusAspnetRolesRestClient();
            var Menu = MGARC.GetAll().Where(p => p.MgmarMenuId == MenuId).Select(p => new {Id=p.MgmarRoleId,p.Name,select=1 });
            var result = Menu.Union(ApiData).GroupBy(p => p.Id).Select(p => p.FirstOrDefault());

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRoles()
        {
            var ApiData = RestClient.GetByCompanyCode(CompanyCode);
            return Json(ApiData, JsonRequestBehavior.AllowGet);
        }
        // GET: AspnetRoles
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Roles";
            return View();
        }

        // GET: AspnetRoles/Details/5
        [ControllerActionFilter]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspnetRoleViewModel aspnetRoleViewModel = RestClient.GetById(id);
            if (aspnetRoleViewModel == null)
            {
                return HttpNotFound();
            }
            return View(aspnetRoleViewModel);
        }

        // GET: AspnetRoles/Create
        [ControllerActionFilter]
        public ActionResult Create()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Role";
            return View();
        }

        // POST: AspnetRoles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Create( AspnetRoleViewModel ARVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var Roles=RestClient.Add(ARVM);
                    //Using this line untill issue is resolved of Id not coming with data
                    var RoleData = RestClient.GetByCompanyCode(CompanyCode).Where(p => p.Name == Roles.Name).FirstOrDefault();
                    if (ARVM.MenuList != null)
                    {
                        IMGMenusAspnetRolesRestClient MGARRC = new MGMenusAspnetRolesRestClient();
                        String[] s = ARVM.MenuList.Split(',');
                        for (int j = 0; j < s.Length; j++)
                        {
                            s[j] = s[j].Trim();
                            var Menu = Convert.ToInt32(s[j]);
                            var Model = new MGMenusAspnetRoleViewModel { MgmarMenuId = Menu, MgmarRoleId = RoleData.Id };
                            MGARRC.Add(Model);
                        }
                    }
                    return RedirectToAction("Index");
                }

                return View(ARVM);
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(ARVM);
            }
        }

        // GET: AspnetRoles/Edit/5
        [ControllerActionFilter]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Web.HttpContext.Current.Session["Title"] = "Edit Role";
            AspnetRoleViewModel aspnetRoleViewModel = RestClient.GetById(id);
            if (aspnetRoleViewModel == null)
            {
                return HttpNotFound();
            }
            return View(aspnetRoleViewModel);
        }

        // POST: AspnetRoles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Edit( AspnetRoleViewModel aspnetRoleViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RestClient.Update(aspnetRoleViewModel);
                    if (aspnetRoleViewModel.MenuList != null)
                    {
                        //delete previous record
                        IMGMenusAspnetRolesRestClient MGARRC = new MGMenusAspnetRolesRestClient();
                        var previousRecord = MGARRC.GetAll().Where(p => p.MgmarRoleId == aspnetRoleViewModel.Id).ToList();
                        foreach(var record in previousRecord)
                        {
                            MGARRC.Delete(record.Id);
                        }

                        String[] s = aspnetRoleViewModel.MenuList.Split(',');
                        for (int j = 0; j < s.Length; j++)
                        {
                            s[j] = s[j].Trim();
                            var Menu = Convert.ToInt32(s[j]);
                            var Model = new MGMenusAspnetRoleViewModel { MgmarMenuId = Menu, MgmarRoleId = aspnetRoleViewModel.Id };
                            MGARRC.Add(Model);
                        }
                    }
                    return RedirectToAction("Index");
                }
                return View(aspnetRoleViewModel);
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(aspnetRoleViewModel);
            }
        }

        // GET: AspnetRoles/Delete/5
        [ControllerActionFilter]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspnetRoleViewModel aspnetRoleViewModel = RestClient.GetById(id);
            System.Web.HttpContext.Current.Session["Title"] = "Delete Role";
            if (aspnetRoleViewModel == null)
            {
                return HttpNotFound();
            }
            return View(aspnetRoleViewModel);
        }

        // POST: AspnetRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(AspnetRoleViewModel ARVM)
        {
            try
            {
                IMGMenusAspnetRolesRestClient MGARC = new MGMenusAspnetRolesRestClient();
                var MapData = MGARC.GetAll().Where(p => p.MgmarRoleId == ARVM.Id).ToList();
                foreach(var record in MapData)
                {
                    MGARC.Delete(record.Id);
                }
                RestClient.Delete(ARVM.Id);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(ARVM);
            }
        }

     
    }
}
