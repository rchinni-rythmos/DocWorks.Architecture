namespace DocWorks.CMS.Api.Authentication
{
    using DocWorks.CMS.Api.Configuration;
    using DocWorks.Core.Common.Helpers;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    /// <summary>
    /// Class for validating captcha response coming from client
    /// </summary>
    public static class CaptchaVerification
    {
        /// <summary>
        /// Validating captcha response coming from client
        /// </summary>
        /// <param name="captchaResponseToken">Captcha response token given by client</param>
        /// <returns>Bool result, indicate whether captcha response is valid or not</returns>
        public static async Task<bool> ValidateCaptchaResponse(string captchaResponseToken)
        {
            var postContent = new FormUrlEncodedContent(new[]
            {
             new KeyValuePair<string, string>("secret", AuthenticationSettings.CaptchaSecret),
             new KeyValuePair<string, string>("response", captchaResponseToken),
            });

            //dynamic postContentExpando = new ExpandoObject();
            //postContentExpando.secret = AuthenticationSettings.CaptchaSecret;
            //postContentExpando.response = captchaResponseToken;

            using (HttpClient objHttpClient = new HttpClient())
            {
                //HttpResponseMessage objHttpResponseMessage = await HttpClientCMSExtension.PostJsonAsync(objHttpClient, AuthenticationSettings.CaptchaVerifySiteUrl, postContent);
                HttpResponseMessage objHttpResponseMessage = await objHttpClient.PostAsync(AuthenticationSettings.CaptchaVerifySiteUrl, postContent);
                dynamic resultContent = await HttpClientCMSExtension.ReadAsync<dynamic>(objHttpResponseMessage.Content);

                bool returnStatus = resultContent.success;
                return returnStatus;
            }
        }
    }
}
