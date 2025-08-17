using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace SyncBridge.Domain.Utility;

public class SyncConstants
{
    public const string TYPE = "AppSettings";
    public const string SYNCKEYPREFIX = "Sync";
    public const string CMMPKEYPREFIX = "CMMP";
    public const string PROFILEKEYPREFIX = "PROFILE";
    public const string SFKEYPREFIX = "SF";
    public const string BASEURL = "BaseURL";
    public const string PATHURL = "PathURL";
    public const string COMPANYCHECK = "Company";
    public const string SALESORDERCHECK = "SalesOrder";
    public const string MEMBERSHIPCHECK = "Membership";
    public const string INVOICECHECK = "Invoice";
    public const string ROASTERCHECK = "Roster";
    public const string PROFILECHECK = "Profile";
    public const string CONTACTCHECK = "Contact";
    public const string ACCOUNTCHECK = "Account";
    public const string PULLCOMPANYMERGECHECK = "PullCompanyMerge";
    public const string PULLPROFILEMERGECHECK = "PullProfileMerge";
    public const string PROFILEROLESCHECK = "ProfileRoles";
    public const string CVENTCONTACTCHECK = "CventContact";
    public const string MERGEACCOUNT = "MergeAccount";
    public const string MERGECONTACT = "MergeContact";
    public const string DELETEREPARENTACCOUNT = "DeleteReparentAccount";
    public const string DELETEREPARENTCONTACT = "DeleteReparentContact";
    public const string UPDATE = "Update";
    public const string NOTIFICATION = "Notification";

    //DB Details
    public const string CONNECTION_STRING = "CosmosDBTriggerSetting";

    //LEASE CONNECTION NAMES
    public const string MEMBERSHIP_LEASE_COLLECTION_NAME = "Membership_Leases";
    public const string PROFILE_LEASE_COLLECTION_NAME = "Profile_Leases";

    //PROFILE DB DETAILS
    public const string PROFILE_DATABASE_NAME = "profile-db";
    public const string PROFILE_COLLECTION_NAME = "Profile";

    //MEMBERSHIP DB DETAILS
    public const string MEMBERSHIP_DATABASE_NAME = "membership-db";
    public const string COMPANY_COLLECTION_NAME = "Company";

    //PREFIX
    public const string PROFILE_LEASE_CONNECTION_PREFIX = "ProfileTrigger";
    public const string COMPANY_LEASE_CONNECTION_PREFIX = "CompanyTrigger";

    //MODULE
    public const string PROFILE = "Profile";
    public const string COMPANY = "Company";

    public const string QUEUE = "cmmp-sf-queue";

    // Email Log Details
    public const string RenewalInvoice = "RenewalInvoice";

    // TIME ZONE
    public const string EASTERNSTANDARDTIME = "Eastern Standard Time";

    public static DateTime getCurrentESTDateTime()
    {
        return TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById(SyncConstants.EASTERNSTANDARDTIME)
        );
    }
}
