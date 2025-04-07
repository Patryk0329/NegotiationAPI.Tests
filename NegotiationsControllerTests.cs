using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NegotiationAPI.Models;
using System.Net.Http.Json;
using System.Net;

namespace NegotiationAPI.Tests
{
    public class NegotiationsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public NegotiationsControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetNegotiationById_ReturnNegotiations_WhenExists()
        {
            //Arrange
            var negotiationId = 2;

            //Act
            var response = await _client.GetAsync($"/Negotiations/{negotiationId}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var negotiation = await response.Content.ReadFromJsonAsync<Negotiation>();
            negotiation.Should().NotBeNull();
            negotiation!.Id.Should().Be(negotiationId);
        }


        [Fact]
        public async Task StartNegotiation_ReturnsCreated_WhenValidRequest()
        {
            // Arrange
            var dto = new StartNegotiationDto
            {
                ProductId = 1,
                CustomerEmail = "test@example.com",
                OfferedPrice = 2500.00m
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Negotiations", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var negotiation = await response.Content.ReadFromJsonAsync<Negotiation>();
            negotiation.Should().NotBeNull();
            negotiation!.CustomerEmail.Should().Be(dto.CustomerEmail);
            negotiation.OfferedPrice.Should().Be(dto.OfferedPrice);
        }


        [Fact]
        public async Task StartNegotiation_ReturnsBadRequest_WhenProductIsInvalid()
        {
            // Arrange
            var dto = new StartNegotiationDto
            {
                ProductId = 999,
                CustomerEmail = "test@example.com",
                OfferedPrice = 1000.00m
            };

            // Act
            var response = await _client.PostAsJsonAsync("/Negotiations", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task RejectNegotiation_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            var dto = new RejectNegotiationDto
            {
                Reason = "Not enough margin"
            };
            var negotiationId = 2;

            // Act - no token
            var response = await _client.PatchAsJsonAsync($"/Negotiations/{negotiationId}/reject", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }


        [Fact]
        public async Task ProposeNewOffer_ReturnsBadRequest_WhenNewPriceIsHigherThanPrevious()
        {
            // Arrange
            var negotiationId = 3;
            var dto = new NewOfferDto
            {
                CustomerEmail = "customer3@customer.com",
                NewPrice = 1100.00m
            };

            // Act
            var response = await _client.PatchAsJsonAsync($"/Negotiations/{negotiationId}/reoffer", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

    }
}
