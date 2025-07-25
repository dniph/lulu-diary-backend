using lulu_diary_backend.Models.API;
using lulu_diary_backend.Models.Database;
using lulu_diary_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace lulu_diary_backend.Controllers
{
    [ApiController]
    [Route("/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance.</param>
        /// <param name="repository">Products repository.</param>
        public ProductsController(ProductsRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Gets a list of all products.
        /// </summary>
        /// <returns>List of products.</returns>
        [HttpGet]
        public async Task<IActionResult> ListAsync()
        {
            var results = await _repository.GetProductsAsync();
            return Ok(results);
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="product">Product data transfer object.</param>
        /// <returns>Created product.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync(ProductDto product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            var result = await _repository.InsertProductAsync(product);
            return Ok(result);
        }

        /// <summary>
        /// Gets a product by its ID.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <returns>Product if found, otherwise NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var result = await _repository.GetProductAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <param name="product">Product update data transfer object.</param>
        /// <returns>Updated product if found, otherwise NotFound.</returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, ProductUpdateDto product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            var existingProduct = await _repository.UpdateProductAsync(id, product);
            if (existingProduct == null)
            {
                return NotFound();
            }

            return Ok(existingProduct);
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <returns>NoContent if deleted, otherwise NotFound.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var product = await _repository.DeleteProductAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
