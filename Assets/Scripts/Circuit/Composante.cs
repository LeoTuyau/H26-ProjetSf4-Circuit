using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Composante : MonoBehaviour
{
    public string Nom { get; private set; }
    [SerializeField] protected bool attach1 = false;
    [SerializeField] protected bool attach2 = false;
    [SerializeField] protected GameObject anchor1;
    [SerializeField] protected GameObject anchor2;
    protected List<(Composante composante, int borne)> connexions = new List<(Composante, int)>();

    protected virtual void Init() { }

    public void Connecter(Composante autre, int maBorne, int autreBorne)
    {
        if (!connexions.Contains((autre, maBorne)))
        {
            connexions.Add((autre, maBorne == 0 ? autreBorne : maBorne));
            autre.connexions.Add((this, autreBorne == 0 ? maBorne : autreBorne));
        }
    }
    public void Deconnecter(Composante autre, int maBorne, int autreBorne)
    {
        connexions.Remove((autre, maBorne));
        autre.connexions.Remove((this, autreBorne));
    }

    public List<(Composante composante, int borne)> GetConnexions() => connexions;

    //Visuel
    public bool getAttach1() { return attach1; }
    public bool getAttach2() { return attach2; }
    public void setAttach1(bool attache) { attach1 = attache; }
    public void setAttach2(bool attache) { attach2 = attache; }
    public void setAnchor1(GameObject anchor)
    {
        anchor1 = anchor;
    }
    public void setAnchor2(GameObject anchor)
    {
        anchor2 = anchor;
    }
    public int getAnchor(GameObject anchor) // Trouver quel anchor on a
    {
        if (anchor == anchor1) return 1;
        if (anchor == anchor2) return 2;
        return 0;
    }
    public void removeAnchor1()
    {
        anchor1 = null;
    }
    public void removeAnchor2()
    {
        anchor2 = null;
    }
    public void RemoveAnchor(GameObject anchor)
    {
        if (anchor1 == anchor)
        {
            anchor1 = null;
        }
        else
        {
            anchor2 = null;
        }
    }

    // ─── Simulation ───────────────────────────────────────────────────

    // Tension aux bornes de cette composante (V)
    public virtual float Tension => 0f;

    // Resistance equivalente (Ohms) — surchargee par Resistance
    public virtual float ValeurOhms => 0f;

    // Courant traversant la composante (A) — injecte par CircuitManager
    public float Courant { get; set; }
    public void SetCourant(float c) => Courant = c;

    // Puissance dissipee (W)
    public float Puissance => Tension * Courant;
}