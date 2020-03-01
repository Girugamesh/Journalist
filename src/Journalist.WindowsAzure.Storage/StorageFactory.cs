using System;
using Journalist.WindowsAzure.Storage.Blobs;
using Journalist.WindowsAzure.Storage.Queues;
using Journalist.WindowsAzure.Storage.Tables;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.Cosmos.Table;
using CommonCloudStorageAccount = Microsoft.Azure.Storage.CloudStorageAccount;

namespace Journalist.WindowsAzure.Storage
{
    public class StorageFactory : IStorageFactory
    {
        public ICloudTable CreateTable(string connectionString, string tableName)
        {
            Require.NotEmpty(connectionString, "connectionString");
            Require.NotEmpty(tableName, "tableName");

            return new CloudTableAdapter(CreateTableFactory(connectionString, tableName));
        }

        public ICloudQueue CreateQueue(string connectionString, string queueName)
        {
            Require.NotEmpty(connectionString, "connectionString");
            Require.NotEmpty(queueName, "queueName");

            return new CloudQueueAdapter(CreateQueueFactory(connectionString, queueName));
        }

        public ICloudQueue CreateQueue(Uri queueUri, string sasToken, string queueName)
        {
            Require.NotNull(queueUri, "queueUri");
            Require.NotEmpty(sasToken, "sasToken");
            Require.NotEmpty(queueName, "queueName");

            return new CloudQueueAdapter(CreateQueueFactory(queueUri, sasToken, queueName));
        }

        public ICloudQueue CreateQueue(Uri queue)
        {
            Require.NotNull(queue, "queue");

            return new CloudQueueAdapter(() => new CloudQueue(queue));
        }

        public ICloudBlobContainer CreateBlobContainer(string connectionString, string containerName)
        {
            Require.NotEmpty(connectionString, "connectionString");
            Require.NotEmpty(containerName, "containerName");

            return new CloudBlobContainerAdapter(CreateBlobContainerFactory(connectionString, containerName));
        }

        private static Func<CloudTable> CreateTableFactory(string connectionString, string tableName)
        {
            return () =>
            {
                var account = CloudStorageAccount.Parse(connectionString);

                var tableClient = account.CreateCloudTableClient();
                var table = tableClient.GetTableReference(tableName);
                table.CreateIfNotExistsAsync().GetAwaiter().GetResult();

                return table;
            };
        }

        private static Func<CloudQueue> CreateQueueFactory(string connectionString, string queueName)
        {
            return () =>
            {
                var account = CommonCloudStorageAccount.Parse(connectionString);
                var queueClient = account.CreateCloudQueueClient();
                var queue = queueClient.GetQueueReference(queueName);
#if NET451
                queue.CreateIfNotExists();
#else
                queue.CreateIfNotExistsAsync().GetAwaiter().GetResult();
#endif

                return queue;
            };
        }

        private static Func<CloudQueue> CreateQueueFactory(Uri queueUri, string sasToken, string queueName)
        {
            return () =>
            {
                var queueClient = new CloudQueueClient(queueUri, new Microsoft.Azure.Storage.Auth.StorageCredentials(sasToken));
                var queue = queueClient.GetQueueReference(queueName);

                queue.CreateIfNotExistsAsync().GetAwaiter().GetResult();

                return queue;
            };
        }

        private static Func<CloudBlobContainer> CreateBlobContainerFactory(string connectionString, string containerName)
        {
            return () =>
            {
                var account = CommonCloudStorageAccount.Parse(connectionString);

                var blobClient = account.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(containerName);
                container.CreateIfNotExistsAsync().GetAwaiter().GetResult();

                return container;
            };
        }
    }
}
