using Lims.Common.Dtos;
using System.Collections.ObjectModel;

namespace Lims.ToolsForClient.Extensions
{
    public static class CollectionExtensions
    {
        public static void FindAndRemove(this Collection<ItemDto> items, ItemDto item)
        {
            items.Remove(items.FirstOrDefault(t => t.ItemId == item.ItemId));
        }
        public static ItemDto FindTaskData(this Collection<ItemDto> items, ItemDto item)
        {
            return items.FirstOrDefault(t => t.ItemId == item.ItemId);
        }
        public static int FindTaskDataIndex(this Collection<ItemDto> items, ItemDto item)
        {
            return items.IndexOf(items.FirstOrDefault(t => t.ItemId == item.ItemId));
        }
        public static void AddRange<T>(this Collection<T> tasks1, List<T> items2)
        {
            foreach (var item in items2)            
                tasks1.Add(item);  
        }
    }
}
