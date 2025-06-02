using System;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;

namespace Normal.GorillaTemplate.Cosmetics {
    public class CosmeticsSync : RealtimeComponent<CosmeticsSyncModel> {
        [Serializable]
        public struct CosmeticEntry {
            public Cosmetic cosmetic;
        }

        [SerializeField]
        private List<CosmeticEntry> _cosmetics = new List<CosmeticEntry>();

        private readonly HashSet<string> _enabledCosmetics = new HashSet<string>();

#if UNITY_EDITOR
        private void Reset() {
            var cosmeticsInChildren = GetComponentsInChildren<Cosmetic>(true);

            _cosmetics.Clear();
            foreach (var cosmetic in cosmeticsInChildren) {
                _cosmetics.Add(new CosmeticEntry() {
                    cosmetic = cosmetic,
                });
            }
        }
#endif

        public void SetEnabled(Cosmetic cosmetic, bool value) {
            SetEnabled(cosmetic.cosmeticName, value);
        }

        public void SetEnabled(string cosmeticName, bool value) {
            var cosmeticIsCurrentlyEnabled = _enabledCosmetics.Contains(cosmeticName);
            if (cosmeticIsCurrentlyEnabled == value) {
                return;
            }

            if (value) {
                _enabledCosmetics.Add(cosmeticName);
            } else {
                _enabledCosmetics.Remove(cosmeticName);
            }

            UpdateModel();
        }

        public void Clear() {
            _enabledCosmetics.Clear();
            UpdateModel();
        }

#region Serialization

        [Serializable]
        private class JsonData {
            public List<string> enabledCosmetics = new List<string>();

            public void CopyFromHashSet(HashSet<string> source) {
                enabledCosmetics.Clear();
                foreach (var cosmeticName in source) {
                    enabledCosmetics.Add(cosmeticName);
                }
            }

            public void CopyToHashSet(HashSet<string> destination) {
                destination.Clear();
                foreach (var cosmeticName in enabledCosmetics) {
                    destination.Add(cosmeticName);
                }
            }
        }

        private readonly JsonData _tempData = new JsonData();

        public string SaveJson() {
            _tempData.CopyFromHashSet(_enabledCosmetics);
            var json = JsonUtility.ToJson(_tempData, true);
            return json;
        }

        public bool TryLoadJson(string json) {
            try {
                JsonUtility.FromJsonOverwrite(json, _tempData);
                _tempData.CopyToHashSet(_enabledCosmetics);
                UpdateModel();
                return true;
            } catch (Exception e) {
                Debug.LogError($"Failed to parse cosmetics JSON data: {e}\n{json}");
                return false;
            }
        }

#endregion

#region Sync

        protected override void OnRealtimeModelReplaced(CosmeticsSyncModel previousModel, CosmeticsSyncModel currentModel) {
            if (previousModel != null) {
                previousModel.serializedDataDidChange -= OnSerializedDataChanged;
            }

            if (currentModel != null) {
                currentModel.serializedDataDidChange += OnSerializedDataChanged;

                if (currentModel.isFreshModel) {
                    UpdateModel();
                }
            }
        }

        private void OnSerializedDataChanged(CosmeticsSyncModel currentModel, string value) {
            TryLoadJson(value);

            foreach (var entry in _cosmetics) {
                var shouldBeEnabled = _enabledCosmetics.Contains(entry.cosmetic.cosmeticName);
                entry.cosmetic.cosmeticEnabled = shouldBeEnabled;
            }
        }

        private struct RecursiveGuard : IDisposable {
            public static bool isRecursing => __count > 0;

            private static int __count;

            public static RecursiveGuard Create() {
                __count++;
                return new RecursiveGuard();
            }

            public void Dispose() {
                __count--;
            }
        }

        private void UpdateModel() {
            if (RecursiveGuard.isRecursing) {
                return;
            }

            using var guard = RecursiveGuard.Create();

            if (model == null) {
                var json = SaveJson();
                OnSerializedDataChanged(null, json);
            } else {
                var json = SaveJson();
                model.serializedData = json;
            }
        }

        public bool TryGetIsOwnedRemotelyInHierarchy(out bool value) {
            if (model == null) {
                value = default;
                return false;
            }

            value = isOwnedRemotelyInHierarchy;
            return true;
        }

#endregion
    }

    [RealtimeModel]
    public partial class CosmeticsSyncModel {
        [RealtimeProperty(1, true, true)]
        private string _serializedData;
    }
}
