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
using DotNetNuke.Common;

namespace BiteTheBullet.BtbShoutbox.Components
{
    /// <summary>
    /// Provides strong typed access to settings used by module
    /// </summary>
    public class BtbShoutboxSettings
    {
        ModuleController controller;
        int tabModuleId;

        public BtbShoutboxSettings(int tabModuleId)
        {
            controller = new ModuleController();
            this.tabModuleId = tabModuleId;
        }

        protected T ReadSetting<T>(string settingName, T defaultValue)
        {
            Hashtable settings = controller.GetTabModuleSettings(this.tabModuleId);

            T ret = default(T);

            if (settings.ContainsKey(settingName))
            {
                System.ComponentModel.TypeConverter tc = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
                try
                {
                    ret = (T)tc.ConvertFrom(settings[settingName]);
                }
                catch
                {
                    ret = defaultValue;
                }
            }
            else
                ret = defaultValue;

            return ret;
        }

        protected void WriteSetting(string settingName, string value)
        {
            controller.UpdateTabModuleSetting(this.tabModuleId, settingName, value);
        }

        #region public properties

        /// <summary>
        /// get/set if the message input control is at the top
        /// of the module or the bottom.
        /// </summary>
        /// <remarks>if this is true then the control will be at the top of
        /// the page otherwise at the bottom.</remarks>
        public bool InputControlAtTop
        {
            get { return ReadSetting<Boolean>("inputControlAtTop", false); }
            set { WriteSetting("inputControlAtTop", value.ToString()); }
        }

        /// <summary>
        /// get/set if we should check the message for profanity before
        /// allowing it be posted
        /// </summary>
        public bool FilterProfanity
        {
            get { return ReadSetting<Boolean>("filterProfanity", true); }
            set { WriteSetting("filterProfanity", value.ToString()); }
        }

        /// <summary>
        /// defines the number of items to display, this will limit
        /// the shout messages to the latest x many shouts
        /// </summary>
        public int LimitItemsCount
        {
            get { return ReadSetting<int>("limitItemCount", 10); }
            set { WriteSetting("limitItemCount", value.ToString()); }
        }

        /// <summary>
        /// get/set the template used to display the shout message
        /// </summary>
        public string Template
        {
            get 
            {
                DesktopModuleController controller = new DesktopModuleController();
                string moduleFolder = controller.GetDesktopModuleByName("BtbShoutbox").FolderName;


                string resourceFile = Globals.ResolveUrl(string.Format("~/DesktopModules/{0}/App_LocalResources/Settings.ascx.resx", moduleFolder));
                string defaultTemplate = Localization.GetString("DefaultTemplate", resourceFile);

                string template = ReadSetting<string>("template", defaultTemplate);

                if (string.IsNullOrEmpty(template))
                    return defaultTemplate;
                else
                    return template;

            }
            set { WriteSetting("template", value); }
        }

        /// <summary>
        /// get/set if only registered members of the portal are allowed to shout
        /// messages
        /// </summary>
        public bool OnlyAllowRegisteredMembers
        {
            get { return ReadSetting<Boolean>("onlyAllowRegisteredMembers", true); }
            set { WriteSetting("onlyAllowRegisteredMembers", value.ToString()); }
        }

        /// <summary>
        /// get/set if the shout UI should poll the server automatically to check for new
        /// shout messages to display
        /// </summary>
        public bool AutoRefresh
        {
            get { return ReadSetting<Boolean>("autoRefresh", false); }
            set { WriteSetting("autoRefresh", value.ToString()); }
        }

        /// <summary>
        /// read only property to determine if the shout should
        /// be sent to the admin via an email
        /// </summary>
        public bool SendAdminEmail
        {
            get
            {
                return !string.IsNullOrEmpty(AdminEmail);
            }
        }

        /// <summary>
        /// get/set the Admin email to send the shout posts to
        /// </summary>
        public string AdminEmail
        {
            get { return ReadSetting<string>("adminEmail", null); }
            set { WriteSetting("adminEmail", value); }
        }

        /// <summary>
        /// defines the number of character that a shout can contain, anything over
        /// this value will be truncate to the nearest whole word.
        /// </summary>
        /// <remarks>The default is 1000 chars, to turn off truncating set this value to 0</remarks>
        public int ShoutLengthLimit
        {
            get { return ReadSetting<int>("shoutLengthLimit", 1000); }
            set { WriteSetting("shoutLengthLimit", value.ToString()); }
        }

        /// <summary>
        /// get/set the user property which is used to display the caption
        /// for authenticated users
        /// </summary>
        public BiteTheBullet.BtbShoutbox.Components.BtbShoutboxController.DisplayUserProperty UserNameCaption
        {
            get 
            {
                string setting = ReadSetting<string>("userNameCaption", "DisplayName");
                return (BiteTheBullet.BtbShoutbox.Components.BtbShoutboxController.DisplayUserProperty)Enum.Parse(typeof(BiteTheBullet.BtbShoutbox.Components.BtbShoutboxController.DisplayUserProperty), setting);
            }
            set { WriteSetting("userNameCaption", value.ToString()); }
        }

        /// <summary>
        /// get/set if we should display the link to allow them to get more
        /// older posts
        /// </summary>
        public bool DisplayOlderPostLink
        {
            get 
            {
                return ReadSetting<bool>("displayOlderPostLink", true);
            }
            set { WriteSetting("displayOlderPostLink", value.ToString()); }
        }

        /// <summary>
        /// get/set the ages of shouts in days which we should just delete
        /// from the database.
        /// </summary>
        /// <remarks>if this value if -1 then no shouts will be deleted</remarks>
        public int PurgeOldShouts
        {
            get { return ReadSetting<int>("purgeOldShouts", -1); }
            set { WriteSetting("purgeOldShouts", value.ToString()); }
        }

        /// <summary>
        /// get/set if the flood control feature is enabled, this stops a user from
        /// voting too many times on the same item in a row
        /// </summary>
        public bool IsFloodControlEnabled
        {
            get { return ReadSetting<Boolean>("isFloodControlEnabled", true); }
            set { WriteSetting("isFloodControlEnabled", value.ToString()); }
        }

        #endregion
    }
}
