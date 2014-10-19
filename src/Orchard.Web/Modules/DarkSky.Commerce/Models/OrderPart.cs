using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.ContentManagement.Utilities;
using Orchard.Core.Common.Models;
using Orchard.Environment.Extensions;
using Orchard.Security;

namespace DarkSky.Commerce.Models {
    public class OrderPart : ContentPart<OrderPartRecord> {
        internal readonly LazyField<IList<OrderDetail>> DetailsField = new LazyField<IList<OrderDetail>>();

        public IUser User {
            get { return this.As<CommonPart>().Owner; }
            set { this.As<CommonPart>().Owner = value; }
        }

        public IShopAspect Shop {
            get { return this.As<CommonPart>().Container.As<IShopAspect>(); }
            set { this.As<CommonPart>().Container = value; }
        }

        public OrderStatus Status {
            get { return Record.Status; }
            set { Record.Status = value; }
        }

        public IList<OrderDetail> Details {
            get { return DetailsField.Value; }
        }

        public DateTime? CompletedUtc {
            get { return Record.CompletedUtc; }
            set { Record.CompletedUtc = value; }
        }

        public Transaction Transaction {
            get { return Record.Transaction; }
            set { Record.Transaction = value; }
        }

        public Totals Totals() {
            return Details.Aggregate(new Totals(), (current, item) => current + item.Totals());
        }
    }

    [OrchardFeature("DarkSky.Commerce.Orders")]
    public class OrderPartRecord : ContentPartRecord {
        public virtual OrderStatus Status { get; set; }
        public virtual DateTime? CompletedUtc { get; set; }
        public virtual Transaction Transaction { get; set; }
    }
}