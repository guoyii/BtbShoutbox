using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.Exceptions;

namespace BiteTheBullet.BtbShoutbox.Components.Ajax.Actions
{
    public class ShoutMessage : AjaxAction
    {
        public override string ProcessAction(HttpRequest request)
        {
            BtbShoutboxInfo info = new BtbShoutboxInfo();

            info.ModuleId = this.ModuleId;
            info.Message = request["message"];
            info.UserName = request["username"];

            try
            {
                BtbShoutboxController controller = new BtbShoutboxController();

                if (ModuleSettings.OnlyAllowRegisteredMembers && CurrentUser().UserID == -1)
                {
                    return ProcessAction(request);
                }
                else if (CurrentUser() != null && CurrentUser().UserID != -1)
                {
                    if (ModuleSettings.UserNameCaption == BtbShoutboxController.DisplayUserProperty.DisplayName)
                        info.UserName = this.CurrentUser().DisplayName;
                    else
                        info.UserName = this.CurrentUser().Username;

                    info.Email = this.CurrentUser().Email;
                }

                if (ModuleSettings.FilterProfanity)
                {
                    //validate we are free from profanity in the post before
                    //commiting to the database
                    if (!controller.ValidateShoutForProfanity(info))
                    {
                        //halt
                        base.ajaxResult = false;
                        base.message = "Profanity is not allowed in the shout.";

                        return base.ProcessAction(request);
                    }
                }

                controller.AddBtbShoutbox(info, ModuleSettings.ShoutLengthLimit);

                //if we have an admin email sent the posted shout via
                //email
                if (ModuleSettings.SendAdminEmail)
                    info.SendShoutEmail(PortalId, info, ModuleSettings.AdminEmail);

                base.ajaxResult = true;
                return base.ProcessAction(request);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                base.ajaxResult = false;
                return base.ProcessAction(request);
            }
        }
    }
}