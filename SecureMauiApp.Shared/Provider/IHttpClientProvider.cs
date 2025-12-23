using System;
using System.Collections.Generic;
using System.Text;

namespace SecureMauiApp.Shared.Provider
{
    public interface IHttpClientProvider
    {
        HttpClient GetHttpClient(string baseAddress);
    }
}
