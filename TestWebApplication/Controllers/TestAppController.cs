using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestWebApplication.models;

namespace TestWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreeNodesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public TreeNodesController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetNodes()
        {
            return Ok(await _dbContext.TreeNodes.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNode(int id)
        {
            var node = await _dbContext.TreeNodes.FindAsync(id);
            if (node == null)
            {
                return NotFound();
            }
            return Ok(node);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNode(TreeNode node)
        {
            _dbContext.TreeNodes.Add(node);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetNode), new { id = node.Id }, node);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNode(int id, TreeNode node)
        {
            if (id != node.Id)
            {
                return BadRequest();
            }

            _dbContext.Entry(node).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TreeNodeExists(id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNode(int id)
        {
            var node = await _dbContext.TreeNodes.FindAsync(id);
            if (node == null)
            {
                return NotFound();
            }

            if (node.Children.Any())
            {
                throw new SecureException("You have to delete all children nodes first");
            }

            _dbContext.TreeNodes.Remove(node);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool TreeNodeExists(int id)
        {
            return _dbContext.TreeNodes.Any(e => e.Id == id);
        }
    }
}
