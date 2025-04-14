using Microsoft.AspNetCore.Mvc.Testing;


namespace CollisionEvents.IntegrationTests.Common
{
    [CollectionDefinition("WebClient")]
    public class WebClientFixtureCollection : ICollectionFixture<WebClientFixture>
    {

    }

    public class WebClientFixture
    {
        public HttpClient Client { get; }


        public WebClientFixture()
        {
            WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>();
            Client = factory.CreateClient();
        }
    }
}
