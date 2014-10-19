namespace DarkSky.Commerce.Models {
    public struct Totals {
        public decimal SubTotal;
        public decimal Vat;
        public decimal Total;

        public static Totals operator +(Totals a, Totals b) {
            return new Totals {
                SubTotal = a.SubTotal + b.SubTotal,
                Vat = a.Vat + b.Vat,
                Total = a.Total + b.Total
            };
        }

        public static Totals operator -(Totals a, Totals b) {
            return new Totals {
                SubTotal = a.SubTotal - b.SubTotal,
                Vat = a.Vat - b.Vat,
                Total = a.Total - b.Total
            };
        }

        public static bool operator ==(Totals a, Totals b) {
            return a.SubTotal == b.SubTotal
                   && a.Vat == b.Vat
                   && a.Total == b.Total;
        }

        public static bool operator !=(Totals a, Totals b) {
            return !(a == b);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Totals && Equals((Totals) obj);
        }

        public bool Equals(Totals b) {
            return SubTotal == b.SubTotal && Vat == b.Vat && Total == b.Total;
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = SubTotal.GetHashCode();
                hashCode = (hashCode * 397) ^ Vat.GetHashCode();
                hashCode = (hashCode * 397) ^ Total.GetHashCode();
                return hashCode;
            }
        }
    }
}