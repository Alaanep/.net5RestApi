using Catalogue.Api.Dtos;
using Catalogue.Api.Entities;
namespace Catalogue.Api{
    public static class Extensions{
        public static ItemDto AsDto(this Item item){
            return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
        }
    }
}