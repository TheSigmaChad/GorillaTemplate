using Normal.GorillaTemplate.Keyboard;
using TMPro;
using UnityEngine;

namespace Normal.GorillaTemplate {
    public class ATM : Keyboard.Keyboard {
        [SerializeField]
        [Tooltip("The SKU to purchase from the Meta Horizon Store.")]
        private string _metaSKU;
        [SerializeField]
        [Tooltip("The currency code of the virtual currency to grant in PlayFab.")]
        private string _virtualCurrencyCode = "BN";
        [SerializeField]
        [Tooltip("The amount of virtual currency to grant.")]
        private int _virtualCurrencyAmount = 1000;

        [Header("UI")]

        [SerializeField]
        private TMP_Text _balanceText;
        [SerializeField]
        private TMP_Text _nameText;
        [SerializeField]
        private TMP_Text _priceText;

        private async void Start() {
            // Load the user's account balance from PlayFab
            PlayFabManager.onCurrencyBalanceChanged += OnBalanceChanged;
            OnBalanceChanged(_virtualCurrencyCode, PlayFabManager.GetCurrencyBalance(_virtualCurrencyCode));

            // Load the pricing details for the specified SKU from the Meta Horizon Store
            _nameText.text = string.Empty;
            _priceText.text = string.Empty;

            if (MetaPlatformManager.isConfigured) {
                var productDetails = await MetaHorizonStoreManager.GetProductDetailsAsync(_metaSKU);

                if (productDetails != null) {
                    _nameText.text = productDetails.Name;
                    _priceText.text = productDetails.FormattedPrice;
                }
            }
        }

        private void OnDestroy() {
            PlayFabManager.onCurrencyBalanceChanged -= OnBalanceChanged;
        }

        private void OnBalanceChanged(string currencyCode, int newBalance) {
            if (_balanceText != null && currencyCode == _virtualCurrencyCode) {
                _balanceText.text = $"Account balance:\n{newBalance} Bananas";
            }
        }

        public override void NotifyButtonPressed(KeyboardButtonData data) {
            base.NotifyButtonPressed(data);

            if (data.Type == KeyboardButtonType.Enter) {
                _ = MetaHorizonStoreManager.PurchaseAsync(_metaSKU, async purchase => {
                    return await PlayFabManager.AddCurrencyAsync(_virtualCurrencyCode, _virtualCurrencyAmount);
                });
            }
        }
    }
}
