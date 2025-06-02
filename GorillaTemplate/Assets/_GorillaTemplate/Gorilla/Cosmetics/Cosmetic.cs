using UnityEngine;

namespace Normal.GorillaTemplate.Cosmetics {
    [ExecuteAlways]
    public class Cosmetic : MonoBehaviour {
        public virtual string cosmeticName => gameObject.name;

        public virtual bool cosmeticEnabled {
            get => isActiveAndEnabled;
            set => gameObject.SetActive(value);
        }

        protected virtual void OnEnable() {
            var sync = GetComponentInParent<CosmeticsSync>();

            if (sync == null) {
                return;
            }

            sync.SetEnabled(this, true);
        }

        protected virtual void OnDisable() {
            var sync = GetComponentInParent<CosmeticsSync>();

            if (sync == null) {
                return;
            }

            sync.SetEnabled(this, false);
        }
    }
}
