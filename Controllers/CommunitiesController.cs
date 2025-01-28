using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reddit;
using Reddit.Models;
using Reddit.Repositories;

namespace Reddit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommunityRepository _communityRepository;

        public CommunitiesController(ApplicationDbContext context, ICommunityRepository communityRepository)
        {
            _context = context;
            _communityRepository = communityRepository;
        }

        // GET: api/Communities
        [HttpGet]
        public async Task<PagedList<Community>> GetCommunities
            (
            int pageNumber = 1,
            int pageSize = 3,
            string? searchKey = null,
            string? sortKey = null,
            bool? isAscending = null
            )
        {
            return await _communityRepository.GetCommunities(pageNumber, pageSize, searchKey, sortKey, isAscending);
        }

        // GET: api/Communities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Community>> GetCommunity(int id)
        {
            var community = await _context.Communities.FindAsync(id);

            if (community == null)
            {
                return NotFound();
            }

            return community;
        }

        // PUT: api/Communities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommunity(int id, Community community)
        {
            if (id != community.Id)
            {
                return BadRequest();
            }

            _context.Entry(community).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommunityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Communities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Community>> PostCommunity(Community community)
        {
            _context.Communities.Add(community);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCommunity", new { id = community.Id }, community);
        }

        // DELETE: api/Communities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommunity(int id)
        {
            var community = await _context.Communities.FindAsync(id);
            if (community == null)
            {
                return NotFound();
            }

            _context.Communities.Remove(community);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommunityExists(int id)
        {
            return _context.Communities.Any(e => e.Id == id);
        }

        [HttpPost("JoinCommunity")]
        public async Task<IActionResult> JoinCommunity(int userID, int communityID)
        {
            var community = await _context.Communities.FindAsync(communityID);

            if (community == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(userID);

            if (user == null)
            {
                return NotFound();
            }

            user.SubscribedCommunities.Add(community);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
