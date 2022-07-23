using Microsoft.AspNetCore.Mvc;
using Catalogue.Api.Repositories;
using Catalogue.Api.Entities;
using Catalogue.Api.Dtos;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Catalogue.Api.Controllers{
    [ApiController] 
    [Route("items")]
    public class ItemsController: ControllerBase{
        private readonly IItemsRepo repo;
        private readonly ILogger<ItemsController>logger;

        public ItemsController(IItemsRepo repo, ILogger<ItemsController>logger){
            this.repo = repo;
            this.logger=logger;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetItemsAsync(string name=null){ 
            var items = (await repo.GetItemsAsync()).Select(item => item.AsDto());
            if(!string.IsNullOrWhiteSpace(name)){
                items=items.Where(item=>item.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }
            logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved{items.Count()} items.");
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
        public async Task< ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto){
            Item item = new (){
                Id=Guid.NewGuid(),
                Name=itemDto.Name,
                Description=itemDto.Description,
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

            existingItem.Name=itemDto.Name;
            existingItem.Description=itemDto.Description;
            existingItem.Price=itemDto.Price;
           
            await repo.UpdateItemAsync(existingItem);
            return NoContent();
        }

        //DELETE/items/{id}
        [HttpDelete("{id}")]
        public async Task< ActionResult> DeleteItemAsync(Guid id){
            var existingItem = await repo.GetItemAsync(id);
            if(existingItem is null){
                return NotFound();
            }
            await repo.DeleteItemAsync(id);
            return NoContent();
        }

    }
} 