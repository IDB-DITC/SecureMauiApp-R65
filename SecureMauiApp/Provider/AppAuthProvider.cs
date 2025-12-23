

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SecureMauiApp.Shared.Model;
using SecureMauiApp.Shared.Provider;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;

namespace SecureMauiApp.Provider
{


  public class MauiAuthenticationStateProvider : AuthenticationStateProvider, ICustomAuthenticationStateProvider
  {
    //TODO: Place this in AppSettings or Client config file
    protected string LoginUri { get; set; } = "https://localhost:7209/Token/";

    public LoginStatus LoginStatus { get; set; } = LoginStatus.None;
    protected ClaimsPrincipal currentUser = new ClaimsPrincipal(new ClaimsIdentity());


    private readonly IHttpClientProvider _clientProvider;
    public MauiAuthenticationStateProvider(IHttpClientProvider clientProvider)
    {
      _clientProvider = clientProvider;
      //Android Emulator uses 10.0.2.2 to refer to localhost
      //LoginUri =  DeviceInfo.Platform == DevicePlatform.Android ? LoginUri.Replace("localhost", "10.0.2.2") : LoginUri;
    }

    private HttpClient GetHttpClient(string? baseAddress = null)
    {
      return _clientProvider.GetHttpClient(baseAddress ?? LoginUri);

      //#if WINDOWS || MACCATALYST
      //            return new HttpClient()
      //            { BaseAddress = new Uri(baseAddress ?? LoginUri) };
      //#else
      //            return new HttpClient(new HttpsClientHandlerService().GetPlatformMessageHandler())
      //            { BaseAddress = new Uri(baseAddress ?? LoginUri) };
      //#endif
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
      string? token = await SecureStorage.GetAsync(AuthConstants.JwtToken);
      if (token == null)
      {
        return await Task.FromResult(new AuthenticationState(currentUser));
      }
      var jwtToken = ConvertJwtStringToJwtSecurityToken(token);
      //var decodedToken = DecodeJwt(jwtToken);
      var claims = jwtToken.Claims.Select(c => new Claim(c.Type, c.Value)).ToList();
      var identity = new ClaimsIdentity(claims, AuthConstants.JwtToken);
      currentUser = new ClaimsPrincipal(identity);
      var isTokenValid = jwtToken.ValidTo > DateTime.UtcNow;
      if (!isTokenValid)
      {
        currentUser = new ClaimsPrincipal(new ClaimsIdentity());
      }
      var authState = new AuthenticationState(currentUser);
      var task = Task.FromResult(authState);
      NotifyAuthenticationStateChanged(task);
      return await task;
    }

    public Task<string?> GetTokenAsync()
    {
      return SecureStorage.GetAsync(AuthConstants.JwtToken);
    }


    public JsonWebToken ConvertJwtStringToJwtSecurityToken(string? jwt)
    {
      var handler = new JsonWebTokenHandler();
      var token = handler.ReadJsonWebToken(jwt);

      return token;
    }

    public DecodedToken DecodeJwt(JsonWebToken token)
    {
      //var keyId = token.Kid;
      //var audience = token.Audiences.ToList();
      //var claims = token.Claims.Select(claim => new Claim(claim.Type, claim.Value)).ToList();

      return new DecodedToken(
          token.Kid,
          token.Issuer,
          token.Audiences,
          token.Claims,
          token.ValidTo,
          token.Alg,
          token.EncodedToken,
          token.Subject,
          token.ValidFrom,
          token.EncodedHeader,
          token.EncodedPayload
      );
    }



    public Task LogInAsync(LoginModel loginModel)
    {

      var loginTask = LogInAsyncCore(loginModel);
      NotifyAuthenticationStateChanged(loginTask);

      return loginTask;

      async Task<AuthenticationState> LogInAsyncCore(LoginModel loginModel)
      {
        var user = await LoginWithProviderAsync(loginModel);
        currentUser = user;

        return new AuthenticationState(currentUser);
      }
    }
    public void Logout()
    {
      LoginStatus = LoginStatus.None;
      currentUser = new ClaimsPrincipal(new ClaimsIdentity());
      SecureStorage.Remove(AuthConstants.JwtToken);
      NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(currentUser)));
    }

    private async Task<ClaimsPrincipal> LoginWithProviderAsync(LoginModel loginModel)
    {
      ClaimsPrincipal authenticatedUser;
      LoginStatus = LoginStatus.None;
      try
      {
        var httpClient = _clientProvider.GetHttpClient(LoginUri);
        var loginData = new { UserName = loginModel.Email, loginModel.Password };

        var response = await httpClient.PostAsJsonAsync("login", loginData);

        LoginStatus = response.IsSuccessStatusCode ? LoginStatus.Success : LoginStatus.Failed;

        if (LoginStatus == LoginStatus.Success)
        {
          var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
          await SecureStorage.SetAsync(AuthConstants.JwtToken, result!.Token);

          var jwtToken = ConvertJwtStringToJwtSecurityToken(result!.Token);
          //var decodedToken = DecodeJwt(jwtToken);
          var claims = jwtToken.Claims.Select(c => new Claim(c.Type, c.Value)).ToList();
          var identity = new ClaimsIdentity(claims, AuthConstants.JwtToken);

          authenticatedUser = new ClaimsPrincipal(identity);
        }
        else
          authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());


      }
      catch (Exception ex)
      {
        authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());
      }

      return authenticatedUser;
    }

    class LoginResponse
    {
      public string Name { get; set; } = string.Empty;
      public string Token { get; set; } = string.Empty;
    }
  }
}