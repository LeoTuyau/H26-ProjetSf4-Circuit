using UnityEngine;

public class Pile : Composante
{
    [SerializeField] private float tension = 9f;

    public override float Tension => tension;
    public override float ValeurOhms => 0f; // source idéale

    public void SetTension(float v) => tension = Mathf.Max(0f, v);

    private void OnValidate() => tension = Mathf.Max(0f, tension);
}