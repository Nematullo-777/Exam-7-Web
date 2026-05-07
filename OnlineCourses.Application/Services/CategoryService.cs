using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineCourses.Application.Common;
using OnlineCourses.Application.DTOs.Categories.Request;
using OnlineCourses.Application.DTOs.Categories.Response;
using OnlineCourses.Application.Interfaces.Repositories;
using OnlineCourses.Application.Interfaces.Services;
using OnlineCourses.Domain.Entities;
using OnlineCourses.Domain.Enums;

namespace OnlineCourses.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICacheService _cacheService;
    
    // Используем константу, чтобы не ошибиться в буквах
    private const string CacheKey = "all_categories";

    public CategoryService(ICategoryRepository categoryRepository, ICacheService cacheService)
    {
        _categoryRepository = categoryRepository;
        _cacheService = cacheService;
    }

    public async Task<Result<List<GetCategoryDto>>> GetAllCategoriesAsync()
    {
        // Пытаемся взять из кэша
        var cached = await _cacheService.GetAsync<List<GetCategoryDto>>(CacheKey);

        if (cached != null)
            return Result<List<GetCategoryDto>>.Ok(cached);

        // Если в кэше пусто — идем в базу
        var categories = await _categoryRepository.GetAllAsync();

        var result = categories.Select(c => new GetCategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        }).ToList();

        // Сохраняем в кэш на 10 минут
        await _cacheService.SetAsync(CacheKey, result, TimeSpan.FromMinutes(10));

        return Result<List<GetCategoryDto>>.Ok(result);
    }

    public async Task<Result<GetCategoryDto?>> GetCategoryByIdAsync(Guid id)
    {
        string singleCacheKey = $"category_{id}";
        var cached = await _cacheService.GetAsync<GetCategoryDto>(singleCacheKey);
        
        if (cached != null)
            return Result<GetCategoryDto?>.Ok(cached);

        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
            return Result<GetCategoryDto?>.Fail("Category not found", ErrorType.NotFound);

        var dto = new GetCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };

        await _cacheService.SetAsync(singleCacheKey, dto, TimeSpan.FromMinutes(10));

        return Result<GetCategoryDto?>.Ok(dto);
    }

    public async Task<Result<CreateCategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto request)
    {
        try
        {
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description
            };

            await _categoryRepository.AddAsync(category);

            // ОБЯЗАТЕЛЬНО: Очищаем общий список в кэше, чтобы новая категория появилась сразу
            await _cacheService.RemoveAsync(CacheKey);

            return Result<CreateCategoryResponseDto>.Ok(new CreateCategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            });
        }
        catch (Exception ex)
        {
            return Result<CreateCategoryResponseDto>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<UpdateCategoryResponseDto>> UpdateCategoryAsync(Guid id, UpdateCategoryDto request)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                return Result<UpdateCategoryResponseDto>.Fail("Category not found", ErrorType.NotFound);

            category.Name = request.Name;
            category.Description = request.Description;

            await _categoryRepository.UpdateAsync(category);

            // Очищаем и список, и конкретную категорию
            await _cacheService.RemoveAsync(CacheKey);
            await _cacheService.RemoveAsync($"category_{id}");

            return Result<UpdateCategoryResponseDto>.Ok(new UpdateCategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            });
        }
        catch (Exception ex)
        {
            return Result<UpdateCategoryResponseDto>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }

    public async Task<Result<DeleteCategoryResponseDto>> DeleteCategoryAsync(Guid id)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                return Result<DeleteCategoryResponseDto>.Fail("Not found");

            await _categoryRepository.DeleteAsync(category);

            // Очищаем кэш после удаления
            await _cacheService.RemoveAsync(CacheKey);
            await _cacheService.RemoveAsync($"category_{id}");

            return Result<DeleteCategoryResponseDto>.Ok(new DeleteCategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            });
        }
        catch (Exception ex)
        {
            return Result<DeleteCategoryResponseDto>.Fail($"Error: {ex.Message}", ErrorType.Unknown);
        }
    }
}