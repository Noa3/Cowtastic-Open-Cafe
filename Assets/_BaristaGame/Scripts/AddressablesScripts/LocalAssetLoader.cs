using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace _BaristaGame.Scripts.AddressablesScripts
{
    public class LocalAssetLoader<T>
    {
        protected T CachedAsset;

        protected async Task<T> LoadAssetAsync(string assetId)
        {
            IList<IResourceLocation> locations = Addressables.LoadResourceLocationsAsync(assetId).WaitForCompletion();
            if (locations.Any() == false)
            {
                return CachedAsset;
            }

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetId);
            CachedAsset = await handle.Task;

            if (EqualityComparer<T>.Default.Equals(CachedAsset, default(T)))
            {
                throw new NullReferenceException($"Asset of type {typeof(T)} with ID {assetId} is null on attempt to load it from addressables");
            }

            return CachedAsset;
        }

        protected void UnloadAsset()
        {
            if (EqualityComparer<T>.Default.Equals(CachedAsset, default(T)))
                return;

            Addressables.Release(CachedAsset);
            CachedAsset = default(T);
            
        }
    }
}