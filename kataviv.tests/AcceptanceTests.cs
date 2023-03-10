using kataviv.Features.Ads;
using kataviv.Features.Weathers;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

#nullable disable

namespace kataviv.tests
{
    public class AcceptanceTests
    {
        private TestServer server;
        private HttpClient client;
        private Guid[] ids = new[] { Guid.NewGuid(), Guid.NewGuid(), };
        private Coordinates[] coords = { new(-30.75269, 152.24706), new(-2.70778, -78.86541) };
        private JsonSerializerOptions serializerOptions;

        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("Testing", "true");
            var factory = new WebApplicationFactory<Program>();
            server = factory.Server;
            client = server.CreateClient();

            using (var scope = factory.Services.CreateScope())
            {
                serializerOptions = scope.ServiceProvider.GetService<IOptions<JsonOptions>>()!.Value.SerializerOptions;

                var db = scope.ServiceProvider.GetService<AdDbContext>();
                db!.Reset();

                var repository = scope.ServiceProvider.GetService<AdRepository>();
                repository!.Seed(new Collection<Ad>
                {
                    new(ids[0], "Published house ad", "Published house ad description", coords[0], Adkind.House, AdStatus.Published),
                    new(ids[1], "Draft apartment ad", "Draft apartment ad description", coords[1], Adkind.Apartment, AdStatus.Draft),
                });
            }
        }

        [Test]
        public async Task ItGetsPublishedAds()
        {
            var expected = new AdOut(
                "Published house ad",
                "Published house ad description",
                new Coordinates(-30.75269, 152.24706),
                Adkind.House,
                new Weather(42, 1, 100, 90)
            );

            var response = await client.GetAsync(($"/ads/{ids[0]}"));
            Assert.That(response.IsSuccessStatusCode);

            var content = await response.Content.ReadFromJsonAsync<AdOut>(serializerOptions);
            Assert.That(content, Is.EqualTo(expected));
        }

        [Test]
        public async Task ItDoesNotGetDraftAds()
        {
            var response = await client.GetAsync(($"/ads/{ids[1]}"));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task ItCreatesDraftAd()
        {
            var payload = new AdIn
            {
                title = "My spot",
                description = "Superb spot",
                location = new(51.22292, -117.41451),
                type = Adkind.Apartment
            };

            var creatingResponse = await client.PostAsJsonAsync($"/ads", payload);
            Assert.That(creatingResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var id = await creatingResponse.Content.ReadFromJsonAsync<Guid>(serializerOptions);
            Assert.That(id, Is.Not.EqualTo(Guid.Empty));

            var retrievingResponse = await client.GetAsync($"ads/{id}");
            Assert.That(retrievingResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task ItPublishesAdToMakeItAccessible()
        {
            var creatingPayload = new AdIn
            {
                title = "My spot",
                description = "Superb spot",
                location = new Coordinates(-24.04950, -54.77016),
                type = Adkind.CarPark
            };

            var expected = new AdOut(
                "My spot",
                "Superb spot",
                new Coordinates(-24.04950, -54.77016),
                Adkind.CarPark,
                new Weather(42, 1, 100, 90)
            );

            var creatingResponse = await client.PostAsJsonAsync($"/ads", creatingPayload);
            Assert.That(creatingResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var id = await creatingResponse.Content.ReadFromJsonAsync<Guid>();

            var publishingPayload = AdStatus.Published;
            var retrievingResponse = await client.PutAsJsonAsync($"ads/{id}/status", publishingPayload);
            Assert.That(retrievingResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

            var gettingResponse = await client.GetAsync($"ads/{id}");
            Assert.That(gettingResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var result = await gettingResponse.Content.ReadFromJsonAsync<AdOut>(serializerOptions);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}