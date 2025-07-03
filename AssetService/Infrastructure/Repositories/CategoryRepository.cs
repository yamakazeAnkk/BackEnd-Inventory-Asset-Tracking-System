using AssetService.Domain.Entities;
using AssetService.Domain.Interfaces;
using AssetService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Logs;
using SharedKernel.Responses;
using System.Linq.Expressions;

namespace AssetService.Infrastructure.Repositories
{
    public class CategoryRepository : ICategory
    {
        private readonly AssetDbContext dbContext;
        private readonly ILogger<CategoryRepository> logger;
        public CategoryRepository(AssetDbContext dbContext, ILogger<CategoryRepository> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }
        public async Task<Response> CreateAsync(Category entity)
        {
            try
            {
                var category = await GetByAsync(c => c.CategoryName.Equals(entity.CategoryName));
                if (category is not null)
                {
                    logger.LogWarning($"Category with name {entity.CategoryName} already exists.");
                    return new Response(false, $"Category with name {entity.CategoryName} already exists.");
                }
                var currentEntity = dbContext.Categories.Add(entity).Entity;
                await dbContext.SaveChangesAsync();
                if (currentEntity is not null && currentEntity.Id != Guid.Empty)
                {
                    logger.LogInformation($"Category {entity.CategoryName} created successfully.");
                    return new Response(true, $"Category {entity.CategoryName} created successfully.");
                }
                else
                {
                    logger.LogError($"Error occurred while creating {entity.CategoryName}.");
                    return new Response(false, $"Error occurred while creating {entity.CategoryName}.");
                }
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error occurred while creating the category.");
            }
        }

        public async Task<Response> DeleteAsync(Category entity)
        {
            try
            {
                var category = await FindByIdAsync(entity.Id);
                if (category is null)
                {
                    logger.LogWarning($"Category with ID {entity.Id} does not exist.");
                    return new Response(false, $"Category with ID {entity.Id} does not exist.");
                }
                dbContext.Categories.Remove(category);
                await dbContext.SaveChangesAsync();
                logger.LogInformation($"Category {entity.Id} deleted successfully.");
                return new Response(true, $"Category {entity.Id} deleted successfully.");
            }
            catch
            (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error occurred while deleting the category.");
            }
        }

        public async Task<Category> FindByIdAsync(Guid id)
        {
            try
            {
                var category = await dbContext.Categories.FindAsync(id);
                return category is not null ? category : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occurred while finding the category by ID.");
            }
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            try
            {
                var categories = await dbContext.Categories.AsNoTracking().ToListAsync();
                return categories is not null ? categories : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occurred while retrieving all categories");
            }
        }

        public async Task<Category> GetByAsync(Expression<Func<Category, bool>> predicate)
        {
            try
            {
                var category = await dbContext.Categories.Where(predicate).AsNoTracking().FirstOrDefaultAsync();
                return category is not null ? category : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("Error occurred while retrieving the category by predicate");
            }
        }

        public async Task<Response> UpdateAsync(Category entity)
        {
            try
            {
                var category = await FindByIdAsync(entity.Id);
                if (category is null)
                {
                    logger.LogWarning($"Category with ID {entity.Id} does not exist.");
                    return new Response(false, $"Category with ID {entity.Id} does not exist.");
                }
                dbContext.Entry(category).State = EntityState.Detached;
                dbContext.Categories.Update(entity);
                await dbContext.SaveChangesAsync();
                logger.LogInformation($"Category {entity.CategoryName} updated successfully.");
                return new Response(true, $"Category {entity.CategoryName} updated successfully.");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error occurred while updating the category.");
            }
        }
    }
}
