using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;

namespace MigrationToStacksTable
{
    public class MembershipDbClient 
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly DynamoDBOperationConfig _defaultOperationConfig;
        private readonly DynamoDBOperationConfig _accountIdConfig;

        public MembershipDbClient()
        {
            AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
            clientConfig.RegionEndpoint = Settings.Region;
            var credentials = new BasicAWSCredentials(Settings.AwsAccessKey, Settings.AwsAccessSecret);
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials, clientConfig);
        
            _dynamoDbContext = new DynamoDBContext(client);

            _defaultOperationConfig = new DynamoDBOperationConfig
            {
                OverrideTableName = Settings.OriginalMembershipTable
            };
            _accountIdConfig = new DynamoDBOperationConfig
            {
                IndexName = "AwsAccountId",
                OverrideTableName = Settings.OriginalMembershipTable
            };
        }

        public async Task<UserAuthentication> GetUserByEmail(string email)
        {
            return await UserFromDbByEmail(email);
        }

        private async Task<UserAuthentication> UserFromDbByEmail(string email)
        {
            var result = await _dynamoDbContext
                .QueryAsync<UserAuthentication>(email, _defaultOperationConfig)
                .GetRemainingAsync();

            return result.FirstOrDefault();
        }

        public async Task<List<UserAuthentication>> AllUsers()
        {
            return await _dynamoDbContext.ScanAsync<UserAuthentication>(new List<ScanCondition>(), _defaultOperationConfig).GetRemainingAsync();
        }
        public async Task
            <List<UserAccount>> GetUsersByAccountId(string accountId)
        {
            var query = await _dynamoDbContext
                .QueryAsync<UserAccount>(accountId, _accountIdConfig)
                .GetRemainingAsync();
            return query.ToList();
        }

        public async void Upsert(UserAuthentication newUser)
        {
            await _dynamoDbContext.SaveAsync(newUser, _defaultOperationConfig);
        }
    }
    public enum DdbCreateResult
    {
        Success = 0,
        AlreadyExist = 10
    }
    public class UserAuthentication
    {
        [DynamoDBHashKey]
        public string EmailAsUsername { get; set; }

        public string AwsAccountId { get; set; }

        public string PasswordHash { get; set; }

        public string RecoveryToken { get; set; }

        public DateTime? RecoveryTokenIssued { get; set; }

        [DynamoDBVersion]
        public long? VersionId { get; set; }

        public CfnCredential CfnCredential { get; set; }

        public CredentialRes CfnCredentialRes { get; set; }

        public Dictionary<string, List<ProvisionedStack>> ProvisionedStacks { get; set; }

        public CreatedStripeCustomer StripeData { get; set; }

        public bool UsePrivateBucket { get; set; }

        public DateTime? TcAcceptedTs { get; set; }
    }
    public class CreatedStripeCustomer
    {
        public string SubscriptionId { get; set; }
        public string CustomerId { get; set; }
        public bool IsActive { get; set; }
        public Plan Plan { get; set; }
    }
    public class Plan
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
        public DateTime Created { get; set; }
        public int Amount { get; set; }
        public string Interval { get; set; }
        public int IntervalCount { get; set; }
        public bool LiveMode { get; set; }
        public string StatementDescriptor { get; set; }
        public int? TrialPeriodDays { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
    public class CredentialRes 
    {
        public string Username { get; set; }
        public string TempPasscode { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public string StackCreationRole { get; set; }
        public string TemplateBucket { get; set; }
    }
    public class ProvisionedStack
    {
        public string Id { get; set; }

        [DynamoDBIgnore]
        public string Name
        {
            get { return Id.Split('/')[1]; }
        }

        [DynamoDBIgnore]
        public string Region
        {
            get { return Id.Split(':')[3]; }
        }

        public string ChangeSetId { get; set; }

        public UpdateStatus UpdateStatus { get; set; }

        public DateTime LastChecked { get; set; }
    }
    public enum UpdateStatus
    {
        NoUpdate, RequireManual, ChangeSetReady, StackBusy, TemplateError, AccessDenied
    }
    public class UserAccount
    {
        public string EmailAsUsername { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey]
        public string AwsAccountId { get; set; }
    }
    public class CfnCredential
    {
        public CfnCredential()
        {
            Created = new DateTime(1900, 1, 1);
        }

        public DateTime Created { get; set; }

        public string PasscodeHash { get; set; }

        public string TemplateKey { get; set; }

        public string StackId { get; set; }
    }

}
