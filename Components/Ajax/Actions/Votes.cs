using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Common;
using BiteTheBullet.BtbShoutbox.Components.FloodControl;

namespace BiteTheBullet.BtbShoutbox.Components.Ajax.Actions
{
    public class Votes : AjaxAction
    {
        public override string ProcessAction(HttpRequest request)
        {
            int itemId = -1;
            int direction = 1;
            int votes = 0;
            string voteImg = "";

            int.TryParse(request["itemId"], out itemId);
            int.TryParse(request["direction"], out direction);

            if (itemId > 0)
            {
                BtbShoutboxController controller = new BtbShoutboxController();
                
                //flood control, means we wont allow more than one vote in either
                //direction from the same IP on the given post
                if (ModuleSettings.IsFloodControlEnabled)
                {
                    VotingFloodControl floodControl = new VotingFloodControl();
                    bool cancel = floodControl.CancelAction(new FloodControl.Action() { 
                                        ItemId = itemId,
                                        Ip = request.UserHostAddress,
                                        ActionType = "VOTING",
                                        Parameter = direction.ToString()
                                    });

                    if (cancel)
                    {
                        base.ajaxResult = false;
                        base.message = "Please wait to later to vote again on this item";
                        return base.ProcessAction(request);
                    }
                }


                if (direction == 1)
                    votes = controller.VoteShoutUp(itemId);
                else
                    votes = controller.VoteShoutDown(itemId);


                //based on the result return img
                if (votes > 0)
                    voteImg = Globals.ResolveUrl("~/DesktopModules/BtbShoutbox/img/up_solid.png");
                else if (votes < 0)
                    voteImg = Globals.ResolveUrl("~/DesktopModules/BtbShoutbox/img/down_solid.png");

                base.ajaxResult = true;
            }
            else
                base.ajaxResult = false;


            return string.Format("{{\"result\":{0}, \"votes\":{1}, \"voteImg\":\"{2}\"}}",
                                    ajaxResult.ToString().ToLower(),
                                    votes,
                                    voteImg);
        }
    }
}