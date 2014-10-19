using System;
using System.Collections.Generic;

namespace DarkSky.Commerce.Models {
    public class ShoppingCartRecord {
        public virtual int Id { get; set; }
        public virtual Guid Guid { get; set; }
        public virtual DateTime CreatedUtc { get; set; }
        public virtual IList<ShoppingCartItem> Items { get; set; }
        public virtual bool IsClosed { get; set; }
        public virtual DateTime? ClosedUtc { get; set; }

        internal ShoppingCartRecord() {
            Items = new List<ShoppingCartItem>();
        }

        internal static ShoppingCartRecord New() {
            return new ShoppingCartRecord {
                CreatedUtc = DateTime.UtcNow,
                Guid = Guid.NewGuid()
            };
        }
    }
}