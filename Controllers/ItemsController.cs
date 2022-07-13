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
        //POST/items
        [HttpPost]
        public ActionResult<ItemDto> CreateItem(CreateItemDto itemDto){
            Item item = new (){
                Id=Guid.NewGuid(),
                Name=itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            repo.CreateItem(item);
            return CreatedAtAction(nameof(GetItem), new{id=item.Id}, item.AsDto());
        }
        //PUT/items/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateItem(Guid id, UpdateItemDto itemDto){
            var existingItem=repo.GetItem(id);
            if(existingItem is null){
                return NotFound();
            }
            Item updatedItem = existingItem with {
                Name = itemDto.Name,
                Price = itemDto.Price
            };
            repo.UpdateItem(updatedItem);
            return NoContent();
        }
        //DELETE/items/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteItem(Guid id){
            var existingItem = repo.GetItem(id);
            if(existingItem is null){
                return NotFound();
            }
            repo.DeleteItem(id);
            return NoContent();
        }

    }
} 