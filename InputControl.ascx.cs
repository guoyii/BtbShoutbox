using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using BiteTheBullet.BtbShoutbox.Components;

namespace BiteTheBullet.BtbShoutbox
{
    public partial class InputControl : ShoutboxBaseModule
    {
        bool onlyAllowRegistered;
        string signedInUsersOnly = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (OnlyAllowRegistered && UserInfo.UserID == -1)
            {
                pnlOpenUser.Visible = false;

                signedInUsersOnly = Localization.GetString("SignInToUse", this.LocalResourceFile);
                shoutPostButton.Disabled = true;
            }
            else
            {
                shoutPostButton.Disabled = false;
                pnlOpenUser.Visible = UserInfo.UserID ==  -1;
            }

            imgWarning.Src = Page.ResolveUrl(string.Format("~/DesktopModules/BtbShoutbox/{0}", imgWarning.Src));
            txtUsername.Value = UserDisplayName;
            txtMessage.InnerText = SignedInUsersOnly;

            divRefresh.Visible = !HideRefreshButton;
        }

        public bool HideRefreshButton
        {
            get;
            set;
        }

        public bool OnlyAllowRegistered
        {
            get { return onlyAllowRegistered; }
            set { onlyAllowRegistered = value; }
        }

        /// <summary>
        /// gets the display name of the logged in user or 
        /// anonymous
        /// </summary>
        public string UserDisplayName
        {
            get
            {
                if (UserInfo.UserID != -1)
                {
                    return UserInfo.DisplayName;
                }
                else
                    return Localization.GetString("AnonymousCaption", this.LocalResourceFile);
            }
        }

       
        public string SignedInUsersOnly
        {
            get { return signedInUsersOnly; }
        }

    }
}