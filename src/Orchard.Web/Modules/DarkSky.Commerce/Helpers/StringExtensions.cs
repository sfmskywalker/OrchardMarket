using System;

namespace DarkSky.Commerce.Helpers {
    public static class StringExtensions {
         public static TEnum ParseEnum<TEnum>(this string value) where TEnum:struct {
             if (string.IsNullOrWhiteSpace(value)) return default(TEnum);
             TEnum result;
             Enum.TryParse(value, true, out result);
             return result;
         }
    }
}