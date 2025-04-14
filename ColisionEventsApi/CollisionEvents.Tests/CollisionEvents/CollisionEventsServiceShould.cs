using AutoFixture;
using CollisionEvents.Application.CollisionEvents;
using CollisionEvents.Contracts.Api.CollisionEvents.Requests;
using CollisionEvents.Contracts.Api.CollisionEvents.Responses;
using CollisionEvents.Contracts.Api.Common;
using CollisionEvents.Contracts.Infrastructure.Repositories;
using CollisionEvents.Domain.Dto;
using CollisionEvents.Domain.Entities;
using FluentAssertions;
using Moq;

namespace CollisionEvents.UnitTests.CollisionEvents
{
    public class CollisionEventsServiceShould
    {
        [Theory]
        [MemberData(nameof(ValidateAddCollisionEventsTestCases))]
        public async Task ValidateAddCollisionEvents(ValidateAddCollisionEventsTestCase testCase)
        {
            // Arrange
            var sut = new TestClass_CollisionEventsService(testCase.CollisionEventMessageRepository, testCase.SatelliteOperatorRepository);

            // Act
            var result = await sut.TestMethod_ValidateAddCollisionEventsRequestAsync(testCase.OperatorId, testCase.CollisionEventsToAdd);

            // Assert
            result.Should().BeEquivalentTo(testCase.ExpectedResult);
        }

        public static IEnumerable<object[]> ValidateAddCollisionEventsTestCases()
        {
            Fixture specimenBuilders = new();

            DateTimeOffset ancientDateTimeOffset = new DateTimeOffset(2000, 4, 13, 14, 30, 0, TimeSpan.FromHours(-3));
            DateTimeOffset futureDateTimeOffset = new DateTimeOffset(3000, 4, 13, 14, 30, 0, TimeSpan.FromHours(-3));

            string operatorId = specimenBuilders.Create<string>();

            Mock<ICollisionEventMessageRepository> collisionEventMessageRepositoryWithNoDuplicatesMock = new Mock<ICollisionEventMessageRepository>();
            collisionEventMessageRepositoryWithNoDuplicatesMock.Setup(x => x.ValidateExistingMessageIdsAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(Array.Empty<OperatorMessageIds>());

            Mock<ISatelliteOperatorRepository> satelliteOperatorRepositoryWithOperatorMock = new Mock<ISatelliteOperatorRepository>();
            satelliteOperatorRepositoryWithOperatorMock.Setup(x => x.GetSatelliteOperatorAsync(It.IsAny<string>())).ReturnsAsync(specimenBuilders.Create<SatelliteOperator>());

            #region RequestWithDuplicatedMessageIds error code

            string messageId = specimenBuilders.Create<string>();

            var requestsWithDuplicatedMessageIds = specimenBuilders.CreateMany<AddCollisionEventsRequest>(4);

            foreach (var request in requestsWithDuplicatedMessageIds)
            {
                request.OperatorId = operatorId;
                request.MessageId = messageId;
                request.CollisionDate = futureDateTimeOffset;
            }

            var expectedResult = new List<ErrorDetail<AddCollisionEventsErrorCode>>
            {
                new ErrorDetail<AddCollisionEventsErrorCode>(AddCollisionEventsErrorCode.RequestWithDuplicatedMessageIds, messageId)
            };

            yield return new object[]
            {
                 new ValidateAddCollisionEventsTestCase(
                     operatorId,
                     collisionEventMessageRepositoryWithNoDuplicatesMock.Object,
                     satelliteOperatorRepositoryWithOperatorMock.Object,
                     requestsWithDuplicatedMessageIds,
                     new VoidOrErrorResponse<AddCollisionEventsErrorCode>(expectedResult))
            };

            #endregion RequestWithDuplicatedMessageIds error code

            #region InvalidCollisionDate error code

            var requestsWithExpiredDates = specimenBuilders.CreateMany<AddCollisionEventsRequest>(4);

            foreach (var request in requestsWithExpiredDates)
            {
                request.OperatorId = operatorId;
                request.CollisionDate = ancientDateTimeOffset;
            }

            var expectedResult2 = new List<ErrorDetail<AddCollisionEventsErrorCode>>
            {
                new ErrorDetail<AddCollisionEventsErrorCode>(AddCollisionEventsErrorCode.InvalidCollisionDate, string.Join(";", requestsWithExpiredDates.Select(x => x.MessageId).OrderBy(x => x)))
            };

            yield return new object[]
            {
                new ValidateAddCollisionEventsTestCase(
                    operatorId,
                    collisionEventMessageRepositoryWithNoDuplicatesMock.Object,
                    satelliteOperatorRepositoryWithOperatorMock.Object,
                    requestsWithExpiredDates,
                    new VoidOrErrorResponse<AddCollisionEventsErrorCode>(expectedResult2))
            };

            #endregion InvalidCollisionDate error code

            #region MessageIdAlreadyReceived error code

            var requestsWithMessageIdAlreadyReceived = specimenBuilders.CreateMany<AddCollisionEventsRequest>(4);

            foreach (var request in requestsWithMessageIdAlreadyReceived)
            {
                request.OperatorId = operatorId;
                request.CollisionDate = futureDateTimeOffset;
            }

            var receivedMessageId = requestsWithMessageIdAlreadyReceived.First().MessageId;

            var expectedResult3 = new List<ErrorDetail<AddCollisionEventsErrorCode>>
            {
                new ErrorDetail<AddCollisionEventsErrorCode>(AddCollisionEventsErrorCode.MessageIdAlreadyReceived, receivedMessageId)
            };

            Mock<ICollisionEventMessageRepository> collisionEventMessageRepositoryWithReceivedMessageIdMock = new Mock<ICollisionEventMessageRepository>();
            collisionEventMessageRepositoryWithReceivedMessageIdMock.Setup(x => x.ValidateExistingMessageIdsAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync([new OperatorMessageIds { MessageId = receivedMessageId, OperatorId = operatorId }]);

            yield return new object[]
            {
                new ValidateAddCollisionEventsTestCase(
                    operatorId,
                    collisionEventMessageRepositoryWithReceivedMessageIdMock.Object,
                    satelliteOperatorRepositoryWithOperatorMock.Object,
                    requestsWithMessageIdAlreadyReceived,
                    new VoidOrErrorResponse<AddCollisionEventsErrorCode>(expectedResult3))
            };

            #endregion MessageIdAlreadyReceived error code

            #region InvalidOperatorId error code (the operator can only send his own messages)

            var requestsWithOtherOperatorIds = specimenBuilders.CreateMany<AddCollisionEventsRequest>(4);

            foreach (var request in requestsWithOtherOperatorIds)
            {
                request.CollisionDate = futureDateTimeOffset;
            }

            var expectedResult4 = new List<ErrorDetail<AddCollisionEventsErrorCode>>
            {
                new ErrorDetail<AddCollisionEventsErrorCode>(AddCollisionEventsErrorCode.InvalidOperatorId, operatorId)
            };

            yield return new object[]
            {
                new ValidateAddCollisionEventsTestCase(
                    operatorId,
                    collisionEventMessageRepositoryWithNoDuplicatesMock.Object,
                    satelliteOperatorRepositoryWithOperatorMock.Object,
                    requestsWithOtherOperatorIds,
                    new VoidOrErrorResponse<AddCollisionEventsErrorCode>(expectedResult4))
            };

            #endregion InvalidOperatorId error code (the operator can only send his own messages)

            #region InvalidOperatorId error code (invalid operator id)

            var requestsWithValidData = specimenBuilders.CreateMany<AddCollisionEventsRequest>(4);

            foreach (var request in requestsWithValidData)
            {
                request.CollisionDate = futureDateTimeOffset;
            }

            var expectedResult5 = new List<ErrorDetail<AddCollisionEventsErrorCode>>
            {
                new ErrorDetail<AddCollisionEventsErrorCode>(AddCollisionEventsErrorCode.InvalidOperatorId, operatorId)
            };

            Mock<ISatelliteOperatorRepository> satelliteOperatorRepositoryWithNoOperatorMock = new Mock<ISatelliteOperatorRepository>();
            satelliteOperatorRepositoryWithNoOperatorMock.Setup(x => x.GetSatelliteOperatorAsync(It.IsAny<string>())).ReturnsAsync((SatelliteOperator?)null);

            yield return new object[]
            {
                new ValidateAddCollisionEventsTestCase(
                    operatorId,
                    collisionEventMessageRepositoryWithNoDuplicatesMock.Object,
                    satelliteOperatorRepositoryWithNoOperatorMock.Object,
                    requestsWithValidData,
                    new VoidOrErrorResponse<AddCollisionEventsErrorCode>(expectedResult5))
            };

            #endregion InvalidOperatorId error code (invalid operator id)
        }

        [Theory]
        [MemberData(nameof(ValidateDeleteCollisionEventsTestCases))]

        public async Task ValidateDeleteCollisionEvents(ValidateDeleteCollisionEventsTestCase testCase)
        {
            // Arrange
            var sut = new TestClass_CollisionEventsService(testCase.CollisionEventMessageRepository, testCase.SatelliteOperatorRepository);

            // Act
            var result = await sut.TestMethod_ValidateDeleteCollisionEventsAsync(testCase.OperatorId, testCase.CollisionEventsToAdd);

            // Assert
            result.Should().BeEquivalentTo(testCase.ExpectedResult);
        }

        public static IEnumerable<object[]> ValidateDeleteCollisionEventsTestCases()
        {
            Fixture specimenBuilders = new();

            string operatorId = specimenBuilders.Create<string>();

            Mock<ISatelliteOperatorRepository> satelliteOperatorRepositoryWithOperatorMock = new Mock<ISatelliteOperatorRepository>();
            satelliteOperatorRepositoryWithOperatorMock.Setup(x => x.GetSatelliteOperatorAsync(It.IsAny<string>())).ReturnsAsync(specimenBuilders.Create<SatelliteOperator>());

            var requestsFromTheOperator = specimenBuilders.CreateMany<DeleteCollisionEventsRequest>(4);

            foreach (var request in requestsFromTheOperator)
                request.OperatorId = operatorId;

            #region InvalidOperatorId error code (the operator can only request to delete his own messages)

            var requestsWithOtherOperatorIds = specimenBuilders.CreateMany<DeleteCollisionEventsRequest>(4);

            Mock<ICollisionEventMessageRepository> collisionEventMessageRepositoryMock = new Mock<ICollisionEventMessageRepository>();
            collisionEventMessageRepositoryMock.Setup(x => x.ValidateExistingMessageIdsAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(requestsWithOtherOperatorIds.Select(x => new OperatorMessageIds { MessageId = x.MessageId, OperatorId = x.OperatorId }).ToArray());

            var expectedResult = new List<ErrorDetail<DeleteCollisionEventsErrorCode>>
            {
                new ErrorDetail<DeleteCollisionEventsErrorCode>(DeleteCollisionEventsErrorCode.InvalidOperatorId, operatorId)
            };

            yield return new object[]
            {
                new ValidateDeleteCollisionEventsTestCase(
                    operatorId,
                    collisionEventMessageRepositoryMock.Object,
                    satelliteOperatorRepositoryWithOperatorMock.Object,
                    requestsWithOtherOperatorIds,
                    new VoidOrErrorResponse<DeleteCollisionEventsErrorCode>(expectedResult))
            };

            #endregion InvalidOperatorId error code (the operator can only request to delete his own messages)

            #region InvalidOperatorId error code (the operator can only delete his own messages)

            Mock<ICollisionEventMessageRepository> collisionEventMessageRepositoryFromOtherOperatorsMock = new Mock<ICollisionEventMessageRepository>();
            collisionEventMessageRepositoryFromOtherOperatorsMock.Setup(x => x.ValidateExistingMessageIdsAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(requestsFromTheOperator.Select(x => new OperatorMessageIds { MessageId = x.MessageId, OperatorId = specimenBuilders.Create<string>() }).ToArray());

            var expectedResult3 = new List<ErrorDetail<DeleteCollisionEventsErrorCode>>
            {
                new ErrorDetail<DeleteCollisionEventsErrorCode>(DeleteCollisionEventsErrorCode.InvalidOperatorId, operatorId)
            };

            yield return new object[]
            {
                new ValidateDeleteCollisionEventsTestCase(
                    operatorId,
                    collisionEventMessageRepositoryFromOtherOperatorsMock.Object,
                    satelliteOperatorRepositoryWithOperatorMock.Object,
                    requestsFromTheOperator,
                    new VoidOrErrorResponse<DeleteCollisionEventsErrorCode>(expectedResult3))
            };

            #endregion InvalidOperatorId error code (the operator can only delete his own messages)

            #region InvalidOperatorId error code (non existant operator)

            var requestsWithValidData = specimenBuilders.CreateMany<DeleteCollisionEventsRequest>(4);

            var expectedResult1 = new List<ErrorDetail<DeleteCollisionEventsErrorCode>>
            {
                new ErrorDetail<DeleteCollisionEventsErrorCode>(DeleteCollisionEventsErrorCode.InvalidOperatorId, operatorId)
            };

            Mock<ISatelliteOperatorRepository> satelliteOperatorRepositoryWithNoOperatorMock = new Mock<ISatelliteOperatorRepository>();
            satelliteOperatorRepositoryWithNoOperatorMock.Setup(x => x.GetSatelliteOperatorAsync(It.IsAny<string>())).ReturnsAsync((SatelliteOperator?)null);

            yield return new object[]
            {
                new ValidateDeleteCollisionEventsTestCase(
                    operatorId,
                    collisionEventMessageRepositoryMock.Object,
                    satelliteOperatorRepositoryWithNoOperatorMock.Object,
                    requestsWithValidData,
                    new VoidOrErrorResponse<DeleteCollisionEventsErrorCode>(expectedResult1))
            };

            #endregion InvalidOperatorId error code (non existant operator)

            #region RequestWithDuplicatedMessageIds error code 

            var requestsWithDuplicatedIds = specimenBuilders.CreateMany<DeleteCollisionEventsRequest>(4);

            var messageId = specimenBuilders.Create<string>();

            foreach (var request in requestsWithDuplicatedIds)
            {
                request.OperatorId = operatorId;
                request.MessageId = messageId;
            }

            var expectedResult2 = new List<ErrorDetail<DeleteCollisionEventsErrorCode>>
            {
                new ErrorDetail<DeleteCollisionEventsErrorCode>(DeleteCollisionEventsErrorCode.RequestWithDuplicatedMessageIds, requestsWithDuplicatedIds.First().MessageId)
            };

            Mock<ICollisionEventMessageRepository> collisionEventMessageRepositoryDuplicatedMessagesMock = new Mock<ICollisionEventMessageRepository>();
            collisionEventMessageRepositoryDuplicatedMessagesMock.Setup(x => x.ValidateExistingMessageIdsAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(requestsWithDuplicatedIds.Select(x => new OperatorMessageIds { MessageId = x.MessageId, OperatorId = operatorId }).ToArray());

            yield return new object[]
            {
                new ValidateDeleteCollisionEventsTestCase(
                    operatorId,
                    collisionEventMessageRepositoryDuplicatedMessagesMock.Object,
                    satelliteOperatorRepositoryWithOperatorMock.Object,
                    requestsWithDuplicatedIds,
                    new VoidOrErrorResponse<DeleteCollisionEventsErrorCode>(expectedResult2))
            };

            #endregion RequestWithDuplicatedMessageIds error code

            #region InvalidMessageId error code

            Mock<ICollisionEventMessageRepository> collisionEventMessageRepositoryFromTheOperatorsMock = new Mock<ICollisionEventMessageRepository>();
            collisionEventMessageRepositoryFromTheOperatorsMock.Setup(x => x.ValidateExistingMessageIdsAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(Array.Empty<OperatorMessageIds>());

            var expectedResult4 = new List<ErrorDetail<DeleteCollisionEventsErrorCode>>
            {
                new ErrorDetail<DeleteCollisionEventsErrorCode>(DeleteCollisionEventsErrorCode.InvalidMessageId, string.Join(";", requestsFromTheOperator.Select(x => x.MessageId).OrderBy(x => x)))
            };

            yield return new object[]
            {
                new ValidateDeleteCollisionEventsTestCase(
                    operatorId,
                    collisionEventMessageRepositoryFromTheOperatorsMock.Object,
                    satelliteOperatorRepositoryWithOperatorMock.Object,
                    requestsFromTheOperator,
                    new VoidOrErrorResponse<DeleteCollisionEventsErrorCode>(expectedResult4))
            };

            #endregion InvalidMessageId error code
        }
    }

    #region Auxilliary classes

    public class ValidateAddCollisionEventsTestCase
    {
        public string OperatorId { get; init; }

        public ICollisionEventMessageRepository CollisionEventMessageRepository { get; init; }

        public ISatelliteOperatorRepository SatelliteOperatorRepository { get; init; }

        public IEnumerable<AddCollisionEventsRequest> CollisionEventsToAdd { get; init; }

        public VoidOrErrorResponse<AddCollisionEventsErrorCode> ExpectedResult { get; init; }

        public ValidateAddCollisionEventsTestCase(
            string operatorId,
            ICollisionEventMessageRepository collisionEventMessageRepository,
            ISatelliteOperatorRepository satelliteOperatorRepository,
            IEnumerable<AddCollisionEventsRequest> collisionEventsToAdd,
            VoidOrErrorResponse<AddCollisionEventsErrorCode> expectedResult)
        {
            OperatorId = operatorId;
            CollisionEventMessageRepository = collisionEventMessageRepository;
            SatelliteOperatorRepository = satelliteOperatorRepository;
            CollisionEventsToAdd = collisionEventsToAdd;
            ExpectedResult = expectedResult;
        }
    }

    public class ValidateDeleteCollisionEventsTestCase
    {
        public string OperatorId { get; init; }

        public ICollisionEventMessageRepository CollisionEventMessageRepository { get; init; }

        public ISatelliteOperatorRepository SatelliteOperatorRepository { get; init; }

        public IEnumerable<DeleteCollisionEventsRequest> CollisionEventsToAdd { get; init; }

        public VoidOrErrorResponse<DeleteCollisionEventsErrorCode> ExpectedResult { get; init; }

        public ValidateDeleteCollisionEventsTestCase(
            string operatorId,
            ICollisionEventMessageRepository collisionEventMessageRepository,
            ISatelliteOperatorRepository satelliteOperatorRepository,
            IEnumerable<DeleteCollisionEventsRequest> collisionEventsToDelete,
            VoidOrErrorResponse<DeleteCollisionEventsErrorCode> expectedResult)
        {
            OperatorId = operatorId;
            CollisionEventMessageRepository = collisionEventMessageRepository;
            SatelliteOperatorRepository = satelliteOperatorRepository;
            CollisionEventsToAdd = collisionEventsToDelete;
            ExpectedResult = expectedResult;
        }
    }

    public class TestClass_CollisionEventsService(
        ICollisionEventMessageRepository collisionEventMessageRepository,
        ISatelliteOperatorRepository satelliteOperatorRepository)
        : CollisionEventsService(collisionEventMessageRepository, satelliteOperatorRepository)
    {
        public async Task<VoidOrErrorResponse<AddCollisionEventsErrorCode>> TestMethod_ValidateAddCollisionEventsRequestAsync(string operatorId, IEnumerable<AddCollisionEventsRequest> collisionEventsToAdd)
            => await ValidateAddCollisionEventsRequestAsync(operatorId, collisionEventsToAdd);

        public async Task<VoidOrErrorResponse<DeleteCollisionEventsErrorCode>> TestMethod_ValidateDeleteCollisionEventsAsync(string operatorId, IEnumerable<DeleteCollisionEventsRequest> collisionEventsToDelete)
            => await ValidateDeleteCollisionEventsAsync(operatorId, collisionEventsToDelete);
    }

    #endregion Auxilliary classes
}