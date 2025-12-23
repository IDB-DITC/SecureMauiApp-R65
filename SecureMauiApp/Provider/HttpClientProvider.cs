using SecureMauiApp.Shared.Provider;
using System.Net.Http.Headers;

namespace SecureMauiApp.Provider

{


  public class HttpClientProvider : IHttpClientProvider
  {
    public HttpClient GetHttpClient(string baseAddress)
    {

      baseAddress = DeviceInfo.Platform == DevicePlatform.Android ? baseAddress.Replace("localhost", "10.0.2.2") : baseAddress;

      var httpClient = new HttpClient(new HttpsClientHandlerService().GetPlatformMessageHandler())
      {
        BaseAddress = new Uri(baseAddress)

      };
      return httpClient;
    }
  }
}