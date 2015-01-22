using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiteTheBullet.BtbShoutbox.Components.FloodControl
{
    public abstract class FloodControl
    {
        const string FLOOD_LIST_CACHE_KEY = "BtbShoutBoxFloodList";

        IDictionary<string, IList<Action>> floodList = null;

        public FloodControl()
        {
        }


        protected IDictionary<string, IList<Action>> FloodList
        {
            get
            {
                if (floodList == null)
                {
                    object o = HttpContext.Current.Cache[FLOOD_LIST_CACHE_KEY];
                    if (o != null)
                        floodList = o as IDictionary<string, IList<Action>>;
                    else
                        floodList = new Dictionary<string, IList<Action>>();
                }

                return floodList;
            }

            set
            {
                HttpContext.Current.Cache.Add(FLOOD_LIST_CACHE_KEY,
                                              value,
                                              null,
                                              DateTime.Now.AddHours(1),
                                              System.Web.Caching.Cache.NoSlidingExpiration,
                                              System.Web.Caching.CacheItemPriority.Normal, 
                                              null);

                floodList = null;
            }
        }


        /// <summary>
        /// records an new event for a give ip
        /// </summary>
        /// <param name="a"></param>
        protected void RecordEvent(Action a)
        {
            var fl = FloodList;

            if(fl.ContainsKey(a.Ip))
            {
                var current = fl[a.Ip];
                current.Add(a);

                fl[a.Ip] = current;
            }
            else
            {
                IList<Action> actions = new List<Action>();
                actions.Add(a);
                fl.Add(a.Ip, actions);
            }

            FloodList = fl;
        }

        /// <summary>
        /// returns a list of all actions carried out by the ip of
        /// the user
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        protected IList<Action> UserActions(string ip)
        {
            if (FloodList.ContainsKey(ip))
                return FloodList[ip];
            else
                return new List<Action>();
        }

        /// <summary>
        /// method that the concrete class should implement to determine
        /// if we need to cancel the action requested by the user
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public abstract bool CancelAction(Action a);
    }
}