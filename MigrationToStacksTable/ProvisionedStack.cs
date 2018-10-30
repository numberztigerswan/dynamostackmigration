using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MigrationToStacksTable.Model
{
    public class ProvisionedStack
    {
        [DynamoDBHashKey]
        [DynamoDBGlobalSecondaryIndexRangeKey]
        public string TemplateTag { get; set; }

        [DynamoDBRangeKey]
        public string StackId { get; set; }

        [DynamoDBGlobalSecondaryIndexHashKey]
        public string UserEmail { get; set; }


        [DynamoDBVersion]
        public long? VersionId { get; set; }

        [DynamoDBIgnore]
        public string Name
        {
            get { return StackId.Split('/')[1]; }
        }

        [DynamoDBIgnore]
        public string Region
        {
            get { return StackId.Split(':')[3]; }
        }

        public ChangeSet CustomBucketChangeSet { get; set; }

        public ChangeSet DefaultBucketChangeSet { get; set; }

        public DateTime LastChecked { get; set; }
    }
    public class ChangeSet
    {
        public string ChangeSetId { get; set; }

        public UpdateStatus UpdateStatus { get; set; }
    }
}
