namespace Orleans.StorageProvider.RavenDB
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using System.Threading.Tasks;
    using Raven.Client;
    using Raven.Imports.Newtonsoft.Json.Utilities;

    internal static class AsyncDocumentSessionExtensions
    {
        private static readonly ConcurrentDictionary<Type, MethodInfo> LoadAsyncMethodInfoCache = new ConcurrentDictionary<Type, MethodInfo>(); 

        public static async Task<IGrainState> LoadAsync(this IAsyncDocumentSession session, IGrainState grainState, string id)
        {
            var methodInfo = LoadAsyncMethodInfoCache.GetOrAdd(
                grainState.GetType(),
                t => session.GetType().GetGenericMethod("LoadAsync", typeof(string)).MakeGenericMethod(grainState.GetType()));
            return await((dynamic)methodInfo.Invoke(session, new object[] { id }));
        }
    }
}