using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task<List<Category>> GetAllAsync() =>
        await context.Categories
            .Include(c => c.Courses)
            .OrderBy(c => c.Name)
            .ToListAsync();

    public async Task<Category?> GetByIdAsync(Guid id) =>
        await context.Categories
            .Include(c => c.Courses)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null) =>
        await context.Categories
            .AnyAsync(c => c.Name == name && (excludeId == null || c.Id != excludeId));

    public async Task<Category> CreateAsync(Category category)
    {
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        context.Categories.Update(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteAsync(Category category)
    {
        context.Categories.Remove(category);
        await context.SaveChangesAsync();
    }
}
