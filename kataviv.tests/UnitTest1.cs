using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace kataviv.tests
{
    public class AcceptanceTests
    {
        private TestServer server;
        private HttpClient client;

        [SetUp]
        public void Setup()
        {
            var factory = new WebApplicationFactory<Program>();
            server = factory.Server;
            client = server.CreateClient();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}