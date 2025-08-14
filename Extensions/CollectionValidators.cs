namespace EasyClaimsCore.API.Extensions
{
    public static class CollectionValidators
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? obj)
        {
            return obj == null || !obj.Any();
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T>? obj)
        {
            return obj == null || obj.Count == 0;
        }

        public static bool IsNullOrEmpty<T>(this IList<T>? obj)
        {
            return obj == null || obj.Count == 0;
        }

        public static bool IsNullOrEmpty<T>(this List<T>? obj)
        {
            return obj == null || obj.Count == 0;
        }
    }
}