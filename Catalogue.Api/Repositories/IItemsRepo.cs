using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalogue.Api.Entities;

namespace Catalogue.Api.Repositories;

public interface IItemsRepo
    {
        Task<Item> GetItemAsync(Guid id);
        Task<IEnumerable<Item>> GetItemsAsync();
        Task CreateItemAsync(Item item);
        Task UpdateItemAsync(Item item);
        Task DeleteItemAsync(System.Guid id);
    }