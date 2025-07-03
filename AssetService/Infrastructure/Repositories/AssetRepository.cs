using AssetService.Domain.Entities;
using AssetService.Domain.Interfaces;
using AssetService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Logs;
using SharedKernel.Responses;
using System.Linq.Expressions;

namespace AssetService.Infrastructure.Repositories
{
    public class AssetRepository : IAsset
    {
        private readonly AssetDbContext dbContext;
        private readonly ILogger<AssetRepository> logger;
        public AssetRepository(AssetDbContext dbContext, ILogger<AssetRepository> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<Response> CreateAsync(Asset entity)
        {
            try
            {
                var getAsset = await GetByAsync(a => a.AssetCode.Equals(entity.AssetCode));
                if (getAsset is not null)
                {
                    logger.LogWarning($"Asset with {entity.AssetCode} code already exists.");
                    return new Response(false, $"Asset with {entity.AssetCode} code already exists.");
                }

                var currentEntity = dbContext.Assets.Add(entity).Entity;
                await dbContext.SaveChangesAsync();
                if (currentEntity is not null && currentEntity.Id.ToString() != "")
                {
                    logger.LogInformation($"Asset {entity.AssetCode} created successfully.");
                    return new Response(true, "Asset created successfully.");
                }
                else
                {
                    logger.LogError($"Error occurred while creating {entity.AssetCode}.");
                    return new Response(false, $"Error occured while creating {entity.AssetCode}");
                }
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error occurred while creating the asset.");
            }
        }

        public async Task<Response> DeleteAsync(Asset entity)
        {
            try
            {
                var getAsset = await FindByIdAsync(entity.Id);
                if (getAsset is null)
                {
                    logger.LogWarning($"Asset with ID {entity.Id} does not exist.");
                    return new Response(false, $"Asset with ID {entity.Id} does not exist.");
                }
                dbContext.Assets.Remove(getAsset);
                await dbContext.SaveChangesAsync();
                logger.LogInformation($"Asset {entity.Id} deleted successfully.");
                return new Response(true, $"Asset {entity.Id} deleted successfully.");

            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error occurred while deleting the asset.");
            }
        }

        public async Task<Asset> FindByIdAsync(Guid id)
        {
            try
            {
                var asset = await dbContext.Assets.FindAsync(id);
                return asset is not null ? asset : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occurred while finding the asset by ID.");
            }
        }

        public async Task<IEnumerable<Asset>> GetAllAsync()
        {
            try
            {
                var assets = await dbContext.Assets.AsNoTracking().ToListAsync();
                return assets is not null ? assets : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occurred while retrieving all assets.");
            }
        }

        public async Task<Asset> GetByAsync(Expression<Func<Asset, bool>> predicate)
        {
            try
            {
                var asset = await dbContext.Assets.Where(predicate).AsNoTracking().FirstOrDefaultAsync();
                return asset is not null ? asset : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occurred while retrieving the asset by predicate.");
            }
        }

        public async Task<Response> UpdateAsync(Asset entity)
        {
            try
            {
                var asset = await FindByIdAsync(entity.Id);
                if (asset is null)
                {
                    logger.LogWarning($"Asset with ID {entity.Id} does not exist.");
                    return new Response(false, $"Asset with ID {entity.Id} does not exist.");
                }
                dbContext.Entry(asset).State = EntityState.Detached;
                dbContext.Assets.Update(entity);
                await dbContext.SaveChangesAsync();
                logger.LogInformation($"Asset {entity.Id} updated successfully.");
                return new Response(true, $"Asset {entity.Id} updated successfully.");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error occurred while updating the asset.");
            }
        }
    }
}
