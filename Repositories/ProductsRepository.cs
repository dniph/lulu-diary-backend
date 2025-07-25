using lulu_diary_backend.Context;
using lulu_diary_backend.Models.API;
using lulu_diary_backend.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace lulu_diary_backend.Repositories
{
    public class ProductsRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductsRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsRepository"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="logger">Logger instance.</param>
        public ProductsRepository(AppDbContext context, ILogger<ProductsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets all products from the database.
        /// </summary>
        /// <returns>List of products.</returns>
        public async Task<IList<Product>> GetProductsAsync()
        {
            return await _context.Products.OrderBy(p => p.Id).ToListAsync();
        }

        /// <summary>
        /// Inserts a new product into the database.
        /// </summary>
        /// <param name="dto">Product data transfer object.</param>
        /// <returns>The created product.</returns>
        public async Task<Product> InsertProductAsync(ProductDto dto)
        {
            var product = new Product()
            {
                Name = dto.Name,
                Price = dto.Price,
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        /// <summary>
        /// Gets a product by its ID.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <returns>Product if found, otherwise null.</returns>
        public async Task<Product?> GetProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found.", id);
            }
            return product;
        }

        /// <summary>
        /// Updates an existing product in the database.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <param name="dto">Product update data transfer object.</param>
        /// <returns>Updated product if found, otherwise null.</returns>
        public async Task<Product?> UpdateProductAsync(int id, ProductUpdateDto dto)
        {
            var existingItem = await _context.Products.FindAsync(id);
            if (existingItem == null)
            {
                _logger.LogWarning("Product update failed: product with ID {ProductId} not found.", id);
                return null;
            }
            
            existingItem.Price = dto.Price;
            await _context.SaveChangesAsync();
            
            return existingItem;
        }

        /// <summary>
        /// Deletes a product from the database by its ID.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <returns>Deleted product if found, otherwise null.</returns>
        public async Task<Product?> DeleteProductAsync(int id)
        {
            var productItem = await _context.Products.FindAsync(id);
            if (productItem != null)
            {
                _context.Products.Remove(productItem);
                await _context.SaveChangesAsync();
                return productItem;
            }
            _logger.LogWarning("Product deletion failed: product with ID {ProductId} not found.", id);
            return null;
        }
    }
}
