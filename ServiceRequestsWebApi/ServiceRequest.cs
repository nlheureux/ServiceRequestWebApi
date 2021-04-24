using System;

namespace ServiceRequestsWebApi
{
    public class ServiceRequest
    {
       public Guid id { get; set; }

        public string buildingCode { get; set; }

        public string description { get; set; }

        public string createdBy { get; set; }

        public DateTime createdDate { get; set; }

        public string lastModifiedBy { get; set; }

        public DateTime lastModifiedDate { get; set; }

        enum currentStatus
        {
            NotApplicable,
            Created,
            InProgress,
            Complete,
            Canceled
        };
    }
}