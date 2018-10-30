using System;
using System.Collections.Generic;
using System.Linq;

namespace MigrationToStacksTable
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DoWork();
        }

        private static void DoWork()
        {
            var membership = new MembershipDbClient();
            var usersTask = membership.AllUsers();
            usersTask.Wait();

            var stackDbClient = new ProvisionedStackDbClient();
            var stackCount = 0;
            foreach (var user in usersTask.Result)
            {
                var stacks = ToStackCollection(user);
                stackCount += stacks.Count();
                foreach (var item in stacks)
                {
                    stackDbClient.Upsert(item);
                }
            }
            Console.WriteLine("Number of entries expected in new table: " + stackCount);
            Console.WriteLine(usersTask.Result);
            
        }

        private static List<Model.ProvisionedStack> ToStackCollection(UserAuthentication user)
        {
            var list = new List<Model.ProvisionedStack>();
            if (user.ProvisionedStacks != null)
            {
                foreach (var stackCollection in user.ProvisionedStacks?.ToList())
                {
                    foreach (var s in stackCollection.Value)
                    {
                        var stack = new Model.ProvisionedStack
                        {
                            StackId = s.Id,
                            UserEmail = user.EmailAsUsername,
                            TemplateTag = stackCollection.Key,
                            CustomBucketChangeSet = ConditionalChangeSet(s.ChangeSetId, s.UpdateStatus, false),
                            DefaultBucketChangeSet = ConditionalChangeSet(s.ChangeSetId, s.UpdateStatus, true),
                            LastChecked = s.LastChecked
                        };
                        list.Add(stack);
                    }
                }
            }
            return list;
        }
        private static Model.ChangeSet ConditionalChangeSet(string id, UpdateStatus status, bool isDefault)
        {
            if (id == null)
            {
                return null;
            }
            if (isDefault)
            {
                if (id.StartsWith("BC-Default-"))
                {
                    return new Model.ChangeSet { ChangeSetId = id, UpdateStatus = status };
                }
            }
            else
            {
                if (id.StartsWith("BC-Custom-"))
                {
                    return new Model.ChangeSet { ChangeSetId = id, UpdateStatus = status };
                }
            }
            return null;
        }

    }
}
