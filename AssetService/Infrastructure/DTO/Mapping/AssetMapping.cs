using AssetService.Infrastructure.Entity;

namespace AssetService.Infrastructure.DTO.Mapping
{
    public static class AssetMapping
    {
        public static Asset ToEntity(AssetDTO asset) => new Asset()
        {
            Id = asset.Id,
            AssetCode = asset.AssetCode,
            AssetName = asset.AssetName,
            Status = asset.Status,
            IsFaulty = asset.IsFaulty,
        };

        public static (AssetDTO?, IEnumerable<AssetDTO>?) FromEntity(Asset asset, IEnumerable<Asset>? assets)
        {
            // return single
            if(asset is not null || assets is null)
            {
                var singleAsset = new AssetDTO(
                    Id: asset!.Id,
                    AssetCode: asset.AssetCode,
                    AssetName: asset.AssetName,
                    Status: asset.Status,
                    IsFaulty: asset.IsFaulty
                );
                return (singleAsset, null);
            }

            // return multiple
            if(assets is not null || asset is null)
            {
                var _assets = assets!.Select(a => new AssetDTO(
                    Id: a.Id,
                    AssetCode: a.AssetCode,
                    AssetName: a.AssetName,
                    Status: a.Status,
                    IsFaulty: a.IsFaulty
                )).ToList();
                return (null, _assets);
            }

            return (null, null);    
        }

    }
}
