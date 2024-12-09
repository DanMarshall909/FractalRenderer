using UnityEngine;

public class FractalRandomPan : MonoBehaviour
{
    public Material fractalMaterial; // Reference to the fractal material
    public float transitionSpeed = 0.5f; // Speed of panning and zooming
    public float pauseDuration = 0.3f; // Pause duration at each position
    public float minZoom = 0.5f; // Minimum zoom level
    public float maxZoom = 2f; // Maximum zoom level
    public float easeInStrength = 2f; // Strength of the ease-in (higher = sharper)

    private Vector2 _currentCenter; // Current fractal center
    private Vector2 _targetCenter; // Target fractal center
    private float _currentZoom; // Current zoom level
    private float _targetZoom; // Target zoom level
    private float _transitionProgress = 0f; // Progress of the current transition
    private bool _isTransitioning = true; // Whether we are currently transitioning
    private float _pauseTimer = 0f; // Timer for the pause duration

    void Start()
    {
        // Initialize fractal parameters
        if (fractalMaterial == null)
        {
            Debug.LogError("Fractal Material is not assigned in the Inspector!");
            enabled = false;
            return;
        }

        var initialCenter = fractalMaterial.GetVector("_Center");
        _currentCenter = new Vector2(initialCenter.x, initialCenter.y);
        _targetCenter = GenerateRandomCoordinates();
        _currentZoom = fractalMaterial.GetFloat("_Zoom");
        _targetZoom = Random.Range(minZoom, maxZoom);
    }

    void Update()
    {
        if (_isTransitioning)
        {
            // Smoothly interpolate towards the target with ease-in
            _transitionProgress += transitionSpeed * Time.deltaTime;
            float easedProgress = EaseInOut(_transitionProgress, easeInStrength);

            _currentCenter = Vector2.Lerp(_currentCenter, _targetCenter, easedProgress);
            _currentZoom = Mathf.Lerp(_currentZoom, _targetZoom, easedProgress);

            // Update shader parameters
            fractalMaterial.SetVector("_Center", new Vector4(_currentCenter.x, _currentCenter.y, 0.0f, 0.0f));
            fractalMaterial.SetFloat("_Zoom", _currentZoom);

            // If transition is complete, start the pause timer
            if (_transitionProgress >= 1f)
            {
                _isTransitioning = false;
                _pauseTimer = pauseDuration;
            }
        }
        else
        {
            // Countdown the pause timer
            _pauseTimer -= Time.deltaTime;

            // When the pause is over, pick a new target and start transitioning
            if (_pauseTimer <= 0f)
            {
                _targetCenter = GenerateRandomCoordinates();
                _targetZoom = Random.Range(minZoom, maxZoom);
                _transitionProgress = 0f;
                _isTransitioning = true;
            }
        }
    }

    // Generate random fractal coordinates
    private Vector2 GenerateRandomCoordinates()
    {
        return new Vector2(
            Random.Range(-2.0f, 2.0f), // X range
            Random.Range(-1.5f, 1.5f)  // Y range
        );
    }

    // Ease-in-out function with adjustable acceleration strength
    private float EaseInOut(float t, float strength)
    {
        if (t < 0.5f)
        {
            return Mathf.Pow(2f * t, strength) / 2f;
        }
        else
        {
            return 1f - Mathf.Pow(-2f * t + 2f, strength) / 2f;
        }
    }
}
