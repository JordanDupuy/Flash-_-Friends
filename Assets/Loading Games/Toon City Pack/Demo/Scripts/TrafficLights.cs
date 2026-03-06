using UnityEngine;

public enum LightColor { Red, Yellow, Green, None }

public class TrafficLights : MonoBehaviour
{

    public LightColor activeLight;
    private MeshRenderer mr;
    private Material[] _mats;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        SetLight(activeLight);
    }

    // On centralise l'initialisation pour éviter les erreurs
    private void Init()
    {
        if (mr == null) mr = GetComponent<MeshRenderer>();
        if (_mats == null && mr != null) _mats = mr.materials;
    }

    public void SetLight(LightColor color)
    {
        activeLight = color;
        Init(); // On s'assure que les matériaux sont chargés

        if (_mats == null || _mats.Length < 4) return;

        int activeIndex = -1;
        switch (color)
        {
            case LightColor.Green: activeIndex = 1; break;
            case LightColor.Yellow: activeIndex = 2; break;
            case LightColor.Red: activeIndex = 3; break;
        }

        for (int i = 1; i <= 3; i++)
        {
            if (_mats[i] == null) continue;

            if (i == activeIndex)
            {
                _mats[i].EnableKeyword("_EMISSION");
                // On utilise la couleur du matériau avec une intensité raisonnable
                Color baseColor = _mats[i].color;
                _mats[i].SetColor("_EmissionColor", baseColor);
            }
            else
            {
                _mats[i].DisableKeyword("_EMISSION");
                _mats[i].SetColor("_EmissionColor", Color.black);
            }
        }
    }

    // Cette fonction sert uniquement pour l'aperçu dans l'éditeur
    private void OnValidate()
    {
        // On utilise sharedMaterials dans l'éditeur pour éviter les fuites de mémoire
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null && renderer.sharedMaterials.Length >= 4)
        {
            // On ne peut pas modifier les matériaux directement en OnValidate sans risque
            // donc on vérifie juste que l'objet est prêt.
        }
    }
}