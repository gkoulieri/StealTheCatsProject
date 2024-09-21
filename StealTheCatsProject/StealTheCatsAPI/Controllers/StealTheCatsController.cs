using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StealTheCatsAPI.Data;
using StealTheCatsAPI.Models;
using System.Text.Json;

namespace StealTheCatsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StealTheCatsController : ControllerBase
    {
        private readonly CatsDataContext _catsDataContext;
        private readonly HttpClient _httpClient;

        // Constructor to inject dependencies
        public StealTheCatsController(CatsDataContext catsDataContext, HttpClient httpClient) =>
            (_catsDataContext, _httpClient) = (catsDataContext, httpClient);

        // Endpoint to fetch cat images 
        [HttpPost("fetch-cats")]
        public async Task<IActionResult> FetchCatImages()
        {
            const int requiredNumberOfCats = 25;
            var apiUrl = $"https://api.thecatapi.com/v1/images/search?limit={requiredNumberOfCats}";
            var response = await _httpClient.GetFromJsonAsync<List<CatImages>>(apiUrl);

            var currentCount = await _catsDataContext.Cats.CountAsync();

            if (response == null || response.Count == 0)
            {
                return BadRequest("Failed to fetch data from the Cat API.");
            }

            if (currentCount >= 25)
            {
                return BadRequest("The number of cat records in the database has reached the limit of 25.");
            }

            var catsToAdd = response.Take(25 - currentCount).ToList();

            // Add new cat images to the database
            foreach (var catImage in catsToAdd)
            {
                if (await _catsDataContext.Cats.CountAsync(c => c.CatId == catImage.Id) == 0)
                {
                    var newCat = new CatEntity
                    {
                        CatId = catImage.Id,
                        Width = catImage.Width,
                        Height = catImage.Height,
                        Image = catImage.Url,
                        Created = DateTime.UtcNow
                    };

                    _catsDataContext.Cats.Add(newCat);
                }
            }

            await _catsDataContext.SaveChangesAsync();
            return Ok("Cats fetched and stored successfully.");
        }

        // Endpoint to fetch and store cat breed tags
        [HttpPost("fetch-tags")]
        public async Task<IActionResult> FetchTags()
        {
            var apiUrl = "https://api.thecatapi.com/v1/breeds";
            var response = await _httpClient.GetFromJsonAsync<List<CatBreed>>(apiUrl);

            if (response == null || response.Count == 0)
            {
                return BadRequest("Failed to fetch data from the Cat API.");
            }

            // Get existing tags to avoid duplicates
            var existingTags = await _catsDataContext.Tags
                .AsNoTracking()
                .Select(t => t.Name.ToLower())
                .ToListAsync();

            var newTags = new List<TagEntity>();

            // Process each breed to extract tags
            foreach (var breed in response)
            {
                var tags = breed.Temperament?.Split(',')?.Select(tag => tag.Trim()).Where(tag => !string.IsNullOrEmpty(tag)).ToList() ?? new List<string>();

                foreach (var tagName in tags)
                {
                    var tagNameLower = tagName.ToLower();

                    if (!existingTags.Contains(tagNameLower))
                    {
                        var tag = new TagEntity
                        {
                            Name = tagName,
                            Created = DateTime.UtcNow
                        };

                        newTags.Add(tag);
                        existingTags.Add(tagNameLower); // Prevent future duplicates
                    }
                }
            }

            // Add new tags to the database if any
            if (newTags.Any())
            {
                _catsDataContext.Tags.AddRange(newTags);
                await _catsDataContext.SaveChangesAsync();
            }

            return Ok("Tags fetched and stored successfully.");
        }

        // Endpoint to link tags to existing cats
        [HttpPost("link-tags-to-cats")]
        public async Task<IActionResult> LinkTagsToCats()
        {
            var cats = await _catsDataContext.Cats.ToListAsync();
            var tags = await _catsDataContext.Tags.ToListAsync();

            if (cats.Count == 0 || tags.Count == 0)
            {
                return BadRequest("No cats or tags found.");
            }

            using var transaction = await _catsDataContext.Database.BeginTransactionAsync();

            try
            {
                var catTagEntities = new List<CatTagEntity>();

                // Create relationships between cats and tags
                foreach (var cat in cats)
                {
                    foreach (var tag in tags)
                    {
                        if (await _catsDataContext.CatTags.CountAsync(ct => ct.CatId == cat.Id && ct.TagId == tag.Id) == 0)
                        {
                            catTagEntities.Add(new CatTagEntity
                            {
                                CatId = cat.Id,
                                TagId = tag.Id,
                                Cat = cat,
                                Tag = tag
                            });
                        }
                    }
                }

                // Save new relationships if any
                if (catTagEntities.Count > 0)
                {
                    _catsDataContext.CatTags.AddRange(catTagEntities);
                    await _catsDataContext.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return Ok("Cat-Tag relationships created successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        // Endpoint to get a specific cat by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCatById(int id)
        {
            var cat = await _catsDataContext.Cats
                .Include(c => c.CatTags)
                .ThenInclude(ct => ct.Tag)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cat == null)
            {
                return NotFound();
            }

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
            };

            return new JsonResult(cat, jsonOptions);
        }

        // Endpoint for paginated cat retrieval
        [HttpGet("paged")]
        public async Task<IActionResult> GetCats([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Page number and page size must be greater than 0.");
            }

            var totalCount = await _catsDataContext.Cats.CountAsync();
            var cats = await _catsDataContext.Cats
                .Include(c => c.CatTags)
                .ThenInclude(ct => ct.Tag)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Cats = cats.Select(cat => new
                {
                    cat.Id,
                    cat.Width,
                    cat.Height,
                    cat.Image,
                    CatTags = cat.CatTags.Select(ct => new { ct.TagId, ct.Tag.Name }).ToList()
                }).ToList() 
            };

            return Ok(result);
        }

        // Endpoint to get cats filtered by tag
        [HttpGet("tag")]
        public async Task<IActionResult> GetCatsByTag([FromQuery] string tag, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var tagEntity = await _catsDataContext.Tags.FirstOrDefaultAsync(t => t.Name == tag);

            if (tagEntity == null)
            {
                return NotFound("Tag not found.");
            }

            var catIds = await _catsDataContext.CatTags
                .Where(ct => ct.TagId == tagEntity.Id)
                .Select(ct => ct.CatId)
                .ToListAsync();

            // Include both CatTags and the associated Tags
            var query = _catsDataContext.Cats
                .Include(c => c.CatTags)
                    .ThenInclude(ct => ct.Tag) // Include the Tag entity to fetch the name
                .Where(c => catIds.Contains(c.Id));

            var totalCount = await query.CountAsync();
            var cats = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return Ok(new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Cats = cats.Select(cat => new
                {
                    cat.Id,
                    cat.CatId,
                    cat.Width,
                    cat.Height,
                    cat.Image,
                    Created = cat.Created,
                    CatTags = cat.CatTags?.Select(ct => new
                    {
                        ct.TagId,
                        TagName = ct.Tag?.Name  
                    }).Where(tag => tag.TagName != null)  
                }).ToList()
            });
        }
    }
}
