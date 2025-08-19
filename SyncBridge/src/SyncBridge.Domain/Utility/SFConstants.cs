namespace CventSalesforceSyncApi.Domain.Utilities
{
    public class SFConstants
    {
        public const string TYPE = "AppSettings";
        public const string SYNCKEYPREFIX = "Sync";
        public const string CVENTKEYPREFIX = "Cvent";
        public const string SFKEYPREFIX = "SF.Cvent";
        public const string BASEURL = "BaseURL";
        public const string PATHURL = "PathURL";
        public const string NOTIFICATION = "Notification";
        public const string UPDATE = "Update";

        //Module
        //public const string PROFILE = "Profile";
        public const string EVENT = "Event";
        public const string VENUE = "Venue";
        public const string ATTENDEE = "Attendee";
        public const string TICKETTYPE = "TicketType";
        public const string SALESORDER = "SalesOrder";
        public const string RECEIPT = "Receipt";
        public const string SfIdUpdateUrl = "PutSfInfoById";
        public static class Scopes
        {
            //public const string SFSSVC = nameof(SFSSVC);
            public const string MEBSVC = nameof(MEBSVC);
        }

        public static class Permissions
        {
            /// <summary>
            /// Read permission
            /// </summary>
            //public const string SFSSVC_SFT_ADM = "SFSSVC:SFT:ADM";
            public const string MEBSVC_MEB_ADM = "MEBSVC:MEB:ADM";
        }
    }
}
