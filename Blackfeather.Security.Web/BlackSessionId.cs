using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;
using Blackfeather.Data.Encoding;
using Blackfeather.Extension.Arrays;
using Blackfeather.Security.Cryptography;

namespace Blackfeather.Security.Web
{
    public class BlackSessionId : ISessionIDManager
    {
        private const int MACHINE_VALIDATION_KEY_SIZE = 64;
        private const int MACHINE_DECRYPTION_KEY_SIZE = 24;

        private SessionStateSection sessionStateConfig;
        private AppSettingsSection appSettingsConfig;
        private byte[] machineValidationKey;
        private byte[] machineDecryptionKey;
        private byte[] sharedSalt;
        private int sessionAge = 60; // Minutes

        public void Initialize()
        {
            var autogenKeysFieldInfo = typeof(HttpRuntime).GetField("s_autogenKeys", BindingFlags.NonPublic | BindingFlags.Static);
            machineValidationKey = new byte[MACHINE_VALIDATION_KEY_SIZE];
            machineDecryptionKey = new byte[MACHINE_DECRYPTION_KEY_SIZE];

            if (autogenKeysFieldInfo != null)
            {
                var machineAutogenKeys = (byte[])autogenKeysFieldInfo.GetValue(null);
                machineValidationKey = machineAutogenKeys.Slice(0, MACHINE_VALIDATION_KEY_SIZE);
                machineDecryptionKey = machineAutogenKeys.Slice(MACHINE_VALIDATION_KEY_SIZE, MACHINE_VALIDATION_KEY_SIZE + MACHINE_DECRYPTION_KEY_SIZE);
                var virtualPathHash = StringComparer.InvariantCultureIgnoreCase.GetHashCode(HttpRuntime.AppDomainAppVirtualPath);

                machineValidationKey[0] = (byte)(virtualPathHash & 0xff);
                machineValidationKey[1] = (byte)((virtualPathHash & 0xff00) >> 8);
                machineValidationKey[2] = (byte)((virtualPathHash & 0xff0000) >> 16);
                machineValidationKey[3] = (byte)((virtualPathHash & 0xff000000) >> 24);

                machineDecryptionKey[0] = (byte)(virtualPathHash & 0xff);
                machineDecryptionKey[1] = (byte)((virtualPathHash & 0xff00) >> 8);
                machineDecryptionKey[2] = (byte)((virtualPathHash & 0xff0000) >> 16);
                machineDecryptionKey[3] = (byte)((virtualPathHash & 0xff000000) >> 24);
            }

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

            sharedSalt = machineValidationKey.Slice(4, 8).Append(machineDecryptionKey.Slice(4, 8));
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
                case HttpCookieMode.UseDeviceProfile:
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
                case HttpCookieMode.UseDeviceProfile:
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

        public bool ValidateTimeForSingleDay(DateTime dateTime)
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
