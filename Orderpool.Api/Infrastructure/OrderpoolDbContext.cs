using DomainBase;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Orderpool.Api.Infrastructure
{
    public class OrderpoolDbContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;

        public OrderpoolDbContext(DbContextOptions<OrderpoolDbContext> options)
            : base(options)
        {
            
        }

        public OrderpoolDbContext(DbContextOptions<OrderpoolDbContext> options, IMediator mediator)
            : this(options)
        {
            _mediator = mediator;
        }


        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await _mediator.DispatchDomainEventsAsync(this);
            var result = await base.SaveChangesAsync(cancellationToken);

            return result > 0;
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, OrderpoolDbContext ctx)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}
