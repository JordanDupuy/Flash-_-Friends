using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Réglages Marche")]
    public float speed = 5f;

    [Header("Réglages Caméra")]
    public float mouseSensitivity = 0.1f;
    public Transform playerCamera; // Glisse ta caméra ici dans l'inspecteur
    private float _xRotation = 0f;

    [Header("Inputs")]
    public InputActionAsset inputAsset;
    private InputAction _avancer, _reculer, _gauche, _droite;
    private CharacterController _controller;

    void Awake()
    {
        var map = inputAsset.FindActionMap("Control");
        _avancer = map.FindAction("Avancer");
        _reculer = map.FindAction("Reculer");
        _gauche = map.FindAction("Gauche");
        _droite = map.FindAction("Droite");
    }

    void OnEnable() => inputAsset.Enable();
    void OnDisable() => inputAsset.Disable();

    void Start()
    {
        _controller = GetComponent<CharacterController>();

        // BLOQUE LE CURSEUR (Indispensable pour un FPS)
        // Cela cache la souris et l'empêche de sortir de la fenêtre du jeu
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void HandleMovement()
    {
        float fwd = _avancer.ReadValue<float>();
        float bwd = _reculer.ReadValue<float>();
        float lft = _gauche.ReadValue<float>();
        float rgt = _droite.ReadValue<float>();

        float moveX = rgt - lft;
        float moveZ = fwd - bwd;

        // On utilise transform.forward et right du CORPS
        Vector3 move = (transform.forward * moveZ) + (transform.right * moveX);
        _controller.Move(move.normalized * speed * Time.deltaTime);

        if (!_controller.isGrounded)
            _controller.Move(Vector3.down * 9.81f * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        // 1. On récupère le mouvement de la souris
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        // 2. Rotation GAUCHE/DROITE (On fait pivoter tout le corps)
        transform.Rotate(Vector3.up * mouseX);

        // 3. Rotation HAUT/BAS (On fait pivoter seulement la caméra)
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f); // Empêche de se briser le cou
        playerCamera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }
}