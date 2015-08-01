/* 
 The MIT License (MIT)

 Copyright (c) 2013 - 2015 Timothy D Meadows II

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.
*/

using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;
using Blackfeather.Data.Encoding;
using Blackfeather.Extension.Arrays;
using Blackfeather.Security.Cryptography;

namespace Blackfeather.Security.Web
{
    // BUG: If a user's session is greater than session age but extend's into the next calendar day. It will expire. Should last into the next calendar day, or, days if needed.
    // TODO: Add more detailed error checking!
    // TODO: Add extendable properties to Web.config for server admins, and, developers to control various settings such as:
    //       1) Method of specifying a custom private, and, public key at any "resonable" size. Will also need to accept multiple input encoding types.
    //       2) Method of specifying a custom shared salt. Will also need to accept multiple input encoding types.
    //       3) Method of specifying a custom session age in minutes.
    // TODO: Need the ability to attach custom identifiable data once a user has authenticated. This means the previously set session data will have to carry over to the new session id when modified! Will add the final layer of security if used.
    // TODO: Get AUD001 audited by at least one other knowledgable developer or industry expert!
    public class BlackSessionId : ISessionIDManager
    {
        private SessionStateSection sessionStateConfig;
        private AppSettingsSection appSettingsConfig;
        private byte[] machineValidationKey;
        private byte[] machineDecryptionKey;
        private byte[] sharedSalt;
        private int sessionAge = 60; // Minutes

        public void Initialize()
        {
            var machineKeys = MachineKeys.GetMachineKeys();
            machineDecryptionKey = MachineKeys.GetMachineDecryptionKey(machineKeys);
            machineValidationKey = MachineKeys.GetMachineValidationKey(machineKeys);

            if (sessionStateConfig == null)
            {
                var webConfig = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
                sessionStateConfig = (SessionStateSection)webConfig.GetSection("system.web/sessionState");
            }

            if (appSettingsConfig == null)
            {
                var webConfig = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
                appSettingsConfig = (AppSettingsSection)webConfig.GetSection("appSettings");
            }

            sharedSalt = machineValidationKey.Slice(4, 8).Append(machineDecryptionKey.Slice(4, 8)); // AUD001: Needs audit, but should be safe?
        }

        public bool InitializeRequest(HttpContext context, bool suppressAutoDetectRedirect, out bool supportSessionIDReissue)
        {
            switch (sessionStateConfig.Cookieless)
            {
                case HttpCookieMode.UseCookies:
                    supportSessionIDReissue = false;
                    return false;
                default:
                    supportSessionIDReissue = true;
                    return context.Response.IsRequestBeingRedirected;
            }
        }

        public string GetSessionID(HttpContext context)
        {
            string sessionId = null;

            switch (sessionStateConfig.Cookieless)
            {
                case HttpCookieMode.UseCookies:
                    sessionId = GetCookieOrQueryId(context, true);
                    break;
                case HttpCookieMode.UseUri:
                    sessionId = GetCookieOrQueryId(context, false);
                    break;
                case HttpCookieMode.AutoDetect:
                case HttpCookieMode.UseDeviceProfile:
                    sessionId = GetCookieOrQueryId(context, context.Request.Browser.Cookies);
                    break;
            }

            return !Validate(sessionId) ? null : sessionId;
        }

        private string GetCookieOrQueryId(HttpContext context, bool useCookies)
        {
            var sessionId = string.Empty;

            if (useCookies)
            {
                var httpCookie = context.Request.Cookies[sessionStateConfig.CookieName];
                if (httpCookie != null)
                {
                    sessionId = httpCookie.Value;
                }
            }
            else
            {
                if (context.Request.QueryString.HasKeys())
                {
                    var httpQueryStringKey = context.Request.QueryString[sessionStateConfig.CookieName];
                    if (!string.IsNullOrEmpty(httpQueryStringKey))
                    {
                        sessionId = httpQueryStringKey;
                    }
                }
            }

            return sessionId;
        }

        public string CreateSessionID(HttpContext context)
        {
            var sessionData = new UTF8Encoding().GetBytes(DateTime.Now.ToBinary().ToString());
            var machineKey = machineDecryptionKey.ToHash(Hash.DigestType.Tiger,
                KeyDevination.DevinationType.Pbkdf2, sharedSalt).Data.ToHex();
            var machineKeyVerifier = machineValidationKey.ToHash(Hash.DigestType.Tiger,
                KeyDevination.DevinationType.Pbkdf2, sharedSalt).Data.ToHex();
            var sessionIdentifiableData = (context.Request.UserAgent + context.Request.UserHostAddress).ToHash(Hash.DigestType.Tiger, KeyDevination.DevinationType.Pbkdf2, sharedSalt);
            var sessionId = sessionData.ToCipher(Cryptology.CipherDigestType.Tiger,
                KeyDevination.DevinationType.Pbkdf2, Cryptology.CipherType.AesCtr, Cryptology.CipherPaddingType.None,
                machineKey + machineKeyVerifier + sessionIdentifiableData, sharedSalt);

            return sessionId.Data.ToBase64();
        }

        public void SaveSessionID(HttpContext context, string id, out bool redirected, out bool cookieAdded)
        {
            redirected = false;
            cookieAdded = false;

            switch (sessionStateConfig.Cookieless)
            {
                case HttpCookieMode.UseCookies:
                    SaveSessionToCookieOrQueryId(context, id, true);
                    cookieAdded = true;
                    break;
                case HttpCookieMode.UseUri:
                    SaveSessionToCookieOrQueryId(context, id, false);
                    redirected = true;
                    break;
                case HttpCookieMode.AutoDetect:
                case HttpCookieMode.UseDeviceProfile: // Seems to be the same thing as auto detect?
                    SaveSessionToCookieOrQueryId(context, id, context.Request.Browser.Cookies);
                    redirected = !context.Request.Browser.Cookies;
                    break;
            }
        }

        private void SaveSessionToCookieOrQueryId(HttpContext context, string id, bool useCookies)
        {
            if (useCookies)
            {
                context.Response.Cookies.Add(new HttpCookie(sessionStateConfig.CookieName, id));
            }
            else
            {
                var url = context.Request.Path;
                var queryStringText = GetQueryStringText(context.Request.QueryString);

                if (context.Request.QueryString.AllKeys.Contains(sessionStateConfig.CookieName))
                {
                    url += string.Format("?{0}={1}", sessionStateConfig.CookieName, HttpUtility.UrlEncode(id));
                }
                else
                {
                    if (!string.IsNullOrEmpty(queryStringText))
                    {
                        url += string.Format("?{0}&{1}={2}", queryStringText, sessionStateConfig.CookieName, HttpUtility.UrlEncode(id));
                    }
                    else
                    {
                        url += string.Format("?{0}={1}", sessionStateConfig.CookieName, HttpUtility.UrlEncode(id));
                    }
                }

                context.Response.Redirect(url, false);
                context.ApplicationInstance.CompleteRequest();
            }
        }

        private string GetQueryStringText(NameValueCollection queryStringData)
        {
            var queryStringText = string.Empty;
            foreach (var key in queryStringData.AllKeys)
            {
                if (string.IsNullOrEmpty(queryStringText))
                {
                    queryStringText = string.Format("{0}={1}", key, queryStringData[key]);
                }
                else
                {
                    queryStringText = string.Format("&{0}={1}", key, queryStringData[key]);
                }
            }

            return queryStringText;
        }

        public void RemoveSessionID(HttpContext context)
        {
            switch (sessionStateConfig.Cookieless)
            {
                case HttpCookieMode.UseDeviceProfile: // Seems to be the same thing as auto detect?
                case HttpCookieMode.AutoDetect:
                case HttpCookieMode.UseCookies:
                    if (context.Response.Cookies.AllKeys.Contains(sessionStateConfig.CookieName))
                    {
                        context.Response.Cookies.Remove(sessionStateConfig.CookieName);
                    }
                    break;
            }
        }

        public bool Validate(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            var sessionCipherString = HttpUtility.UrlDecode(id);
            if (sessionCipherString != null && sessionCipherString.Contains(" "))
            {
                sessionCipherString = sessionCipherString.Replace(" ", "+");
            }

            var context = HttpContext.Current;
            var sessionCipher = sessionCipherString.FromBase64();
            var machineKey = machineDecryptionKey.ToHash(Hash.DigestType.Tiger,
                KeyDevination.DevinationType.Pbkdf2, sharedSalt).Data.ToHex();
            var machineKeyVerifier = machineValidationKey.ToHash(Hash.DigestType.Tiger,
                KeyDevination.DevinationType.Pbkdf2, sharedSalt).Data.ToHex();
            var sessionIdentifiableData =
                (context.Request.UserAgent + context.Request.UserHostAddress).ToHash(Hash.DigestType.Tiger,
                    KeyDevination.DevinationType.Pbkdf2, sharedSalt);
            var sessionBinaryDateTime = sessionCipher.FromCipher(Cryptology.CipherDigestType.Tiger,
                KeyDevination.DevinationType.Pbkdf2,
                Cryptology.CipherType.AesCtr, Cryptology.CipherPaddingType.None,
                machineKey + machineKeyVerifier + sessionIdentifiableData, sharedSalt);

            if (sessionBinaryDateTime == null)
            {
                return false;
            }

            var sessionBinaryDateTimeString = new UTF8Encoding().GetString(sessionBinaryDateTime);
            var sessionDateTime = DateTime.FromBinary(Convert.ToInt64(sessionBinaryDateTimeString));

            return ValidateTimeForSingleDay(sessionDateTime);
        }

        public bool ValidateTimeForSingleDay(DateTime dateTime) // Temporary method until a proper one can be written. Good for testing though!
        {
            var currentDataTime = DateTime.Now;
            if (currentDataTime.Date.CompareTo(dateTime.Date) == 0)
            {
                var timeOffset = currentDataTime.TimeOfDay - dateTime.TimeOfDay;
                if (timeOffset.TotalMinutes >= sessionAge)
                {
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}
