﻿using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_CouponAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _db;

        public CouponRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(Coupon coupon)
        {
            await _db.Coupons.AddAsync(coupon);
        }

        public async Task<ICollection<Coupon>> GetAllAsync()
        {
            return await _db.Coupons.ToListAsync();
        }

        public async Task<Coupon> GetAsync(int id)
        {
            return await _db.Coupons.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Coupon> GetAsync(string name)
        {
            return await _db.Coupons.FirstOrDefaultAsync(u => u.Name == name);
        }

        public async Task RemoveAsync(Coupon coupon)
        {
            _db.Coupons.Remove(coupon);
        }

        public async Task UpdateAsync(Coupon coupon)
        {
            _db.Coupons.Update(coupon);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
