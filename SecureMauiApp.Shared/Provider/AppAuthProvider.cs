

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SecureMauiApp.Shared.Model;
using Microsoft.AspNetCore.Components.Authorization;

namespace SecureMauiApp.Shared.Provider

{

    public interface ICustomAuthenticationStateProvider
    {
        public LoginStatus LoginStatus { get; set; }
        Task<AuthenticationState> GetAuthenticationStateAsync();
        Task LogInAsync(LoginModel loginModel);
        void Logout();

    Task<string> GetTokenAsync();


    }

    public static class AuthConstants
    {
        public const string JwtToken = "JwtToken";
    }
    public static class AuthExtensions
    {
        public static bool IsAuthenticated(this ClaimsPrincipal user)
        {
            return user.Identity != null && user.Identity.IsAuthenticated;
        }
        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty;
        }
        public static string GetName(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty;
        }
        public static IEnumerable<string> GetRoles(this ClaimsPrincipal user)
        {
            return user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
        }
        public static bool IsInRole(this ClaimsPrincipal user, string role)
        {
            return user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == role);
        }
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
        public static DateTime? GetExpiration(this ClaimsPrincipal user)
        {
            var expClaim = user.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            if (long.TryParse(expClaim, out long exp))
            {
                return DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
            }
            return null;
        }
        public static string GetToken(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == "token")?.Value ?? string.Empty;
        }
        public static string GetRefreshToken(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == "refresh_token")?.Value ?? string.Empty;
        }
        public static string GetFullName(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(c => c.Type == "full_name")?.Value ?? string.Empty;
        }
        
    }
}