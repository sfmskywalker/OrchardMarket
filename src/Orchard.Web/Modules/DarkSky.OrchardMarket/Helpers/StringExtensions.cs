namespace DarkSky.OrchardMarket.Helpers {
    public static class StringExtensions {
         public static string TrimSafe(this string s) {
             return s != null ? s.Trim() : string.Empty;
         }

         public static string Slugify(this string s) {
             if (s == null)
                 return null;

             return s.Replace(' ', '-');
         }
    }
}