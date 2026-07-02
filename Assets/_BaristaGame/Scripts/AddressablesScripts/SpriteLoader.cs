using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _BaristaGame.Scripts.AddressablesScripts
{
    public class SpriteLoader : MonoBehaviour
    {
        private LocalSpriteLoader _loader;
        public Image targetImage;   

        private async void Start()
        {
            if (targetImage == null)
            {
                Debug.LogError("TargetImage не установлены.");
                return;
            }

            try
            {
                _loader = new LocalSpriteLoader();
                var imageFile = await GetAddressablesSprite("Loading");
                targetImage.sprite = imageFile;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Ошибка при загрузке спрайтов: {ex.Message}");
            }
        }

        private async Task<Sprite> GetAddressablesSprite(string spriteName)
        {
            return await _loader.LoadSpriteAsync(spriteName);
        }
    }
}
