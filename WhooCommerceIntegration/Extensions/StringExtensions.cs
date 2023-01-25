using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Extensions
{
    public static class StringExtensions
    {
        public static string FormatWith(this string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            return string.Format(format, args);
        }

        public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            return string.Format(provider, format, args);
        }

        public static string FormatWith(this string format, object source)
        {
            return FormatWith(format, null, source);
        }

        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        public static Exception GetBottomException(this Exception e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            while (e.InnerException != null)
            {
                e = e.InnerException;
            }

            return e;
        }

        //public static string FormatWith(this string format, IFormatProvider provider, object source)
        //{
        //    if (format == null)
        //        throw new ArgumentNullException("format");

        //    Regex r = new Regex(@"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+",
        //      RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        //    List<object> values = new List<object>();
        //    string rewrittenFormat = r.Replace(format, delegate(Match m)
        //    {
        //        Group startGroup = m.Groups["start"];
        //        Group propertyGroup = m.Groups["property"];
        //        Group formatGroup = m.Groups["format"];
        //        Group endGroup = m.Groups["end"];

        //        values.Add((propertyGroup.Value == "0")
        //          ? source
        //          : DataBinder.Eval(source, propertyGroup.Value));

        //        return new string('{', startGroup.Captures.Count) + (values.Count - 1) + formatGroup.Value
        //          + new string('}', endGroup.Captures.Count);
        //    });

        //    return string.Format(provider, rewrittenFormat, values.ToArray());
        //}
    }
}
