using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement.Utilities;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Security;

namespace DarkSky.Helpdesk.Models {
    public class SupportTicketPart : ContentPart<SupportTicketPartRecord> {
        internal LazyField<ContentItem> ContentField = new LazyField<ContentItem>();

        public string Subject {
            get { return this.As<TitlePart>().Title; }
            set { this.As<TitlePart>().Title = value; }
        }

        public string Message {
            get { return this.As<BodyPart>().Text; }
            set { this.As<BodyPart>().Text = value; }
        }

        public SeverityLevel Severity {
            get { return Record.Severity; }
            set { Record.Severity = value; }
        }

        public SupportTicketStatus Status {
            get { return Record.Status; }
            set { Record.Status = value; }
        }

        public ContentItem Content {
            get { return ContentField.Value; }
            set { ContentField.Value = value; }
        }

        public HelpdeskPart Helpdesk {
            get { return this.As<CommonPart>().Container.As<HelpdeskPart>(); }
            set { this.As<CommonPart>().Container = value; }
        }

        public DateTime TimeStamp {
            get { return this.As<CommonPart>().CreatedUtc ?? DateTime.MinValue; }
        }

        public IUser User {
            get { return this.As<CommonPart>().Owner; }
            set { this.As<CommonPart>().Owner = value; }
        }
    }

    public class SupportTicketPartRecord : ContentPartRecord {
        public virtual int? ContentId { get; set; }
        public virtual SeverityLevel Severity { get; set; }
        public virtual SupportTicketStatus Status { get; set; }
    }

    public enum SupportTicketStatus {
        New,
        Active,
        InProgress,
        Closed
    }

    public enum SeverityLevel {
        Low,
        Medium,
        High
    }
}