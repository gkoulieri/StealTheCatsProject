using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using StealTheCatsAPI.Controllers;
using StealTheCatsAPI.Data;
using StealTheCatsAPI.Models;

public class StealTheCatsControllerTests
{
    private readonly StealTheCatsController _controller; 
    private readonly CatsDataContext _catsDataContext; 

    public StealTheCatsControllerTests()
    {
        // Set up in-memory database options
        var options = new DbContextOptionsBuilder<CatsDataContext>()
            .UseInMemoryDatabase("CatDBTest") 
            .Options;

        _catsDataContext = new CatsDataContext(options); 
        _controller = new StealTheCatsController(_catsDataContext, new HttpClient()); // Initialize controller with context and HttpClient
    }

    [Fact]
    public async Task GetCatById_ShouldReturnCat_WhenExists()
    {
        var catId = 1; // ID of the cat to retrieve
        var catEntity = new CatEntity // Create a new CatEntity object
        {
            Id = catId,  
            CatId = "1",  
            Width = 640,  
            Height = 480,  
            Image = "http://example.com/cat1.jpg",  
            Created = DateTime.UtcNow  
        };

        // Add the cat entity to the in-memory database
        _catsDataContext.Cats.Add(catEntity);
        await _catsDataContext.SaveChangesAsync(); // Save changes to the database

        var result = await _controller.GetCatById(catId); // Call the method to get the cat by ID

        var jsonResult = Assert.IsType<JsonResult>(result);  
        var returnedCat = Assert.IsType<CatEntity>(jsonResult.Value); // Assert that the returned value is a CatEntity
        Assert.Equal(catId, returnedCat.Id); // Assert that the returned cat's ID matches the expected ID
    }
}
