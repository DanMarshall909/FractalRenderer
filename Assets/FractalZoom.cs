using Fractal;
using UnityEngine;
using UnityEngine.InputSystem;

public class FractalZoom : MonoBehaviour
{
    public Material fractalMaterial; // Reference to the fractal material
    public float zoomSpeed = 5f; // Speed of zooming
    public float panSpeed = 1f; // Speed of panning

    private Vector2 fractalCenter; // Current fractal center
    private float currentZoom; // Current zoom level
    private InputSystem_Actions inputActions; // Input actions instance

    void Awake()
    {
        // Initialize input actions
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Enable();

        // Register callbacks for zoom and look
        inputActions.Player.Zoom.performed += OnZoom;
        inputActions.Player.Look.performed += OnPan;
    }

    void OnDisable()
    {
        inputActions.Disable();

        // Unregister callbacks
        inputActions.Player.Zoom.performed -= OnZoom;
        inputActions.Player.Look.performed -= OnPan;
    }

    void Start()
    {
        // Initialize fractal center and zoom
        fractalCenter = new Vector2(0.0f, 0.0f);
        currentZoom = fractalMaterial.GetFloat("_Zoom");
    }

    void OnZoom(InputAction.CallbackContext context)
    {
        // Read the scroll value as a Vector2
        Vector2 scrollDelta = context.ReadValue<Vector2>();

        // Use only the vertical component for zooming
        float scroll = scrollDelta.y;

        // Adjust zoom level
        currentZoom -= scroll * zoomSpeed * Time.deltaTime;
        currentZoom = Mathf.Clamp(currentZoom, 0.1f, 100f);

        // Update shader
        fractalMaterial.SetFloat("_Zoom", currentZoom);
    }


    void OnPan(InputAction.CallbackContext context)
    {
        // Get pointer delta
        Vector2 delta = context.ReadValue<Vector2>();

        // Adjust fractal center
        fractalCenter += delta * panSpeed * Time.deltaTime;

        // Update shader
        fractalMaterial.SetVector("_Center", new Vector4(fractalCenter.x, fractalCenter.y, 0.0f, 0.0f));
    }
}
