using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiteTheBullet.BtbShoutbox.Components.Ajax.Actions
{
    public class ListOldMessages : AjaxAction
    {
        public override string ProcessAction(HttpRequest request)
        {
            int lastItemId = -1;
            string itemId = request["lastItemId"];

            BtbShoutboxController controller = new BtbShoutboxController();

            if (itemId != null && int.TryParse(itemId, out lastItemId))
            {
                List<BtbShoutboxInfo> shouts = controller.GetBtbShoutboxArchive(this.ModuleId,
                                                                                lastItemId,
                                                                                ModuleSettings.LimitItemsCount);
                return AjaxShoutsResponse(ModuleSettings, shouts);
            }
            else
                return base.ProcessAction(request);

        }
    }
}