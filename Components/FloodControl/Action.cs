using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiteTheBullet.BtbShoutbox.Components.FloodControl
{
    /// <summary>
    /// class to hold the data about the action we are monitoring for
    /// flood control
    /// </summary>
    public class Action
    {
        public Action()
        {
            Time = DateTime.Now;
        }

        public int ItemId { get; set; }

        public string Ip { get; set; }

        public string ActionType { get; set; }

        public string Parameter { get; set; }

        public DateTime Time { get; set; }
    }
}