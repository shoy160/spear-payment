using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Spear.Payment.Sdk
{
    public static class Extensions
    {
        /// <summary> 起始时间 </summary>
        private static readonly DateTime ZoneTime = new DateTime(1970, 1, 1);
        private const string DefaultIp = "127.0.0.1";

        /// <summary> 字典化对象 </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static IDictionary<string, object> ToDictionary(this object source)
        {
            if (source == null)
                return new Dictionary<string, object>();
            var type = source.GetType();
            if (type == typeof(IDictionary<string, object>) || type == typeof(Dictionary<string, object>))
                return (IDictionary<string, object>)source;
            var dictTypes = new[] { typeof(IDictionary<string, string>), typeof(Dictionary<string, string>) };
            if (dictTypes.Contains(type))
                return ((IDictionary<string, string>)source).ToDictionary(k => k.Key, v => (object)v.Value);
            var dict = new Dictionary<string, object>();
            if (type.IsValueType)
                return dict;
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                dict.Add(prop.Name, prop.GetValue(source, null));
            }
            return dict;
        }

        /// <summary> Url格式 </summary>
        /// <param name="dict"></param>
        /// <param name="encode"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        internal static string ToUrl(this IDictionary<string, object> dict, bool encode = true, Encoding encoding = null)
        {
            var sb = new StringBuilder();
            encoding = encoding ?? Encoding.UTF8;
            foreach (var item in dict)
            {
                var value = item.Value?.ToString() ?? string.Empty;
                sb.AppendFormat("{0}={1}&", item.Key, encode ? HttpUtility.UrlEncode(value, encoding) : value);
            }
            return sb.ToString().TrimEnd('&');
        }

        /// <summary> 将网关数据转成Xml格式数据 </summary>
        /// <returns></returns>
        public static string ToXml(this IDictionary<string, object> dict)
        {
            if (dict.Count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            sb.Append("<xml>");
            foreach (var item in dict)
            {
                sb.AppendFormat(item.Value is string ? "<{0}><![CDATA[{1}]]></{0}>" : "<{0}>{1}</{0}>", item.Key,
                    item.Value);
            }
            sb.Append("</xml>");

            return sb.ToString();
        }

        public static string Md5(this string str)
        {
            var md5 = MD5.Create();
            var dataByte = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (var x in dataByte)
            {
                sb.Append(x.ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary> 小驼峰命名法 </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static string ToCamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
                return s;
            var charArray = s.ToCharArray();
            for (var index = 0; index < charArray.Length; ++index)
            {
                var flag = index + 1 < charArray.Length;
                if (!(index > 0 & flag) || char.IsUpper(charArray[index + 1]))
                    charArray[index] = char.ToLower(charArray[index], CultureInfo.InvariantCulture);
                else
                    break;
            }
            return new string(charArray);
        }

        /// <summary> unescape </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string UnEscape(this object value)
        {
            if (value == null)
                return string.Empty;
            var type = value.GetType();
            if (type.IsEnum)
                return Convert.ToInt32(value).ToString();
            if (type == typeof(bool))
                return ((bool)value ? 1 : 0).ToString();
            var stringBuilder = new StringBuilder();
            var pattern = value.ToString();
            var length = pattern.Length;
            var index = 0;
            while (index != length)
                stringBuilder.Append(Uri.IsHexEncoding(pattern, index) ? Uri.HexUnescape(pattern, ref index) : pattern[index++]);
            return stringBuilder.ToString();
        }

        internal static long Timestamp(this DateTime dateTime)
        {
            var timeSpan = dateTime.ToUniversalTime() - ZoneTime;
            return (long)timeSpan.TotalMilliseconds;
        }

        public static string SignDict(this IDictionary<string, object> dict, string secret, long timestamp = 0, bool filterEmpty = true)
        {
            if (filterEmpty)
                dict = dict.Where(t => t.Value != null && !string.IsNullOrWhiteSpace(t.Value.ToString()))
                    .ToDictionary(k => k.Key, v => v.Value);
            //13位时间戳
            timestamp = timestamp > 0 ? timestamp : DateTime.Now.Timestamp();
            var array = new SortedDictionary<string, object>(dict).Select(t =>
                $"{t.Key}={t.Value.UnEscape()}");
            var unSigned = string.Concat(string.Join("&", array), secret, timestamp);
            return timestamp + unSigned.Md5().ToLower();
        }

        internal static void Sign(this IDictionary<string, object> dict, string secret)
        {
            var sign = SignDict(dict, secret);
            dict.Add("sign", sign);
        }

        internal static bool VerifySign(this IDictionary<string, object> dict, string secret, string sign)
        {
            if (string.IsNullOrWhiteSpace(sign) || sign.Length != 45)
                return false;
            var timestamp = Convert.ToInt64(sign.Substring(0, 13));
            var checkSign = SignDict(dict, secret, timestamp);
            return string.Equals(checkSign, sign, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool IsIp(this string ipAddress)
        {
            return Regex.IsMatch(ipAddress, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
    }
}
