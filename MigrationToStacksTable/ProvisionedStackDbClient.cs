using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MigrationToStacksTable
{
    public class ProvisionedStackDbClient
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly DynamoDBOperationConfig _defaultOperationConfig;
        private readonly DynamoDBOperationConfig _userEmailTemplateTagGsiConfig;


        public ProvisionedStackDbClient()
        {

            AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
            clientConfig.RegionEndpoint = Settings.Region;
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(new BasicAWSCredentials(Settings.AwsAccessKey,Settings.AwsAccessSecret), clientConfig);

            _dynamoDbContext = new DynamoDBContext(client);

            _defaultOperationConfig = new DynamoDBOperationConfig
            {
                OverrideTableName = Settings.StacksTable
            };
            _userEmailTemplateTagGsiConfig = new DynamoDBOperationConfig
            {
                IndexName = "GSI",
                OverrideTableName = Settings.StacksTable
            };
        }
        public async void Upsert(Model.ProvisionedStack newStack)
        {
            Thread.Sleep(10000);
            await _dynamoDbContext.SaveAsync(newStack, _defaultOperationConfig);
        }
    }
}
