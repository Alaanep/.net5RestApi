using Catalogue.Entities;
using System.Collections.Generic;

namespace Catalogue.Repositories{
    public class InMemItemsRepo : IItemsRepo
    {
        private List<Item> items = new(){
            new Item {Id=Guid.NewGuid(), Name="Potion", Price=9, CreatedDate=DateTimeOffset.UtcNow},
            new Item {Id=Guid.NewGuid(), Name="Iron Sword", Price=20, CreatedDate=DateTimeOffset.UtcNow},
            new Item {Id=Guid.NewGuid(), Name="Bronze Shield", Price=18, CreatedDate=DateTimeOffset.UtcNow}
        };

        public IEnumerable<Item> GetItems() => items;
        public Item GetItem(Guid id) => items.Where(item => item.Id == id).SingleOrDefault();
    }
} 