using System;
using System.Collections.Generic;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive;
using System.Reactive.Linq;
#endif

namespace Codeplex.OAuth
{
    public static class ObservableForCompatible
    {

#if WINDOWS_PHONE
        public static IObservable<IList<T>> Buffer<T>(this IObservable<T> source, int count)
        {
            return source.BufferWithCount(count);
        }
#endif

        public static IObservable<TSource> Create<TSource>(Func<IObserver<TSource>, IDisposable> subscribe)
        {
#if WINDOWS_PHONE
            return Observable.CreateWithDisposable(subscribe);
#else
            return Observable.Create(subscribe);
#endif
        }
    }
}
