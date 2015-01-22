using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;

using BiteTheBullet.BtbShoutbox.Components;

namespace BiteTheBullet.BtbShoutbox
{
    public partial class Settings : ModuleSettingsBase
    {

        /// <summary>
        /// handles the loading of the module setting for this
        /// control
        /// </summary>
        public override void LoadSettings()
        {
            try
            {
                if (!IsPostBack)
                {
                    BtbShoutboxSettings settings = new BtbShoutboxSettings(this.TabModuleId);
                    chkFilter.Checked = settings.FilterProfanity;
                    txtItemLimit.Text = settings.LimitItemsCount.ToString();
                    txtTemplate.Text = settings.Template;
                    chkAutoRefresh.Checked = settings.AutoRefresh;
                    chkOnlyRegistered.Checked = settings.OnlyAllowRegisteredMembers;
                    txtEmailAdmin.Text = settings.AdminEmail;
                    txtCharacterLimit.Text = settings.ShoutLengthLimit.ToString();
                    chkTopOfModule.Checked = settings.InputControlAtTop;
                    chkDisplayOlderLink.Checked = settings.DisplayOlderPostLink;
                    txtPurgeAgeLimit.Text = settings.PurgeOldShouts.ToString();
                    chkFloodControlEnabled.Checked = settings.IsFloodControlEnabled;

                    rbUsername.Checked = settings.UserNameCaption == BtbShoutboxController.DisplayUserProperty.Username;
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        /// <summary>
        /// handles updating the module settings for this control
        /// </summary>
        public override void UpdateSettings()
        {
            try
            {
                int limit = 25;
                int purgeAgeLimit = -1;

                Int32.TryParse(txtItemLimit.Text, out limit);
                Int32.TryParse(txtPurgeAgeLimit.Text, out purgeAgeLimit);

                int charLimit = 1000;
                Int32.TryParse(txtCharacterLimit.Text, out charLimit);

                BtbShoutboxSettings settings = new BtbShoutboxSettings(this.TabModuleId);

                settings.FilterProfanity = chkFilter.Checked;
                settings.LimitItemsCount = limit;
                settings.Template = txtTemplate.Text;
                settings.OnlyAllowRegisteredMembers = chkOnlyRegistered.Checked;
                settings.AutoRefresh = chkAutoRefresh.Checked;
                settings.AdminEmail = txtEmailAdmin.Text;
                settings.ShoutLengthLimit = charLimit;
                settings.InputControlAtTop = chkTopOfModule.Checked;
                settings.UserNameCaption = rbDisplayName.Checked ? BtbShoutboxController.DisplayUserProperty.DisplayName :
                                                                    BtbShoutboxController.DisplayUserProperty.Username;
                settings.DisplayOlderPostLink = chkDisplayOlderLink.Checked;
                settings.PurgeOldShouts = purgeAgeLimit;
                settings.IsFloodControlEnabled = chkFloodControlEnabled.Checked;

                BtbShoutboxInfo.outputTemplate = null;
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        protected void lnkReloadDefault_Click(object sender, EventArgs e)
        {
            //reload the default template from the resource file
            BtbShoutboxSettings settings = new BtbShoutboxSettings(this.TabModuleId);
            settings.Template = string.Empty;
            txtTemplate.Text = settings.Template;

            BtbShoutboxInfo.outputTemplate = null;
            
        }
    }
}