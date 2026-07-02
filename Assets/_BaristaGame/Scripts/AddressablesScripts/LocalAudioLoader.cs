using UnityEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _BaristaGame.Scripts.AddressablesScripts
{
    public class LocalAudioLoader : LocalAssetLoader<AudioClip>
    {
        private const string _audioIdPath = "";
        private const string _resourcesAudioPathPrefix = "Audio/VoiceLines";
        private static readonly Regex _indexedClipPattern = new(@"^(.*?)(?:[_\-\s])(\d+)$", RegexOptions.Compiled);
        private static readonly object _resourceCacheLock = new();
        private static AudioClip[] _resourceAudioCache;
        private bool _loadedFromAddressables;

        public async Task<AudioClip> LoadAudioAsync(string audioName)
        {
            if (string.IsNullOrWhiteSpace(audioName))
            {
                throw new ArgumentException("Audio name is null or empty.", nameof(audioName));
            }

            CachedAsset = default;
            _loadedFromAddressables = false;

            string audioId = _audioIdPath + audioName;

            await LoadAssetAsync(audioId);

            if (CachedAsset != null)
            {
                _loadedFromAddressables = true;
                return CachedAsset;
            }

            AudioClip resourcesClip = ResolveResourcesAudioClip(audioName);
            if (resourcesClip != null)
            {
                CachedAsset = resourcesClip;
                return CachedAsset;
            }

            throw new NullReferenceException($"AudioClip with ID {audioId} was not found in Addressables or Resources/{_resourcesAudioPathPrefix}/");
        }


        public void UnloadAudio()
        {
            if (_loadedFromAddressables)
            {
                UnloadAsset();
            }
            else
            {
                CachedAsset = default;
            }

            _loadedFromAddressables = false;
        }

        private static AudioClip ResolveResourcesAudioClip(string audioName)
        {
            AudioClip direct = Resources.Load<AudioClip>($"{_resourcesAudioPathPrefix}/{audioName}");
            if (direct != null)
            {
                return direct;
            }

            foreach (string alias in BuildSimpleAliases(audioName))
            {
                AudioClip aliasClip = Resources.Load<AudioClip>($"{_resourcesAudioPathPrefix}/{alias}");
                if (aliasClip != null)
                {
                    return aliasClip;
                }
            }

            return FindBestMatchingResourceClip(audioName);
        }

        private static IEnumerable<string> BuildSimpleAliases(string audioName)
        {
            var aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(audioName))
            {
                return aliases;
            }

            aliases.Add(audioName);

            if (TryParseIndexedName(audioName, out string baseName, out int index))
            {
                aliases.Add($"{baseName}_{index}");
                aliases.Add($"{baseName}_{index:00}");
            }

            return aliases;
        }

        private static AudioClip FindBestMatchingResourceClip(string requestedName)
        {
            AudioClip[] clips = GetResourceAudioCache();
            if (clips == null || clips.Length == 0)
            {
                return null;
            }

            NameParts requested = ToNameParts(requestedName);
            AudioClip bestClip = null;
            int bestScore = int.MaxValue;
            int bestLengthDelta = int.MaxValue;

            foreach (AudioClip clip in clips)
            {
                if (clip == null || string.IsNullOrWhiteSpace(clip.name))
                {
                    continue;
                }

                NameParts candidate = ToNameParts(clip.name);
                int score = ComputeMatchScore(requested, candidate);
                if (score < 0)
                {
                    continue;
                }

                int lengthDelta = Math.Abs(candidate.BaseToken.Length - requested.BaseToken.Length);
                if (score < bestScore || (score == bestScore && lengthDelta < bestLengthDelta))
                {
                    bestScore = score;
                    bestLengthDelta = lengthDelta;
                    bestClip = clip;
                }
            }

            return bestClip;
        }

        private static int ComputeMatchScore(NameParts requested, NameParts candidate)
        {
            if (string.IsNullOrEmpty(requested.BaseToken) || string.IsNullOrEmpty(candidate.BaseToken))
            {
                return -1;
            }

            bool indexCompatible = requested.Index < 0 || candidate.Index < 0 || requested.Index == candidate.Index;
            if (!indexCompatible)
            {
                return -1;
            }

            if (requested.BaseToken == candidate.BaseToken)
            {
                return 0;
            }

            if (candidate.BaseToken.EndsWith(requested.BaseToken, StringComparison.Ordinal) ||
                requested.BaseToken.EndsWith(candidate.BaseToken, StringComparison.Ordinal))
            {
                return 1;
            }

            if (candidate.BaseToken.Contains(requested.BaseToken, StringComparison.Ordinal) ||
                requested.BaseToken.Contains(candidate.BaseToken, StringComparison.Ordinal))
            {
                return 2;
            }

            return -1;
        }

        private static AudioClip[] GetResourceAudioCache()
        {
            if (_resourceAudioCache != null)
            {
                return _resourceAudioCache;
            }

            lock (_resourceCacheLock)
            {
                if (_resourceAudioCache == null)
                {
                    _resourceAudioCache = Resources.LoadAll<AudioClip>(_resourcesAudioPathPrefix);
                }
            }

            return _resourceAudioCache;
        }

        private static bool TryParseIndexedName(string name, out string baseName, out int index)
        {
            baseName = name;
            index = -1;

            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            Match match = _indexedClipPattern.Match(name.Trim());
            if (!match.Success || match.Groups.Count < 3)
            {
                return false;
            }

            baseName = match.Groups[1].Value.Trim();
            return int.TryParse(match.Groups[2].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out index);
        }

        private static NameParts ToNameParts(string name)
        {
            if (TryParseIndexedName(name, out string baseName, out int index))
            {
                return new NameParts(NormalizeToken(baseName), index);
            }

            return new NameParts(NormalizeToken(name), -1);
        }

        private static string NormalizeToken(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var sb = new StringBuilder(value.Length);
            foreach (char c in value)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(char.ToLowerInvariant(c));
                }
            }

            return sb.ToString();
        }

        private readonly struct NameParts
        {
            public NameParts(string baseToken, int index)
            {
                BaseToken = baseToken;
                Index = index;
            }

            public string BaseToken { get; }
            public int Index { get; }
        }
    }
}