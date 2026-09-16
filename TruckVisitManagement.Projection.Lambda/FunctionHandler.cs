using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using TruckVisitManagement.Application.Commands.Abstractions;
using TruckVisitManagement.Application.Queries.Abstractions;
using TruckVisitManagement.Domain.VisitManagement.ValueObjects;

namespace TruckVisitManagement.Projection.Lambda
{
    public class FunctionHandler : IFunctionHandler
    {
        private readonly IVisitWriteStore _visitWriteStore;
        private readonly IVisitReadStore _visitReadStore;

        public FunctionHandler(
            IVisitWriteStore visitWriteStore,
            IVisitReadStore visitReadStore)
        {
            _visitWriteStore = visitWriteStore;
            _visitReadStore = visitReadStore;
        }

        public async Task HandleAsync(DynamoDBEvent streamEvent, ILambdaContext context)
        {
            ArgumentNullException.ThrowIfNull(streamEvent);

            foreach (var record in streamEvent.Records)
            {
                var visitId = ResolveVisitId(record);

                if (string.Equals(record.EventName?.Value, "REMOVE", StringComparison.OrdinalIgnoreCase))
                {
                    await _visitReadStore.DeleteAsync(visitId);
                    continue;
                }

                var visit = await _visitWriteStore.GetByIdAsync(visitId);
                if (visit is null)
                {
                    context.Logger.LogLine($"Visit '{visitId}' not found in DynamoDB. Deleting projection to keep index aligned.");
                    await _visitReadStore.DeleteAsync(visitId);
                    continue;
                }

                await _visitReadStore.UpsertAsync(visit);
            }
        }

        private static VisitId ResolveVisitId(DynamoDBEvent.DynamodbStreamRecord record)
        {
            var keys = record.Dynamodb.Keys;
            keys.TryGetValue("VisitId", out var visitIdAttribute);

            return VisitId.From(Guid.Parse(visitIdAttribute!.S));
        }
    }
}
