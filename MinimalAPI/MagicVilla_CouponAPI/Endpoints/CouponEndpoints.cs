using AutoMapper;
using FluentValidation;
using MagicVilla_CouponAPI.Models.DTO;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Repository.IRepository;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MagicVilla_CouponAPI.Endpoints
{
    public static class CouponEndpoints
    {
        public static void ConfigureCouponEnpoints(this WebApplication app)
        {
            app.MapGet("/api/couopon", GetAllCoupons)
                .WithName("GetCoupons").Produces<ApiResponse>(200).Produces(400)
                .RequireAuthorization("AdminOnly");

            app.MapGet("/api/couopon{id:int}", GetCoupon)
                .WithName("GetCoupon").Produces<ApiResponse>(200).Produces(400)
                .AddEndpointFilter(async (context, next) =>
                {
                    var id = context.GetArgument<int>(2);
                    if (id == 0)
                    {
                        return Results.BadRequest("Cannot have 0 in id.");
                    }

                    Console.WriteLine("Before filter");

                    var result = await next(context);

                    Console.WriteLine("After filter");

                    return result;
                }).AddEndpointFilter(async (context, next) =>
                {
                    Console.WriteLine("Before 2nd filter");

                    var result = await next(context);

                    Console.WriteLine("After 2nd filter");

                    return result;
                });

            app.MapPost("/api/coupon", CreateCoupon)
                .WithName("CreateCoupon").Accepts<CouponCreateDto>("application/json").Produces<ApiResponse>(201).Produces(400);

            app.MapPut("/api/coupon", UpdateCoupon)
                .WithName("UpdateCoupon").Accepts<CouponUpdateDto>("application/json").Produces<ApiResponse>(200).Produces(400);

            app.MapDelete("/api/coupon{id:int}", DeleteCoupon)
                .WithName("DeleteCoupon").Produces<ApiResponse>(204).Produces(400);
        }

        private async static Task<IResult> GetCoupon(ICouponRepository _couponRepo, int id)
        {
            Console.WriteLine("Endpoint Executed");

            ApiResponse response = new();
            response.Result = await _couponRepo.GetAsync(id);
            response.isSuccessful = true;
            response.StatusCode = HttpStatusCode.OK;

            return Results.Ok(response);
        }

        [Authorize]
        private async static Task<IResult> CreateCoupon([FromBody] CouponCreateDto couponCreateDto, 
            ICouponRepository _couponRepo, IMapper _mapper, IValidator<CouponCreateDto> _validation)
        {
            ApiResponse response = new() { isSuccessful = false, StatusCode = HttpStatusCode.BadRequest };

            var validationResult = await _validation.ValidateAsync(couponCreateDto);
            if (!validationResult.IsValid)
            {
                response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
                return Results.BadRequest(response);
            }
            if (await _couponRepo.GetAsync(couponCreateDto.Name.ToLower()) != null)
            {
                response.ErrorMessages.Add("Coupon name already exists.");
                return Results.BadRequest(response);
            }

            Coupon coupon = _mapper.Map<Coupon>(couponCreateDto);
            coupon.Created = DateTime.Now;
            coupon.LastUpdated = DateTime.Now;

            await _couponRepo.CreateAsync(coupon);
            await _couponRepo.SaveAsync();

            CouponDto couponDto = _mapper.Map<CouponDto>(coupon);

            response.Result = couponDto;
            response.isSuccessful = true;
            response.StatusCode = HttpStatusCode.Created;

            return Results.Ok(response);

            //return Results.Created($"/api/coupon{coupon.Id}", coupon);
            //return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, couponDto);
        }

        [Authorize]
        private async static Task<IResult> UpdateCoupon([FromBody] CouponUpdateDto couponUpdateDto, 
            ICouponRepository _couponRepo, IMapper _mapper, IValidator<CouponUpdateDto> _validation)
        {
            ApiResponse response = new() { isSuccessful = false, StatusCode = HttpStatusCode.BadRequest };

            var validationResult = await _validation.ValidateAsync(couponUpdateDto);
            if (!validationResult.IsValid)
            {
                response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
                return Results.BadRequest(response);
            }

            Coupon coupon = await _couponRepo.GetAsync(couponUpdateDto.Id);//FIX put method
            if (coupon == null)
            {
                response.ErrorMessages.Add("Coupon doesn't exist");
                return Results.BadRequest(response);
            }

            await _couponRepo.UpdateAsync(_mapper.Map<Coupon>(couponUpdateDto));
            await _couponRepo.SaveAsync();

            response.Result = _mapper.Map<CouponDto>(await _couponRepo.GetAsync(coupon.Id));
            response.isSuccessful = true;
            response.StatusCode = HttpStatusCode.OK;

            return Results.Ok(response);
        }

        [Authorize]
        private async static Task<IResult> DeleteCoupon(ICouponRepository _couponRepo, int id)
        {
            ApiResponse response = new() { isSuccessful = false, StatusCode = HttpStatusCode.BadRequest };

            Coupon coupon = await _couponRepo.GetAsync(id);
            if (coupon == null)
            {
                response.ErrorMessages.Add("Coupon doesn't exist");
                return Results.BadRequest(response);
            }

            await _couponRepo.RemoveAsync(coupon);
            await _couponRepo.SaveAsync();

            response.isSuccessful = true;
            response.StatusCode = HttpStatusCode.NoContent;
            return Results.Ok(response);
        }

        private async static Task<IResult> GetAllCoupons(ICouponRepository _couponRepo, ILogger<Program> _logger)
        {
            ApiResponse response = new();

            _logger.Log(LogLevel.Information, "Getting all coupons.");

            response.Result = await _couponRepo.GetAllAsync();
            response.isSuccessful = true;
            response.StatusCode = HttpStatusCode.OK;

            return Results.Ok(response);
        }
    }
}
