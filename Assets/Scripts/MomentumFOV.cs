using UnityEngine;

public class MomentumFOV : MonoBehaviour
{
    [Header("FOV Settings")]
    [SerializeField] float baseFOV = 90f;
    [SerializeField] float maxExtraFOV = 10f;
    [SerializeField] float velocityForMaxFOV = 20f;

    [Header("Smoothing")]
    [SerializeField] float fovInSpeed = 8f;
    [SerializeField] float fovOutSpeed = 4f;

    [Header("Divide Impulse")]
    [SerializeField] float divideImpulseFOV = 5f;
    [SerializeField] float impulseDecaySpeed = 10f;

    Camera cam;
    float impulse;
    float currentFOV;

    Rigidbody velocitySource;

    void Awake()
    {
        cam = GetComponent<Camera>();
        currentFOV = baseFOV;
    }

    void Update()
    {
        if (velocitySource == null)
        {
            return;
        }

        float speed = velocitySource.linearVelocity.magnitude;

        float velocity01 = Mathf.Clamp01(speed / velocityForMaxFOV);
        float velocityFOV = velocity01 * maxExtraFOV;

        impulse = Mathf.MoveTowards(impulse, 0f, impulseDecaySpeed * Time.deltaTime);

        float targetFOV = baseFOV + velocityFOV + impulse;

        float lerpSpeed = targetFOV > currentFOV ? fovInSpeed : fovOutSpeed;
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, lerpSpeed * Time.deltaTime);

        cam.fieldOfView = currentFOV;
    }

    public void SetVelocitySource(Rigidbody rb)
    {
        velocitySource = rb;
    }

    public void AddDivideImpulse(float strength01)
    {
        impulse += divideImpulseFOV * Mathf.Clamp01(strength01);
    }
}
