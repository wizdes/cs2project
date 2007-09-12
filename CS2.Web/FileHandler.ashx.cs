using System.Web;
using System.Web.Services;

namespace CS2.Web
{
    public class FileHandler : IHttpHandler
    {
        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            context.Response.TransmitFile(context.Request.QueryString["f"]);
        }

        public bool IsReusable
        {
            get { return false; }
        }

        #endregion
    }
}