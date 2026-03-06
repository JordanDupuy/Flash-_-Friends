using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class AlbumManager : MonoBehaviour
{
    public static AlbumManager Instance;

    [Header("Configuration")]
    public GameObject photoPrefab; // Une RawImage avec un aspect de photo
    public Transform gridParent;   // L'objet "Content" de ton ScrollView
    public GameObject albumPanel;  // Ton Panel principal d'album

    void Awake() => Instance = this;

    public void ToggleAlbum()
    {
        // On inverse l'état (si c'est ouvert, on ferme, et inversement)
        bool isOpening = !albumPanel.activeSelf;
        albumPanel.SetActive(isOpening);

        if (isOpening)
        {
            AfficherPhotos();
            // Optionnel : Bloquer le mouvement du joueur ici
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Optionnel : Libérer le mouvement du joueur ici
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void AfficherPhotos()
    {
        // 1. On vide l'album actuel pour ne pas doubler les photos
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        // 2. On récupère les fichiers dans le dossier secret
        string path = Application.persistentDataPath;
        string[] files = Directory.GetFiles(path, "*.png");

        // 3. Pour chaque fichier, on crée une image dans la grille
        foreach (string filePath in files)
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);

            GameObject newPhoto = Instantiate(photoPrefab, gridParent);
            newPhoto.GetComponent<RawImage>().texture = tex;
        }
    }

    void Start()
    {
        ResetAlbumOnDisk();
    }

    void ResetAlbumOnDisk()
    {
        string path = Application.persistentDataPath;

        // On vérifie si le dossier existe (par sécurité)
        if (Directory.Exists(path))
        {
            // On récupère la liste de tous les fichiers .png
            string[] files = Directory.GetFiles(path, "*.png");

            foreach (string file in files)
            {
                File.Delete(file); // On supprime chaque fichier un par un
            }

            Debug.Log("Album réinitialisé : " + files.Length + " photos supprimées.");
        }
    }
}