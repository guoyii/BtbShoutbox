using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiteTheBullet.BtbShoutbox.Components.Ajax.Actions
{
    public class ListMessages : AjaxAction
    {
        public override string ProcessAction(HttpRequest request)
        {
            int lastItemId = -1;
            List<BtbShoutboxInfo> shouts;

            
            BtbShoutboxController controller = new BtbShoutboxController();

            if (request["lastItemId"] != null && Int32.TryParse(request["lastItemId"], out lastItemId))
            {
                shouts = controller.GetBtbShoutboxUpdates(this.ModuleId, lastItemId);
            }
            else
                shouts = controller.GetBtbShoutboxs(this.ModuleId, ModuleSettings.LimitItemsCount);

            return AjaxShoutsResponse(ModuleSettings, shouts);
        }
    }
}