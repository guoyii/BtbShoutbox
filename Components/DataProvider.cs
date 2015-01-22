using System;
using System.Data;
using DotNetNuke;
using DotNetNuke.Framework;

namespace BiteTheBullet.BtbShoutbox.Components
{
    public abstract class DataProvider
    {

        #region common methods

        /// <summary>
        /// var that is returned in the this singleton
        /// pattern
        /// </summary>
        private static DataProvider instance = null;

        /// <summary>
        /// private static cstor that is used to init an
        /// instance of this class as a singleton
        /// </summary>
        static DataProvider()
        {
            instance = (DataProvider)Reflection.CreateObject("data", "BiteTheBullet.BtbShoutbox.Components", "");
        }

        /// <summary>
        /// Exposes the singleton object used to access the database with
        /// the conrete dataprovider
        /// </summary>
        /// <returns></returns>
        public static DataProvider Instance()
        {
            return instance;
        }

        #endregion


        #region Abstract methods

        /* implement the methods that the dataprovider should */

        public abstract IDataReader GetBtbShoutboxs(int moduleId, int itemCountLimit);
        public abstract IDataReader GetBtbShoutBoxArchive(int moduleId, int lastItemId, int limit);
        public abstract IDataReader GetBtbShoutBoxUpdates(int moduleId, int itemId);
        public abstract IDataReader GetBtbShoutbox(int moduleId, int itemId);
        public abstract void AddBtbShoutbox(int moduleId, string message, string username, string email, int? replyToId);
        public abstract void UpdateBtbShoutbox(int moduleId, int itemId, string message, string username, string email);
        public abstract void DeleteBtbShoutbox(int moduleId, int itemId);
        public abstract int VoteShoutUp(int itemId);
        public abstract int VoteShoutDown(int itemId);
        public abstract void DeleteOldBtbShoutbox(int moduleId, int shoutAgeLimit);

        #endregion

    }



}
