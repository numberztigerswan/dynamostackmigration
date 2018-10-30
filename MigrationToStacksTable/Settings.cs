namespace MigrationToStacksTable
{
    public static class Settings
    {
        public static string OriginalMembershipTable = "Dev-GlenfiddichBase-MembershipTable-1UQI8BTPAWZQI";
        //public static string NewMembershipTable = "Dev-MonkeyShoulder-Membership";
        public static string StacksTable = "Dev-MonkeyShoulder-Stacks";

        public static string AwsAccessKey = "";
        public static string AwsAccessSecret = "";

        public static Amazon.RegionEndpoint Region =  Amazon.RegionEndpoint.EUWest1;
    }
}
