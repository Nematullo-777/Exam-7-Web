using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IStudentRepository
{
    Task<List<ApplicationUser>> GetAllStudentsAsync(int page, int pageSize);
    Task<ApplicationUser?> GetByIdAsync(string id);
    Task<int> GetTotalCountAsync();
    Task<ApplicationUser> UpdateAsync(ApplicationUser user);
    Task DeleteAsync(ApplicationUser user);
}
