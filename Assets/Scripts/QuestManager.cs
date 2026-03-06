using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Quêtes")]
    public List<PhotoQuest> activeQuests = new List<PhotoQuest>();
    public List<PhotoQuest> startingQuests; // Glisse tes ScriptableObjects ici

    [Header("UI de Fin")]
    public GameObject endGamePanel; // Ton panel avec le message de félicitations

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Assure-toi que le jeu tourne normalement au début
        Time.timeScale = 1f;

        // On initialise les premières quêtes
        foreach (var q in startingQuests)
        {
            if (q != null) activeQuests.Add(Instantiate(q));
        }

        // On affiche la première quête au démarrage sur le HUD
        if (activeQuests.Count > 0)
        {
            HUDController.Instance.UpdateQuestDisplay(activeQuests[0].title, activeQuests[0].description);
        }

        // On cache le panel de fin par sécurité au départ
        if (endGamePanel != null) endGamePanel.SetActive(false);
    }

    public void CheckPhotoValidation(List<string> tagsInPhoto)
    {
        // On crée une copie pour éviter les erreurs de modification de liste pendant la boucle
        List<PhotoQuest> questsToRemove = new List<PhotoQuest>();

        foreach (PhotoQuest quest in activeQuests)
        {
            bool tousLesCriteresRemplis = true;

            foreach (string tagReq in quest.requiredTags)
            {
                if (!tagsInPhoto.Contains(tagReq))
                {
                    tousLesCriteresRemplis = false;
                    break;
                }
            }

            if (tousLesCriteresRemplis)
            {
                questsToRemove.Add(quest);
            }
        }

        if (questsToRemove.Count > 0)
        {
            foreach (var q in questsToRemove)
            {
                CompleteQuest(q);
            }
        }
        else
        {
            HUDController.Instance.ShowFeedback("Il manque quelque chose dans le cadre...", Color.white);
        }
    }

    void CompleteQuest(PhotoQuest quest)
    {
        activeQuests.Remove(quest);

        HUDController.Instance.AddVibes(quest.goodVibesPoints);
        HUDController.Instance.ShowFeedback("QUÊTE RÉUSSIE : " + quest.title, Color.green);

        if (quest.nextQuest != null)
        {
            PhotoQuest next = Instantiate(quest.nextQuest);
            activeQuests.Add(next);
            HUDController.Instance.UpdateQuestDisplay(next.title, next.description);
        }
        else if (activeQuests.Count == 0)
        {
            // S'il n'y a plus de quêtes du tout, on termine le jeu
            TerminerLeJeu();
        }
    }

    void TerminerLeJeu()
    {
        Debug.Log("Concours terminé !");

        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);

            // On débloque la souris pour cliquer sur le bouton Quitter
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // On peut mettre le jeu en pause
            Time.timeScale = 0f;
        }

        AudioSource musique = Object.FindFirstObjectByType<AudioSource>();
        if (musique != null)
        {
            musique.Stop();
            // Ou musique.volume = 0.2f; pour la baisser un peu
        }


        HUDController.Instance.UpdateQuestDisplay("Bravo !", "Toutes les quêtes sont finies.");
    }

    // Fonction à relier à ton bouton "Quitter" dans le EndGamePanel
    public void QuitterLeJeu()
    {
        Debug.Log("Fermeture de l'application...");
        Application.Quit();
    }
}