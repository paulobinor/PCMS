using pcms.Application.Dto;
using pcms.Domain.Entities;

namespace pcms.Application.Interfaces
{
    public interface IUserManagementService
    {
        Task<ApiResponse<tokenDto>> Login(UserLogin userLogin);
        Task<ApiResponse<string>> Register(RegisterUser registerUser);
    }
}