using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement.Utilities;
using Orchard.Environment.Extensions;

namespace DarkSky.Commerce.Models {
    public class InvoicePart : ContentPart<InvoicePartRecord>, ITitleAspect {
        internal readonly LazyField<OrderPart> OrderField = new LazyField<OrderPart>();
        internal readonly LazyField<IList<InvoiceDetail>> DetailsField = new LazyField<IList<InvoiceDetail>>();

        public OrderPart Order {
            get { return OrderField.Value; }
            set { OrderField.Value = value; }
        }

        public IShopAspect Shop {
            get { return this.As<ICommonPart>().Container.As<IShopAspect>(); }
            set { this.As<ICommonPart>().Container = value; }
        }

        public int? OrderId {
            get { return Record.OrderId; }
            set { Record.OrderId = value; }
        }

        public Payment Payment {
            get { return Record.Payment; }
            set { Record.Payment = value; }
        }

        public InvoiceStatus Status {
            get { return Record.Status; }
            set { Record.Status = value; }
        }

        public IList<InvoiceDetail> Details {
            get { return DetailsField.Value; }
        }

        public DateTime? CompletedUtc {
            get { return Record.CompletedUtc; }
            set { Record.CompletedUtc = value; }
        }

        public string Title {
            get { return string.Format("Invoice {0}", Id); }
        }

        public decimal SubTotal {
            get { return Details.Select(x => x.SubTotal()).Sum(); }
        }

        public decimal Vat {
            get { return Details.Select(x => x.Vat()).Sum(); }
        }

        public decimal Total {
            get { return Details.Select(x => x.Total()).Sum(); }
        }

        public Transaction Transaction {
            get { return Record.Transaction; }
            set { Record.Transaction = value; }
        }
    }

    [OrchardFeature("DarkSky.Commerce.Orders")]
    public class InvoicePartRecord : ContentPartRecord {
        public virtual int? OrderId { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual InvoiceStatus Status { get; set; }
        public virtual DateTime? CompletedUtc { get; set; }
        public virtual Transaction Transaction { get; set; }
    }
}