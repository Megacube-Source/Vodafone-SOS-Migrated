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
    public class UserGuidesController : ApiController
    {
        // GET: UserGuides
        private SOSEDMV10Entities db = new SOSEDMV10Entities();
        [HttpGet]
        public IHttpActionResult GetReleaseNotes()
        {
            var xx = (from aa in db.GKeyValues
                      where aa.GkvKey == "ReleaseNotes"
                      select new
                      {
                          aa.GkvValue
                      }).FirstOrDefault();
            return Ok(xx);

            

        }
    }
}