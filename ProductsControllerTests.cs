using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NegotiationAPI.Models;
using System.Net.Http.Json;
using System.Net;

namespace NegotiationAPI.Tests
{
    public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ProductsControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllProducts_ReturnsProductList_WithStatus200()
        {
            // Act
            var response = await _client.GetAsync("/Products");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var products = await response.Content.ReadFromJsonAsync<List<Product>>();
            products.Should().NotBeNullOrEmpty();
            products.Should().Contain(p => p.ProductName == "Laptop");
        }

        [Fact]
        public async Task GetProductById_ReturnsProduct_WhenExists()
        {
            // Arrange
            var existingId = 1;

            // Act
            var response = await _client.GetAsync($"/Products/{existingId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var product = await response.Content.ReadFromJsonAsync<Product>();
            product.Should().NotBeNull();
            product.Id.Should().Be(existingId);
            product.ProductName.Should().Be("Laptop");
        }

        [Fact]
        public async Task GetProductById_Returns404_WhenProductNotFound()
        {
            // Arrange
            var nonExistentId = 999;

            // Act
            var response = await _client.GetAsync($"/Products/{nonExistentId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AddProduct_ReturnsUnauthorized_WhenNotAuthenticated()
        {
            // Arrange
            var newProduct = new CreateProductDto
            {
                ProductName = "Tablet",
                BasePrice = 1234.56m
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Products", newProduct);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }





    }
}
