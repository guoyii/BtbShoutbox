using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;

using BiteTheBullet.BtbShoutbox.Components;
using DotNetNuke.Common;

namespace BiteTheBullet.BtbShoutbox
{
    public partial class ViewBtbShoutbox : ShoutboxBaseModule
    {
        BtbShoutboxSettings tabModuleSettings = null;

        protected void Page_Load(object sender, EventArgs e)
        {           

            try
            {
                jQuery.RequestRegistration();

                ResolveJsCssElements();

                btbShoutModuleId.Value = SymmetricHelper.Encrypt(ModuleId.ToString());
                btbShoutTabModuleId.Value = SymmetricHelper.Encrypt(TabModuleId.ToString());

                //write out the encrypted portalId
                WritePortalIdCookie();

                if (!IsPostBack)
                {
                    SetupUI(TabModuleSettings);

                    BtbShoutboxController controller = new BtbShoutboxController();
                    Repeater1.DataSource = controller.GetBtbShoutboxs(this.ModuleId, TabModuleSettings.LimitItemsCount);
                    Repeater1.DataBind();
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        private void ResolveJsCssElements()
        {
            string moduleFolder = "BtbShoutbox";

            StringBuilder buffer = new StringBuilder();
            buffer.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>",
                                Page.ResolveUrl(string.Format("~/DesktopModules/{0}/js/jquery.simplemodal-1.3.3.min.js", moduleFolder)));
            buffer.Append("\r\n");

#if !DEBUG
            buffer.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>",
                                Page.ResolveUrl(string.Format("~/DesktopModules/{0}/js/btbshoutbox.min.js", moduleFolder)));
#else
            buffer.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>",
                                Page.ResolveUrl(string.Format("~/DesktopModules/{0}/js/btbshoutbox.js", moduleFolder)));
#endif
            buffer.Append("\r\n");

            litScript.Text = buffer.ToString();

            string ieCssFix = "<link type=\"text/css\" href=\"{0}\" rel=\"stylesheet\" media=\"screen\" />";
            litCss.Text = string.Format(ieCssFix, Page.ResolveUrl(string.Format("~/DesktopModules/{0}/basic_ie.css", moduleFolder)));
        }

        private void SetupUI(BtbShoutboxSettings settings)
        {
            InputControl inputControl = Page.LoadControl("~/DesktopModules/BtbShoutbox/InputControl.ascx") as InputControl;
            inputControl.OnlyAllowRegistered = settings.OnlyAllowRegisteredMembers;
            inputControl.HideRefreshButton = settings.AutoRefresh;
            inputControl.ID = "InputControl.ascx";

            if (settings.InputControlAtTop)
            {
                phTopInput.Controls.Add(inputControl);
            }
            else
                phBottomInput.Controls.Add(inputControl);

            divDisplayOlder.Visible = settings.DisplayOlderPostLink;    
        
            //work out if we have enabled the purge for the older 
            //shouts, if so just check if we need to delete any
            //old ones.
            if (settings.PurgeOldShouts > 0)
            {
                BtbShoutboxController controller = new BtbShoutboxController();
                controller.DeleteOldShouts(this.ModuleId, settings.PurgeOldShouts);
            }


        }

        /// <summary>
        /// Read only property to determine if auto refresh is enabled
        /// for the module
        /// </summary>
        public bool EnableAutoRefresh
        {
            get
            {
                return TabModuleSettings.AutoRefresh;
            }
        }

        protected BtbShoutboxSettings TabModuleSettings
        {
            get
            {
                if (tabModuleSettings == null)
                {
                    tabModuleSettings = new BtbShoutboxSettings(this.TabModuleId);
                }

                return tabModuleSettings;
            }
        }

        
        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item |
                e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Literal content = (Literal)e.Item.FindControl("chatItem");
                BtbShoutboxInfo shout = (BtbShoutboxInfo)e.Item.DataItem;
                content.Text = shout.CreateShoutHtml(TabModuleSettings, UserInfo);
            }
        }

    }
}