using MagicVilla_CouponAPI.Models.DTO;

namespace MagicVilla_CouponAPI.Repository.IRepository
{
    public interface IAuthRepository
    {
        bool IsUniqueUser(string username);
        Task<LoginResponseDto> Authenticate(LoginRequestDto loginRequestDto);
        Task<UserDto> Register(RegistrationRequestDto requestDto);
    }
}
