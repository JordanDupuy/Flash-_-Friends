using UnityEngine;

public class StreetLight : MonoBehaviour
{

    public Light[] lights;
    public bool isOn;
    public int emissionMaterialIndex = 1; // L'index du matériau de l'ampoule

    private Material _lampMaterial;
    private Color _originalColor;

    private void Awake()
    {
        // On récupère le matériau une seule fois au début
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null && mr.materials.Length > emissionMaterialIndex)
        {
            _lampMaterial = mr.materials[emissionMaterialIndex];
            _originalColor = _lampMaterial.color;
        }
    }

    private void Start()
    {
        SetLight(isOn);
    }

    public void SetLight(bool state)
    {
        this.isOn = state;

        // 1. Gestion des sources de lumière (objets Light)
        foreach (Light l in lights)
        {
            if (l != null) l.enabled = state;
        }

        // 2. Gestion de l'aspect visuel (Émission du matériau)
        if (_lampMaterial != null)
        {
            if (isOn)
            {
                // On active l'émission pour que l'ampoule brille
                _lampMaterial.EnableKeyword("_EMISSION");
                // On prend la couleur de la première lumière pour l'ampoule
                Color lightColor = (lights.Length > 0) ? lights[0].color : Color.white;
                _lampMaterial.SetColor("_EmissionColor", lightColor * 2f); // *2 pour l'intensité
                _lampMaterial.color = lightColor;
            }
            else
            {
                // On éteint l'émission
                _lampMaterial.DisableKeyword("_EMISSION");
                _lampMaterial.SetColor("_EmissionColor", Color.black);
                _lampMaterial.color = _originalColor;
            }
        }
    }

    // Permet de tester en cochant/décochant "Is On" dans l'inspecteur
    private void OnValidate()
    {
        if (_lampMaterial != null) SetLight(isOn);
    }
}