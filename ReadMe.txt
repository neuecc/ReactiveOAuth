/*---------------------------------------------------------------------------
 * ReactiveOAuth
 * ver 0.4.0.0 (Jun. 21th, 2011)
 *
 * created and maintained by neuecc <ils@neue.cc>
 * licensed under Microsoft Public License(Ms-PL)
 * http://neue.cc/
 * http://reactiveoauth.codeplex.com/
 *-------------------------------------------------------------------------*/

// Description

OAuth library for .NET Framework 4 Client Profile, Silverlight4 and Windows Phone 7.
ReactiveOAuth is based on Reactive Extensions(Rx)
All network access return IObservable<T> and everything is asynchronous.
Rx is included in Windows Phone 7.
If you use ReactiveOAuth then can share code between WPF and Windows Phone 7.

// Notice

currently target Rx version is Build 1.0.10605(Stable)

// Tutorial

please see Console/WPF/Silverlight/WP7 samples.

// Code

If you want to upload to Twitpic(or other OAuthEcho provider),
then you can copy, use Sample/TwitpicClient.cs.

// History

2011-06-21
Add
    TwitpicClient Sample
Fix Bug
    can't generate right signature when added realm

2011-06-16
Fix
    Target Rx for 1.0.10605(Stable)

2011-05-10
Fix
    Target Rx for 1.1.10425.0

2011-01-23
Fix Bug
    wrong UrlEncode
Add
    Silverlight Project and Sample
    NuGet Online package(id:ReactiveOAuth)

2010-09-12
1st Release