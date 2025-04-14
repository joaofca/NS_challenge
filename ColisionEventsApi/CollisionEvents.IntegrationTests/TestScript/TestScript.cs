using System.Text;
using System.Text.Json;
using AutoFixture;
using CollisionEvents.Contracts.Api.CollisionEvents.Requests;
using CollisionEvents.Contracts.Api.CollisionEvents.Responses;
using CollisionEvents.Contracts.Api.CollisionWarnings.Responses;
using CollisionEvents.Contracts.Api.Common;
using CollisionEvents.Contracts.Api.SatelliteOperators.Requests;
using CollisionEvents.Contracts.Api.SatelliteOperators.Responses;
using CollisionEvents.IntegrationTests.Common;
using FluentAssertions;

namespace CollisionEvents.IntegrationTests.TestScript
{
    /// <summary>
    /// This test was created with the sole purpose of testing this API, by building its in memory database step by step
    /// </summary>
    [Collection("WebClient")]
    public class TestScript
    {
        private WebClientFixture _webClientFixture;

        private const string OperatorId = "OP-123";
        private const string OperatorIdHeader = "OperatorId";

        private const string MessageId1 = "8f3d9f4a";
        private const string MessageId2 = "e9146f33";
        private const string MessageId3 = "d27bcb79";
        private const string MessageId4 = "d27bcb78";

        private const string SatelliteId1 = "SAT-1034";
        private const string SatelliteId2 = "SAT-2089";
        private const string SatelliteId3 = "SAT-5503";

        public TestScript(WebClientFixture webClientFixture)
        {
            _webClientFixture = webClientFixture;
        }

        [Fact]
        public async Task RunTestScript()
        {
            // Try to create a new satellite operator with an invalid email
            await CannotCreateInvalidOperatorWithInvalidEmail();

            // Create a new valid satellite operator
            await CreateValidOperator();

            // Try to create new collision event messages with invalid operator id
            await CannotCreateCollisionEventMessagesWithInvalidOperatorId();

            // Create new collision event messages
            await CreateCollisionEventMessages();

            // Get collision warnings
            await GetCollisionWarnings();

            // Try to delete collision messages with invalid operator id
            await CannotDeleteCollisionEventMessagesWithInvalidMessageIds();

            // Delete collision messages 
            await DeleteCollisionEventMessages();

            // Get collision warnings
            await GetCollisionWarningsAfterDeletionOfTwo();

            // Delete all collision messages 
            await DeleteAllCollisionEventMessages();

            // Get no collision warnings
            await GetCollisionWarningsAfterDeletionOfAll();
        }

        public async Task CannotCreateInvalidOperatorWithInvalidEmail()
        {
            Fixture specimenBuilders = new Fixture();

            var request = new AddSatelliteOperatorRequest
            {
                OperatorId = OperatorId,
                OperatorEmail = "invalid.email"
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/satelliteOperator")
            {
                Content = content
            };

            var result = await _webClientFixture.Client.SendAsync(requestMessage);

            var responseString = await result.Content.ReadAsStringAsync();

            var errorResult = JsonSerializer.Deserialize<List<ErrorDetail<AddSatelliteOperatorErrorCode>>>(responseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.UnprocessableEntity);
            errorResult!.Select(x => x.ErrorCode).Should().Contain(AddSatelliteOperatorErrorCode.InvalidOperatorEmail);
        }

        public async Task CreateValidOperator()
        {
            Fixture specimenBuilders = new Fixture();

            var request = new AddSatelliteOperatorRequest
            {
                OperatorId = OperatorId,
                OperatorEmail = "valid@email.net"
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/satelliteOperator")
            {
                Content = content
            };

            var result = await _webClientFixture.Client.SendAsync(requestMessage);

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        public async Task CannotCreateCollisionEventMessagesWithInvalidOperatorId()
        {
            Fixture specimenBuilders = new Fixture();

            AddCollisionEventsRequest[] addCollisionEventsRequests = specimenBuilders.CreateMany<AddCollisionEventsRequest>(4).ToArray();

            foreach (var item in addCollisionEventsRequests)
                item.CollisionProbability = 0.75f;

            var json = JsonSerializer.Serialize(addCollisionEventsRequests);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/collisionEvents")
            {
                Content = content
            };
            requestMessage.Headers.Add(OperatorIdHeader, OperatorId);

            var result = await _webClientFixture.Client.SendAsync(requestMessage);

            var responseString = await result.Content.ReadAsStringAsync();

            var errorResult = JsonSerializer.Deserialize<List<ErrorDetail<AddCollisionEventsErrorCode>>>(responseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.UnprocessableEntity);
            errorResult!.Select(x => x.ErrorCode).Should().Contain(AddCollisionEventsErrorCode.InvalidOperatorId);
        }

        public async Task CreateCollisionEventMessages()
        {
            Fixture specimenBuilders = new Fixture();

            var addCollisionEventsRequests = new AddCollisionEventsRequest[]
            {
                new AddCollisionEventsRequest
                {
                    MessageId = MessageId1,
                    CollisionEventId = specimenBuilders.Create<string>(),
                    SatelliteId = SatelliteId1,
                    OperatorId = OperatorId,
                    ChaserObjectId = specimenBuilders.Create<string>(),
                    CollisionDate = DateTimeOffset.Parse("2025-04-15T07:22:11.478Z"),
                    CollisionProbability = 0.763f
                },
                new AddCollisionEventsRequest
                {
                    MessageId = MessageId2,
                    CollisionEventId = specimenBuilders.Create<string>(),
                    SatelliteId = SatelliteId2,
                    OperatorId = OperatorId,
                    ChaserObjectId = specimenBuilders.Create<string>(),
                    CollisionDate = DateTimeOffset.Parse("2025-04-15T13:51:40.254Z"),
                    CollisionProbability = 0.788f
                },
                new AddCollisionEventsRequest
                {
                    MessageId = MessageId3,
                    CollisionEventId = specimenBuilders.Create<string>(),
                    SatelliteId = SatelliteId3,
                    OperatorId = OperatorId,
                    ChaserObjectId = specimenBuilders.Create<string>(),
                    CollisionDate = DateTimeOffset.Parse("2025-04-15T21:18:27.936Z"),
                    CollisionProbability = 0.895f
                },
                new AddCollisionEventsRequest
                {
                    MessageId = MessageId4,
                    CollisionEventId = specimenBuilders.Create<string>(),
                    SatelliteId = SatelliteId3,
                    OperatorId = OperatorId,
                    ChaserObjectId = specimenBuilders.Create<string>(),
                    CollisionDate = DateTimeOffset.Parse("2025-04-16T21:18:27.936Z"),
                    CollisionProbability = 0.895f
                }
            };

            var json = JsonSerializer.Serialize(addCollisionEventsRequests);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/collisionEvents")
            {
                Content = content
            };
            requestMessage.Headers.Add(OperatorIdHeader, OperatorId);

            var result = await _webClientFixture.Client.SendAsync(requestMessage);

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        public async Task GetCollisionWarnings()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/collisionWarning");
            requestMessage.Headers.Add(OperatorIdHeader, OperatorId);

            var result = await _webClientFixture.Client.SendAsync(requestMessage);

            var responseString = await result.Content.ReadAsStringAsync();

            var successResult = JsonSerializer.Deserialize<GetCollisionWarningsResponse>(responseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            successResult!.CollisionWarnings.Should().HaveCount(3);
            successResult.CollisionWarnings.Select(x => x.SatelliteId).Should().BeEquivalentTo([SatelliteId1, SatelliteId2, SatelliteId3]);
        }

        public async Task CannotDeleteCollisionEventMessagesWithInvalidMessageIds()
        {
            Fixture specimenBuilders = new Fixture();

            var deleteCollisionEventsRequest = specimenBuilders.Create<DeleteCollisionEventsRequest>();

            deleteCollisionEventsRequest.OperatorId = OperatorId;

            DeleteCollisionEventsRequest[] deleteCollisionEventsRequests = new DeleteCollisionEventsRequest[]
            {
                deleteCollisionEventsRequest
            };

            var json = JsonSerializer.Serialize(deleteCollisionEventsRequests);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/collisionEvents")
            {
                Content = content
            };
            requestMessage.Headers.Add(OperatorIdHeader, OperatorId);

            var result = await _webClientFixture.Client.SendAsync(requestMessage);

            var responseString = await result.Content.ReadAsStringAsync();

            var errorResult = JsonSerializer.Deserialize<List<ErrorDetail<DeleteCollisionEventsErrorCode>>>(responseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.UnprocessableEntity);
            errorResult!.Select(x => x.ErrorCode).Should().Contain(DeleteCollisionEventsErrorCode.InvalidMessageId);
        }

        public async Task DeleteCollisionEventMessages()
        {
            DeleteCollisionEventsRequest[] deleteCollisionEventsRequests = new DeleteCollisionEventsRequest[]
            {
                new DeleteCollisionEventsRequest
                {
                    MessageId = MessageId2,
                    OperatorId = OperatorId
                },
                new DeleteCollisionEventsRequest
                {
                    MessageId = MessageId3,
                    OperatorId = OperatorId
                }
            };

            var json = JsonSerializer.Serialize(deleteCollisionEventsRequests);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/collisionEvents")
            {
                Content = content
            };
            requestMessage.Headers.Add(OperatorIdHeader, OperatorId);

            var result = await _webClientFixture.Client.SendAsync(requestMessage);

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        public async Task GetCollisionWarningsAfterDeletionOfTwo()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/collisionWarning");
            requestMessage.Headers.Add(OperatorIdHeader, OperatorId);

            var result = await _webClientFixture.Client.SendAsync(requestMessage);

            var responseString = await result.Content.ReadAsStringAsync();

            var successResult = JsonSerializer.Deserialize<GetCollisionWarningsResponse>(responseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            successResult!.CollisionWarnings.Should().HaveCount(2);
            successResult.CollisionWarnings.Select(x => x.SatelliteId).Should().BeEquivalentTo([SatelliteId1, SatelliteId3]);
        }

        public async Task DeleteAllCollisionEventMessages()
        {
            DeleteCollisionEventsRequest[] deleteCollisionEventsRequests = new DeleteCollisionEventsRequest[]
            {
                new DeleteCollisionEventsRequest
                {
                    MessageId = MessageId1,
                    OperatorId = OperatorId
                },
                new DeleteCollisionEventsRequest
                {
                    MessageId = MessageId4,
                    OperatorId = OperatorId
                }
            };

            var json = JsonSerializer.Serialize(deleteCollisionEventsRequests);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/collisionEvents")
            {
                Content = content
            };
            requestMessage.Headers.Add(OperatorIdHeader, OperatorId);

            var result = await _webClientFixture.Client.SendAsync(requestMessage);

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        public async Task GetCollisionWarningsAfterDeletionOfAll()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/collisionWarning");
            requestMessage.Headers.Add(OperatorIdHeader, OperatorId);

            var result = await _webClientFixture.Client.SendAsync(requestMessage);

            var responseString = await result.Content.ReadAsStringAsync();

            var successResult = JsonSerializer.Deserialize<GetCollisionWarningsResponse>(responseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            successResult!.CollisionWarnings.Should().BeEmpty();
        }
    }
}