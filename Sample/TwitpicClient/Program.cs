using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codeplex.OAuth;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Net;
using System.Reactive.Subjects;

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

        var file = File.ReadAllBytes(@"C:\Users\neuecc\Desktop\codecontracts_unproven.jpg");

        client.UploadPicture("testpicture", file)
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