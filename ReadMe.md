ReactiveOAuth
===
OAuth library for .NET Framework 4 Client Profile, Silverlight 4 and Windows Phone 7.

Info
---
This project is obsolete, plaease check [AsyncOAuth](https://github.com/neuecc/AsyncOAuth/).

Archive, import from codeplex.

Description
---
OAuth library for .NET Framework 4 Client Profile, Silverlight 4 and Windows Phone 7. ReactiveOAuth is based on [Reactive Extensions(Rx)](http://msdn.microsoft.com/en-us/devlabs/ee794896.aspx). All network access return IObservable<T> and everything is asynchronous.

Rx is included in Windows Phone 7. If you use ReactiveOAuth then can share code between WPF and Windows Phone 7.

Features
---
* support .NET Framework 4 Client Profile, Silverlight 4 and Windows Phone 7.
* support twitter's xAuth
* easy operation and high affinity for streaming api.
* NuGet Online Package available ([ReactiveOAuth](http://nuget.org/List/Packages/ReactiveOAuth), [ReactiveOAuth-WP7](http://nuget.org/List/Packages/ReactiveOAuth-WP7)

Tutorial for twitter gettoken/get/post/streaming
---

GetAccessToken
---

![image](https://cloud.githubusercontent.com/assets/46207/24585069/70f37fb4-17bc-11e7-803f-f2337d23aa32.png)

GetRequestToken -> BuildAuthorizeUrl(and navigate browser)

```csharp
// global variable
const string ConsumerKey = "consumerkey";
const string ConsumerSecret = "consumersecret";
RequestToken requestToken;
AccessToken accessToken;

private void GetRequestTokenButton_Click(object sender, RoutedEventArgs e)
{
    var authorizer = new OAuthAuthorizer(ConsumerKey, ConsumerSecret);
    authorizer.GetRequestToken("http://twitter.com/oauth/request_token")
        .Select(res => res.Token)
        .ObserveOnDispatcher()
        .Subscribe(token =>
        {
            requestToken = token;
            var url = authorizer.BuildAuthorizeUrl("http://twitter.com/oauth/authorize", token);
            webBrowser1.Navigate(new Uri(url)); // navigate browser
        });
}    
```

![image](https://cloud.githubusercontent.com/assets/46207/24585070/76fe0eec-17bc-11e7-92e3-be9470cf6909.png)

(user input pincode) -> GetAccessToken(and other parameter)

```csharp
private void GetAccessTokenButton_Click(object sender, RoutedEventArgs e)
{
    var pincode = PinCodeTextBox.Text; // userinput

    var authorizer = new OAuthAuthorizer(ConsumerKey, ConsumerSecret);
    authorizer.GetAccessToken("http://twitter.com/oauth/access_token", requestToken, pincode)
        .ObserveOnDispatcher()
        .Subscribe(res =>
        {
            // response has Token and extra data(twitter is user_id and screen_name)
            UserIdTextBlock.Text = res.ExtraData["user_id"].First();
            ScreenNameTextBlock.Text = res.ExtraData["screen_name"].First();
            accessToken = res.Token; // AccessToken
        });
}
```

Get TimeLine
---
![image](https://cloud.githubusercontent.com/assets/46207/24585071/7d17f91e-17bc-11e7-82ea-6c29e822539b.png)


sorry, image's language is Japanese:)

```csharp
// set url and parameters.
// parameter can use Collection Initializer
private void GetTimeLineButton_Click(object sender, RoutedEventArgs e)
{
    var client = new OAuthClient(ConsumerKey, ConsumerSecret, accessToken)
    {
        Url = "http://api.twitter.com/1/statuses/home_timeline.xml",
        Parameters = { { "count", 20 }, { "page", 1 } }
    };
    client.GetResponseText() // OAuthClient have three methods, GetResponse/GetResponseText/GetResponseLines
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
            ex => MessageBox.Show(ex.ToString())); // easy error handling
}
```

Post Status
---
![image](https://cloud.githubusercontent.com/assets/46207/24585072/827ed7b0-17bc-11e7-93c9-dc64ab31481c.png)

```csharp
// if post then set MethodType = MethodType.Post
private void PostButton_Click(object sender, RoutedEventArgs e)
{
    var postText = PostTextBox.Text; // user input
    
    var client = new OAuthClient(ConsumerKey, ConsumerSecret, accessToken)
    {
        MethodType = MethodType.Post,
        Url = "http://api.twitter.com/1/statuses/update.xml",
        Parameters = { { "status", postText } }
    };
    client.GetResponseText() // post and GetResponse
        .Select(s => XElement.Parse(s))
        .ObserveOnDispatcher()
        .Subscribe(x => MessageBox.Show("Post Success:" + x.Element("text").Value));
}
```

StreamingAPI
---
![image](https://cloud.githubusercontent.com/assets/46207/24585075/88b6c2fa-17bc-11e7-8513-172d573a45cb.png)

WPF StreamingAPI sample
read JSON using with [DynamicJson](http://dynamicjson.codeplex.com/)
Rx x ReactiveOAuth x DynamicJson make easy for use StreamingAPI.

```csharp
IDisposable streamingHandle = Disposable.Empty;

private void StreamingStartButton_Click(object sender, RoutedEventArgs e)
{
    var client = new OAuthClient(ConsumerKey, ConsumerSecret, accessToken)
    {
         Url = "https://userstream.twitter.com/2/user.json"
    };
    streamingHandle = client.GetResponseLines()
        .Where(s => !string.IsNullOrWhiteSpace(s)) // filter invalid data
        .Select(s => DynamicJson.Parse(s))
        .Where(d => d.text()) // has text is "status"
        .ObserveOnDispatcher()
        .Subscribe(
            d => StreamingViewListBox.Items.Add(d.user.screen_name + ":" + d.text));
}

// call Dispose is stop streaming
private void StreamingStopButton_Click(object sender, RoutedEventArgs e)
{
    streamingHandle.Dispose();
}
```

Twitpic Upload
---

TwitpicClient.cs is available in Sample/TwitpicClient.cs.
You can use copy the single .cs file.

```csharp
// CameraCaptureTask's Completed Event
void camera_Completed(object sender, PhotoResult e)
{
    if (e.TaskResult == TaskResult.OK)
    {
        // photo picture(Stream) to byte array
        var stream = e.ChosenPhoto;
        var buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);

        // set key, sercret, token
        new TwitpicClient(ConsumerKey, ConsumerSecret, accessToken)
            .UploadPicture(e.OriginalFileName, "from WP7!", buffer)
            .ObserveOnDispatcher()
            .Catch((WebException ex) =>
            {
                MessageBox.Show(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
                return Observable.Empty<string>();
            })
            .Subscribe(s => MessageBox.Show(s), ex => MessageBox.Show(ex.ToString()));
    }
}
```
