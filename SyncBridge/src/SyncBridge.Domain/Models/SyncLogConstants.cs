using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncBridge.Domain.Models;

public class SyncLogConstants
{
    public static class SourceSystem
    {
        public const string CMMP = "CMMP";
        public const string Salesforce = "Salesforce";
        public const string Cvent = "CVENT";
        public const string Notification = "Notification";
        public const string Cosmos = "Cosmos";
    }

    public static class Status
    {
        public const string Ready = "READY";
        public const string InProgress = "INPROGRESS"; //A handler has picked up the request and is currently processing the sync.
        public const string Completed = "COMPLETED"; // The sync process, including updates to both Salesforce and downstream systems (e.g., CMMP), has completed successfully.
        public const string Failed = "FAILED"; //Sync did not complete. Eligible for retry.
        public const string Retry_Inititated = "RETRY_INITIATED"; // The retry handler has picked up this record for processing
        public const string Aborted = "ABORTED"; //Final failure after exhausting all retries. No further retries will be attempted.
        public const string Manually_Corrected = "MANUALLY_CORRECTED"; //Sync previously failed or aborted, but was fixed manually and marked as corrected.
        public const string Completed_With_Latest_Request = "COMPLETED_WITH_LATEST_REQUEST"; //This sync request was not processed directly, as its changes were already included in the latest sync that completed successfully
    }

    public static class ErrorCode
    {
        public const string SF_Failed = "SF_FAILED";
        public const string SF_Id_Update_Failed = "SF_ID_UPDATE_FAILED";
    }

    public static class Action
    {
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";
        public const string Retry = "Retry";
    }

    public static class Handler
    {
        public const string CMMPtoSFunction = "CMMPtoSfFunction";
        public const string Salesforce = "Salesforce";
        public const string SFtoCMMPunction = "SFtoCMMPunction";
        public const string CVENTtoSFunction = "CVENTtoSFunction";
        public const string CMMP = "CMMP";
        public const string RetryFunction = "RetryFunction";
    }
}
