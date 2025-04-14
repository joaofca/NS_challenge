using CollisionEvents.Contracts.Infrastructure.Repositories;

namespace CollisionEvents.Infrastructure.Repositories
{
    public abstract class RepositoryBase : IRepositoryBase
    {
        protected AppDbContext _dbContext;

        protected RepositoryBase(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
