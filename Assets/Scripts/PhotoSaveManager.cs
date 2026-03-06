using UnityEngine;
using System.IO;

public class PhotoSaveManager : MonoBehaviour
{
    public static PhotoSaveManager Instance;
    public RenderTexture photoPaper; // Ta Render Texture de 1920x1080
    public Camera photoCamera;       // Ta caméra de jeu

    void Awake() => Instance = this;

    public string SavePhoto()
    {
        // 1. On dit à la caméra de dessiner sur la "feuille" au lieu de l'écran    
        photoCamera.targetTexture = photoPaper;

        // 2. On force la caméra à prendre une photo TOUT DE SUITE
        photoCamera.Render();

        // 3. On prépare la transformation en fichier PNG
        Texture2D screenshot = new Texture2D(photoPaper.width, photoPaper.height, TextureFormat.RGB24, false);
        RenderTexture.active = photoPaper;
        screenshot.ReadPixels(new Rect(0, 0, photoPaper.width, photoPaper.height), 0, 0);
        screenshot.Apply();

        // 4. On débranche la caméra de la feuille pour qu'elle filme à nouveau l'écran
        photoCamera.targetTexture = null;
        RenderTexture.active = null;

        // 5. Enregistrement sur le disque
        string date = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string path = Path.Combine(Application.persistentDataPath, "Photo_" + date + ".png");
        byte[] bytes = screenshot.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        Debug.Log("Photo enregistrée : " + path);

        // On détruit la texture temporaire pour ne pas saturer la mémoire
        Destroy(screenshot);

        return path;
    }
}