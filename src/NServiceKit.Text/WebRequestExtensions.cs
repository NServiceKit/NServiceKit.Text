using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace NServiceKit.Text
{
    /// <summary>A web request extensions.</summary>
    public static class WebRequestExtensions
    {
        /// <summary>The JSON.</summary>
        public const string Json = "application/json";

        /// <summary>The XML.</summary>
        public const string Xml = "application/xml";

        /// <summary>The form URL encoded.</summary>
        public const string FormUrlEncoded = "application/x-www-form-urlencoded";

        /// <summary>Information describing the multi part form.</summary>
        public const string MultiPartFormData = "multipart/form-data";

        /// <summary>A string extension method that gets JSON from URL.</summary>
        /// <param name="url">           The URL to act on.</param>
        /// <param name="requestFilter"> A filter specifying the request.</param>
        /// <param name="responseFilter">A filter specifying the response.</param>
        /// <returns>The JSON from URL.</returns>
        public static string GetJsonFromUrl(this string url, 
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return url.GetStringFromUrl(Json, requestFilter, responseFilter);
        }

        /// <summary>A string extension method that gets XML from URL.</summary>
        /// <param name="url">           The URL to act on.</param>
        /// <param name="requestFilter"> A filter specifying the request.</param>
        /// <param name="responseFilter">A filter specifying the response.</param>
        /// <returns>The XML from URL.</returns>
        public static string GetXmlFromUrl(this string url,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return url.GetStringFromUrl(Xml, requestFilter, responseFilter);
        }

        public static string GetStringFromUrl(this string url, string acceptContentType = "*/*",

            /// <summary>A filter specifying the response.</summary>
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, acceptContentType: acceptContentType, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>A string extension method that posts a string to URL.</summary>
        /// <param name="url">              The URL to act on.</param>
        /// <param name="requestBody">      The request body.</param>
        /// <param name="contentType">      Type of the content.</param>
        /// <param name="acceptContentType">Type of the accept content.</param>
        /// <returns>A string.</returns>
        /// ### <param name="requestFilter"> A filter specifying the request.</param>
        /// ### <param name="responseFilter">A filter specifying the response.</param>
        public static string PostStringToUrl(this string url, string requestBody = null,
            string contentType = null, string acceptContentType = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "POST",
                requestBody: requestBody, contentType: contentType,
                acceptContentType: acceptContentType, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PostToUrl(this string url, string formData = null, string acceptContentType = "*/*",

            /// <summary>A filter specifying the response.</summary>
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "POST",
                contentType: FormUrlEncoded, requestBody: formData,
                acceptContentType: acceptContentType, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PostToUrl(this string url, object formData = null, string acceptContentType = "*/*",

            /// <summary>A filter specifying the response.</summary>
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return SendStringToUrl(url, method: "POST",
                contentType: FormUrlEncoded, requestBody: postFormData,
                acceptContentType: acceptContentType, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>A string extension method that posts a JSON to URL.</summary>
        /// <param name="url">           The URL to act on.</param>
        /// <param name="json">          The JSON.</param>
        /// <param name="requestFilter"> A filter specifying the request.</param>
        /// <param name="responseFilter">A filter specifying the response.</param>
        /// <returns>A string.</returns>
        public static string PostJsonToUrl(this string url, string json,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "POST", requestBody: json, contentType: Json, acceptContentType: Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>A string extension method that posts a JSON to URL.</summary>
        /// <param name="url">           The URL to act on.</param>
        /// <param name="data">          The data.</param>
        /// <param name="requestFilter"> A filter specifying the request.</param>
        /// <param name="responseFilter">A filter specifying the response.</param>
        /// <returns>A string.</returns>
        public static string PostJsonToUrl(this string url, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "POST", requestBody: data.ToJson(), contentType: Json, acceptContentType: Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>A string extension method that posts an XML to URL.</summary>
        /// <param name="url">           The URL to act on.</param>
        /// <param name="xml">           The XML.</param>
        /// <param name="requestFilter"> A filter specifying the request.</param>
        /// <param name="responseFilter">A filter specifying the response.</param>
        /// <returns>A string.</returns>
        public static string PostXmlToUrl(this string url, string xml,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "POST", requestBody: xml, contentType: Xml, acceptContentType: Xml,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }
#if !XBOX && !SILVERLIGHT && !MONOTOUCH
        /// <summary>A string extension method that posts an XML to URL.</summary>
        /// <param name="url">           The URL to act on.</param>
        /// <param name="data">          The data.</param>
        /// <param name="requestFilter"> A filter specifying the request.</param>
        /// <param name="responseFilter">A filter specifying the response.</param>
        /// <returns>A string.</returns>
        public static string PostXmlToUrl(this string url, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "POST", requestBody: data.ToXml(), contentType: Xml, acceptContentType: Xml,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }
#endif

        /// <summary>A string extension method that puts string to URL.</summary>
        /// <param name="url">              The URL to act on.</param>
        /// <param name="requestBody">      The request body.</param>
        /// <param name="contentType">      Type of the content.</param>
        /// <param name="acceptContentType">Type of the accept content.</param>
        /// <returns>A string.</returns>
        /// ### <param name="requestFilter"> A filter specifying the request.</param>
        /// ### <param name="responseFilter">A filter specifying the response.</param>
        public static string PutStringToUrl(this string url, string requestBody = null,
            string contentType = null, string acceptContentType = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "PUT",
                requestBody: requestBody, contentType: contentType,
                acceptContentType: acceptContentType, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PutToUrl(this string url, string formData = null, string acceptContentType = "*/*",

            /// <summary>A filter specifying the response.</summary>
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "PUT",
                contentType: FormUrlEncoded, requestBody: formData,
                acceptContentType: acceptContentType, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PutToUrl(this string url, object formData = null, string acceptContentType = "*/*",

            /// <summary>A filter specifying the response.</summary>
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return SendStringToUrl(url, method: "PUT",
                contentType: FormUrlEncoded, requestBody: postFormData,
                acceptContentType: acceptContentType, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>A string extension method that puts JSON to URL.</summary>
        /// <param name="url">           The URL to act on.</param>
        /// <param name="json">          The JSON.</param>
        /// <param name="requestFilter"> A filter specifying the request.</param>
        /// <param name="responseFilter">A filter specifying the response.</param>
        /// <returns>A string.</returns>
        public static string PutJsonToUrl(this string url, string json,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "PUT", requestBody: json, contentType: Json, acceptContentType: Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>A string extension method that puts JSON to URL.</summary>
        /// <param name="url">           The URL to act on.</param>
        /// <param name="data">          The data.</param>
        /// <param name="requestFilter"> A filter specifying the request.</param>
        /// <param name="responseFilter">A filter specifying the response.</param>
        /// <returns>A string.</returns>
        public static string PutJsonToUrl(this string url, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "PUT", requestBody: data.ToJson(), contentType: Json, acceptContentType: Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>A string extension method that puts XML to URL.</summary>
        /// <param name="url">           The URL to act on.</param>
        /// <param name="xml">           The XML.</param>
        /// <param name="requestFilter"> A filter specifying the request.</param>
        /// <param name="responseFilter">A filter specifying the response.</param>
        /// <returns>A string.</returns>
        public static string PutXmlToUrl(this string url, string xml,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "PUT", requestBody: xml, contentType: Xml, acceptContentType: Xml,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

#if !XBOX && !SILVERLIGHT && !MONOTOUCH
        /// <summary>A string extension method that puts XML to URL.</summary>
        /// <param name="url">           The URL to act on.</param>
        /// <param name="data">          The data.</param>
        /// <param name="requestFilter"> A filter specifying the request.</param>
        /// <param name="responseFilter">A filter specifying the response.</param>
        /// <returns>A string.</returns>
        public static string PutXmlToUrl(this string url, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "PUT", requestBody: data.ToXml(), contentType: Xml, acceptContentType: Xml,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }
#endif

        public static string DeleteFromUrl(this string url, string acceptContentType = "*/*",

            /// <summary>A filter specifying the response.</summary>
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "DELETE", acceptContentType: acceptContentType, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string OptionsFromUrl(this string url, string acceptContentType = "*/*",

            /// <summary>A filter specifying the response.</summary>
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "OPTIONS", acceptContentType: acceptContentType, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string HeadFromUrl(this string url, string acceptContentType = "*/*",

            /// <summary>A filter specifying the response.</summary>
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendStringToUrl(url, method: "HEAD", acceptContentType: acceptContentType, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>A string extension method that sends a string to URL.</summary>
        /// <param name="url">              The URL to act on.</param>
        /// <param name="method">           The method.</param>
        /// <param name="requestBody">      The request body.</param>
        /// <param name="contentType">      Type of the content.</param>
        /// <param name="acceptContentType">Type of the accept content.</param>
        /// <returns>A string.</returns>
        public static string SendStringToUrl(this string url, string method = null,
            string requestBody = null, string contentType = null, string acceptContentType = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            var webReq = (HttpWebRequest)WebRequest.Create(url);
            if (method != null)
                webReq.Method = method;
            if (contentType != null)
                webReq.ContentType = contentType;

            webReq.Accept = acceptContentType;
            webReq.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            webReq.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            if (requestFilter != null)
            {
                requestFilter(webReq);
            }

            if (requestBody != null)
            {
                using (var reqStream = webReq.GetRequestStream())
                using (var writer = new StreamWriter(reqStream))
                {
                    writer.Write(requestBody);
                }
            }

            using (var webRes = webReq.GetResponse())
            using (var stream = webRes.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                if (responseFilter != null)
                {
                    responseFilter((HttpWebResponse)webRes);
                }
                return reader.ReadToEnd();
            }
        }

        public static byte[] GetBytesFromUrl(this string url, string acceptContentType = "*/*",

            /// <summary>A filter specifying the response.</summary>
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return url.SendBytesToUrl(acceptContentType:acceptContentType, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static byte[] PostBytesToUrl(this string url, byte[] requestBody = null, string contentType = null, string acceptContentType = "*/*",

            /// <summary>A filter specifying the response.</summary>
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendBytesToUrl(url, method: "POST",
                contentType: contentType, requestBody: requestBody,
                acceptContentType: acceptContentType, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static byte[] PutBytesToUrl(this string url, byte[] requestBody = null, string contentType = null, string acceptContentType = "*/*",

            /// <summary>A filter specifying the response.</summary>
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendBytesToUrl(url, method: "PUT",
                contentType: contentType, requestBody: requestBody,
                acceptContentType: acceptContentType, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>A string extension method that sends the bytes to URL.</summary>
        /// <param name="url">              The URL to act on.</param>
        /// <param name="method">           The method.</param>
        /// <param name="requestBody">      The request body.</param>
        /// <param name="contentType">      Type of the content.</param>
        /// <param name="acceptContentType">Type of the accept content.</param>
        /// <returns>A byte[].</returns>
        public static byte[] SendBytesToUrl(this string url, string method = null,
            byte[] requestBody = null, string contentType = null, string acceptContentType = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            var webReq = (HttpWebRequest)WebRequest.Create(url);
            if (method != null)
                webReq.Method = method;

            if (contentType != null)
                webReq.ContentType = contentType;

            webReq.Accept = acceptContentType;
            webReq.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            webReq.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            if (requestFilter != null)
            {
                requestFilter(webReq);
            }

            if (requestBody != null)
            {
                using (var req = webReq.GetRequestStream())
                {
                    req.Write(requestBody, 0, requestBody.Length);                    
                }
            }

            using (var webRes = webReq.GetResponse())
            {
                if (responseFilter != null)
                    responseFilter((HttpWebResponse)webRes);

                using (var stream = webRes.GetResponseStream())
                {
                    return stream.ReadFully();
                }
            }
        }

        /// <summary>An Exception extension method that query if 'ex' is any 300.</summary>
        /// <param name="ex">The ex to act on.</param>
        /// <returns>true if any 300, false if not.</returns>
        public static bool IsAny300(this Exception ex)
        {
            var status = ex.GetStatus();
            return status >= HttpStatusCode.MultipleChoices && status < HttpStatusCode.BadRequest;
        }

        /// <summary>An Exception extension method that query if 'ex' is any 400.</summary>
        /// <param name="ex">The ex to act on.</param>
        /// <returns>true if any 400, false if not.</returns>
        public static bool IsAny400(this Exception ex)
        {
            var status = ex.GetStatus();
            return status >= HttpStatusCode.BadRequest && status < HttpStatusCode.InternalServerError;
        }

        /// <summary>An Exception extension method that query if 'ex' is any 500.</summary>
        /// <param name="ex">The ex to act on.</param>
        /// <returns>true if any 500, false if not.</returns>
        public static bool IsAny500(this Exception ex)
        {
            var status = ex.GetStatus();
            return status >= HttpStatusCode.InternalServerError && (int)status < 600;
        }

        /// <summary>An Exception extension method that query if 'ex' is bad request.</summary>
        /// <param name="ex">The ex to act on.</param>
        /// <returns>true if bad request, false if not.</returns>
        public static bool IsBadRequest(this Exception ex)
        {
            return HasStatus(ex as WebException, HttpStatusCode.BadRequest);
        }

        /// <summary>An Exception extension method that query if 'ex' is not found.</summary>
        /// <param name="ex">The ex to act on.</param>
        /// <returns>true if not found, false if not.</returns>
        public static bool IsNotFound(this Exception ex)
        {
            return HasStatus(ex as WebException, HttpStatusCode.NotFound);
        }

        /// <summary>An Exception extension method that query if 'ex' is unauthorized.</summary>
        /// <param name="ex">The ex to act on.</param>
        /// <returns>true if unauthorized, false if not.</returns>
        public static bool IsUnauthorized(this Exception ex)
        {
            return HasStatus(ex as WebException, HttpStatusCode.Unauthorized);
        }

        /// <summary>An Exception extension method that query if 'ex' is forbidden.</summary>
        /// <param name="ex">The ex to act on.</param>
        /// <returns>true if forbidden, false if not.</returns>
        public static bool IsForbidden(this Exception ex)
        {
            return HasStatus(ex as WebException, HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// An Exception extension method that query if 'ex' is internal server error.
        /// </summary>
        /// <param name="ex">The ex to act on.</param>
        /// <returns>true if internal server error, false if not.</returns>
        public static bool IsInternalServerError(this Exception ex)
        {
            return HasStatus(ex as WebException, HttpStatusCode.InternalServerError);
        }

        /// <summary>A string extension method that gets response status.</summary>
        /// <param name="url">The URL to act on.</param>
        /// <returns>The response status.</returns>
        public static HttpStatusCode? GetResponseStatus(this string url)
        {
            try
            {
                var webReq = (HttpWebRequest)WebRequest.Create(url);
                using (var webRes = webReq.GetResponse())
                {
                    var httpRes = webRes as HttpWebResponse;
                    return httpRes != null ? httpRes.StatusCode : (HttpStatusCode?)null;
                }
            }
            catch (Exception ex)
            {
                return ex.GetStatus();
            }
        }

        /// <summary>A WebException extension method that gets the status.</summary>
        /// <param name="ex">The ex to act on.</param>
        /// <returns>The status.</returns>
        public static HttpStatusCode? GetStatus(this Exception ex)
        {
            return GetStatus(ex as WebException);
        }

        /// <summary>A WebException extension method that gets the status.</summary>
        /// <param name="webEx">The webEx to act on.</param>
        /// <returns>The status.</returns>
        public static HttpStatusCode? GetStatus(this WebException webEx)
        {
            if (webEx == null) return null;
            var httpRes = webEx.Response as HttpWebResponse;
            return httpRes != null ? httpRes.StatusCode : (HttpStatusCode?)null;
        }

        /// <summary>A WebException extension method that query if 'webEx' has status.</summary>
        /// <param name="webEx">     The webEx to act on.</param>
        /// <param name="statusCode">The status code.</param>
        /// <returns>true if status, false if not.</returns>
        public static bool HasStatus(this WebException webEx, HttpStatusCode statusCode)
        {
            return GetStatus(webEx) == statusCode;
        }

        /// <summary>An Exception extension method that gets response body.</summary>
        /// <param name="ex">The ex to act on.</param>
        /// <returns>The response body.</returns>
        public static string GetResponseBody(this Exception ex)
        {
            var webEx = ex as WebException;
            if (webEx == null || webEx.Status != WebExceptionStatus.ProtocolError) return null;

            var errorResponse = ((HttpWebResponse)webEx.Response);
            using (var reader = new StreamReader(errorResponse.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// A NameValueCollection extension method that converts the queryParams to a form URL
        /// encoded.
        /// </summary>
        /// <param name="queryParams">The queryParams to act on.</param>
        /// <returns>queryParams as a string.</returns>
        public static string ToFormUrlEncoded(this NameValueCollection queryParams)
        {
            var sb = new StringBuilder();
            foreach (string key in queryParams)
            {
                var values = queryParams.GetValues(key);
                if (values == null) continue;

                foreach (var value in values)
                {
                    if (sb.Length > 0)
                        sb.Append('&');

                    sb.AppendFormat("{0}={1}", key.UrlEncode(), value.UrlEncode());
                }
            }

            return sb.ToString();
        }
    }
}