using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NouvelleQuete", menuName = "PhotoGame/Quete")]
public class PhotoQuest : ScriptableObject
{
    [Header("Identité de la Quête")]
    public string title;
    [TextArea] public string description;

    [Header("Paramètres de Validation")]
    // Liste des tags à trouver (ex: "Chien", "PNJ")
    public List<string> requiredTags = new List<string>();

    [Header("Récompenses")]
    public int goodVibesPoints = 50;

    [Header("Progression")]
    public PhotoQuest nextQuest; // La quête qui s'activera après celle-ci
}