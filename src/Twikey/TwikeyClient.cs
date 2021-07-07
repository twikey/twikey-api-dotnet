using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;

namespace Twikey
{
    public class TwikeyClient
    {
        private static readonly string s_Utf8 = "UTF-8";
        private static readonly string s_defaultUserHeader = "twikey/.net-1.0";
        private static readonly string s_prodEnvironment = "https://api.twikey.com/creditor";
        private static readonly string s_testEnvironment = "https://api.beta.twikey.com/creditor";
        private static readonly long s_maxSessionAge = 23 * 60 * 60 * 60; // max 1day, but use 23 to be safe
        private static readonly HttpClient client = new HttpClient();
        private static readonly string s_saltOwn = "own";
        private readonly string _apiKey;
        private readonly string _endpoint;
        private string _userAgent = s_defaultUserHeader;
        private long _lastLogin;
        private string _sessionToken;
        private string _privateKey;

        /// <param name="apiKey">API key</param>
        /// <param name="test">Use the test environment</param>
        public TwikeyClient(string apiKey, bool test)
        {
            _apiKey = apiKey;
            _endpoint = test ? s_testEnvironment : s_prodEnvironment;
            //TODO
        }

        public TwikeyClient WithUserAgent(string userAgent)
        {
            _userAgent = userAgent;
            return this;
        }

        public TwikeyClient WithPrivateKey(string privateKey)
        {
            _privateKey = privateKey;
            return this;
        }

        /// <param name="apiKey"> API key</param>
        public TwikeyClient(string apikey) : this(apikey, false) { }

        protected string GetSessionToken()
        {
            long systemCurrentTimeMillis = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            if ((systemCurrentTimeMillis - _lastLogin) > s_maxSessionAge)
            {
                HttpRequestMessage request = new HttpRequestMessage();
                request.RequestUri = new Uri(_endpoint);
                request.Method = HttpMethod.Post;
                request.Headers.Add("User-Agent", _userAgent);

                Dictionary<string, string> parameters = new Dictionary<string, string>()
                                                            {
                                                                {"apiToken",_apiKey}
                                                            };
                if (_privateKey != null)
                {
                    long otp = 0L;
                    parameters.Add("otp", otp.ToString());
                }
                request.Content = new FormUrlEncodedContent(parameters);

                HttpResponseMessage response = client.SendAsync(request).Result;
                try
                {
                    _sessionToken = response.Headers.GetValues("Authorization").First<string>();
                    _lastLogin = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                }
                catch (ArgumentNullException ignore)
                {
                    _lastLogin = 0;
                    throw new UnauthenticatesException();
                }

            }

            return _sessionToken;
        }

        public class UserException : Exception
        {
            public UserException(String apiError) : base(apiError) { }
        }

        public class UnauthenticatesException : UserException
        {
            public UnauthenticatesException() : base("Not authenticated") { }
        }


    }
}
