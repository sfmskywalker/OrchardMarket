using System;
using System.Globalization;
using System.Linq;
using System.Text;
using DarkSky.OrchardMarket.Helpers;

namespace DarkSky.OrchardMarket.Models {
    public class VersionRange {
        public Version Minimum { get; set; }
        public Version Maximum { get; set; }
        public bool IsMinimumInclusive { get; set; }
        public bool IsMaximumInclusive { get; set; }

        public VersionRange() {
            Minimum = new Version();
            Maximum = new Version();
        }

        public override string ToString() {
            if (Minimum != null && IsMinimumInclusive && Maximum == null && !IsMaximumInclusive)
                return Minimum.ToString();
            if (Minimum != null && Maximum != null && (Minimum == Maximum && IsMinimumInclusive) && IsMaximumInclusive)
                return "[" + Minimum + "]";
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(IsMinimumInclusive ? "[" : "(");

            stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}, {1}", new object[] {
                Minimum,
                Maximum
            });

            stringBuilder.Append(IsMaximumInclusive ? "]" : ")");
            return stringBuilder.ToString();
        }

        public static VersionRange ParseVersionSpec(string value) {
            VersionRange result;
            if (TryParse(value, out result))
                return result;
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid version string format", new object[] {
                value
            }));
        }

        public static bool TryParse(string value, out VersionRange result) {
            var versionSpec = new VersionRange();
            value = value.TrimSafe();
            Version result1;
            if (Version.TryParse(value, out result1)) {
                result = new VersionRange() {
                    Minimum = result1,
                    IsMinimumInclusive = true
                };
                return true;
            }
            
            result = default(VersionRange);
            if (value.Length < 3)
                return false;

            switch (value.First()) {
              case '(':
                versionSpec.IsMinimumInclusive = false;
                break;
              case '[':
                versionSpec.IsMinimumInclusive = true;
                break;
              default:
                return false;
            }
            switch (value.Last()) {
              case ')':
                versionSpec.IsMaximumInclusive = false;
                break;
              case ']':
                versionSpec.IsMaximumInclusive = true;
                break;
              default:
                return false;
            }
            value = value.Substring(1, value.Length - 2);
            var strArray = value.Split(new[] {
              ','
            });
            if (strArray.Length > 2)
              return false;
            var input1 = strArray[0];
            var input2 = strArray.Length == 2 ? strArray[1] : strArray[0];
            if (!string.IsNullOrWhiteSpace(input1)) {
              if (!Version.TryParse(input1, out result1))
                return false;
              versionSpec.Minimum = result1;
            }
            if (!string.IsNullOrWhiteSpace(input2)) {
              if (!Version.TryParse(input2, out result1))
                return false;
              versionSpec.Maximum = result1;
            }
            result = versionSpec;
            return true;
        }
    }
}