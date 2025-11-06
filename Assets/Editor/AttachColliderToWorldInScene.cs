using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class AttachColliderToWorldInScene : EditorWindow
    {
        [MenuItem("Tools/Add BoxColliders to World Children")]
        public static void AddBoxCollidersToWorld()
        {
            var world = GameObject.Find("World");

            if (world == null)
            {
                Debug.LogError("GameObject named 'World' not found in the scene.");
                return;
            }

            var addedCollidersCount = AddCollidersRecursively(world);

            Debug.Log($"Added BoxColliders to {addedCollidersCount} GameObjects under 'World'.");
        }

        private static int AddCollidersRecursively(GameObject obj)
        {
            var count = 0;

            if (obj.GetComponent<BoxCollider>() == null)
            {
                obj.AddComponent<BoxCollider>();
                count++;
            }

            foreach (Transform child in obj.transform)
            {
                count += AddCollidersRecursively(child.gameObject);
            }

            return count;
        }
    }
}