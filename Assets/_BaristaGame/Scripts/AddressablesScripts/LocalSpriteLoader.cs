using UnityEngine;
using System;
using System.Threading.Tasks;

namespace _BaristaGame.Scripts.AddressablesScripts
{
    public class LocalSpriteLoader : LocalAssetLoader<Sprite>
    {
        private const string _spriteIdPath = "";

        public async Task<Sprite> LoadSpriteAsync(string spriteName)
        {
            var spriteId = _spriteIdPath + spriteName;
            
            await LoadAssetAsync(spriteId);

            if (CachedAsset == null)
            {
                throw new NullReferenceException($"Sprite with ID {spriteId} is null on attempt to load it from addressables");
            }
            
            return CachedAsset;
        }
    }
}