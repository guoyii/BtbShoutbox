using System;
using System.Collections;
using System.Data;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using System.Web.SessionState;
using BiteTheBullet.BtbShoutbox.Components.Ajax;

namespace BiteTheBullet.BtbShoutbox
{
    /// <summary>
    /// Generic handler that all cartviper ajax requests will call from jquery
    /// </summary>
    public class AjaxHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest (HttpContext context) 
        {
            AjaxAction action = AjaxActionFactory.CreateAction(context.Request);

            //disable caching of this response
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();
            context.Response.Cache.SetExpires(DateTime.MinValue);
            context.Response.Clear();
            context.Response.ContentType = "application/json";

            //output the json response to the client
            if(action != null)
                context.Response.Write(action.ProcessAction(context.Request));

            context.Response.Flush();
            context.Response.End();
        }
     
        public bool IsReusable {
            get {
                return false;
            }
        }

    }
}