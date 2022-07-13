using Microsoft.AspNetCore.Mvc;
using Catalogue.Repositories;
using Catalogue.Entities;
using Catalogue.Dtos;

namespace Catalogue.Controllers{
    [ApiController] 
    [Route("items")]
    public class ItemsController: ControllerBase{
        private readonly IItemsRepo repo;

        public ItemsController(IItemsRepo repo){
            this.repo = repo;
        }

        [HttpGet]
        public IEnumerable<ItemDto> GetItems(){ 
            var items = repo.GetItems().Select(item => item.AsDto());
            return items;
        }
        [HttpGet("{id}")]
        public ActionResult<ItemDto> GetItem(Guid id){
            var item = repo.GetItem(id);
            if(item is null) return NotFound();
            return item.AsDto();
        }

    }
} 