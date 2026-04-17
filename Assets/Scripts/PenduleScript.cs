using UnityEngine;

public class PenduleScript : MonoBehaviour
{
    public float length = 1f;      // longueur du pendule
    public float gravity = 9.81f;  // gravité
    public float damping = 0.999f; // perte d'énergie (friction)

    private float angle = 45f * Mathf.Deg2Rad; // angle initial (en radians)
    private float angularVelocity = 0f;

    public GraphScript graph;
    public GameObject graphPanel;


public void UpdateLength(float newValue)
    {
        length = newValue;
    }

    public void UpdateGravity(float newValue)
    {
        gravity = newValue;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // équation du pendule
        float angularAcceleration = 0;
        if (length != 0) {
            angularAcceleration = -(gravity / length) * Mathf.Sin(angle);
        }

        // physique
        angularVelocity += angularAcceleration * dt;
        angularVelocity *= damping; // friction
        angle += angularVelocity * dt;

        // appliquer rotation (convertir en degrés)
        transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);

        graph.AddValue(angle);
    }
}
