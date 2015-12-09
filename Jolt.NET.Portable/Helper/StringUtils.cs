using System;
using System.Text;

namespace Jolt.NET.Helper
{
    /// <summary>
    /// This class provides extensions methods to use with a string.
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Combines urls similiar to <see cref="System.IO.Path.Combine(string[])"/>
        /// </summary>
        public static string UrlCombine(this string baseUrl, params string[] appends)
        {
            if (appends == null || appends.Length == 0) return baseUrl;

            var urlBuilder = new StringBuilder();
            urlBuilder.Append(tryCreateRelativeOrAbsolute(baseUrl));
            foreach (var item in appends)
            {
                var tempUrl = tryCreateRelativeOrAbsolute(item);
                urlBuilder.Append(tempUrl);
            }
            return RemoveSlashFromPathIfNeeded(urlBuilder.ToString());
        }

        private static string tryCreateRelativeOrAbsolute(string s)
        {
            Uri uri;
            Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out uri);
            string tempUrl = AppendSlashToPathIfNeeded(uri.ToString());
            return tempUrl;
        }

        #region UrlPath.cs from .NET 3.5
        
        internal static string AppendSlashToPathIfNeeded(string path)
        {

            if (path == null) return null;

            int l = path.Length;
            if (l == 0) return path;

            if (path[l - 1] != '/')
                path += '/';

            return path;
        }

        internal static string RemoveSlashFromPathIfNeeded(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            int l = path.Length;
            if (l <= 1 || path[l - 1] != '/')
            {
                return path;
            }

            return path.Substring(0, l - 1);
        }

        #endregion
    }
}
