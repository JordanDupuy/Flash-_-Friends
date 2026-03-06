using UnityEngine;

public class MemoryCleaner : MonoBehaviour
{
    void Update()
    {
        // On ne le fait pas à chaque frame, mais par exemple toutes les 30 secondes
        if (Time.frameCount % 1800 == 0)
        {
            Clean();
        }
    }

    public void Clean()
    {
        // Libère les assets chargés en mémoire qui ne sont plus référencés
        Resources.UnloadUnusedAssets();

        // Force le Garbage Collector C# à passer immédiatement
        System.GC.Collect();
    }
}