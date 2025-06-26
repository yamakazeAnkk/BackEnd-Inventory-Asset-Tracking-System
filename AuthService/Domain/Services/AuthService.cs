using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Dtos.Auth;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using AuthService.Application.Dtos.User;

namespace AuthService.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthService(AuthDbContext context, ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        public Task<LoginResponse> GetUserByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponse> GetUserByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponse> GetUserByPhoneAsync(string phone)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponse> GetUserByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if(user == null)
            {
                throw new Exception("Email not found.");
            }
            if(!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new Exception("Invalid password.");
            }
            var token = _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            return new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };
        }

        public Task<LoginResponse> LogoutAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            if(await _context.Users.AnyAsync(u => u.Email == request.Email)){
                throw new Exception("Email already exists.");
            }
            if(!string.IsNullOrEmpty(request.Username) && await _context.Users.AnyAsync(u => u.Username == request.Username)){
                throw new Exception("Username already exists.");
            }
            var user = _mapper.Map<User>(request);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.IsActive = true;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var token = _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var userReadDto = _mapper.Map<UserReadDto>(user);
            return new RegisterResponse
            {
                Data = userReadDto,
                Status = true,
                Message = "User registered successfully",
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };
        }
    }
}