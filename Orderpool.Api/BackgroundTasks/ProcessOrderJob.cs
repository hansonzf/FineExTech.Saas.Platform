using MediatR;
using Orderpool.Api.Models.OrderWatcherAggregate;
using Orderpool.Api.Pipeline;
using Quartz;

namespace Orderpool.Api.BackgroundTasks
{
    [DisallowConcurrentExecution]
    public class ProcessOrderJob : IJob
    {
        private readonly IOrderWatchRepository _repository;
        private readonly IMediator _mediator;

        public ProcessOrderJob(IOrderWatchRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            OrderWatcher standbyWatcher = default;

            while (true)
            {
                standbyWatcher = await _repository.NextAsync();
                if (standbyWatcher is null)
                    break;

                var ctx = new ProcessOrderContext(standbyWatcher);
                var result = await _mediator.Send(ctx);
                await _repository.SaveAsync(result);
            }
        }
    }
}
