using MagicVilla_CouponAPI.Models;

namespace MagicVilla_CouponAPI.Repository.IRepository
{
    public interface ICouponRepository
    {
        Task<ICollection<Coupon>> GetAllAsync();
        Task<Coupon> GetAsync(int id);
        Task<Coupon> GetAsync(string name);
        Task CreateAsync(Coupon coupon);
        Task UpdateAsync(Coupon coupon);
        Task RemoveAsync(Coupon coupon);
        Task SaveAsync();
    }
}
