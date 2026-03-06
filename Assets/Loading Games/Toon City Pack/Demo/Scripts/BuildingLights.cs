using UnityEngine;

public class BuildingLights : MonoBehaviour
{
    [Header("Configuration")]
    public int windowMaterialIndex = 0;
    [ColorUsage(true, true)] // Permet de régler l'intensité HDR pour le Bloom
    public Color lightColor = Color.yellow;
    public bool areLightsOn;

    private Material _material;
    private Color _defaultColor;

    private void Start()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();

        // On récupère une référence au matériel pour éviter de le chercher à chaque fois
        _material = mr.materials[windowMaterialIndex];
        _defaultColor = _material.color;

        SetLights(areLightsOn);
    }

    public void SetLights(bool isOn)
    {
        areLightsOn = isOn;

        if (isOn)
        {
            // On garde le shader d'origine mais on active l'émission
            _material.EnableKeyword("_EMISSION");
            _material.SetColor("_EmissionColor", lightColor);
            _material.color = lightColor; // Change aussi la couleur de base
        }
        else
        {
            _material.DisableKeyword("_EMISSION");
            _material.SetColor("_EmissionColor", Color.black);
            _material.color = _defaultColor;
        }
    }

    // Permet de tester dans l'éditeur sans lancer le jeu
    private void OnValidate()
    {
        if (_material != null) SetLights(areLightsOn);
    }
}