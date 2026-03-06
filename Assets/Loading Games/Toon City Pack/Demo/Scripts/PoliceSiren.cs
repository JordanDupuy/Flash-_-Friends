using UnityEngine;

public class PoliceSiren : MonoBehaviour
{

    public GameObject blueLightObject, redLightObject;
    public bool isSirenOn;
    public float colorInterval = 0.5f;

    private float timer;
    private MeshRenderer mr;
    private Material[] _mats;
    private bool _isBlueActive;

    private void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        // On récupère les instances de matériaux (index 3: bleu, index 4: rouge)
        _mats = mr.materials;
    }

    private void Update()
    {
        if (!isSirenOn)
        {
            StopSiren();
            return;
        }

        timer += Time.deltaTime;

        if (timer > colorInterval)
        {
            _isBlueActive = !_isBlueActive;
            UpdateSirenVisuals();
            timer = 0;
        }
    }

    private void UpdateSirenVisuals()
    {
        // Gestion des GameObjects (Lumières physiques)
        if (blueLightObject) blueLightObject.SetActive(_isBlueActive);
        if (redLightObject) redLightObject.SetActive(!_isBlueActive);

        // Gestion des Matériaux (L'aspect visuel du plastique qui brille)
        // Index 3 = Bleu | Index 4 = Rouge
        SetMaterialEmission(3, _isBlueActive);
        SetMaterialEmission(4, !_isBlueActive);
    }

    private void SetMaterialEmission(int matIndex, bool active)
    {
        if (_mats.Length <= matIndex) return;

        if (active)
        {
            _mats[matIndex].EnableKeyword("_EMISSION");
            // On récupère la couleur du plastique et on la booste pour l'effet néon
            Color emissionColor = _mats[matIndex].color;
            _mats[matIndex].SetColor("_EmissionColor", emissionColor * 4f);
        }
        else
        {
            _mats[matIndex].DisableKeyword("_EMISSION");
            _mats[matIndex].SetColor("_EmissionColor", Color.black);
        }
    }

    private void StopSiren()
    {
        if (blueLightObject) blueLightObject.SetActive(false);
        if (redLightObject) redLightObject.SetActive(false);
        SetMaterialEmission(3, false);
        SetMaterialEmission(4, false);
    }
}