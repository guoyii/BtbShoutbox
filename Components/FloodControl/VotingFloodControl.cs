using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiteTheBullet.BtbShoutbox.Components.FloodControl
{
    public class VotingFloodControl : FloodControl
    {
        /// <summary>
        /// the time limit in minutes before the restricted action can be carried out
        /// by the same ip on the same even
        /// </summary>
        int timeLimitInMinutes = 5;

        /// <summary>
        /// determines if the current action being requested should be cancelled
        /// since the request is too soon after an already existing action
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public override bool CancelAction(Action a)
        {
            //get all the same actions for this IP
            IList<Action> usersActions = base.UserActions(a.Ip);

            //remove any actions which are older than the purge limit
            usersActions = usersActions
                                .Where(ua => (DateTime.Now - ua.Time).Minutes < timeLimitInMinutes)
                                .ToList();


            //work out if we are allowed to make the action
            //again in the same time frame
            bool cancelAction = usersActions
                                    .Where(ua => ua.ActionType == a.ActionType &&
                                                    ua.Ip == a.Ip &&
                                                    ua.ItemId == a.ItemId &&
                                                    ua.Parameter == a.Parameter)
                                    .Count() > 0;
            if (!cancelAction)
                base.RecordEvent(a);

            return cancelAction;
        }
    }
}