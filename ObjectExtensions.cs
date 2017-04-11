using System.Reflection;
using System.Text;

namespace System
{
    static class ObjectExtensions
    {
        public static object copy(this object src)
        {
            var method = src.GetType().GetTypeInfo().GetDeclaredMethod("copy");
            return method?.Invoke(src, null);
        }

        public static T copy<T>(this T src) where T : class
        {
            return ((object)src).copy() as T;
        }

        public static string FormatJava(string format, params object[] args)
        {
            var currentIndex = 0;
            var argCounter = 0;
            var builder = new StringBuilder();
            while (currentIndex != -1)
            {
                var varIndex = format.IndexOf("%", currentIndex, StringComparison.Ordinal);
                if (varIndex == -1)
                {
                    builder.Append(format.Substring(currentIndex));
                }
                else
                {
                    builder.Append(format.Substring(currentIndex, varIndex - currentIndex));
                    builder.Append(args[argCounter++]);
                    currentIndex = format.IndexOf(";", varIndex, StringComparison.Ordinal);
                }
            }
            return builder.ToString();
        }
    }


}
