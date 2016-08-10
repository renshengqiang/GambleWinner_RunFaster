
namespace MoreFun.Utils
{
    public class CollectionUtil
    {
        public static bool IsValidIndex(System.Collections.IList collection, int index)
        {
            if(null != collection &&
                0 <= index &&
                index < collection.Count &&
                null != collection[index])
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
