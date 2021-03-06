using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
    public class Shortcut
    {
        [MenuItem("Tools/Open scene/Menu scene &1")]
        private static void OpenMenuScene()
        {
            OpenScene("MenuScene");
        }
        private static void OpenScene(string name)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/Scenes/" + name + ".unity");
            }    
        }

        [MenuItem("Tools/Data/Clear player data")]
        private static void ClearPlayerData()
        {
            if (File.Exists(Application.persistentDataPath + "/playerData.dat"))
            {
                File.Delete(Application.persistentDataPath + "/playerData.dat");
                Debug.LogError("Clear player data");
            }
        }
        
        [MenuItem("Tools/Data/Clear player prefs")]
        private static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            Debug.LogError("Clear player prefs");
        }

        [MenuItem("Tools/ConfigScene")]
        private static void OpenConfigScene()
        {
            EditorUtility.OpenPropertyEditor(Resources.Load<SceneData>("_Data/SceneConfig"));
        }
    }
}