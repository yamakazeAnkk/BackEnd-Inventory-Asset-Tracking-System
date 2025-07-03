using AssetService.Domain.Entities;
using SharedKernel.Responses;
using SharedKernel.Logs;
using System.Linq.Expressions;
using AssetService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AssetService.Domain.Interfaces;

namespace AssetService.Infrastructure.Repositories
{
    public class BrandRepository : IBrand
    {
        private readonly AssetDbContext dbContext;
        private readonly ILogger<BrandRepository> logger;

        public BrandRepository(AssetDbContext dbContext, ILogger<BrandRepository> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }
        public async Task<Response> CreateAsync(Brand entity)
        {
            try
            {
                var brand = await GetByAsync(b => b.BrandName.Equals(entity.BrandName));
                if (brand is not null)
                {
                    logger.LogWarning($"Brand with name {entity.BrandName} already exists.");
                    return new Response(false, $"Brand with name {entity.BrandName} already exists.");
                }
                var currentEntity = dbContext.Brands.Add(entity).Entity;
                await dbContext.SaveChangesAsync();
                if (currentEntity is not null && currentEntity.Id.ToString() != "")
                {
                    logger.LogInformation($"Brand {entity.BrandName} created successfully.");
                    return new Response(true, $"Brand {entity.BrandName} created successfully.");
                }
                else
                {
                    logger.LogError($"Error occurred while creating {entity.BrandName}.");
                    return new Response(false, $"Error occurred while creating {entity.BrandName}.");
                }
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error occurred while creating the brand.");
            }
        }

        public async Task<Response> DeleteAsync(Brand entity)
        {
            try
            {
                var brand = await FindByIdAsync(entity.Id);
                if (brand is null)
                {
                    logger.LogWarning($"Brand with ID {entity.Id} does not exist.");
                    return new Response(false, $"Brand with ID {entity.Id} does not exist.");
                }
                dbContext.Brands.Remove(brand);
                await dbContext.SaveChangesAsync();
                logger.LogInformation($"Brand {entity.BrandName} deleted successfully.");
                return new Response(true, $"Brand {entity.BrandName} deleted successfully.");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error occurred while deleting the brand.");
            }
        }

        public async Task<Brand> FindByIdAsync(Guid id)
        {
            try
            {
                var brand = await dbContext.Brands.FindAsync(id);
                return brand is not null ? brand : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occurred while finding the brand by ID.");
            }
        }

        public async Task<IEnumerable<Brand>> GetAllAsync()
        {
            try
            {
                var brands = await dbContext.Brands.AsNoTracking().ToListAsync();
                return brands is not null ? brands : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occurred while retrieving all brands.");
            }
        }

        public async Task<Brand> GetByAsync(Expression<Func<Brand, bool>> predicate)
        {
            try
            {
                var brand = await dbContext.Brands.Where(predicate).AsNoTracking().FirstOrDefaultAsync();
                return brand is not null ? brand : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occurred while retrieving the brand by predicate.");
            }
        }

        public async Task<Response> UpdateAsync(Brand entity)
        {
            try
            {
                var brand = await FindByIdAsync(entity.Id);
                if (brand is null)
                {
                    logger.LogWarning($"Brand with ID {entity.Id} does not exist.");
                    return new Response(false, $"Brand with ID {entity.Id} does not exist.");
                }
                dbContext.Entry(brand).State = EntityState.Detached;
                dbContext.Update(entity);
                await dbContext.SaveChangesAsync();
                logger.LogInformation($"Brand {entity.BrandName} updated successfully.");
                return new Response(true, $"Brand {entity.BrandName} updated successfully.");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error occurred while updating the brand.");
            }
        }
    }
}
