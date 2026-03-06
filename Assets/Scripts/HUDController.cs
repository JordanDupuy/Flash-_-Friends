using UnityEngine;
using TMPro; // N'oublie pas d'avoir TextMeshPro d'installé

public class HUDController : MonoBehaviour
{
    public static HUDController Instance;

    [Header("Textes de l'Interface")]
    public TextMeshProUGUI vibesText;         // Le texte du score
    public TextMeshProUGUI questTitleText;    // Le titre de la quête en haut à gauche
    public TextMeshProUGUI questDescText;     // La description de la quête
    public TextMeshProUGUI feedbackText;      // Le texte au centre (ex: "Quête Réussie")

    private int totalVibes = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 1. Mise à jour du score (AddVibes)
    public void AddVibes(int amount)
    {
        totalVibes += amount;
        if (vibesText != null)
            vibesText.text = "Good Vibes: " + totalVibes.ToString();
    }

    // 2. Affichage du message de succès (ShowFeedback)
    public void ShowFeedback(string message, Color couleur)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = couleur;
            feedbackText.gameObject.SetActive(true);

            // On cache le message après 3 secondes
            CancelInvoke("HideFeedback");
            Invoke("HideFeedback", 3f);
        }
    }

    private void HideFeedback()
    {
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    // 3. Mise à jour des textes de quête
    public void UpdateQuestDisplay(string title, string description)
    {
        if (questTitleText != null) questTitleText.text = title;
        if (questDescText != null) questDescText.text = description;
    }
}