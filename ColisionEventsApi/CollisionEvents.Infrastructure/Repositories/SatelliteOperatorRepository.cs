using CollisionEvents.Contracts.Infrastructure.Repositories;
using CollisionEvents.Domain.Entities;

namespace CollisionEvents.Infrastructure.Repositories
{
    public class SatelliteOperatorRepository : RepositoryBase, ISatelliteOperatorRepository
    {
        public SatelliteOperatorRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task<SatelliteOperator?> GetSatelliteOperatorAsync(string operatorId)
            => await _dbContext.SatelliteOperators.FindAsync(operatorId);

        public async Task AddSatelliteOperator(SatelliteOperator newSatelliteOperator)
            => await _dbContext.AddAsync(newSatelliteOperator);
    }
}
