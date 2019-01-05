using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace AspNetCore.Security.Jwt
{
    internal static class MiscExtensions
    {
        /// <summary>
        /// Is valid request.
        /// </summary>
        /// <typeparam name="TModel">The request model</typeparam>
        /// <param name="httpRequest">The http request</param>
        /// <param name="condition">the valid condition</param>
        /// <returns>true/false based on the evaluation of the condition</returns>
        internal static bool IsValid<TModel>(this HttpRequest httpRequest, Func<TModel, bool> condition)
        {
            // Allows using several time the stream in ASP.Net Core
            httpRequest.EnableRewind();

            using (MemoryStream m = new MemoryStream())
            {
                httpRequest.Body.CopyTo(m);

                var bodyString = Encoding.UTF8.GetString(m.ToArray());

                httpRequest.Body.Position = 0;

                var model = JsonConvert.DeserializeObject<TModel>(bodyString);

                return condition(model);
            }
        }
    }
}
