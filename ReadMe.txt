/*--------------------------------------------------------------------------
* ReactiveOAuth
* ver 0.3.0.1 (Jun. 16th, 2011)
*
* created and maintained by neuecc <ils@neue.cc>
* licensed under Microsoft Public License(Ms-PL)
* http://neue.cc/
* http://reactiveoauth.codeplex.com/
*--------------------------------------------------------------------------*/

// Description

OAuth library for .NET Framework 4 Client Profile, Silverlight4 and Windows Phone 7.
ReactiveOAuth is based on Reactive Extensions(Rx)
- http://msdn.microsoft.com/en-us/data/gg577609
All network access return IObservable<T> and everything is asynchronous.

Rx is included in Windows Phone 7.
If you use ReactiveOAuth then can share code between WPF and Windows Phone 7.

// Features

* support Console/WPF, Silverlight4 and Windows Phone 7.
* easy operation and high affinity for streaming api.

// Notice

currently target Rx version is Build 1.1.10425.0

// Tutorial

please see Console/WPF/Silverlight/WP7 samples.

// history

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