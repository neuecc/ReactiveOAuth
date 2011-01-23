using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Browser;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Codeplex.OAuth;

namespace Silverlight
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            WebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp);
            WebRequest.RegisterPrefix("https://", WebRequestCreator.ClientHttp);
        }

        // set your consumerkey and secret
        const string ConsumerKey = "";
        const string ConsumerSecret = "";

        RequestToken requestToken;
        AccessToken accessToken;

        private string ReadWebException(Exception e)
        {
            var ex = e as WebException;
            if (ex != null)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
            else return e.ToString();
        }

        // Get RequestToken flow sample
        // create authorizer and call "GetRequestToken" and "BuildAuthorizeUrl"
        private void GetRequestTokenButton_Click(object sender, RoutedEventArgs e)
        {
            var authorizer = new OAuthAuthorizer(ConsumerKey, ConsumerSecret);
            authorizer.GetRequestToken("http://twitter.com/oauth/request_token")
                .Select(res => requestToken = res.Token)
                .ObserveOnDispatcher()
                .Subscribe(token =>
                {
                    var url = authorizer.BuildAuthorizeUrl("http://twitter.com/oauth/authorize", token);
                    new MyHyperlink() { NavigateUri = new Uri(url), TargetName = "_blank" }.Browse();
                    MessageBox.Show("check browser, allow oauth and enter pincode");
                }, ex => MessageBox.Show(ReadWebException(ex)));
        }

        // Get AccessToken flow sample
        // TokenResponse's ExtraData is ILookup.
        // if twitter, you can take "user_id" and "screen_name".
        private void GetAccessTokenButton_Click(object sender, RoutedEventArgs e)
        {
            if (requestToken == null) { MessageBox.Show("at first, get requestToken"); return; }

            var pincode = PinCodeTextBox.Text;
            var authorizer = new OAuthAuthorizer(ConsumerKey, ConsumerSecret);
            authorizer.GetAccessToken("http://twitter.com/oauth/access_token", requestToken, pincode)
                .ObserveOnDispatcher()
                .Subscribe(res =>
                {
                    AuthorizedTextBlock.Text = "Authorized";
                    UserIdTextBlock.Text = res.ExtraData["user_id"].First();
                    ScreenNameTextBlock.Text = res.ExtraData["screen_name"].First();
                    accessToken = res.Token;
                }, ex => MessageBox.Show(ReadWebException(ex)));
        }

        // Twitter Read Sample
        // set parameters can use Collection Initializer
        // if you want to set webrequest parameters then use ApplyBeforeRequest
        private void GetTimeLineButton_Click(object sender, RoutedEventArgs e)
        {
            if (accessToken == null) { MessageBox.Show("at first, get accessToken"); return; }

            var client = new OAuthClient(ConsumerKey, ConsumerSecret, accessToken)
            {
                Url = "http://api.twitter.com/1/statuses/home_timeline.xml",
                Parameters = { { "count", 20 }, { "page", 1 } }
            };
            client.GetResponseText()
                .Select(s => XElement.Parse(s))
                .SelectMany(x => x.Descendants("status"))
                .Select(x => new
                {
                    Text = x.Element("text").Value,
                    Name = x.Element("user").Element("screen_name").Value
                })
                .ObserveOnDispatcher()
                .Subscribe(
                    a => TimeLineViewListBox.Items.Add(a.Name + ":" + a.Text),
                    ex => MessageBox.Show(ReadWebException(ex)));
        }

        // Twitter Post Sample
        // if post then set MethodType = MethodType.Post
        private void PostButton_Click(object sender, RoutedEventArgs e)
        {
            if (accessToken == null) { MessageBox.Show("at first, get accessToken"); return; }

            var postText = PostTextBox.Text;
            var client = new OAuthClient(ConsumerKey, ConsumerSecret, accessToken)
            {
                MethodType = MethodType.Post,
                Url = "http://api.twitter.com/1/statuses/update.xml",
                Parameters = { { "status", postText } }
            };
            client.GetResponseText()
                .Select(s => XElement.Parse(s))
                .Subscribe(x => MessageBox.Show("Post Success:" + x.Element("text").Value),
                    ex => MessageBox.Show(ReadWebException(ex)));
        }

        class MyHyperlink : HyperlinkButton
        {
            public void Browse()
            {
                this.OnClick();
            }
        }
    }
}
