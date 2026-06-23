#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class FindMissingScripts : EditorWindow
{
    [MenuItem("Tools/Find Missing Scripts")]
    public static void FindAll()
    {
        int count = 0;

        foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            FindInGO(go, ref count);
        }

        if (count == 0)
            Debug.Log("Nenhum Missing Script encontrado!");
        else
            Debug.Log("Total de Missing Scripts encontrados: " + count);
    }

    static void FindInGO(GameObject go, ref int count)
    {
        Component[] components = go.GetComponents<Component>();
        foreach (Component c in components)
        {
            if (c == null)
            {
                Debug.LogWarning("Missing Script em: " + go.name, go);
                count++;
            }
        }

        foreach (Transform child in go.transform)
        {
            FindInGO(child.gameObject, ref count);
        }
    }
}
#endif