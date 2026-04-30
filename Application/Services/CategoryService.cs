using Application.Common;
using Application.DTOs.CategoryDTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<Result<List<CategoryDto>>> GetAllAsync()
    {
        var categories = await categoryRepository.GetAllAsync();
        var dtos = categories.Select(MapToDto).ToList();
        return Result<List<CategoryDto>>.Success(dtos);
    }

    public async Task<Result<CategoryDto>> GetByIdAsync(Guid id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if (category is null)
            return Result<CategoryDto>.NotFound("Category not found.");

        return Result<CategoryDto>.Success(MapToDto(category));
    }

    public async Task<Result<CategoryDto>> CreateAsync(CreateCategoryDto dto)
    {
        if (await categoryRepository.ExistsByNameAsync(dto.Name))
            return Result<CategoryDto>.Failure("A category with this name already exists.");

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description
        };

        var created = await categoryRepository.CreateAsync(category);
        return Result<CategoryDto>.Success(MapToDto(created));
    }

    public async Task<Result<CategoryDto>> UpdateAsync(Guid id, UpdateCategoryDto dto)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if (category is null)
            return Result<CategoryDto>.NotFound("Category not found.");

        if (await categoryRepository.ExistsByNameAsync(dto.Name, excludeId: id))
            return Result<CategoryDto>.Failure("A category with this name already exists.");

        category.Name = dto.Name;
        category.Description = dto.Description;

        var updated = await categoryRepository.UpdateAsync(category);
        return Result<CategoryDto>.Success(MapToDto(updated));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if (category is null)
            return Result<bool>.NotFound("Category not found.");

        if (category.Courses.Any())
            return Result<bool>.Failure("Cannot delete a category that has courses assigned to it.");

        await categoryRepository.DeleteAsync(category);
        return Result<bool>.Success(true);
    }

    private static CategoryDto MapToDto(Category c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Description = c.Description,
        CourseCount = c.Courses?.Count ?? 0
    };
}
