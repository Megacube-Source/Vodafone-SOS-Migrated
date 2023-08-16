//Code Review for this file (from security perspective) done
using System.Web.Mvc;
using Vodafone_SOS_WebApp.Helper;

namespace Vodafone_SOS_WebApp.Controllers
{
    [SessionExpire]
    [HandleCustomError]
    public class LAtachmentsController : Controller
    {
        ILAttachmentsRestClient LARC = new LAttachmentsRestClient();
        // GET: LAtachments
        [ControllerActionFilter]
        public ActionResult Index()
        {
            return View();
        }

        [ControllerActionFilter]
        public ActionResult DeleteAtachment(int id, int FormTypeId,string FormType)
        {
            LARC.Delete(id);
            return RedirectToAction("Edit",FormType, new { id = FormTypeId });
        }
    }
}