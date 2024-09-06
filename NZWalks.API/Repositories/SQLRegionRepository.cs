using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Repositories
{
    public class SQLRegionRepository : IRegionRepository
    {
        private readonly NZWalksDbContext nZWalksDbContext;

        public SQLRegionRepository(NZWalksDbContext nZWalksDbContext)
        {
            this.nZWalksDbContext = nZWalksDbContext;
        }

        public async Task<List<Region>> GetAllAsync()
        {
            var region = await nZWalksDbContext.Regions.ToListAsync();
            return region;
        }

        public async Task<Region> GetByIdAsync(Guid Id)
        {
            var region= await nZWalksDbContext.Regions.FirstOrDefaultAsync(i=>i.Id==Id);
            return region;
        }

        public async Task<Region> CreateAsync(Region region)
        {

            //adding to db
            await nZWalksDbContext.Regions.AddAsync(region);
            await nZWalksDbContext.SaveChangesAsync();

            return region;
            
        }

        public async Task<Region?> UpdateAsync(Guid Id, Region region)
        {
           var existingRegion = await nZWalksDbContext.Regions.FirstOrDefaultAsync(i => i.Id == Id);
            if (existingRegion == null)
            {
                return null;
            }

            existingRegion.Code=region.Code;
            existingRegion.Name=region.Name;
            existingRegion.RegionImageUrl=region.RegionImageUrl;

            await nZWalksDbContext.SaveChangesAsync();

            return existingRegion;
        }

        public async Task<Region> DeleteAsync(Guid Id)
        {

            var region = await nZWalksDbContext.Regions.FirstOrDefaultAsync(i => i.Id == Id);
            if(region==null)
            {
                return null;
            }
            nZWalksDbContext.Regions.Remove(region);
            await nZWalksDbContext.SaveChangesAsync();


            return region;
        }
    }
}
