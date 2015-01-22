using System;
using System.Web;
using System.Resources;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;

using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Roles;
using DotNetNuke.Common;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Users;
using DotNetNuke.Common.Utilities;


namespace BiteTheBullet.BtbShoutbox.Components.Ajax
{
    /// <summary>
    /// Base class which all ajax actions should implement
    /// </summary>
    /// <remarks>This provides a number of help methods that are common
    /// in all ajax action concrete classes</remarks>
    public abstract class AjaxAction
    {
        protected bool ajaxResult = false;
        /// <summary>
        /// concrete class should set this value
        /// </summary>
        protected string message = "";

        /// <summary>
        /// holds the current portal settings we are working in
        /// </summary>
        protected PortalSettings currentPortalSetings = null;
        
        /// <summary>
        /// holds a flag to indicate if we have lazy loaded the current
        /// user already
        /// </summary>
        bool hasLoadedUser = false;

        /// <summary>
        /// current user
        /// </summary>
        UserInfo user = null;

        //holds the tabModuleId
        int tabModuleId = -2;

        /// <summary>
        /// holds the moduleId
        /// </summary>
        int moduleId = -2;

        /// <summary>
        /// holds the settings object of the module
        /// </summary>
        BtbShoutboxSettings settings = null;
       
        /// <summary>
        /// implement with logic that the ajax action implements.
        /// </summary>
        /// <remarks>Returns a string containing the json response</remarks>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual string ProcessAction(HttpRequest request)
        {
            return string.Format("{{\"result\":{0}, \"message\":\"{1}\"}}",
                                    ajaxResult.ToString().ToLower(),
                                    message);
        }       

        /// <summary>
        /// Readonly access to the PortalId for the
        /// current portal context
        /// </summary>
        protected int PortalId
        {
            get
            {
                HttpCookie cartCookie = HttpContext.Current.Request.Cookies[ShoutboxBaseModule.PORTAL_COOKIE_NAME];
                if (cartCookie != null)
                {
                    string pid = cartCookie["PortalId"];
                    if (SymmetricHelper.CanSafelyEncrypt &&
                        !string.IsNullOrEmpty(pid))
                    {
                        try
                        {
                            pid = SymmetricHelper.Decrypt(pid);
                            return Int32.Parse(pid);
                        }
                        catch (Exception ex)
                        {
                            //the portal cookie is correct, rethrow
                            throw ex;
                        }
                    }
                }

                throw new ApplicationException("Portal cookie is not valid");
            }
        }

        /// <summary>
        /// read only access to the encrypted moduleId
        /// </summary>
        protected int ModuleId
        {
            get 
            {
                if (moduleId == -2)
                {
                    try
                    {
                        string moduleIdStr = HttpContext.Current.Request["moduleId"];
                        moduleId = int.Parse(SymmetricHelper.Decrypt(moduleIdStr));
                    }
                    catch (Exception)
                    {
                        moduleId = -1;
                    }
                }
                return moduleId; 
            }
        }

        /// <summary>
        /// read only access to the encrypted tabModuleId
        /// </summary>
        protected int TabModuleId
        {
            get 
            {
                if (tabModuleId == -2)
                {
                    try
                    {
                        string tabModuleIdStr = HttpContext.Current.Request["tabModuleId"];
                        tabModuleId = int.Parse(SymmetricHelper.Decrypt(tabModuleIdStr));
                    }
                    catch (Exception)
                    {
                        tabModuleId = -1;
                    }
                }
                return tabModuleId;
            }
        }


        /// <summary>
        /// readonly property to determine if the user is authenticated
        /// </summary>
        protected bool IsAuthenticated
        {
            get
            {
                return Thread.CurrentPrincipal.Identity.IsAuthenticated;
            }
        }



        /// <summary>
        /// method to return the current user
        /// </summary>
        /// <returns></returns>
        protected UserInfo CurrentUser()
        {
            if (!hasLoadedUser)
            {
                if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
                {
                    user = UserController.GetUserByName(CurrentPortalSettings.PortalId,
                                                            Thread.CurrentPrincipal.Identity.Name);
                }
                else
                    user = null;
            }

            return user;
        }

        /// <summary>
        /// escapes the string for output as json
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected string JsonEncode(string input)
        {
            return input.Replace("\"", "\\\"");
        }


        /// <summary>
        /// read only access to the language the ajax request was generated from
        /// </summary>
        /// <remarks>using this we can then load the correct SharedResource file to 
        /// lookup the string.
        /// <para>If this value is not supplied in the ajax post data then current thread will
        /// be used to determine the language (this maybe not the same as the language
        /// selected by the user in DNN)</para></remarks>
        public virtual string Language
        {
            get
            {
                var lang = HttpContext.Current.Request["language"];
                if (string.IsNullOrEmpty(lang))
                    lang = System.Threading.Thread.CurrentThread.CurrentCulture.ToString();

                return lang;
            }
        }

        /// <summary>
        /// read only access the the portalsettings for the current portal we 
        /// are working in
        /// </summary>
        public PortalSettings CurrentPortalSettings
        {
            get
            {
                if (currentPortalSetings == null)
                    currentPortalSetings = new PortalSettings(this.PortalId);

                return currentPortalSetings;
            }
        }

        /// <summary>
        /// readonly accces the module settings
        /// </summary>
        protected BtbShoutboxSettings ModuleSettings
        {
            get
            {
                if(settings == null)
                    settings = new BtbShoutboxSettings(this.TabModuleId);

                return settings;
            }
        }


        /// <summary>
        /// cstor
        /// </summary>
        public AjaxAction()
        {
        }

        #region methods 

        /// <summary>
        /// Renders a control to a string
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static string RenderControl(Control control)
        {
            StringWriter tw = new StringWriter();

            //Simple rendering - just write the control to the text writer
            //works well for single controls without containers
            Html32TextWriter writer = new Html32TextWriter(tw);
            control.RenderControl(writer);
            writer.Close();

            return tw.ToString();
        }


        /// <summary>
        /// method to return the iis application path
        /// </summary>
        /// <remarks>this will include the domain and iis virtual directory, this
        /// value is returned from the current url used to request the page</remarks>
        /// <returns></returns>
        public string GetApplicationPath()
        {
            string applicationPath = "";
            HttpRequest request = HttpContext.Current.Request;

            if (request.Url != null)
                applicationPath = request.Url.AbsoluteUri.Substring(
                 0, request.Url.AbsoluteUri.ToLower().IndexOf(
                  request.ApplicationPath.ToLower(),
                  request.Url.AbsoluteUri.ToLower().IndexOf(
                  request.Url.Authority.ToLower()) +
                  request.Url.Authority.Length) +
                  request.ApplicationPath.Length);

            return applicationPath;
        }

        protected string AjaxShoutsResponse(BtbShoutboxSettings settings, IList<BtbShoutboxInfo> shouts)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append("{\"messages\":[");

            for (int i = 0; i < shouts.Count; i++)
            {
                BtbShoutboxInfo shout = shouts[i];

                buffer.Append("{");
                buffer.AppendFormat("\"itemId\": {0}, \"message\": \"{1}\", \"username\":\"{2}\",\"html\":\"{3}\"",
                                    shout.ItemId,
                                    JsonEncode(shout.Message),
                                    shout.UserName,
                                    JsonEncode(shout.CreateShoutHtml(settings, CurrentUser()))
                                    );
                buffer.Append("}");

                if (i < shouts.Count - 1)
                    buffer.Append(",");
            }

            buffer.Append("]}");

            return buffer.ToString();
        }

        #endregion

  }
}
