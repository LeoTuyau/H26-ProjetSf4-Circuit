using UnityEngine;

public class Resistance : Composante
{
    [SerializeField] private float valeur = 100f; // Ohms

    public override float ValeurOhms => valeur;
    public override float Tension => valeur * Courant; // loi d'Ohm : V = R × I

    public void SetValeur(float v) => valeur = Mathf.Max(0.001f, v);

    private void OnValidate() => valeur = Mathf.Max(0.001f, valeur);
}