using Microsoft.AspNetCore.Mvc;
using Catalogue.Repositories;
using Catalogue.Entities;

namespace Catalogue.Controllers{
    [ApiController] 
    [Route("items")]
    public class ItemsController: ControllerBase{
        private readonly InMemItemsRepo repo;

        public ItemsController(){
            repo = new InMemItemsRepo();
        }

        [HttpGet]
        public IEnumerable<Item> GetItems(){
            var items = repo.GetItems();
            return items;
        }

    }
}