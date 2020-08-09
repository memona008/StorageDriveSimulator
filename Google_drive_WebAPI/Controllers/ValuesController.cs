using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace Google_drive_WebAPI.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public void Home()
        {
        }
        public JsonResult CreateFolder([FromBody]String name, [FromBody]int parent)
        {
            Object data = null;
            try
            {
                var f = DS.DAL.FolderDAO.createFolder(name, parent);
                if (f != 0)
                {
                    data = new
                    {
                        created = true
                    };
                }
                else
                {
                    data = new
                    {
                        created = false
                    };
                }

            }
            catch (Exception ex)
            {

                data = new
                {
                    exp = ex,
                    created = false
                };
            }

            var result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.Data = data;
            return result;
        }

    }
}
