using DomainBase;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LocationApi.Infrastructure
{
    public class LocationDbContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public LocationDbContext(DbContextOptions<LocationDbContext> options)
            : base(options)
        { }

        public LocationDbContext(
            DbContextOptions<LocationDbContext> options, 
            IMediator mediator, 
            ILogger<LocationDbContext> logger)
            : this(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger;
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _mediator.DispatchDomainEventsAsync(this);
                var result = await base.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }


    }
}
