using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    public class fiCoreMenuItems {
        // We support backup for *all* components derived from BaseBehavior
        [MenuItem("CONTEXT/CommonBaseBehavior/Reset Metadata")]
        public static void BackupBaseBehavior(MenuCommand command) {
            if (fiGraphMetadataPersistentStorage.Instance.SavedGraphs.ContainsKey(command.context)) {
                fiGraphMetadataPersistentStorage.Instance.SavedGraphs[command.context] = new fiGraphMetadata();
            }
        }
    }
}