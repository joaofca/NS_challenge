using AutoFixture;
using AutoFixture.AutoMoq;
using CollisionEvents.Application.CollisionWarnings;
using CollisionEvents.Contracts.Api.CollisionWarnings.Responses;
using CollisionEvents.Contracts.Api.Common;
using CollisionEvents.Contracts.Infrastructure.Repositories;
using CollisionEvents.Domain.Entities;
using FluentAssertions;

namespace CollisionEvents.UnitTests.CollisionWarnings
{
    public class CollisionWarningsServiceShould
    {
        [Theory]
        [MemberData(nameof(BuildGetCollisionWarningsTestCases))]
        public async Task BuildGetCollisionWarnings(BuildGetCollisionWarningsResponseTestCase testCase)
        {
            // Arrange
            var specimenBuilders = new Fixture().Customize(new AutoMoqCustomization());
            var sut = specimenBuilders.Create<TestClass_CollisionWarningsService>();

            // Act
            var result = sut.TestMethod_BuildGetCollisionWarningsResponse(testCase.CollisionEventMessages);

            // Assert
            result.Should().BeEquivalentTo(testCase.ExpectedResult);
        }

        public static IEnumerable<object[]> BuildGetCollisionWarningsTestCases()
        {
            Fixture fixture = new Fixture();

            DateTime closeFutureDateTime = DateTime.Now.AddMinutes(5);
            DateTime notSoCloseFutureDateTime = DateTime.Now.AddMinutes(10);

            DateTimeOffset closeFutureDateTimeOffset = new DateTimeOffset(closeFutureDateTime.Year, closeFutureDateTime.Month, closeFutureDateTime.Day, closeFutureDateTime.Hour, closeFutureDateTime.Minute, closeFutureDateTime.Second, TimeSpan.FromHours(-3));
            DateTimeOffset notSoCloseFutureDateTimeOffset = new DateTimeOffset(notSoCloseFutureDateTime.Year, notSoCloseFutureDateTime.Month, notSoCloseFutureDateTime.Day, notSoCloseFutureDateTime.Hour, notSoCloseFutureDateTime.Minute, notSoCloseFutureDateTime.Second, TimeSpan.FromHours(-3));
            DateTimeOffset futureDateTimeOffset = new DateTimeOffset(3000, 4, 13, 14, 30, 0, TimeSpan.FromHours(-3));

            var satellite1 = fixture.Create<string>();
            var satellite2 = fixture.Create<string>();

            var chaserObject = fixture.Create<string>();

            var CollisionEventMessages = new CollisionEventMessage[]
            {
                // Valid
                new CollisionEventMessage
                {
                    ChaserObjectId = chaserObject,
                    CollisionDate = notSoCloseFutureDateTime,
                    CollisionEventId = fixture.Create<string>(),
                    CollisionProbability = 0.8f,
                    IsDeleted = false,
                    MessageId = fixture.Create<string>(),
                    OperatorId = fixture.Create<string>(),
                    SatelliteId = satellite1
                },
                // Valid but in the future
                new CollisionEventMessage
                {
                    ChaserObjectId = fixture.Create<string>(),
                    CollisionDate = futureDateTimeOffset,
                    CollisionEventId = fixture.Create<string>(),
                    CollisionProbability = 1f,
                    IsDeleted = false,
                    MessageId = fixture.Create<string>(),
                    OperatorId = fixture.Create<string>(),
                    SatelliteId = satellite1
                },
                // Valid
                new CollisionEventMessage
                {
                    ChaserObjectId = chaserObject,
                    CollisionDate = notSoCloseFutureDateTime,
                    CollisionEventId = fixture.Create<string>(),
                    CollisionProbability = 0.75f,
                    IsDeleted = false,
                    MessageId = fixture.Create<string>(),
                    OperatorId = fixture.Create<string>(),
                    SatelliteId = satellite2
                },
                // Valid but in the future
                new CollisionEventMessage
                {
                    ChaserObjectId = fixture.Create<string>(),
                    CollisionDate = futureDateTimeOffset,
                    CollisionEventId = fixture.Create<string>(),
                    CollisionProbability = 1f,
                    IsDeleted = false,
                    MessageId = fixture.Create<string>(),
                    OperatorId = fixture.Create<string>(),
                    SatelliteId = satellite2
                },
            };

            var expectedResults = new GetCollisionWarningsResponse
            {
                CollisionWarnings = new List<GetCollisionWarningsResponseDetails>
                {
                    new GetCollisionWarningsResponseDetails
                    {
                        ChaserObjectId = chaserObject,
                        EarliestColisionDate = notSoCloseFutureDateTime,
                        HighestCollisionProbability = 0.8f,
                        SatelliteId = satellite1
                    },
                    new GetCollisionWarningsResponseDetails
                    {
                        ChaserObjectId = chaserObject,
                        EarliestColisionDate = notSoCloseFutureDateTime,
                        HighestCollisionProbability = 0.75f,
                        SatelliteId = satellite2
                    }
                }
            };

            yield return new object[]
            {
                new BuildGetCollisionWarningsResponseTestCase(CollisionEventMessages, expectedResults)
            };
        }

        public void ValidateRequest()
        {
            // same type of validation tests as in CollisionEventsServiceShould
            // test operatorId validity
        }
    }

    #region Auxiliary classes

    public class BuildGetCollisionWarningsResponseTestCase
    {
        public CollisionEventMessage[] CollisionEventMessages { get; init; }

        public GetCollisionWarningsResponse ExpectedResult { get; init; }

        public BuildGetCollisionWarningsResponseTestCase(
            CollisionEventMessage[] collisionEventMessages,
            GetCollisionWarningsResponse expectedResult)
        {
            CollisionEventMessages = collisionEventMessages;
            ExpectedResult = expectedResult;
        }
    }

    public class TestClass_CollisionWarningsService(
        ICollisionEventMessageRepository collisionEventMessageRepository,
        ISatelliteOperatorRepository satelliteOperatorRepository) : CollisionWarningsService(collisionEventMessageRepository, satelliteOperatorRepository)
    {
        public GetCollisionWarningsResponse TestMethod_BuildGetCollisionWarningsResponse(CollisionEventMessage[] collisionEventMessages)
            => BuildGetCollisionWarningsResponse(collisionEventMessages);

        public async Task<VoidOrErrorResponse<GetCollisionWarningsErrorCode>> TestMethod_ValidateRequestAsync(string operatorId)
            => await ValidateRequestAsync(operatorId);
    }

    #endregion Auxiliary classes
}
