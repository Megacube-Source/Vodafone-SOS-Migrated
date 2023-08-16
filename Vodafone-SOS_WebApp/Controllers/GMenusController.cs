//Code Review for this file (from security perspective) done

using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;
using Vodafone_SOS_WebApp.ViewModels;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class GMenusController : PrimaryController
    {
        IGMenusRestClient RestClient = new GMenusRestClient();
        [ControllerActionFilter]
        public JsonResult GetMenuItems(string id)
        {
            var grp = Convert.ToInt32(id);
            var result = new[]{ new { text = "<a href='/GMenus/Create/"+grp+"'>Create New Menu</a>", parentid = "-1", id = 0 },
                    new{text = "<a href='/GMenus/Edit/"+grp+"'>Edit Menu</a>", parentid = "-1", id =1},
                    new{text = "<a href='/GMenus/Delete/"+grp+"'>Delete Menu</a>", parentid = "-1", id =2}};
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        [ControllerActionFilter]
        public JsonResult GetMenuList()
        {
            var ApiData = RestClient.GetAll().Select(p => new {id=p.Id,parentid=p.GmParentId,text=p.GmMenuName,value=p.Id });
            return Json(ApiData,JsonRequestBehavior.AllowGet);
        }
        
        [ControllerActionFilter]
        public JsonResult GetSelectedMenuList(string RoleId)
        {
           var ApiData = RestClient.GetAll().Select(p => new {id=p.Id,parentid=p.GmParentId,text=p.GmMenuName,value=p.Id,select=0 });
           IMGMenusAspnetRolesRestClient MGARC = new MGMenusAspnetRolesRestClient();
           var SelectedData = MGARC.GetAll().Where(p=>p.MgmarRoleId==RoleId).Select(p => new { id = p.MgmarMenuId,parentid=p.GmParentId,text= p.GmMenuName,value=p.Id,select=1 });
           var result = SelectedData.Union(ApiData).GroupBy(p=>p.id).Select(p=>p.FirstOrDefault());
           return Json(result, JsonRequestBehavior.AllowGet);
       }

        // GET: GMenus
        [ControllerActionFilter]
        public ActionResult Index()
        {
            System.Web.HttpContext.Current.Session["Title"] = "Manage Menus";
            return View();
        }

        // GET: GMenus/Details/5
        [ControllerActionFilter]
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GMenuViewModel gMenuViewModel =RestClient.GetById(id);
            if (gMenuViewModel == null)
            {
                return HttpNotFound();
            }
            return View(gMenuViewModel);
        }

        // GET: GMenus/Create
        [ControllerActionFilter]
        public ActionResult Create(Nullable<int> id)
        {
            System.Web.HttpContext.Current.Session["Title"] = "Create Menu";
            ViewBag.GmParentId = id;//new SelectList(RestClient.GetAll(), "Id", "GmMenuName");
            return View();
        }

        // POST: GMenus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Create( GMenuViewModel GMVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //setting Id =0 for new record
                    GMVM.Id = 0;
                  var Menu=  RestClient.Add(GMVM);
                    if (GMVM.RoleList != null)
                    {
                        IMGMenusAspnetRolesRestClient MGARRC = new MGMenusAspnetRolesRestClient();
                        String[] s = GMVM.RoleList.Split(',');
                        for (int j = 0; j < s.Length; j++)
                        {
                            s[j] = s[j].Trim();
                            var RoleId = s[j];
                            var Model = new MGMenusAspnetRoleViewModel { MgmarMenuId=Menu.Id,MgmarRoleId=RoleId};
                            MGARRC.Add(Model);
                        }
                    }
                    return RedirectToAction("Index");
                }
                return View(GMVM);
            }
            catch(Exception ex)
            {
               // ViewBag.GmParentId = new SelectList(RestClient.GetAll(), "Id", "GmMenuName");
               ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(GMVM);
            }
        }

        // GET: GMenus/Edit/5
        [ControllerActionFilter]
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Web.HttpContext.Current.Session["Title"] = "Edit Menu";
            GMenuViewModel gMenuViewModel = RestClient.GetById(id);
           // ViewBag.GmParentId = new SelectList(RestClient.GetAll(), "Id", "GmMenuName",gMenuViewModel.GmParentId);
            if (gMenuViewModel == null)
            {
                return HttpNotFound();
            }
            return View(gMenuViewModel);
        }

        // POST: GMenus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult Edit( GMenuViewModel gMenuViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RestClient.Update(gMenuViewModel);
                    if (gMenuViewModel.RoleList != null)
                    {
                        IMGMenusAspnetRolesRestClient MGARRC = new MGMenusAspnetRolesRestClient();
                        var previousrecord = MGARRC.GetAll().Where(p => p.MgmarMenuId == gMenuViewModel.Id).ToList();
                        foreach(var record in previousrecord)
                        {
                            MGARRC.Delete(record.Id);
                        }

                        String[] s = gMenuViewModel.RoleList.Split(',');
                        for (int j = 0; j < s.Length; j++)
                        {
                            s[j] = s[j].Trim();
                            var RoleId = s[j];
                            var Model = new MGMenusAspnetRoleViewModel { MgmarMenuId = gMenuViewModel.Id, MgmarRoleId = RoleId };
                            MGARRC.Add(Model);
                        }
                    }
                    return RedirectToAction("Index");
                }
                return View(gMenuViewModel);
            }
            catch(Exception ex)
            {
               // ViewBag.GmParentId = new SelectList(RestClient.GetAll(), "Id", "GmMenuName", gMenuViewModel.GmParentId);
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(gMenuViewModel);
            }
        }

        // GET: GMenus/Delete/5
        [ControllerActionFilter]
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            System.Web.HttpContext.Current.Session["Title"] = "Delete Menu";
            GMenuViewModel gMenuViewModel = RestClient.GetById(id);
            if (gMenuViewModel == null)
            {
                return HttpNotFound();
            }
            return View(gMenuViewModel);
        }

        // POST: GMenus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ControllerActionFilter]
        public ActionResult DeleteConfirmed(GMenuViewModel GMVM)
        {
            try
            {
                IMGMenusAspnetRolesRestClient MGARC = new MGMenusAspnetRolesRestClient();
                var MapData = MGARC.GetAll().Where(p => p.MgmarMenuId==GMVM.Id).ToList();
                foreach (var record in MapData)
                {
                    MGARC.Delete(record.Id);
                }
                RestClient.Delete(GMVM.Id);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Data["ErrorMessage"].ToString();
                return View(GMVM);
            }
        }
    }
}
