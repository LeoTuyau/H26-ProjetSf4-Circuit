using UnityEngine;

public class Aimant2D : MonoBehaviour
{
    // En 2D, on utilise généralement "up" comme axe principal.
    public Vector2 Moment => transform.up;
}
