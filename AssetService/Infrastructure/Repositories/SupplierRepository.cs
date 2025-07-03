using AssetService.Domain.Entities;
using AssetService.Domain.Interfaces;
using AssetService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Logs;
using SharedKernel.Responses;
using System.Linq.Expressions;

namespace AssetService.Infrastructure.Repositories
{
    public class SupplierRepository : ISupplier
    {
        private readonly AssetDbContext dbContext;
        private readonly ILogger<SupplierRepository> logger;
        public SupplierRepository(AssetDbContext dbContext, ILogger<SupplierRepository> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<Response> CreateAsync(Supplier entity)
        {
            try
            {
                var supplier = await GetByAsync(s => s.SupplierName.Equals(entity.SupplierName));
                if (supplier is not null)
                {
                    logger.LogWarning($"Supplier with name {entity.SupplierName} already exists.");
                    return new Response(false, $"Supplier with name {entity.SupplierName} already exists.");
                }
                var currentEntity = dbContext.Suppliers.Add(entity).Entity;
                await dbContext.SaveChangesAsync();
                if (currentEntity is not null && currentEntity.Id != Guid.Empty)
                {
                    logger.LogInformation($"Supplier {entity.SupplierName} created successfully.");
                    return new Response(true, $"Supplier {entity.SupplierName} created successfully.");
                }
                else
                {
                    logger.LogError($"Error occurred while creating {entity.SupplierName}.");
                    return new Response(false, $"Error occurred while creating {entity.SupplierName}.");
                }
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error occurred while creating the supplier.");
            }
        }

        public async Task<Response> DeleteAsync(Supplier entity)
        {
            try
            {
                var supplier = await FindByIdAsync(entity.Id);
                if (supplier is null)
                {
                    logger.LogWarning($"Supplier with ID {entity.Id} does not exist.");
                    return new Response(false, $"Supplier with ID {entity.Id} does not exist.");
                }
                dbContext.Suppliers.Remove(supplier);
                await dbContext.SaveChangesAsync();
                logger.LogInformation($"Supplier {entity.Id} deleted successfully.");
                return new Response(true, $"Supplier {entity.Id} deleted successfully.");
            } 
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error occurred while deleting the supplier.");
            }
        }

        public async Task<Supplier> FindByIdAsync(Guid id)
        {
            try
            {
                var supplier = await dbContext.Suppliers.FindAsync(id);
                return supplier is not null ? supplier : null!;
            } 
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occurred while finding the supplier by ID.");
            }

        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            try
            {
                var suppliers = await dbContext.Suppliers.AsNoTracking().ToListAsync();
                return suppliers is not null ? suppliers : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occurred while retrieving all suppliers.");
            }
        }

        public async Task<Supplier> GetByAsync(Expression<Func<Supplier, bool>> predicate)
        {
            try
            {
                var supplier = await dbContext.Suppliers.AsNoTracking().FirstOrDefaultAsync(predicate);
                return supplier is not null ? supplier : null!;
            } 
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occurred while retrieving the supplier by predicate.");
            }
        }

        public async Task<Response> UpdateAsync(Supplier entity)
        {
            try
            {
                var supplier = await FindByIdAsync(entity.Id);
                if (supplier is null)
                {
                    logger.LogWarning($"Supplier with ID {entity.Id} does not exist.");
                    return new Response(false, $"Supplier with ID {entity.Id} does not exist.");
                }
                dbContext.Entry(supplier).State = EntityState.Detached;
                dbContext.Suppliers.Update(entity);
                await dbContext.SaveChangesAsync();
                logger.LogInformation($"Supplier {entity.SupplierName} updated successfully.");
                return new Response(true, $"Supplier {entity.SupplierName} updated successfully.");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error occurred while updating the supplier.");
            }
        }
    }
}

