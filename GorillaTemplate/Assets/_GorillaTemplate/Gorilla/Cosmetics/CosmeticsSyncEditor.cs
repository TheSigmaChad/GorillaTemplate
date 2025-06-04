#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Normal.GorillaTemplate.Cosmetics {
    [CustomEditor(typeof(CosmeticsSync))]
    public class CosmeticsSyncEditor : UnityEditor.Editor {
        private const string __fileExtension = "json";

        public CosmeticsSync cosmeticsSync => (CosmeticsSync)target;

        public override void OnInspectorGUI() {
            serializedObject.UpdateIfRequiredOrScript();

            if (cosmeticsSync.TryGetIsOwnedRemotelyInHierarchy(out var isOwnedRemotely) == false) {
                isOwnedRemotely = false;
            }

            if (isOwnedRemotely) {
                EditorGUILayout.HelpBox("This component is owned by a remote client, so cosmetics cannot be changed on the local client.", MessageType.Info);
            }

            if (GUILayout.Button("Save To File")) {
                SaveToFile();
            }

            using (new EditorGUI.DisabledScope(isOwnedRemotely)) {
                if (GUILayout.Button("Load From File")) {
                    LoadFromFile();
                }

                var cosmeticsProperty = serializedObject.FindProperty("_cosmetics");
                EditorGUILayout.PropertyField(cosmeticsProperty);

                serializedObject.ApplyModifiedProperties();
            }
        }

        private void SaveToFile() {
            var path = EditorUtility.SaveFilePanel("Save cosmetics configuration to file", "Assets", $"Cosmetics Config.{__fileExtension}", __fileExtension);
            if (string.IsNullOrEmpty(path)) {
                return;
            }

            var json = cosmeticsSync.SaveJson();

            File.WriteAllText(path, json);
        }

        private void LoadFromFile() {
            var path =  EditorUtility.OpenFilePanel("Load cosmetics configuration from file", "Assets", __fileExtension);
            if (string.IsNullOrEmpty(path)) {
                return;
            }

            var json = File.ReadAllText(path);

            cosmeticsSync.TryLoadJson(json);
        }
    }

    [CustomPropertyDrawer(typeof(CosmeticsSync.CosmeticEntry))]
    public class CosmeticEntryDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var cosmeticProperty = property.FindPropertyRelative("cosmetic");
            var cosmetic = cosmeticProperty.objectReferenceValue as Cosmetic;

            var cosmeticName = cosmetic == null ? " " : cosmetic.cosmeticName;

            var cosmeticPropertyFieldPosition = position;
            cosmeticPropertyFieldPosition.width -= 20f;
            EditorGUI.PropertyField(cosmeticPropertyFieldPosition, cosmeticProperty, new GUIContent(cosmeticName));

            using (new EditorGUI.DisabledScope()) {
                var togglePosition = position;
                togglePosition.x = cosmeticPropertyFieldPosition.xMax + 5f;
                togglePosition.width = 15f;

                var previousValue = cosmetic != null && cosmetic.cosmeticEnabled;
                var newValue = GUI.Toggle(togglePosition, previousValue, "");

                if (previousValue != newValue) {
                    var cosmeticsSync = property.serializedObject.targetObject as CosmeticsSync;
                    if (cosmeticsSync != null) {
                        Undo.RegisterFullObjectHierarchyUndo(cosmetic, "Toggle Cosmetic");
                        cosmeticsSync.SetEnabled(cosmetic, newValue);
                    }
                }
            }
        }
    }
}
#endif
