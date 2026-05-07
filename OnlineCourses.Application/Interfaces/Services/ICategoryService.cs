using System;
using OnlineCourses.Application.Common;
using OnlineCourses.Application.DTOs.Categories.Request;
using OnlineCourses.Application.DTOs.Categories.Response;

namespace OnlineCourses.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<Result<List<GetCategoryDto>>> GetAllCategoriesAsync();
    Task<Result<GetCategoryDto?>> GetCategoryByIdAsync(Guid id);
    Task<Result<CreateCategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto request);
    Task<Result<UpdateCategoryResponseDto>> UpdateCategoryAsync(Guid id, UpdateCategoryDto request);
    Task<Result<DeleteCategoryResponseDto>> DeleteCategoryAsync(Guid id);
}
