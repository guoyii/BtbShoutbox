using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using DotNetNuke;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Search;
using System.Web;
using DotNetNuke.Common;

namespace BiteTheBullet.BtbShoutbox.Components
{
    public class BtbShoutboxController : IPortable
    {
        /// <summary>
        /// defines which of the userinfo properties will be
        /// used to display the user name for users which
        /// are authenticated
        /// </summary>
        public enum DisplayUserProperty
        {
            /// <summary>
            /// display name is used as a caption
            /// </summary>
            DisplayName,
            /// <summary>
            /// username is used for the caption
            /// </summary>
            Username
        }

        #region public method

        /// <summary>
        /// read only access to the shared resources file
        /// </summary>
        public static string SharedResourceFile
        {
            get { return Globals.ResolveUrl("~/DesktopModules/BtbShoutbox/app_localresources/sharedresources.resx"); }
        }

        /// <summary>
        /// Gets all the BtbShoutboxInfo objects for items matching the this moduleId
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public List<BtbShoutboxInfo> GetBtbShoutboxs(int moduleId, int itemCountLimit)
        {
            return CBO.FillCollection<BtbShoutboxInfo>(DataProvider.Instance().GetBtbShoutboxs(moduleId, itemCountLimit));
        }

        /// <summary>
        /// Gets a list of all new BtbShoutboxInfo objects after a give item Id
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="lastItemId"></param>
        /// <returns></returns>
        public List<BtbShoutboxInfo> GetBtbShoutboxUpdates(int moduleId, int lastItemId)
        {
            return CBO.FillCollection<BtbShoutboxInfo>(DataProvider.Instance().GetBtbShoutBoxUpdates(moduleId, lastItemId));
        }

        /// <summary>
        /// Returns a list of older posts which are after the given item Id
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="lastItemId"></param>
        /// <param name="itemLimit"></param>
        /// <returns></returns>
        public List<BtbShoutboxInfo> GetBtbShoutboxArchive(int moduleId, int lastItemId, int itemLimit)
        {
            return CBO.FillCollection<BtbShoutboxInfo>(DataProvider.Instance().GetBtbShoutBoxArchive(moduleId,
                                                                                                     lastItemId,
                                                                                                     itemLimit));
        }


        /// <summary>
        /// Get an info object from the database
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public BtbShoutboxInfo GetBtbShoutbox(int moduleId, int itemId)
        {
            return (BtbShoutboxInfo)CBO.FillObject(DataProvider.Instance().GetBtbShoutbox(moduleId, itemId), typeof(BtbShoutboxInfo));
        }


        /// <summary>
        /// Adds a new BtbShoutboxInfo object into the database
        /// </summary>
        /// <param name="info"></param>
        /// <param name="shoutLengthLimit">The maximum length the shout post can be
        /// in characters</param>
        public void AddBtbShoutbox(BtbShoutboxInfo info, int shoutLengthLimit)
        {
            //check we have some content to store
            if (info.Message != string.Empty)
            {
                DataProvider.Instance().AddBtbShoutbox(info.ModuleId,
                                                       info.MessageTruncate(shoutLengthLimit), 
                                                       info.UserName,
                                                       info.Email,
                                                       info.ReplyToId);
            }
        }

        /// <summary>
        /// update a info object already stored in the database
        /// </summary>
        /// <param name="info"></param>
        /// <param name="shoutLengthLimit">The maximum length the shout post can be
        /// in characters</param>
        public void UpdateBtbShoutbox(BtbShoutboxInfo info, int shoutLengthLimit)
        {
            //check we have some content to update
            if (info.Message != string.Empty && info.UserName != string.Empty)
            {
                DataProvider.Instance().UpdateBtbShoutbox(info.ModuleId, 
                                                          info.ItemId, 
                                                          info.MessageTruncate(shoutLengthLimit), 
                                                          info.UserName,
                                                          info.Email);
            }
        }


        /// <summary>
        /// deletes posts older in age then the parameter from the database
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="shoutAgeLimitInDays"></param>
        public void DeleteOldShouts(int moduleId, int shoutAgeLimitInDays)
        {
            DataProvider.Instance().DeleteOldBtbShoutbox(moduleId, shoutAgeLimitInDays);
        }

        /// <summary>
        /// Delete a given item from the database
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="itemId"></param>
        public void DeleteBtbShoutbox(int moduleId, int itemId)
        {
            DataProvider.Instance().DeleteBtbShoutbox(moduleId, itemId);
        }

        /// <summary>
        /// validate the shout does not contain profanity
        /// </summary>
        /// <param name="info"></param>
        /// <returns>true if the item is clean otherwise false</returns>
        public bool ValidateShoutForProfanity(BtbShoutboxInfo info)
        {
            string baseFolder = HttpContext.Current.Server.MapPath("~/DesktopModules/BtbShoutbox");

            return ValidateShoutForProfanity(baseFolder, info);
        }

        /// <summary>
        /// validate the shout does not contain profanity
        /// </summary>
        /// <param name="moduleBaseFolder">Defines the location of the module with the physical
        /// drive. Needed to load the profanity list from disk</param>
        /// <param name="info"></param>
        /// <returns>true if the item is clean otherwise false</returns>
        public bool ValidateShoutForProfanity(string moduleBaseFolder, BtbShoutboxInfo info)
        {
            string filename = "profanity-list.txt";
            string profanity;
            try
            {
                using (StreamReader reader = File.OpenText(Path.Combine(moduleBaseFolder, filename)))
                {
                    profanity = reader.ReadToEnd();
                }

                string[] profanityArray = profanity.Split(',');
                
                string[] message = NormaliseString(info.Message);
                string[] username = NormaliseString(info.UserName);


                //check message for profanity
                foreach (string m in message)
                {
                    foreach (string profanityWord in profanityArray)
                    {
                        if (profanityWord.Equals(m))
                            return false;
                    }
                }

                //check username for profanity
                foreach (string m in username)
                {
                    foreach (string profanityWord in profanityArray)
                    {
                        if (profanityWord.Equals(m))
                            return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return false;
            }


        }

        /// <summary>
        /// Records a up vote for for the item
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns>current vote count of the item</returns>
        public int VoteShoutUp(int itemId)
        {
            return DataProvider.Instance().VoteShoutUp(itemId);
        }

        /// <summary>
        /// Records a down vote for the item 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns>current vote count for the item</returns>
        public int VoteShoutDown(int itemId)
        {
            return DataProvider.Instance().VoteShoutDown(itemId);
        }

        /// <summary>
        /// tokenises a string that can be checked for profanity
        /// removes punctuation etc
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private static string[] NormaliseString(string input)
        {
            StringBuilder buffer = new StringBuilder();

            char[] messageArray = input.ToCharArray();
            foreach (char c in messageArray)
            {
                if (!Char.IsPunctuation(c))
                    buffer.Append(Char.ToLower(c));
            }

            return buffer.ToString().Split(' ');
        }

        #endregion

        #region IPortable Members

        /// <summary>
        /// Exports a module to xml
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <returns></returns>
        public string ExportModule(int moduleID)
        {
            StringBuilder sb = new StringBuilder();

            List<BtbShoutboxInfo> infos = GetBtbShoutboxs(moduleID, 1000);

            if (infos.Count > 0)
            {
                sb.Append("<BtbShoutboxs>");
                foreach (BtbShoutboxInfo info in infos)
                {
                    sb.Append("<BtbShoutbox>");
                    sb.Append("<message>");
                    sb.Append(XmlUtils.XMLEncode(info.Message));
                    sb.Append("</message>");
                    sb.AppendFormat("<username>{0}</username>", XmlUtils.XMLEncode(info.UserName));
                    sb.Append("</BtbShoutbox>");
                }
                sb.Append("</BtbShoutboxs>");
            }

            return sb.ToString();
        }

        /// <summary>
        /// imports a module from an xml file
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <param name="Content"></param>
        /// <param name="Version"></param>
        /// <param name="UserID"></param>
        public void ImportModule(int ModuleID, string Content, string Version, int UserID)
        {
            XmlNode infos = DotNetNuke.Common.Globals.GetContent(Content, "BtbShoutboxs");

            foreach (XmlNode info in infos.SelectNodes("BtbShoutbox"))
            {
                BtbShoutboxInfo BtbShoutboxInfo = new BtbShoutboxInfo();
                BtbShoutboxInfo.ModuleId = ModuleID;
                BtbShoutboxInfo.Message = info.SelectSingleNode("message").InnerText;
                BtbShoutboxInfo.UserName = info.SelectSingleNode("username").InnerText;

                AddBtbShoutbox(BtbShoutboxInfo, 0);
            }
        }

        #endregion
    }
}
