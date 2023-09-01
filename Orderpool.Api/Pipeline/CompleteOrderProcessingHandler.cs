using MediatR;
using Orderpool.Api.Models;
using Orderpool.Api.Models.OrderWatcherAggregate;

namespace Orderpool.Api.Pipeline
{
    public class CompleteOrderProcessingHandler : IRequestHandler<ProcessOrderContext, OrderWatcher>
    {
        public Task<OrderWatcher> Handle(ProcessOrderContext request, CancellationToken cancellationToken)
        {
            var watcher = request.Watcher;
            watcher.Processed(request.HandleResult);

            return Task.FromResult(watcher);
        }
    }
}
