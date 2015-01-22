using System;
using System.Security.Cryptography;
using System.Text;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using System.Reflection;
using System.Collections.Generic;
using System.Web.UI;
using System.Web;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using DotNetNuke.Services.Exceptions;

namespace BiteTheBullet.BtbShoutbox.Components
{
    public class BtbShoutboxInfo
    {
        //private vars exposed thro the
        //properties
        private int moduleId;
        private int itemId;
        private string message;
        private string username = null;
        private DateTime createdDate;
        private string email;

        /// <summary>
        /// holds a reference to the template used to 
        /// render the shout to html
        /// </summary>
        internal static string outputTemplate = null;

        protected string SharedResource
        {
            get { return Globals.ResolveUrl("~/DesktopModules/BtbShoutbox/App_LocalResources/SharedResource.resx"); }
        }

        /// <summary>
        /// empty cstor
        /// </summary>
        public BtbShoutboxInfo()
        {
        }


        #region properties

        public int ModuleId
        {
            get { return moduleId; }
            set { moduleId = value; }
        }

        public int ItemId
        {
            get { return itemId; }
            set { itemId = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public DateTime CreatedDate
        {
            get { return createdDate; }
            set { createdDate = value; }
        }

        public string UserName
        {
            get { return username; }
            set 
            {
                if (string.IsNullOrEmpty(value))
                    username = "Anonymous";
                else
                    username = value; 
            }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        protected string OutputTemplate
        {
            get 
            {
                object o = HttpContext.Current.Items["btbShoutBoxTemplate_" + moduleId];

                if (o != null)
                    return o.ToString();
                else
                    return null;
            }
            set { HttpContext.Current.Items.Add("btbShoutBoxTemplate_" + moduleId, value); }
        }

        public int Vote { get; set; }

        public int ReplyToId { get; set; }

        /// <summary>
        /// read only property to get the age of the shout in units
        /// like hours, min, days, months depending on the "freshness"
        /// of the item
        /// </summary>
        public string ShoutAge
        {
            get 
            {
                TimeSpan age = DateTime.Now - CreatedDate;
                if (age.TotalSeconds < 120)
                {
                    return Localization.GetString("MomentsAgo.Text", SharedResource);
                }
                else if (age.TotalMinutes < 60)
                {
                    return string.Format(Localization.GetString("MinutesAgo.Text", SharedResource), 
                                            Math.Round(age.TotalMinutes,0),
                                            Math.Round(age.TotalMinutes, 0) > 1 ? "s" : "");
                }
                else if (age.TotalHours < 24)
                {
                    return string.Format(Localization.GetString("HoursAgo.Text", SharedResource),
                                            Math.Round(age.TotalHours, 0),
                                            Math.Round(age.TotalHours, 0) > 1 ? "s" : "");
                }
                else if (age.TotalDays < 365)
                {
                    return string.Format(Localization.GetString("DaysAgo.Text", SharedResource), 
                                            Math.Round(age.TotalDays, 0),
                                            Math.Round(age.TotalDays, 0) > 1 ? "s" : "");
                }
                else
                {
                    return string.Format(Localization.GetString("YearsAgo.Text", SharedResource), 
                                            Math.Round(age.TotalDays / 365, 0),
                                            Math.Round(age.TotalDays / 365, 0) > 1 ? "s" : "");
                }
            }
        }

        /// <summary>
        /// readonly property which renders the voting system in html on the
        /// output. This is used in the template
        /// </summary>
        public string Votes
        {
            get 
            {
                StringBuilder buffer = new StringBuilder();

                buffer.Append("<span class=\"BtbShoutVoteSpan\">");
                buffer.AppendFormat("{1} <a href=\"#\" class=\"hypBtbShoutVoteUp\"><img src=\"{0}\" alt=\"\"/></a>",
                                        Globals.ResolveUrl("~/DesktopModules/BtbShoutbox/img/up_outline.png"),
                                        Localization.GetString("ClickToVote.Text", BtbShoutboxController.SharedResourceFile));

                buffer.AppendFormat("<a href=\"#\" class=\"hypBtbShoutVoteDown\"><img src=\"{0}\" alt=\"\"/></a>",
                                        Globals.ResolveUrl("~/DesktopModules/BtbShoutbox/img/down_outline.png"));

                buffer.AppendFormat("<span class=\"BtbShoutResultCaption\">{0}</span><span class=\"BtbShoutResult\">",
                                        Localization.GetString("Rating.Text", BtbShoutboxController.SharedResourceFile));

                if (Vote == 0)
                {
                    buffer.Append("(0)");
                }
                else if (Vote > 0)
                {
                    buffer.AppendFormat("<img src=\"{1}\" alt=\"\"/>{0}", 
                                        Vote,
                                        Globals.ResolveUrl("~/DesktopModules/BtbShoutbox/img/up_solid.png"));
                }
                else
                {
                    buffer.AppendFormat("<img src=\"{1}\" alt=\"\"/>{0}", 
                                        Vote,
                                        Globals.ResolveUrl("~/DesktopModules/BtbShoutbox/img/down_solid.png"));
                }
                buffer.Append("</span>");
                buffer.Append("</span>");

                return buffer.ToString();
            }
        }

        /// <summary>
        /// read only access to the gravatar 
        /// </summary>
        public string Gravatar
        {
            get
            {
                string defaultParameter = "";
                string emailHash;
                string gravatarKey;

                if (string.IsNullOrEmpty(Email))
                {
                    gravatarKey = UserName;
                    defaultParameter = "identicon";
                }
                else
                {
                    gravatarKey = Email;
                }

                MD5 md5 = MD5CryptoServiceProvider.Create();
                byte[] output = md5.ComputeHash(ASCIIEncoding.UTF8.GetBytes(gravatarKey));
                StringBuilder buffer = new StringBuilder();
                foreach (byte b in output)
                {
                    buffer.AppendFormat("{0:x2}", b);
                }
                emailHash = buffer.ToString();

                return string.Format("<img src=\"http://www.gravatar.com/avatar/{0}?d={1}&s=20\" alt=\"Gravatar\" />", emailHash, defaultParameter);
            }
        }

        /// <summary>
        /// Returns a truncated version of the message limited to the next whole word
        /// of the size parameter
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public string MessageTruncate(int size)
        {
            if (Message.Length < size | size == 0)
                return Message;
            else
            {
                int position = Message.IndexOf(' ', size);
                if (position > -1)
                {
                    return Message.Substring(0, position);
                }
                else
                    return Message;
            }
        }


        /// <summary>
        /// renders the shout into the html template
        /// </summary>
        /// <param name="tabModuleId"></param>
        /// <param name="shout"></param>
        /// <returns></returns>
        public string CreateShoutHtml(BtbShoutboxSettings settings, UserInfo currentUser)
        {
            string contentOutput;

            //lists the props which are not escaped
            //as html
            IList<string> dontEscapeProperty = new List<string>();
            dontEscapeProperty.Add("GRAVATAR");
            dontEscapeProperty.Add("VOTES");

            if (OutputTemplate == null)
            {
                //load template
                OutputTemplate = settings.Template;
            }

            if (!string.IsNullOrEmpty(OutputTemplate))
            {
                string template = OutputTemplate;
                IDictionary<string, PropertyInfo> propInfos = CBO.GetProperties<BtbShoutboxInfo>();

                foreach (PropertyInfo propInfo in propInfos.Values)
                {
                    object propertyValue = DataBinder.Eval(this, propInfo.Name);
                    if (propertyValue != null)
                    {
                        //check if the property is to not be html escaped
                        if (dontEscapeProperty.Contains(propInfo.Name.ToUpper()))
                        {
                            template = template.Replace("[" + propInfo.Name.ToUpper() + "]",
                                                            propertyValue.ToString());
                        }
                        else
                        {
                            if (propInfo.Name.ToUpper() == "MESSAGE")
                            {
                                template = template.Replace("[" + propInfo.Name.ToUpper() + "]",
                                                HttpContext.Current.Server.HtmlEncode(propertyValue.ToString()).Replace("\n", "<br/>"));
                            }
                            else
                            {
                                //html encode the data to the template
                                template = template.Replace("[" + propInfo.Name.ToUpper() + "]",
                                                HttpContext.Current.Server.HtmlEncode(propertyValue.ToString()));
                            }                            
                        }
                    }
                }

                template = template.Replace("\r", "");
                template = template.Replace("\n", "");

                contentOutput = template;
            }
            else
            {
                //no template defined
                contentOutput = HttpContext.Current.Server.HtmlEncode(this.Message);
            }

            contentOutput = contentOutput + String.Format("<input type=\"hidden\" id=\"shout{0}\" value=\"{0}\"/>",
                                                         this.ItemId);

            //if the user is admin display the delete button option
            if (currentUser != null && currentUser.IsInRole("Administrators"))
            {
                contentOutput = contentOutput + string.Format("<div><a href=\"#\" class=\"deleteShout\">{0}</a></div><div style=\"clear:both;\"></div>",
                                                                Localization.GetString("DeleteLink.Text", BtbShoutboxController.SharedResourceFile));
            }

            return contentOutput;
        }


        public void SendShoutEmail(int portalId, BtbShoutboxInfo info, string emailTo)
        {
            bool smtpEnableSsl = false;
            string smtpAuth = string.Empty;
            string smtpUsername = string.Empty;
            string smtpPassword = string.Empty;
            string smtpServer = string.Empty;


            smtpAuth = DotNetNuke.Common.Globals.HostSettings["SMTPAuthentication"].ToString();
            smtpUsername = DotNetNuke.Common.Globals.HostSettings["SMTPUsername"].ToString();
            smtpPassword = DotNetNuke.Common.Globals.HostSettings["SMTPPassword"].ToString();
            smtpServer = DotNetNuke.Common.Globals.HostSettings["SMTPServer"].ToString();
            smtpEnableSsl = DotNetNuke.Common.Globals.HostSettings["SMTPEnableSSL"].ToString() == "Y";

            string body = string.Format("username:{0}\r\nemail:{1}\r\nshouted:\"{2}\"",
                                        info.UserName,
                                        info.Email,
                                        info.Message);

            PortalSettings portalSettings = new PortalSettings(portalId);

            try
            {

                Mail.SendMail(portalSettings.Email,
                              emailTo,
                              "",
                              "",
                              "",
                              MailPriority.Normal,
                              "Shout Post",
                              MailFormat.Text,
                              System.Text.Encoding.UTF8,
                              body,
                              new string[0],
                              smtpServer,
                              smtpAuth,
                              smtpUsername,
                              smtpPassword,
                              smtpEnableSsl);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
        }

        #endregion
    }
}
