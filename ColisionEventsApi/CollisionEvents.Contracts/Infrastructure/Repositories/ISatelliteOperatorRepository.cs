using CollisionEvents.Domain.Entities;

namespace CollisionEvents.Contracts.Infrastructure.Repositories
{
    public interface ISatelliteOperatorRepository : IRepositoryBase
    {
        Task<SatelliteOperator?> GetSatelliteOperatorAsync(string operatorId);

        Task AddSatelliteOperator(SatelliteOperator newSatelliteOperator);
    }
}
