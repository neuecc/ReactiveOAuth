using System;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive.Linq;
#endif

namespace Codeplex.OAuth
{
    public class TwitpicClient : OAuthBase
    {
        const string ApiKey = ""; // set your apikey
        static readonly Random random = new Random();

        readonly AccessToken accessToken;

        public TwitpicClient(string consumerKey, string consumerSecret, AccessToken accessToken)
            : base(consumerKey, consumerSecret)
        {
            this.accessToken = accessToken;
        }

        private WebRequest CreateRequest(string url)
        {
            const string ServiceProvider = "https://api.twitter.com/1/account/verify_credentials.json";
            const string Realm = "http://api.twitter.com/";

            var req = WebRequest.Create(url);

            // generate oauth signature and parameters
            var parameters = ConstructBasicParameters(ServiceProvider, MethodType.Get, accessToken);
            // make auth header string
            var authHeader = BuildAuthorizationHeader(new[] { new Parameter("Realm", Realm) }.Concat(parameters));

            // set authenticate headers
            req.Headers["X-Verify-Credentials-Authorization"] = authHeader;
            req.Headers["X-Auth-Service-Provider"] = ServiceProvider;

            return req;
        }

        public IObservable<string> UploadPicture(string filename, byte[] file)
        {
            var req = CreateRequest("http://api.twitpic.com/2/upload.xml"); // choose xml or json
            req.Method = "POST";

            var boundaryKey = random.Next();
            var boundary = "--" + boundaryKey;
            req.ContentType = "multipart/form-data; boundary=" + boundaryKey;

            return Observable.Defer(() =>
                    Observable.FromAsyncPattern<Stream>(req.BeginGetRequestStream, req.EndGetRequestStream)())
                .Do(stream =>
                {
                    using (stream)
                    using (var sw = new StreamWriter(stream, new UTF8Encoding(false)))
                    {
                        sw.WriteLine(boundary);
                        sw.WriteLine("Content-Disposition: form-data; name=\"key\"");
                        sw.WriteLine();
                        sw.WriteLine(ApiKey);
                        sw.WriteLine(boundary);

                        sw.WriteLine("Content-Disposition: form-data; name=\"message\"");
                        sw.WriteLine();
                        sw.WriteLine("");
                        sw.WriteLine(boundary);

                        sw.WriteLine("Content-Disposition: form-data; name=\"media\"; filename=\"" + filename + "\"");
                        sw.WriteLine("Content-Type: application/octet-stream");
                        sw.WriteLine("Content-Transfer-Encoding: binary");
                        sw.WriteLine();
                        sw.Flush();

                        stream.Write(file, 0, file.Length);
                        stream.Flush();

                        sw.WriteLine();
                        sw.WriteLine("--" + boundaryKey + "--");
                        sw.Flush();
                    }
                })
                .SelectMany(_ => Observable.FromAsyncPattern<WebResponse>(req.BeginGetResponse, req.EndGetResponse)())
                .Select(res =>
                {
                    using (res)
                    using (var stream = res.GetResponseStream())
                    using (var sr = new StreamReader(stream, Encoding.UTF8))
                    {
                        return sr.ReadToEnd();
                    }
                });
        }
    }
}