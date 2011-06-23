using System;
using System.Security.Cryptography;
using System.Text;
using Codeplex.OAuth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ReactiveOAuth.Tests
{
    // http://wiki.oauth.net/w/page/12238556/TestCases

    [TestClass]
    public class OAuthTestCases
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [TestCase("abcABC123", "abcABC123")]
        [TestCase("-._~", "-._~")]
        [TestCase("%", "%25")]
        [TestCase("+", "%2B")]
        [TestCase("&=*", "%26%3D%2A")]
        [TestCase('\u000A', "%0A")]
        [TestCase('\u0020', "%20")]
        [TestCase('\u007F', "%7F")]
        [TestCase('\u0080', "%C2%80")]
        [TestCase('\u3001', "%E3%80%81")]
        public void ParameterEncoding()
        {
            TestContext.Run((object source, string expected) =>
            {
                Codeplex.OAuth.Utility.UrlEncode(source.ToString()).Is(expected);
            });
        }

        public static object[] NormalizeRequestParametersSource = new[]
        {
            new object[] {new ParameterCollection { { "name", "" } }, "name="},
            new object[] {new ParameterCollection { { "a", "b" } }, "a=b"},
            new object[] {new ParameterCollection { { "a", "b" }, { "c", "d" } }, "a=b&c=d"},
            new object[] {new ParameterCollection { { "x!y", "a" }, { "x", "a" } }, "x=a&x%21y=a"}
        };

        [TestMethod]
        [TestCaseSource("NormalizeRequestParametersSource")]
        public void NormalizeRequestParameters()
        {
            TestContext.Run((ParameterCollection source, string expected) =>
            {
                source.OrderBy(p => p.Key)
                   .ThenBy(p => p.Value)
                   .ToQueryParameter()
                   .Is(expected);
            });
        }

        [TestMethod]
        [TestCase("cs", "", "bs", "egQqG5AJep5sJ7anhXju1unge2I=")]
        [TestCase("cs", "ts", "bs", "VZVjXceV7JgPq/dOTnNmEfO0Fv8=")]
        [TestCase("kd94hf93k423kf44", "pfkkdhi9sl3r4s00",
            "GET&http%3A%2F%2Fphotos.example.net%2Fphotos&file%3Dvacation.jpg%26oauth_consumer_key%3Ddpf43f3p2l4k3l03%26oauth_nonce%3Dkllo9940pd9333jh%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1191242096%26oauth_token%3Dnnch734d00sl2jdk%26oauth_version%3D1.0%26size%3Doriginal",
            "tR3+Ty81lMeYAr/Fid0kMTYa/WM=")]
        public void HMACSHA1()
        {
            TestContext.Run((string consumerSecret, string tokenSecret, string baseString, string signature) =>
            {
                var hmacKeyBase = consumerSecret.UrlEncode() + "&" + tokenSecret.UrlEncode();
                using (var hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(hmacKeyBase)))
                {
                    var hash = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(baseString));
                    Convert.ToBase64String(hash).Is(signature);
                }
            });
        }

        // http://tools.ietf.org/html/rfc5849
        [TestMethod]
        public void Example()
        {
            new ParameterCollection
            {
                {"b5", "=%3D"},
                {"a3", "a"},
                {"c@", ""},
                {"a2", "r b"},
                {"oauth_consumer_key", "9djdj82h48djs9d2"},
                {"oauth_token", "kkk9d7dh3k39sjv7"},
                {"oauth_signature_method", "HMAC-SHA1"},
                {"oauth_timestamp", "137131201"},
                {"oauth_nonce", "7d8f3e4a"},
                {"c2", ""},
                {"a3", "2 q"},
            }.OrderBy(p => p.Key)
            .ThenBy(p => p.Value)
            .ToQueryParameter()
            .Is("a2=r%20b&a3=2%20q&a3=a&b5=%3D%253D&c%40=&c2=&oauth_consumer_key=9djdj82h48djs9d2&oauth_nonce=7d8f3e4a&oauth_signature_method=HMAC-SHA1&oauth_timestamp=137131201&oauth_token=kkk9d7dh3k39sjv7");
        }
    }
}
