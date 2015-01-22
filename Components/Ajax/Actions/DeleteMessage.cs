using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace BiteTheBullet.BtbShoutbox.Components.Ajax.Actions
{
    public class DeleteMessage : AjaxAction
    {
        public override string ProcessAction(HttpRequest request)
        {
            int itemId = -1;

            if (request["itemId"] != null && Int32.TryParse(request["itemId"], out itemId)
                    && CurrentUser().IsInRole("Administrators"))
            {
                BtbShoutboxController controller = new BtbShoutboxController();

                //return a shout that can be displayed at the bottom
                //of the list to replace the one we just deleted
                List<BtbShoutboxInfo> shouts = controller.GetBtbShoutboxs(this.ModuleId, ModuleSettings.LimitItemsCount + 1);

                controller.DeleteBtbShoutbox(this.ModuleId, itemId);

                StringBuilder buffer = new StringBuilder();

                buffer.Append("{\"result\": true ");

                if (shouts.Count == ModuleSettings.LimitItemsCount + 1)
                {
                    BtbShoutboxInfo shout = shouts[ModuleSettings.LimitItemsCount];

                    buffer.AppendFormat(",\"itemId\": {0}, \"message\": \"{1}\", \"username\":\"{2}\",\"html\":\"{3}\"",
                        shout.ItemId,
                        shout.Message,
                        shout.UserName,
                        JsonEncode(shout.CreateShoutHtml(ModuleSettings, CurrentUser())));
                }

                buffer.Append("}");

                return buffer.ToString();
            }
            else
            {
                base.ajaxResult = false;
                return base.ProcessAction(request);
            }
        }

    }
}