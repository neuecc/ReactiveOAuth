using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Codeplex.OAuth;

namespace Silverlight
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            const string ConsumerKey = "";
            const string ConsumerSecret = "";

            AccessToken accessToken = null;
            string userId, screenName;

            // get accesstoken flow
            // create authorizer and call "GetRequestToken" ,"BuildAuthorizeUrl", "GetAccessToken"
            // TokenResponse's ExtraData is ILookup.
            // if twitter, you can take "user_id" and "screen_name".
            // Run is sync. Subscribe is async.
            var authorizer = new OAuthAuthorizer(ConsumerKey, ConsumerSecret);
            authorizer.GetRequestToken("http://twitter.com/oauth/request_token")
                 .Do(r =>
                 {
                     Console.WriteLine("Check Browser and input PinCode");
                     // Process.Start(authorizer.BuildAuthorizeUrl("http://twitter.com/oauth/authorize", r.Token)); // open browser
                 })
                 .Select(r => new { RequestToken = r.Token, PinCode = Console.ReadLine() })
                 .SelectMany(a => authorizer.GetAccessToken("http://twitter.com/oauth/access_token", a.RequestToken, a.PinCode))
                 .Subscribe(r =>
                 {
                     userId = r.ExtraData["user_id"].First();
                     screenName = r.ExtraData["screen_name"].First();
                     accessToken = r.Token;
                 });

        }
    }
}
