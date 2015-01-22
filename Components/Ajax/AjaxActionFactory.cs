
using System;
using System.Data;
using System.Web;
using BiteTheBullet.BtbShoutbox.Components.Ajax.Actions;


namespace BiteTheBullet.BtbShoutbox.Components.Ajax
{
    /// <summary>
    /// factory pattern used to return the correct ajax action that should be executed
    /// based on the request from the client
    /// </summary>
    public class AjaxActionFactory
    {

        /// <summary>
        /// Creates the ajax action based on the request from the client
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks>the request should contain a parameter with name "action", this
        /// is then used to create the correct ajax action to process the request</remarks>
        public static AjaxAction CreateAction(HttpRequest request)
        {
            string action = request["functionCall"];

            if (action == null)
            {
                return null;
            }

            switch (action)
            {
                case "shoutMessage":
                    return new ShoutMessage();

                case "listMessages":
                    return new ListMessages();

                case "deleteMessage":
                    return new DeleteMessage();

                case "listOldMessages":
                    return new ListOldMessages();

                case "vote":
                    return new Votes();

                default:
                    return null;

            }
        }
    }
}
