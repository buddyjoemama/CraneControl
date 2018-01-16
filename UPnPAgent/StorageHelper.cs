using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net;

namespace UPnPAgent
{
    internal class StorageHelper
    {
        private static CloudTable table = null;

        static StorageHelper()
        {
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            table = tableClient.GetTableReference("Settings");

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();
        }

        internal static void StoreSetting(string settingName, string settingValue)
        {
            SettingsEntity entity = new SettingsEntity(settingName, settingValue);
            table.Execute(TableOperation.InsertOrReplace(entity));
        }

        class SettingsEntity : TableEntity
        {
            public SettingsEntity(String name, String value)
            {
                this.PartitionKey = name;
                this.RowKey = name;
                this.Value = value;
            }

            public String Value { get; set; }
        }
    }
}