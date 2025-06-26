using AssetService.Infrastructure.Data;
using AssetService.Infrastructure.Entity;
using AssetService.Interface;
using SharedKernel.Logs;
using SharedKernel.Responses;
using System.Linq.Expressions;

namespace AssetService.Repository
{
    public class AssetRepository(AssetDbContext dbContext) : IAsset
    {
        public async Task<Response> CreateAsync(Asset entity)
        {
            try
            {
                var getAsset = await GetByAsync(a => a.AssetCode.Equals(entity.AssetCode));
                if (getAsset is not null && string.IsNullOrEmpty(entity.AssetCode)) 
                {
                    return new Response(false, $"Asset with {entity.AssetCode} code already exists.");
                }

                var currentEntity = dbContext.Assets.Add(entity).Entity;
                await dbContext.SaveChangesAsync();
                if(currentEntity is not null && currentEntity.Id.ToString() != "") 
                    return new Response(true, "Asset created successfully.");
                else 
                    return new Response(false, $"Error occured while creating {entity.AssetCode}");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, "Error occurred while creating the asset.");
            }
        }

        public async Task<Response> DeleteAsync(Asset entity)
        {
            //try
            //{

            //}
            //catch (Exception ex)
            //{
            //    LogException.LogExceptions(ex);
            //    return new Response(false, "Error occurred while deleting the asset.");
            //}
            throw new NotImplementedException("DeleteAsync method is not implemented yet.");
        }

        public async Task<Asset> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Asset>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Asset> GetByAsync(Expression<Func<Asset, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<Response> UpdateAsync(Asset entity)
        {
            //try
            //{

            //}
            //catch (Exception ex)
            //{
            //    LogException.LogExceptions(ex);
            //    return new Response(false, "Error occurred while deleting the asset.");
            //}
            throw new NotImplementedException();

        }
    }
}
