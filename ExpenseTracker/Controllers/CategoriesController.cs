namespace ExpenseTracker.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using ExpenseTracker.Services;

    /// <summary>
    /// Controller that exposes CRUD endpoints for `Category` resources.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoriesController"/> class.
        /// </summary>
        /// <param name="service">The category service used to perform data operations.</param>
        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves all categories.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Category}"/> containing all categories.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> Get()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        /// <summary>
        /// Retrieves a specific category by identifier.
        /// </summary>
        /// <param name="id">The identifier of the category to retrieve.</param>
        /// <returns>The requested <see cref="Category"/> if found; otherwise a 404 Not Found.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="category">The category object to create.</param>
        /// <returns>201 Created with the created category, or 400 Bad Request if the model is invalid.</returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<ActionResult<Category>> Create([FromBody] Category category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(category);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="id">The identifier of the category to update. Must match <paramref name="category"/>.Id.</param>
        /// <param name="category">The updated category object.</param>
        /// <returns>204 No Content on success, 400 Bad Request for validation or id mismatch, or 404 Not Found if the category does not exist.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Category category)
        {
            if (id != category.Id) return BadRequest("Id mismatch");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _service.UpdateAsync(category);
            if (!result) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Deletes a category by identifier.
        /// </summary>
        /// <param name="id">The identifier of the category to delete.</param>
        /// <returns>204 No Content on success or 404 Not Found if the category does not exist.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
