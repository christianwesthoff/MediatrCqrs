using System.Collections.Generic;

namespace MediatrTest.Extensions
{
    public static class ObjectExtension
    {
        public static IEnumerable<T> Yield<T>(this T @this)
        {
            yield return @this;
        }
    }
}