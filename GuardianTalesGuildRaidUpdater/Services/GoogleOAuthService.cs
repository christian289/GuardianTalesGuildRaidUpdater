using GuardianTalesGuildRaidUpdater.Models;
using GuardianTalesGuildRaidUpdater.Models.HttpRequests;
using GuardianTalesGuildRaidUpdater.Models.HttpResonses;
using GuardianTalesGuildRaidUpdater.Options;

namespace GuardianTalesGuildRaidUpdater.Services
{
    public class GoogleOAuthService
    {
        const string code_challenge_method = "S256";

        private readonly GoogleOAuthClientOption authOpt;
        private readonly GoogleApiEndPointsOption endpointOpt;
        private readonly HttpClient httpClient;
        private readonly AuthInfo authInfo;

        public GoogleOAuthService(
            IOptionsMonitor<GoogleOAuthClientOption> authOpt,
            IOptionsMonitor<GoogleApiEndPointsOption> endpointOpt,
            HttpClient httpClient,
            AuthInfo authInfo)
        {
            this.authOpt = authOpt.CurrentValue;
            this.endpointOpt = endpointOpt.CurrentValue;
            this.httpClient = httpClient;
            this.authInfo = authInfo;
        }

        public async Task StartGoogleOAuthAsync()
        {
            string state = RandomDataBase64url(32);
            string code_verifier = RandomDataBase64url(32);
            string code_challenge = Base64UriEncodeNoPadding(MakeSha256(code_verifier));
            string redirectUriStr = $"http://{IPAddress.Loopback}:{GetRandomUnusedPort()}/";
            Console.WriteLine($"redirect URI: {redirectUriStr}");

            HttpListener http = new();
            http.Prefixes.Add(redirectUriStr);
            Console.WriteLine("Listening..");
            http.Start();

            #region 이 Region의 방법으로 하면 중복 Encoding이 된다. HttpUtility.ParseQueryString을 사용하는 것이 문제다.
            //UriBuilder builder = new(authorizationEndpoint);
            //var queryStr = HttpUtility.ParseQueryString(builder.Query);
            //queryStr["response_type"] = "code";
            //queryStr["scope"] = Uri.EscapeDataString("openid profile");
            //queryStr["redirect_uri"] = Uri.EscapeDataString(redirectUriStr);
            //queryStr["client_id"] = clientID;
            //queryStr["state"] = state;
            //queryStr["code_challenge"] = code_challenge;
            //queryStr["code_challenge_method"] = code_challenge_method;
            //builder.Query = queryStr.ToString();
            //Uri requestUri = builder.Uri;
            //System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd", $"/c start {requestUri}") { CreateNoWindow = true });
            #endregion

            #region scope 만들기
            // Google OAuth2를 사용할 때 반드시 Scope를 아래 URI링크에서 지정해서 보내야한다.
            // 안그러면 호출하는 API마다 권한 불충분으로 Error가 발생한다.
            // https://developers.google.com/identity/protocols/oauth2/scopes
            // scope가 여러개일 경우 space로 한 칸 띄워서 쓰면된다.
            // 이를테면, 아래와 같이 scope를 지정하면된다.
            // scope=https://www.googleapis.com/auth/calendar https://www.googleapis.com/auth/userinfo.email
            StringBuilder scopeBuilder = new();
            scopeBuilder.Append("https://www.googleapis.com/auth/drive"); scopeBuilder.Append(' ');
            scopeBuilder.Append("https://www.googleapis.com/auth/drive.appdata"); scopeBuilder.Append(' ');
            scopeBuilder.Append("https://www.googleapis.com/auth/drive.file"); scopeBuilder.Append(' ');
            scopeBuilder.Append("https://www.googleapis.com/auth/drive.metadata"); scopeBuilder.Append(' ');
            scopeBuilder.Append("https://www.googleapis.com/auth/drive.metadata.readonly"); scopeBuilder.Append(' ');
            scopeBuilder.Append("https://www.googleapis.com/auth/drive.photos.readonly"); scopeBuilder.Append(' ');
            scopeBuilder.Append("https://www.googleapis.com/auth/drive.readonly"); scopeBuilder.Append(' ');
            scopeBuilder.Append("https://www.googleapis.com/auth/drive.scripts"); scopeBuilder.Append(' ');
            #endregion

            string scope = scopeBuilder.ToString();
            Dictionary<string, string> queryDic = new()
            {
                ["response_type"] = "code",
                ["scope"] = scope,
                ["redirect_uri"] = redirectUriStr,
                ["client_id"] = authOpt.ClientId,
                ["state"] = state,
                ["code_challenge"] = code_challenge,
                ["code_challenge_method"] = code_challenge_method,
            };
            string uriStr = QueryHelpers.AddQueryString(endpointOpt.Authorization, queryDic); // 이 메서드로 하면 Uri.EscapeDataString 이 내부적으로 호출되기 때문에 위에서 안해도 된다.
            // .NET Core 이후부터는 아래의 문자열 치환과 Browser 실행방법이 변경되었다.
            // https://stackoverflow.com/questions/14982746/open-a-browser-with-a-specific-url-by-console-application
            uriStr = uriStr.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {uriStr}") { CreateNoWindow = true });
            HttpListenerContext context = await http.GetContextAsync();

            HttpListenerResponse response = context.Response;
            string responseString = string.Format("<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>");
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            Stream responseOutput = response.OutputStream;
            Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                http.Stop();
                Console.WriteLine("HTTP server stopped.");
            });

            #region Check Error
            if (context.Request.QueryString.Get("error") is not null)
            {
                Console.WriteLine($"OAuth authorization error: {context.Request.QueryString.Get("error")}.");

                return;
            }

            if (context.Request.QueryString.Get("code") is null || context.Request.QueryString.Get("state") is null)
            {
                Console.WriteLine($"Malformed authorization response. {context.Request.QueryString}");

                return;
            }
            #endregion

            string? code = context.Request.QueryString.Get("code");
            string? incoming_state = context.Request.QueryString.Get("state");

            if (incoming_state != state)
            {
                Console.WriteLine($"Received request with invalid state ({incoming_state})");
                
                return;
            }

            Console.WriteLine($"Authorization code: {code}");

            string accessToken = await GetAccessTokenAsync(httpClient, code, redirectUriStr, authOpt.ClientId, code_verifier, authOpt.ClientSecret, scope);
            authInfo.AccessToken = accessToken;
            //string userInfo = await GetUserInfo(httpClient, accessToken);

            //Console.WriteLine(userInfo);

            //string fileList = await GetGoogleDriveFileList(httpClient, accessToken);

            //Console.WriteLine(fileList);
        }

        private string RandomDataBase64url(uint length)
        {
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);

            return Base64UriEncodeNoPadding(bytes);
        }

        private byte[] MakeSha256(string inputStirng)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
            using SHA256 sha256 = SHA256.Create();

            return sha256.ComputeHash(bytes);
        }

        private string Base64UriEncodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }

        private int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            return port;
        }

        private async Task<string> GetAccessTokenAsync(HttpClient httpClient, string code, string uri, string clientId, string code_verifier, string clientSecret, string scope = "")
        {
            HttpRequestMessage httpRequest_GetAccessToken = new(HttpMethod.Post, new Uri(endpointOpt.Token));
            string requestContentBody = $"code={code}&redirect_uri={Uri.EscapeDataString(uri)}&client_id={clientId}&code_verifier={code_verifier}&client_secret={clientSecret}&scope={scope}&grant_type=authorization_code";
            ByteArrayContent content = new(Encoding.ASCII.GetBytes(requestContentBody));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded"); // FormUrlEncodedContent 이걸로 사용할 경우 ASCII 방식의 인코딩이 불가능하고 무조건 Default로 설정된 Latin1 인코딩만 가능하다.
            httpRequest_GetAccessToken.Content = content;
            httpRequest_GetAccessToken.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));
            httpRequest_GetAccessToken.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Xml));
            HttpResponseMessage response_GetAccessToken = await httpClient.SendAsync(httpRequest_GetAccessToken);

            if (!response_GetAccessToken.IsSuccessStatusCode) return string.Empty;

            string res_string = await response_GetAccessToken.Content.ReadAsStringAsync(); //ReadFromJsonAsync<Dictionary<string, string>> 으로 바로하면 Exception 발생한다...왜?
            Dictionary<string, string> res_content = JsonConvert.DeserializeObject<Dictionary<string, string>>(res_string)!;

            if (!res_content.TryGetValue("access_token", out string accessToken)) return string.Empty;

            return accessToken;
        }

        // 응답 받고나면 AuthInfo에 업데이트 할 것
        private async Task<ResGoogleOAuthTokenWithRefreshToken?> GetAccessTokenWithRefreshTokenAsync(HttpClient httpClient, string clientId, string clientSecret, string refreshToken)
        {
            HttpRequestMessage httpRequest_GetAccessTokenWithRefreshToken = new(HttpMethod.Post, new Uri(endpointOpt.RefreshToken))
            {
                Content = new StringContent(JsonConvert.SerializeObject(new ReqGoogleOAuthTokenWithRefreshToken
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    RefreshToken = refreshToken,
                }))
            };
            HttpResponseMessage response_GetAccessTokenWithRefreshToken = await httpClient.SendAsync(httpRequest_GetAccessTokenWithRefreshToken);

            if (!response_GetAccessTokenWithRefreshToken.IsSuccessStatusCode) return null;

            string res_string = await response_GetAccessTokenWithRefreshToken.Content.ReadAsStringAsync();
            ResGoogleOAuthTokenWithRefreshToken res_content = JsonConvert.DeserializeObject<ResGoogleOAuthTokenWithRefreshToken>(res_string)!;

            if (!string.IsNullOrWhiteSpace(res_content.AccessToken)) return null;

            return res_content;
        }
    }
}
