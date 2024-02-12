using AutoMapper;
using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Models.DTO;
using MagicVilla_CouponAPI.Repository.IRepository;

namespace MagicVilla_CouponAPI.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _db;
        private IMapper _mapper;

        public AuthRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public bool IsUniqueUser(string username)
        {
            var user = _db.LocalUsers.FirstOrDefault(x => x.Username == username);

            if (user == null)
            {
                return true;
            }

            return false;
        }

        public Task<LoginResponseDto> Authenticate(LoginRequestDto loginRequestDto)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto> Register(RegistrationRequestDto requestDto)
        {
            LocalUser userObj = new() 
            { 
                Username = requestDto.Username,
                Password = requestDto.Password,
                Name = requestDto.Name,
                Role = "Admin"
            };

            _db.LocalUsers.Add(userObj);
            _db.SaveChanges();

            userObj.Password = "";

            return _mapper.Map<UserDto>(userObj);
        }
    }
}
