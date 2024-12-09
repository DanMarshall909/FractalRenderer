using UnityEngine;

public class Randomizer : MonoBehaviour
{
    public Material fractalMaterial; // Reference to the fractal material
    public float interpolationSpeed = 0.1f; // How quickly to interpolate
    public int frameInterval = 10; // Frames between randomizations

    private Vector4 currentC; // Current fractal parameter
    private Vector4 targetC; // Target fractal parameter
    private int frameCount = 0; // Frame counter

    void Start()
    {
        // Initialize the fractal parameter
        currentC = fractalMaterial.GetVector("_c");
        targetC = GenerateRandomC();
    }

    void Update()
    {
        // Increment frame counter
        frameCount++;

        // Every `frameInterval` frames, generate new target parameters
        if (frameCount >= frameInterval)
        {
            targetC = GenerateRandomC();
            frameCount = 0; // Reset counter
        }

        // Interpolate between currentC and targetC
        currentC = Vector4.Lerp(currentC, targetC, interpolationSpeed * Time.deltaTime);

        // Update the fractal shader parameter
        fractalMaterial.SetVector("_c", currentC);
    }

    // Generate random parameters for the fractal
    private Vector4 GenerateRandomC()
    {
        return new Vector4(
            Random.Range(-1.5f, 1.5f), // Constrain X
            Random.Range(-1.5f, 1.5f), // Constrain Y
            Random.Range(-0.5f, 0.5f), // Constrain Z
            Random.Range(-0.5f, 0.5f)  // Constrain W
        );
    }
}
