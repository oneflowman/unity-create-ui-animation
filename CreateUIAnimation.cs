using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

public class CreateUIAnimation : Editor
{
    [MenuItem("Assets/Create/Animation/Animation (Image)", false, 400)]
    private static void CreateUIAnimationClip()
    {
        // Get the selected sprites
        Object[] selectedObjects = Selection.objects;
        if (selectedObjects.Length == 0)
        {
            Debug.LogError("No sprites selected!");
            return;
        }

        // Ensure all selected objects are Sprites
        Sprite[] sprites = new Sprite[selectedObjects.Length];
        for (int i = 0; i < selectedObjects.Length; i++)
        {
            if (selectedObjects[i] is Sprite)
            {
                sprites[i] = (Sprite)selectedObjects[i];
            }
            else
            {
                Debug.LogError("Only select sprites when creating an animation!");
                return;
            }
        }

        // Sort sprites alphabetically to ensure correct order
        System.Array.Sort(sprites, (a, b) => a.name.CompareTo(b.name));

        // Create new animation clip
        AnimationClip animClip = new AnimationClip();
        EditorCurveBinding binding = new EditorCurveBinding
        {
            type = typeof(Image), // Target the UI Image component
            path = "", // Use the root object
            propertyName = "m_Sprite" // The Image.sprite property
        };

        // Create keyframes for sprite changes
        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[sprites.Length];
        float frameTime = 1f / 10f; // Adjust this for animation speed

        for (int i = 0; i < sprites.Length; i++)
        {
            keyframes[i] = new ObjectReferenceKeyframe
            {
                time = i * frameTime,
                value = sprites[i]
            };
        }

        // Apply keyframes to the animation clip
        AnimationUtility.SetObjectReferenceCurve(animClip, binding, keyframes);

        // Set animation settings
        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(animClip);
        settings.loopTime = true; // Make the animation loop
        AnimationUtility.SetAnimationClipSettings(animClip, settings);

        // Save animation clip in the same folder as the sprites
        string path = AssetDatabase.GetAssetPath(sprites[0]);
        string directory = Path.GetDirectoryName(path);
        string animPath = Path.Combine(directory, "NewUIAnimation.anim");

        AssetDatabase.CreateAsset(animClip, animPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Highlight the new animation file and activate rename mode
        EditorApplication.delayCall += () =>
        {
            Object createdAnim = AssetDatabase.LoadAssetAtPath<AnimationClip>(animPath);
            Selection.activeObject = createdAnim;
            EditorGUIUtility.PingObject(createdAnim);

            // Open rename mode
            typeof(EditorWindow).GetMethod("RenameAsset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                ?.Invoke(null, new object[] { animPath });
        };

        Debug.Log("UI Animation created at: " + animPath);
    }

}
