using System;

namespace MunicipalServicesApp
{
    // Model class to store issue data
    public class IssueReport
    {
        public string ReferenceId { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string AttachmentPath { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string Status { get; set; }
        public int Priority { get; set; }
    }
}
