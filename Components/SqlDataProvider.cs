using System;
using System.Data;
using DotNetNuke.Framework.Providers;
using Microsoft.ApplicationBlocks.Data;

namespace BiteTheBullet.BtbShoutbox.Components
{
    public class SqlDataProvider : DataProvider
    {


        #region vars

        private const string providerType = "data";
        private const string moduleQualifier = "BTB_";

        private ProviderConfiguration providerConfiguration = ProviderConfiguration.GetProviderConfiguration(providerType);
        private string connectionString;
        private string providerPath;
        private string objectQualifier;
        private string databaseOwner;

        #endregion

        #region cstor

        /// <summary>
        /// cstor used to create the sqlProvider with required parameters from the configuration
        /// section of web.config file
        /// </summary>
        public SqlDataProvider()
        {
            Provider provider = (Provider)providerConfiguration.Providers[providerConfiguration.DefaultProvider];
            connectionString = DotNetNuke.Common.Utilities.Config.GetConnectionString();

            if (connectionString == string.Empty)
                connectionString = provider.Attributes["connectionString"];

            providerPath = provider.Attributes["providerPath"];

            objectQualifier = provider.Attributes["objectQualifier"];
            if (objectQualifier != string.Empty && !objectQualifier.EndsWith("_"))
                objectQualifier += "_";

            databaseOwner = provider.Attributes["databaseOwner"];
            if (databaseOwner != string.Empty && !databaseOwner.EndsWith("."))
                databaseOwner += ".";
        }

        #endregion

        #region properties

        public string ConnectionString
        {
            get { return connectionString; }
        }


        public string ProviderPath
        {
            get { return providerPath; }
        }

        public string ObjectQualifier
        {
            get { return objectQualifier; }
        }


        public string DatabaseOwner
        {
            get { return databaseOwner; }
        }

        #endregion

        #region private methods

        private string GetFullyQualifiedName(string name)
        {
            return DatabaseOwner + ObjectQualifier + moduleQualifier + name;
        }

        private object GetNull(object field)
        {
            return DotNetNuke.Common.Utilities.Null.GetNull(field, DBNull.Value);
        }

        #endregion

        #region override methods

        public override IDataReader GetBtbShoutboxs(int moduleId, int itemCountLimit)
        {
            return (IDataReader)SqlHelper.ExecuteReader(connectionString, GetFullyQualifiedName("GetBtbShoutboxs"), moduleId, itemCountLimit);
        }

        public override IDataReader GetBtbShoutBoxArchive(int moduleId, int lastItemId, int limit)
        {
            return (IDataReader)SqlHelper.ExecuteReader(connectionString, GetFullyQualifiedName("GetBtbArchivedShoutboxes"), moduleId, lastItemId, limit);
        }

        public override IDataReader GetBtbShoutBoxUpdates(int moduleId, int itemId)
        {
            return (IDataReader)SqlHelper.ExecuteReader(connectionString, GetFullyQualifiedName("GetBtbShoutboxsUpdates"), moduleId, itemId);
        }

        public override IDataReader GetBtbShoutbox(int moduleId, int itemId)
        {
            return (IDataReader)SqlHelper.ExecuteReader(connectionString, GetFullyQualifiedName("GetBtbShoutbox"), moduleId, itemId);
        }

        public override void AddBtbShoutbox(int moduleId, string message, string username, string email, int? replyToId)
        {
            SqlHelper.ExecuteNonQuery(connectionString, GetFullyQualifiedName("AddBtbShoutbox"), moduleId, message, username, email, replyToId);
        }

        public override void UpdateBtbShoutbox(int moduleId, int itemId, string message, string username, string email)
        {
            SqlHelper.ExecuteNonQuery(connectionString, GetFullyQualifiedName("UpdateBtbShoutbox"), moduleId, itemId, message, username, email);
        }

        public override void DeleteBtbShoutbox(int moduleId, int itemId)
        {
            SqlHelper.ExecuteNonQuery(connectionString, GetFullyQualifiedName("DeleteBtbShoutbox"), moduleId, itemId);
        }

        public override int VoteShoutUp(int itemId)
        {
            object result = SqlHelper.ExecuteScalar(connectionString, GetFullyQualifiedName("ShoutBox_VoteUp"), itemId);
            return int.Parse(result.ToString());
        }

        public override int VoteShoutDown(int itemId)
        {
            object result = SqlHelper.ExecuteScalar(connectionString, GetFullyQualifiedName("ShoutBox_VoteDown"), itemId);
            return int.Parse(result.ToString());
        }

        public override void DeleteOldBtbShoutbox(int moduleId, int shoutAgeLimit)
        {
            SqlHelper.ExecuteNonQuery(connectionString, GetFullyQualifiedName("ShoutBox_PurgeOldShouts"), moduleId, shoutAgeLimit);
        } 


        #endregion
    }
}
