using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PhotoSystem : MonoBehaviour
{
    [Header("Paramètres Photo")]
    public float porteePhoto = 50f;
    public LayerMask couchesCibles;
    public Camera photoCamera;

    [Header("UI & Effets")]
    public Image flashImage;      // L'image blanche du flash
    public GameObject viseurUI;   // L'image du réticule/viseur
    public float vitesseFlash = 5f;

    [Header("Inputs")]
    public InputActionAsset inputAsset;
    private InputAction _actionPhoto;
    private InputAction _actionViser;

    [Header("Feedback")]
    public TextMeshProUGUI feedbackText; // Un texte au centre qui dit "Quête complétée !"

    private bool _estEnTrainDeViser = false;
    private InputAction _actionAlbum;

    void Awake()
    {
        var map = inputAsset.FindActionMap("Control");
        _actionPhoto = map.FindAction("PrendrePhoto");
        _actionViser = map.FindAction("Viser");
        _actionAlbum = map.FindAction("OuvrirAlbum");
    }

    void OnEnable()
    {
        _actionPhoto.Enable();
        _actionViser.Enable();
        _actionAlbum.Enable();

        // Clic gauche : Prendre la photo
        _actionPhoto.performed += ctx => TenterCapture();

        // Clic droit : Dégainer / Rengainer
        _actionViser.started += ctx => AlternerVisée(true);
        _actionViser.canceled += ctx => AlternerVisée(false);
        _actionAlbum.performed += ctx => OuvrirLAlbum();
    }

    void OnDisable()
    {
        _actionPhoto.Disable();
        _actionViser.Disable();
        _actionAlbum.Disable();
    }

    void OuvrirLAlbum()
    {
        Debug.Log("Touche TAB détectée !"); // <-- Ajoute cette ligne

        if (AlbumManager.Instance != null)
        {
            AlbumManager.Instance.ToggleAlbum();
        }
        else
        {
            Debug.LogError("L'AlbumManager est manquant dans la scène !");
        }
    }

    void AlternerVisée(bool viser)
    {
        _estEnTrainDeViser = viser;

        if (viseurUI != null)
            viseurUI.SetActive(viser);

        // Optionnel : Tu peux ralentir la vitesse du joueur ici ou zoomer la caméra
        Debug.Log(viser ? "Appareil dégainé" : "Appareil rengainé");
    }

    void TenterCapture()
    {
        // 1. On vérifie si on vise
        if (!_estEnTrainDeViser) return;

        // 2. On vérifie si l'album est ouvert (avec sécurité si l'album est absent)
        if (AlbumManager.Instance != null && AlbumManager.Instance.albumPanel != null)
        {
            if (AlbumManager.Instance.albumPanel.activeSelf) return;
        }

        // 3. On vérifie si le système de sauvegarde existe
        if (PhotoSaveManager.Instance == null)
        {
            Debug.LogError("Le PhotoSaveManager est manquant dans la scène !");
            return;
        }

        // Si tout est OK, on lance la photo
        StartCoroutine(ProcederAuFlash());
        PhotoSaveManager.Instance.SavePhoto();

        // On crée une liste pour stocker TOUS les tags qu'on voit dans la zone
        List<string> tagsDetectes = new List<string>();

        // On définit une zone de détection (une sphère de 10m de rayon devant nous)
        Vector3 centreZone = photoCamera.transform.position + photoCamera.transform.forward * 10f;
        Collider[] objetsDansCadre = Physics.OverlapSphere(centreZone, 10f, couchesCibles);

        foreach (var objet in objetsDansCadre)
        {
            if (objet.tag != "Untagged" && !tagsDetectes.Contains(objet.tag))
            {
                tagsDetectes.Add(objet.tag);
            }
        }

        // On envoie TOUTE la liste au QuestManager
        if (tagsDetectes.Count > 0)
        {
            QuestManager.Instance.CheckPhotoValidation(tagsDetectes);
        }
        else
        {
            HUDController.Instance.ShowFeedback("Cadre vide...", Color.gray);
        }
    }

    public void ShowFeedback(string message, Color couleur)
    {
        feedbackText.text = message;
        feedbackText.color = couleur;
        feedbackText.gameObject.SetActive(true);
        CancelInvoke("HideFeedback");
        Invoke("HideFeedback", 2f); // Disparaît après 2 secondes
    }

    void HideFeedback() => feedbackText.gameObject.SetActive(false);

    IEnumerator ProcederAuFlash()
    {
        Color c = flashImage.color;
        c.a = 1f;
        flashImage.color = c;
        flashImage.gameObject.SetActive(true);

        while (c.a > 0)
        {
            c.a -= Time.deltaTime * vitesseFlash;
            flashImage.color = c;
            yield return null;
        }

        flashImage.gameObject.SetActive(false);
    }
}