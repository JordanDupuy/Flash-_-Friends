using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string Demo = "Demo";

    [Header("Panels")]
    public GameObject mainPanel;      // Le panel avec Jouer, Touches, Quitter
    public GameObject controlsPanel;  // Le nouveau panel que tu viens de créer

    public void Jouer()
    {
        SceneManager.LoadScene(Demo);
    }

    // Fonction pour afficher les touches
    public void OuvrirTouches()
    {
        mainPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    // Fonction pour revenir au menu principal
    public void FermerTouches()
    {
        mainPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    public void Quitter()
    {
        Debug.Log("Le jeu se ferme...");
        Application.Quit();
    }
}