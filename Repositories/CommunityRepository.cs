using System.Linq.Expressions;
using System;
using Reddit.Models;

namespace Reddit.Repositories
{
    public class CommunityRepository : ICommunityRepository
    {
        private readonly ApplicationDbContext _context;

        public CommunityRepository(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<PagedList<Community>> GetCommunities(int pageNumber, int pageSize, string? searchKey = null, string? sortKey = null, bool? isAscending = null)
        {
            var communities = _context.Communities.AsQueryable();

            if (searchKey != null)
            {
                communities = communities.Where(c => c.Name.Contains(searchKey) || c.Description.Contains(searchKey));
            }

            //Sort
            if (isAscending != null)
            {
                if (isAscending == true)
                {
                    communities = communities.OrderBy(GetSortExpression(sortKey));
                }
                else
                {
                    communities = communities.OrderByDescending(GetSortExpression(sortKey));
                }
            }
            else
            {
                communities = communities.OrderBy(GetSortExpression("id"));
            }

            return await PagedList<Community>.CreateAsync(communities, pageNumber, pageSize);
        }

        public Task<PagedList<Community>> GetCommunities(int pageNumber, int pageSize, string? searchTerm, string? sortTerm = null, bool isAscending = true)
        {
            throw new NotImplementedException();
        }

        private Expression<Func<Community, object>> GetSortExpression(string? sortKey)
        {
            sortKey = sortKey?.ToLower();

            return sortKey switch
            {

                "createdat" => com => com.CreateAt,
                "postscount" => com => com.Posts.Count,
                "subscriberscount" => com => com.Subscribers.Count,
                _ => com => com.Id
            };
        }
    }
}