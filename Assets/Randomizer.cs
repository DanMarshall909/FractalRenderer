using UnityEngine;

public class FractalConstrainedPan : MonoBehaviour
{
    public Material fractalMaterial; // Reference to the fractal material
    public float transitionSpeed = 0.5f; // Speed of panning and zooming
    public float pauseDuration = 1.0f; // Pause duration at each position
    public float minZoom = 0.5f; // Minimum zoom level
    public float maxZoom = 10f; // Maximum zoom level
    public float easeInStrength = 2f; // Strength of the ease-in (higher = sharper)

    private Vector2 currentCenter; // Current fractal center
    private Vector2 targetCenter; // Target fractal center
    private float currentZoom; // Current zoom level
    private float targetZoom; // Target zoom level
    private float transitionProgress = 0f; // Progress of the current transition
    private bool isTransitioning = true; // Whether we are currently transitioning
    private float pauseTimer = 0f; // Timer for the pause duration

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
        currentCenter = new Vector2(initialCenter.x, initialCenter.y);
        targetCenter = SelectInterestingArea();
        currentZoom = fractalMaterial.GetFloat("_Zoom");
        targetZoom = Random.Range(minZoom, maxZoom);
    }

    void Update()
    {
        if (isTransitioning)
        {
            // Smoothly interpolate towards the target with ease-in
            transitionProgress += transitionSpeed * Time.deltaTime;
            float easedProgress = EaseInOut(transitionProgress, easeInStrength);

            currentCenter = Vector2.Lerp(currentCenter, targetCenter, easedProgress);
            currentZoom = Mathf.Lerp(currentZoom, targetZoom, easedProgress);

            // Update shader parameters
            fractalMaterial.SetVector("_Center", new Vector4(currentCenter.x, currentCenter.y, 0.0f, 0.0f));
            fractalMaterial.SetFloat("_Zoom", currentZoom);

            // If transition is complete, start the pause timer
            if (transitionProgress >= 1f)
            {
                isTransitioning = false;
                pauseTimer = pauseDuration;
            }
        }
        else
        {
            // Countdown the pause timer
            pauseTimer -= Time.deltaTime;

            // When the pause is over, pick a new target and start transitioning
            if (pauseTimer <= 0f)
            {
                targetCenter = SelectInterestingArea();
                targetZoom = Random.Range(minZoom, maxZoom);
                transitionProgress = 0f;
                isTransitioning = true;
            }
        }
    }

    // Select an interesting area in the fractal
    private Vector2 SelectInterestingArea()
    {
        Vector2[] interestingAreas = new Vector2[]
        {
            new(-0.745f, 0.113f), // Seahorse Valley
            new(-0.1011f, 0.9563f), // Elephant Valley
            new(-1.25066f, 0.02012f), // Spiral Detail
            new(-1.401f, -0.000f), // Mini Mandelbrot
            new(0.001643721971153f, -0.822467633298876f) // Triple Spiral Arm
        };

        // Choose a random interesting area and add a slight offset for variety
        Vector2 selectedArea = interestingAreas[Random.Range(0, interestingAreas.Length)];
        selectedArea += new Vector2(Random.Range(-0.005f, 0.005f), Random.Range(-0.005f, 0.005f)); // Small random variation
        return selectedArea;
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
