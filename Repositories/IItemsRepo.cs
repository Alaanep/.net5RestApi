using Catalogue.Entities;

namespace Catalogue.Repositories;

public interface IItemsRepo
    {
        Item GetItem(Guid id);
        IEnumerable<Item> GetItems();
    }