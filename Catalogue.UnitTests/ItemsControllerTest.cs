using Catalogue.Api.Controllers;
using Catalogue.Api.Dtos;
using Catalogue.Api.Entities;
using Catalogue.Api.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalogue.UnitTests;

public class ItemsControllerTest
{   
    private readonly Mock<IItemsRepo>repoStub=new();
    private readonly Mock<ILogger<ItemsController>> loggerStub = new();
    private readonly Random rand = new();
    [Fact]
    public async Task GetItemAsync_withUnExistingItem_ReturnsNotFound()
    {
        //arrange
        repoStub.Setup(repo=>repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync((Item)null);
        var controller = new ItemsController(repoStub.Object, loggerStub.Object);
        
        //act
        var result = await controller.GetItemAsync(Guid.NewGuid());

        //assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetTaskAsync_withExistingItem_ReturnsNotFound(){
        //arrange
        var expectedItem = CreateRandomItem();
        repoStub.Setup(repo=>repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync(expectedItem);
        var controller = new ItemsController(repoStub.Object, loggerStub.Object);
        
        //act
        var result = await controller.GetItemAsync(Guid.NewGuid());

        //assert
        result.Value.Should().BeEquivalentTo(expectedItem, options=>options.ComparingByMembers<Item>());
    }

    [Fact]
    public async Task GetItemsAsync_WithExistingItems_ReturnsAllItems()
    {
        // arrange
        var expectedItems = new []{CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };
        repoStub.Setup(repo=>repo.GetItemsAsync()).ReturnsAsync(expectedItems);
        var controller = new ItemsController(repoStub.Object, loggerStub.Object);
        // act
        var actualItems = await controller.GetItemsAsync();
        // assert
        actualItems.Should().BeEquivalentTo(expectedItems, options=>options.ComparingByMembers<Item>());
    }

    [Fact]
    public async Task CreateItemAsync_WithItemToCreate_ReturnsCreatedItem()
    {
        // arrange
        var itemToCreate = new CreateItemDto(){
            Name=Guid.NewGuid().ToString(),
            Price = rand.Next(1000)
        };
        var controller = new ItemsController(repoStub.Object, loggerStub.Object);
        // act
        var result = await controller.CreateItemAsync(itemToCreate);

        // assert
        var createdItem = (result.Result as CreatedAtActionResult).Value as ItemDto;
        createdItem.Should().BeEquivalentTo(createdItem, options => options.ComparingByMembers<ItemDto>().ExcludingMissingMembers());
        createdItem.Id.Should().NotBeEmpty();
        createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMilliseconds(1000));
    }

    [Fact]
    public async Task UpdateItemsAsync_WithExistingItem_ReturnNoContent()
    {
        // arrange
        Item existingItem = CreateRandomItem();
        repoStub.Setup(repo=>repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync(existingItem);
        var controller = new ItemsController(repoStub.Object, loggerStub.Object);
        // act
        var itemId = existingItem.Id;
        var itemToUpdate = new UpdateItemDto(){
            Name=Guid.NewGuid().ToString(),
            Price = rand.Next(1000)
        };
        var result = await controller.UpdateItemAsync(itemId, itemToUpdate);

        // assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteItemAsync_WithExistingItemId_ReturnsNoContent()
    {
        // arrange
        Item existingItem = CreateRandomItem();
        repoStub.Setup(repo=>repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync(existingItem);
        var controller = new ItemsController(repoStub.Object, loggerStub.Object);
        // act
        var result = await controller.DeleteItemAsync(existingItem.Id);
        // assert
        result.Should().BeOfType<NoContentResult>();
    }

    private Item CreateRandomItem(){
        return new Item {
            Id=Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            Price = rand.Next(1000),
            CreatedDate = DateTimeOffset.UtcNow
        };
    }
}