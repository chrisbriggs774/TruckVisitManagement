using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;

namespace TruckVisitManagement.Projection.Lambda
{
    public interface IFunctionHandler
    {
        Task HandleAsync(DynamoDBEvent streamEvent, ILambdaContext context);
    }
}