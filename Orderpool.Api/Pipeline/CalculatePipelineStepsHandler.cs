using MediatR;
using Orderpool.Api.Models;
using Orderpool.Api.Models.OrderWatcherAggregate;

namespace Orderpool.Api.Pipeline
{
    public class CalculatePipelineStepsHandler : IPipelineBehavior<ProcessOrderContext, OrderWatcher>
    {
        public Task<OrderWatcher> Handle(ProcessOrderContext request, RequestHandlerDelegate<OrderWatcher> next, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
