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
        public async Task<IEnumerable<ItemDto>> GetItemsAsync(){ 
            var items = (await repo.GetItemsAsync()).Select(item => item.AsDto());
            return items;
        }
        [HttpGet("{id}")]
        public async Task< ActionResult<ItemDto>> GetItemAsync(Guid id){
            var item = await repo.GetItemAsync(id);
            if(item is null) return NotFound();
            return item.AsDto();
        }
        //POST/items
        [HttpPost]
        public async Task< ActionResult<ItemDto>> CreateItem(CreateItemDto itemDto){
            Item item = new (){
                Id=Guid.NewGuid(),
                Name=itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await repo.CreateItemAsync(item);
            return CreatedAtAction(nameof(GetItemAsync), new{id=item.Id}, item.AsDto());
        }
        //PUT/items/{id}
        [HttpPut("{id}")]
        public async Task< ActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDto){
            var existingItem= await repo.GetItemAsync(id);
            if(existingItem is null){
                return NotFound();
            }
            Item updatedItem = existingItem with {
                Name = itemDto.Name,
                Price = itemDto.Price
            };
            await repo.UpdateItemAsync(updatedItem);
            return NoContent();
        }
        //DELETE/items/{id}
        [HttpDelete("{id}")]
        public async Task< ActionResult> DeleteItem(Guid id){
            var existingItem = await repo.GetItemAsync(id);
            if(existingItem is null){
                return NotFound();
            }
            await repo.DeleteItemAsync(id);
            return NoContent();
        }

    }
} 