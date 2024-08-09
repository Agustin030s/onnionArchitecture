using Application.DTOs.Users;
using Application.Enums;
using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Settings;
using Identity.Helpers;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JWTSettings _jwtSettings;
        private readonly IDateTimeService _dateTimeService;

        public AccountService(UserManager<ApplicationUser> userManager, JWTSettings jwtSettings, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, IDateTimeService dateTimeService)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _dateTimeService = dateTimeService;
        }

        public async Task<Response<AuthenticateResponse>> AuthenticateAsync(AuthenticationRequest request, string ipAddress)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                throw new ApiException($"No hay una cuenta registrada con el email {request.Email}");
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                throw new ApiException($"Las credenciales del usuario no son válidas");
            }

            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);
            AuthenticateResponse authenticateResponse = new AuthenticateResponse();

            authenticateResponse.Id = user.Id;
            authenticateResponse.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authenticateResponse.Email = user.Email;
            authenticateResponse.UserName = user.UserName;

            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            authenticateResponse.Roles = rolesList.ToList();
            authenticateResponse.IsVerified = user.EmailConfirmed;

            var refreshToken = GenerateRefreshToken(ipAddress);
            authenticateResponse.RefreshToken = refreshToken.Token;

            return new Response<AuthenticateResponse>(authenticateResponse, $"Usuario Autenticado {user.UserName}");
        }

        public async Task<Response<string>> RegisterAsync(RegisterRequest request, string origin)
        {
            var repeatedUser = await _userManager.FindByNameAsync(request.UserName);

            if (repeatedUser != null)
            {
                throw new ApiException($"El nombre de usuario {request.UserName} ya existe");
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                Name = request.Name,
                Lastname = request.LastName,
                UserName = request.UserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };

            var repeatedUserEmail = await _userManager.FindByEmailAsync(request.Email);

            if (repeatedUserEmail != null)
            {
                throw new ApiException($"El email {request.Email} ya fue registrado previamente");
            }
            else
            {
                var result = await _userManager.CreateAsync(user, request.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Roles.Basic.ToString());
                    return new Response<string>(user.Id, message: $"Usuario {request.UserName} registrado exitosamente");
                }
                else
                {
                    throw new ApiException($"{result.Errors}");
                }
            }
        }

        private async Task<JwtSecurityToken> GenerateJWToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var rolesClaims = new List<Claim>();

            foreach (var role in roles)
            {
                rolesClaims.Add(new Claim("roles", role));
            }

            string ipAddress = IpHelper.GetIpAddress();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
                new Claim("ip", ipAddress),
            }
            .Union(userClaims)
            .Union(rolesClaims);

            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes),
                    signingCredentials: signingCredentials
                );

            return jwtSecurityToken;
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now,
                CreatedByIp = ipAddress,
            };
        }

        private string RandomTokenString()
        {
            using var randomGenerator = RandomNumberGenerator.Create();
            var randomBytes = new byte[40];
            randomGenerator.GetBytes(randomBytes);  

            return BitConverter.ToString(randomBytes).Replace("-", "");
        }
    }
}
