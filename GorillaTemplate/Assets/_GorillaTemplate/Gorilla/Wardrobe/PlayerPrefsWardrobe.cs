using System.Collections.Generic;
using Normal.GorillaTemplate.Cosmetics;
using Normal.GorillaTemplate.Keyboard;
using UnityEngine;

namespace Normal.GorillaTemplate.Wardrobe {
    /// <summary>
    /// A wardrobe that lets the user browse and equip one item at a time.
    /// The user's loadout is stored locally on their device using Unity's <see cref="PlayerPrefs"/>.
    /// </summary>
    public class PlayerPrefsWardrobe : Keyboard.Keyboard, IWardrobe  {
        /// <summary>
        /// The player manager - we use it to access the local avatar.
        /// </summary>
        [SerializeField]
        private GorillaPlayerManager _gorillaPlayerManager;

        /// <summary>
        /// The object to display when the user is browsing the "no cosmetic" option.
        /// </summary>
        [SerializeField]
        private GameObject _noneWardrobeItem;

        /// <summary>
        /// The local avatar's cosmetics manager instance.
        /// </summary>
        private CosmeticsSync _cosmeticsSync;

        /// <summary>
        /// The key used to save the loadout in Unity's <see cref="PlayerPrefs"/>.
        /// </summary>
        private const string __loadoutKey = "LocalPlayerData/WardrobeLoadout";

        /// <summary>
        /// The list of all items registered with this wardrobe.
        /// </summary>
        private List<WardrobeItem> _items = new List<WardrobeItem>();

        /// <summary>
        /// The index of the item that the user is currently browsing.
        /// </summary>
        private int _browsingIndex;

        private void Awake() {
            _gorillaPlayerManager.playerJoined += OnPlayerJoined;

            // Register all items that belong to this wardrobe
            var items = GetComponentsInChildren<WardrobeItem>(true);
            foreach (var item in items) {
                _items.Add(item);
            }
        }

        private void OnDestroy() {
            _gorillaPlayerManager.playerJoined -= OnPlayerJoined;
        }

        /// <summary>
        /// Invoked when a player joins the room.
        /// </summary>
        private void OnPlayerJoined(GorillaAvatar avatar, bool isLocalPlayer) {
            // Only proceed for the local avatar
            if (isLocalPlayer == false) {
                return;
            }

            // The cosmetics manager exists on the root object of the avatar
            _cosmeticsSync = avatar.GetComponent<CosmeticsSync>();

            // Enable cosmetics using the data on the local device
            LoadLoadout();

            // Create a HashSet for quick lookup
            var enabledCosmetics = new HashSet<string>();
            foreach (var cosmetic in _cosmeticsSync.cosmetics) {
                if (cosmetic.cosmeticEnabled) {
                    enabledCosmetics.Add(cosmetic.cosmeticName);
                }
            }

            // Determine which item index, if any, corresponds to the equipped cosmetics
            _browsingIndex = 0;
            foreach (var item in _items) {
                foreach (var cosmeticName in item.cosmeticNames) {
                    if (enabledCosmetics.Contains(cosmeticName)) {
                        // + 1 to account for _noneWardrobeItem
                        _browsingIndex = _items.IndexOf(item) + 1;
                        break;
                    }
                }
            }

            // Display the equipped item
            BrowseItem(_browsingIndex);
        }

        /// <summary>
        /// Applies the cosmetics state onto the local player using the data on the local device.
        /// </summary>
        private void LoadLoadout() {
            var data = PlayerPrefs.GetString(__loadoutKey);
            if (string.IsNullOrEmpty(data)) {
                return;
            }

            _cosmeticsSync.TryLoadJson(data);
        }

        /// <summary>
        /// Saves the cosmetics state of the local player on the local device.
        /// </summary>
        private void SaveLoadout() {
            var data = _cosmeticsSync.SaveJson();
            PlayerPrefs.SetString(__loadoutKey, data);
        }

        public void RegisterItem(WardrobeItem item) {
            if (_items.Contains(item)) {
                return;
            }

            _items.Add(item);
        }

        public override void NotifyButtonPressed(KeyboardButtonData data) {
            base.NotifyButtonPressed(data);

            if (data.Type == KeyboardButtonType.ArrowLeft) {
                BrowseItem(_browsingIndex - 1);
            } else if (data.Type == KeyboardButtonType.ArrowRight) {
                BrowseItem(_browsingIndex + 1);
            } else if (data.Type == KeyboardButtonType.Enter) {
                EquipItem();
            }
        }

        /// <summary>
        /// Displays the item at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index will be wrapped within the bounds of <see cref="_items"/>.
        /// An index of 0 corresponds to <see cref="_noneWardrobeItem"/>.
        /// </param>
        private void BrowseItem(int index) {
            // + 1 to account for _noneWardrobeItem
            var totalCount = _items.Count + 1;

            // Wraps value between 0 and _items.Count, handles negative values too
            _browsingIndex = (index + totalCount) % totalCount;

            if (_browsingIndex == 0) {
                HideAllItems();

                _noneWardrobeItem.SetActive(true);
            } else {
                HideAllItems();

                // - 1 to account for _noneWardrobeItem
                var itemIndex = _browsingIndex - 1;
                var item = _items[itemIndex];
                DisplayItem(item);
            }
        }

        private void HideAllItems() {
            _noneWardrobeItem.SetActive(false);

            foreach (var item in _items) {
                item.gameObject.SetActive(false);
            }
        }

        private void DisplayItem(WardrobeItem itemToDisplay) {
            itemToDisplay.gameObject.SetActive(true);
        }

        /// <summary>
        /// Equips the item that the user is currently browsing onto their loadout.
        /// </summary>
        /// <remarks>
        /// Un-equips all other items registered to this wardrobe.
        /// </remarks>
        private void EquipItem() {
            // - 1 to account for _noneWardrobeItem
            var itemIndex = _browsingIndex - 1;

            // Loop over all registered items
            for (var i = 0; i < _items.Count; i++) {
                var item = _items[i];

                // Equip the item that is being browsed, and un-equip the other items
                var shouldBeEquipped = i == itemIndex;

                // Loop over all the cosmetics that the item represents
                foreach (var cosmeticName in item.cosmeticNames) {
                    _cosmeticsSync.SetEnabled(cosmeticName, shouldBeEquipped);
                }
            }

            // Save the loadout to the local device
            SaveLoadout();
        }
    }
}
