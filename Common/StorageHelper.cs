using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net;
using System.Linq;

namespace Common.Configuration
{
    public class StorageHelper
    {
        private static CloudTable table = null;

        public static class StorageKeys
        {
            public static String IPAddress = "IPAddress";
            public static String PublicPort = "PublicPort";
            public static String PrivatePort = "PrivatePort";
            public static String PrivateIP = "PrivateIP";
            public static String WebServerPort = "WebServerPort";
        }

        static StorageHelper()
        {
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=craneiotstorage;" +
                "AccountKey=VjzhMrlHGCJv/70Ytpbrpe8HtTpxDC3fQ26B8STBjcJTZZoiwjG2kjlgLeEVWpFsnK8VPHaRkcsCCrh8qItkUg==;" +
                "EndpointSuffix=core.windows.net");

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            table = tableClient.GetTableReference("Settings");

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();
        }

        public static void StoreSetting(string settingName, string settingValue)
        {
            SettingsEntity entity = new SettingsEntity(settingName, settingValue);
            table.Execute(TableOperation.InsertOrReplace(entity));
        }

        public static SettingsEntity GetSetting(String settingName)
        {
            var entity = table.ExecuteQuery(new TableQuery<SettingsEntity>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, settingName))).SingleOrDefault();

            return entity as SettingsEntity;
        }
    }

    public class SettingsEntity : TableEntity
    {
        public SettingsEntity() { }

        public SettingsEntity(String name, String value)
        {
            this.PartitionKey = name;
            this.RowKey = name;
            this.Value = value;
        }

        public String Value { get; set; }
    }
}