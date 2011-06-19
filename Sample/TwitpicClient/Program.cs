using System;
using System.IO;
using System.Net;
using System.Reactive.Linq;
using Codeplex.OAuth;

class Program
{
    static void Main(string[] args)
    {
        // set your keys
        var consumerKey = "";
        var consumerSecret = "";
        var accessTokenKey = "";
        var accessTokenSecret = "";

        var accessToken = new AccessToken(accessTokenKey, accessTokenSecret);
        var client = new TwitpicClient(consumerKey, consumerSecret, accessToken);

        var file = File.ReadAllBytes(@"set upload filepath");

        client.UploadPicture("testpicture", "MESSAGE TEST!", file)
            .Catch((WebException ex) =>
            {
                Console.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
                return Observable.Empty<string>();
            })
            .Subscribe(Console.WriteLine, e => Console.WriteLine(e));

        Console.WriteLine("Waiting Upload...");
        Console.ReadLine();
    }
}