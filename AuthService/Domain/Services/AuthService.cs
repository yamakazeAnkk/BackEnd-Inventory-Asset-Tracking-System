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
using AuthService.Infrastructure.Repositories.Interfaces;
using AuthService.Application.Dtos.User;
using AuthService.Application.Exceptions;

namespace AuthService.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthService(IUserRepository userRepository, ITokenService tokenService, IMapper mapper)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<LoginResponse> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new UserNotFoundException(email, "email");
            }
            return _mapper.Map<LoginResponse>(user);
        }

        public async Task<LoginResponse> GetUserByIdAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new UserNotFoundException(userId, "userId");
            }
            return _mapper.Map<LoginResponse>(user);
        }

        public Task<LoginResponse> GetUserByPhoneAsync(string phone)
        {
            throw new NotImplementedException();
        }

        public async Task<LoginResponse> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null)
            {
                throw new UserNotFoundException(username, "username");
            }
            return _mapper.Map<LoginResponse>(user);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if(user == null)
            {
                throw new InvalidCredentialsException(request.Email);
            }
            if(!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new InvalidCredentialsException(request.Email);
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

        public async Task<LoginResponse> LogoutAsync(string userId)
        {
            var user = await _userRepository.GetUserByEmailAsync(userId);
            if (user == null)
            {
                throw new UserNotFoundException(userId, "userId");
            }
            return _mapper.Map<LoginResponse>(user);
        }

        public Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            if(await _userRepository.UserExistsAsync(request.Email, request.Username)){
                throw new UserAlreadyExistsException(request.Email, request.Username);
            }
            if(!string.IsNullOrEmpty(request.Username) && await _userRepository.GetUserByUsernameAsync(request.Username) != null){
                throw new UserAlreadyExistsException(null, request.Username);    
            }
            var user = _mapper.Map<User>(request);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.IsActive = true;
            await _userRepository.AddUserAsync(user);
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