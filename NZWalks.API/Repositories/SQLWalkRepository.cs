using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class SQLWalkRepository : IWalkRepository
    {
        private readonly NZWalksDbContext nZWalksDbContext;

        public SQLWalkRepository(NZWalksDbContext nZWalksDbContext)
        {
            this.nZWalksDbContext = nZWalksDbContext;
        }
        public async Task<Walk> CreateAsync(Walk walk)
        {
            var createdwalk = await nZWalksDbContext.Walks.AddAsync(walk);
            await nZWalksDbContext.SaveChangesAsync();
            return walk;
        }

        public async Task<List<Walk>> GetAllAsync(string? filterOn=null,string? filterQuery=null,
            string? sortBy=null,bool isAscending = true,int pageNumber=1,int pageSize=1000)
        {
            var walks = nZWalksDbContext.Walks.Include("Difficulty").Include("Region").AsQueryable();

            //filtering
            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if(filterOn.Equals("Name",StringComparison.OrdinalIgnoreCase))
                {
                    walks = walks.Where(x => x.Name.Contains(filterQuery));
                }

            }
            //sorting
            if(!string.IsNullOrWhiteSpace(sortBy))
            {
                if(sortBy.Equals("Name",StringComparison.OrdinalIgnoreCase))
                {
                    walks= isAscending? walks.OrderBy(x=>x.Name) : walks.OrderByDescending(x=>x.Name);
                }
                else if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(x => x.LengthInKm) : walks.OrderByDescending(x => x.LengthInKm);

                }
            }

            //pagination
            var skipResults = (pageNumber - 1) * pageSize;
            
            return await walks.Skip(skipResults).Take(pageSize).ToListAsync();
        }

        public async Task<Walk?> GetByIdAsync(Guid Id)
        {
            var walk = await nZWalksDbContext.Walks.FirstOrDefaultAsync(i => i.Id == Id);
            return walk;
            
        }

        public async Task<Walk?> UpdateAsync(Guid Id, Walk walk)
        {
            var currentWalk=await nZWalksDbContext.Walks.FirstOrDefaultAsync(i=>i.Id==Id);
            if(currentWalk==null)
            {
                return null;
            }
            currentWalk.Name=walk.Name;
            currentWalk.Description=walk.Description;
            currentWalk.LengthInKm=walk.LengthInKm;
            currentWalk.WalkImageUrl=walk.WalkImageUrl;
            currentWalk.DifficultyId=walk.DifficultyId;
            currentWalk.RegionId=walk.RegionId;
            await nZWalksDbContext.SaveChangesAsync();

            return currentWalk;
        }


        public async Task<Walk?> DeleteAsync(Guid Id)
        {
            var walk = await nZWalksDbContext.Walks.FirstOrDefaultAsync(i=>i.Id==Id);
            if(walk==null)
            {
                return null;
            }
            nZWalksDbContext.Walks.Remove(walk);
            await nZWalksDbContext.SaveChangesAsync();
            return walk;
        }

    }
}
