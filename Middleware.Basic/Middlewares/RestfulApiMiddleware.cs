using AspNetCoreDemo.Middleware.Basic.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Middleware.Basic.Middlewares
{
    public class RestfulApiMiddleware : IMiddleware
    {
        private static readonly IList<MediaTypeHeaderValue> SupportedMediaTypes = new[] { new MediaTypeHeaderValue("application/json") };
        private static readonly IList<string> SupportedCharsets = new[] { "utf-8" };
        private static readonly IList<string> SupportedEncodings = new[] { "identity" };
        private static readonly IList<string> SupportedLanguages = new[] { "en-US" };

        private readonly IResourceStore _store;
        private readonly ISerializer _serializer;

        public RestfulApiMiddleware(
            IResourceStore store,
            ISerializer serializer)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var request = context.Request;
            var response = context.Response;

            response.Headers.Add("Server", "DemoServer");

            var match = Regex.Match(request.Path.ToString(), "(?<=^/resources/)[a-z](?=/?$)");
            if (!match.Success)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }
            var escapedResourceId = match.Value;
            var resourceId = Uri.UnescapeDataString(escapedResourceId);
            var refersToResourceCollection = string.IsNullOrEmpty(resourceId);

            object resource = null;
            switch (request.Method)
            {
                case "OPTIONS":
                    if (refersToResourceCollection)
                    {
                        response.Headers.Add("Allow", "OPTIONS, GET, HEAD, POST, DELETE");
                    }
                    else
                    {
                        response.Headers.Add("Allow", "OPTIONS, GET, HEAD, PUT, DELETE");
                    }
                    break;

                case "GET":
                case "HEAD":
                    {
                        var mediaType = SupportedMediaTypes[0].MediaType.ToString();
                        var charset = SupportedCharsets[0];
                        var encoding = SupportedEncodings[0];
                        var language = SupportedLanguages[0];
                        if (response.Headers.TryGetValue("Accept", out var acceptHeaderValues))
                        {
                            mediaType = NegotiateMediaType(acceptHeaderValues);
                            if (mediaType == null)
                            {
                                response.StatusCode = StatusCodes.Status406NotAcceptable;
                                return;
                            }
                        }
                        if (response.Headers.TryGetValue("Accept-Charset", out var acceptCharsetHeaderValue))
                        {
                            charset = NegotiateCharset(acceptCharsetHeaderValue);
                            if (charset == null)
                            {
                                response.StatusCode = StatusCodes.Status406NotAcceptable;
                                return;
                            }
                        }
                        if (response.Headers.TryGetValue("Accept-Encoding", out var acceptEncodingHeaderValue))
                        {
                            encoding = NegotiateEncoding(acceptEncodingHeaderValue);
                            if (encoding == null)
                            {
                                response.StatusCode = StatusCodes.Status406NotAcceptable;
                                return;
                            }
                        }
                        if (response.Headers.TryGetValue("Accept-Language", out var acceptLanguageHeaderValue))
                        {
                            language = NegotiateLanguage(acceptLanguageHeaderValue);
                            if (language == null)
                            {
                                response.StatusCode = StatusCodes.Status406NotAcceptable;
                                return;
                            }
                        }
                        response.Headers.Add("Content-Type", mediaType);
                        response.Headers.Add("Content-Encoding", encoding);
                        response.Headers.Add("Content-Language", language);
                        response.Headers.Add("Cache-Control", "public, max-age=31536000");
                        response.Headers.Add("Accept-Ranges", "none");
                        if (request.Method == "GET")
                        {
                            resource = refersToResourceCollection
                                ? _store.GetResourceCollection()
                                : _store.GetResource(resourceId);
                            if (resource == null)
                            {
                                response.StatusCode = StatusCodes.Status404NotFound;
                                return;
                            }
                            var bodyContent = _serializer.SerializeObject(resource, mediaType);
                            var writer = new StreamWriter(
                                response.Body,
                                new UTF8Encoding(false),
                                1024,
                                true);
                            await writer.WriteAsync(bodyContent);
                        }
                    }
                    response.StatusCode = StatusCodes.Status200OK;
                    break;

                case "PUT":
                    {
                        if (refersToResourceCollection)
                        {
                            response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                            return;
                        }
                        string mediaType = null;
                        var encoding = SupportedEncodings[0];
                        var language = SupportedLanguages[0];
                        if (response.Headers.TryGetValue("Content-Type", out var acceptHeaderValues))
                        {
                            mediaType = NegotiateMediaType(acceptHeaderValues);
                            if (mediaType == null)
                            {
                                response.StatusCode = StatusCodes.Status406NotAcceptable;
                                return;
                            }
                        }
                        if (response.Headers.TryGetValue("Content-Encoding", out var acceptEncodingHeaderValue))
                        {
                            encoding = NegotiateEncoding(acceptEncodingHeaderValue);
                            if (encoding == null)
                            {
                                response.StatusCode = StatusCodes.Status406NotAcceptable;
                                return;
                            }
                        }
                        if (response.Headers.TryGetValue("Content-Language", out var acceptLanguageHeaderValue))
                        {
                            language = NegotiateLanguage(acceptLanguageHeaderValue);
                            if (language == null)
                            {
                                response.StatusCode = StatusCodes.Status406NotAcceptable;
                                return;
                            }
                        }
                        var putResource = _serializer.DeserializeObject(request.Body, mediaType);
                        var created = _store.StoreResource(resourceId, putResource);
                        AddLocationHeader(response, resourceId);
                        if (created)
                        {
                            response.StatusCode = StatusCodes.Status201Created;
                        }
                        else
                        {
                            response.StatusCode = StatusCodes.Status204NoContent;
                        }
                        break;
                    }

                case "POST":
                    {
                        if (!refersToResourceCollection)
                        {
                            response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                            return;
                        }
                        string mediaType = null;
                        var encoding = SupportedEncodings[0];
                        var language = SupportedLanguages[0];
                        if (response.Headers.TryGetValue("Content-Type", out var acceptHeaderValues))
                        {
                            mediaType = NegotiateMediaType(acceptHeaderValues);
                            if (mediaType == null)
                            {
                                response.StatusCode = StatusCodes.Status406NotAcceptable;
                                return;
                            }
                        }
                        if (response.Headers.TryGetValue("Content-Encoding", out var acceptEncodingHeaderValue))
                        {
                            encoding = NegotiateEncoding(acceptEncodingHeaderValue);
                            if (encoding == null)
                            {
                                response.StatusCode = StatusCodes.Status406NotAcceptable;
                                return;
                            }
                        }
                        if (response.Headers.TryGetValue("Content-Language", out var acceptLanguageHeaderValue))
                        {
                            language = NegotiateLanguage(acceptLanguageHeaderValue);
                            if (language == null)
                            {
                                response.StatusCode = StatusCodes.Status406NotAcceptable;
                                return;
                            }
                        }
                        var postedResource = _serializer.DeserializeObject(request.Body, mediaType);
                        var addedResourceId = _store.AddResource(postedResource);
                        AddLocationHeader(response, addedResourceId);
                        response.StatusCode = StatusCodes.Status201Created;
                        break;
                    }

                case "DELETE":
                    if (refersToResourceCollection)
                    {
                        _store.Clear();
                    }
                    else
                    {
                        _store.RemoveResource(resourceId);
                    }
                    response.StatusCode = StatusCodes.Status204NoContent;
                    break;

                default:
                    response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                    return;
            }
        }

        private static void AddLocationHeader(HttpResponse response, string resourceId)
        {
            var escapedResourceId = Uri.EscapeDataString(resourceId);
            var locationUrl = new Uri($"/resources/{escapedResourceId}", UriKind.Relative);
            response.Headers.Add("Location", locationUrl.ToString());
        }

        private object DeserializeObject(Stream body, string mediaType)
        {
            throw new NotImplementedException();
        }

        private static string NegotiateMediaType(IList<string> acceptHeaderValues)
        {
            var acceptedMediaTypes = new List<MediaTypeHeaderValue>(MediaTypeHeaderValue.ParseList(acceptHeaderValues));
            acceptedMediaTypes.Sort(MediaTypeHeaderValueComparer.QualityComparer);
            foreach (var acceptedMediaType in acceptedMediaTypes)
            {
                var match = SupportedMediaTypes.FirstOrDefault(v => acceptedMediaType.IsSubsetOf(v));
                if (match != null) return match.MediaType.ToString();
            }
            return null;
        }

        private static string NegotiateCharset(IList<string> acceptCharsetHeaderValues)
        {
            return Negotiate(acceptCharsetHeaderValues, SupportedCharsets);
        }

        private static string NegotiateEncoding(IList<string> acceptEncodingHeaderValues)
        {
            return Negotiate(acceptEncodingHeaderValues, SupportedEncodings);
        }

        private static string NegotiateLanguage(IList<string> acceptLanguageHeaderValues)
        {
            return Negotiate(acceptLanguageHeaderValues, SupportedLanguages);
        }

        private static string Negotiate(IList<string> acceptedValueStrings, IList<string> supportedValueStrings)
        {
            var acceptedValues = new List<StringWithQualityHeaderValue>(StringWithQualityHeaderValue.ParseList(acceptedValueStrings));
            acceptedValues.Sort(StringWithQualityHeaderValueComparer.QualityComparer);
            foreach (var acceptedValue in acceptedValues)
            {
                var match = supportedValueStrings.FirstOrDefault(v => acceptedValue.Value == v);
                if (match != null) return match;
            }
            return null;
        }

        private string SerializeObject(object resource, string mediaType)
        {
            throw new NotImplementedException();
        }
    }
}
