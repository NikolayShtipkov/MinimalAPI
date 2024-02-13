using AutoMapper;
using FluentValidation;
using MagicVilla_CouponAPI.Models.DTO;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Repository.IRepository;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_CouponAPI.Endpoints
{
    public static class AuthEndpoints
    {
        public static void ConfigureAuthEnpoints(this WebApplication app)
        {
            app.MapPost("/api/login", Login).WithName("Login")
                .Accepts<LoginRequestDto>("application/json").Produces<ApiResponse>(200).Produces(400);
            app.MapPost("/api/register", Register).WithName("Register")
                .Accepts<RegistrationRequestDto>("application/json").Produces<ApiResponse>(200).Produces(400);
        }

        private async static Task<IResult> Login(IAuthRepository _authRepo, [FromBody] LoginRequestDto model)
        {
            ApiResponse response = new() { isSuccessful = false, StatusCode = HttpStatusCode.BadRequest };

            var loginResponse = await _authRepo.Authenticate(model);
            if (loginResponse == null)
            {
                response.ErrorMessages.Add("Username or password is incorrect.");
                return Results.BadRequest(response);
            }

            response.Result = loginResponse;
            response.isSuccessful = true;
            response.StatusCode = HttpStatusCode.OK;

            return Results.Ok(response);
        }

        private async static Task<IResult> Register(IAuthRepository _authRepo, [FromBody] RegistrationRequestDto model)
        {
            ApiResponse response = new() { isSuccessful = false, StatusCode = HttpStatusCode.BadRequest };

            bool isUsernameUnique = _authRepo.IsUniqueUser(model.Username);
            if (!isUsernameUnique)
            {
                response.ErrorMessages.Add("Username already exists.");
                return Results.BadRequest(response);
            }

            var registrationResponse = await _authRepo.Register(model);
            if (registrationResponse == null || string.IsNullOrEmpty(registrationResponse.Username))
            {
                response.ErrorMessages.Add("Not valid for registration.");
                return Results.BadRequest(response);
            }

            response.Result = registrationResponse;
            response.isSuccessful = true;
            response.StatusCode = HttpStatusCode.OK;

            return Results.Ok(response);
        }
    }
}
