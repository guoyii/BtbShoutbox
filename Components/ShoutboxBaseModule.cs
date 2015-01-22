using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Common;

namespace BiteTheBullet.BtbShoutbox.Components
{
    public class ShoutboxBaseModule : PortalModuleBase
    {
        public const string PORTAL_COOKIE_NAME = "BTB_Portal";

        /// <summary>
        /// Readonly property to return the path the ajax handler
        /// </summary>
        /// <remarks>All ajax calls should use this url to send their requests to</remarks>
        public string AjaxHandlerPath
        {
            get { return Globals.ResolveUrl(string.Format("~/DesktopModules/BtbShoutbox/AjaxHandler.ashx?alias={0}", this.PortalAlias.HTTPAlias)); }
        }

        /// <summary>
        /// Writes the portalId to an encrypted cookie
        /// </summary>
        /// <remarks>this is the used by ajax to determine the correct portalId</remarks>
        protected void WritePortalIdCookie()
        {
            WriteEncrytpedCookie<int>(PORTAL_COOKIE_NAME, "PortalId", this.PortalId);
        }

        /// <summary>
        /// Write an encrypted cookie to the users machine
        /// </summary>
        /// <remarks>this cookie is only persisted for the length of the session</remarks>
        /// <typeparam name="T">type of the value we are storing in the cookie</typeparam>
        /// <param name="cookieName">name of the cookie</param>
        /// <param name="valueName">name of the key containing the data within the cookie</param>
        /// <param name="value">value of the cookie to store</param>
        protected void WriteEncrytpedCookie<T>(string cookieName, string valueName, T value)
        {
            WriteEncrytpedCookie<T>(cookieName, valueName, 0, value);
        }


        /// <summary>
        /// Write an encrypted cookie to the users machine
        /// </summary>
        /// <remarks>this cookie is persisted for the number of days in the param expiryInDays</remarks>
        /// <typeparam name="T">type of the value we are storing in the cookie</typeparam>
        /// <param name="cookieName">name of the cookie</param>
        /// <param name="valueName">name of the key containing the data within the cookie</param>
        /// <param name="expiryInDays">Number of days before the cookie expires</param>
        /// <param name="value">value of the cookie to store</param>
        protected void WriteEncrytpedCookie<T>(string cookieName, string valueName, int expiryInDays, T value)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie == null)
            {
                cookie = new HttpCookie(cookieName);
            }

            string cookieValue;

            if (SymmetricHelper.CanSafelyEncrypt)
                cookieValue = SymmetricHelper.Encrypt(value.ToString());
            else
                cookieValue = PortalId.ToString();

            cookie[valueName] = cookieValue;
            if (expiryInDays > 0)
                cookie.Expires = DateTime.Now.AddDays(expiryInDays);

            HttpContext.Current.Response.Cookies.Add(cookie);
        }


        /// <summary>
        /// Reads an encrypted value from a cookie
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cookieName"></param>
        /// <param name="valueName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool ReadEncryptedCookie<T>(string cookieName, string valueName, out T value)
        {
            value = default(T);

            HttpCookie cartCookie = HttpContext.Current.Request.Cookies[cookieName];

            if (cartCookie != null)
            {
                string pid = cartCookie[valueName];
                if (SymmetricHelper.CanSafelyEncrypt &&
                    !string.IsNullOrEmpty(pid))
                {
                    try
                    {
                        pid = SymmetricHelper.Decrypt(pid);
                        System.ComponentModel.TypeConverter tc = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
                        try
                        {
                            value = (T)tc.ConvertFrom(pid);
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    catch (Exception)
                    {
                        //the portal cookie is correct, rethrow
                        return false;
                    }
                }
            }

            return false;
        }
    }
}