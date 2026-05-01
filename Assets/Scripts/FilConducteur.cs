using UnityEngine;
using TMPro;

public class FilConducteur : MonoBehaviour
{
    [Header("Courant")]
    public bool courantEntreDansLEcran = true;

    [Header("Symbole")]
    public TMP_Text symboleTexte;
    public Vector3 decalageLocalSymbole = Vector3.zero;
    public float tailleSymbole = 0.7f;
    public float fontSizeSymbole = 16f;

    void Start()
    {
        InitialiserSymbole();
        MettreAJourSymbole();
    }

    void LateUpdate()
    {
        CentrerSymbole();
    }

    public void MettreCourantEntrant()
    {
        courantEntreDansLEcran = true;
        MettreAJourSymbole();
    }

    public void MettreCourantSortant()
    {
        courantEntreDansLEcran = false;
        MettreAJourSymbole();
    }

    void InitialiserSymbole()
    {
        if (symboleTexte == null)
            return;

        symboleTexte.transform.SetParent(transform, false);
        symboleTexte.alignment = TextAlignmentOptions.Center;
        symboleTexte.fontSize = fontSizeSymbole;
        symboleTexte.transform.localScale = Vector3.one * tailleSymbole;

        CentrerSymbole();
    }

    void CentrerSymbole()
    {
        if (symboleTexte == null)
            return;

        symboleTexte.transform.localPosition = decalageLocalSymbole;
        symboleTexte.transform.localRotation = Quaternion.identity;
    }

    void MettreAJourSymbole()
    {
        if (symboleTexte == null)
            return;

        // Convention physique :
        // × = courant entrant dans l’écran
        // • = courant sortant de l’écran
        symboleTexte.text = courantEntreDansLEcran ? "×" : "•";
    }

    public Vector2 GetDirectionChamp(Vector2 positionBoussole)
    {
        Vector2 radial = positionBoussole - (Vector2)transform.position;

        if (radial.sqrMagnitude < 0.0001f)
            return Vector2.zero;

        Vector2 directionChamp;

        if (courantEntreDansLEcran)
        {
            // sens horaire
            directionChamp = new Vector2(radial.y, -radial.x);
        }
        else
        {
            // sens antihoraire
            directionChamp = new Vector2(-radial.y, radial.x);
        }

        return directionChamp.normalized;
    }
}