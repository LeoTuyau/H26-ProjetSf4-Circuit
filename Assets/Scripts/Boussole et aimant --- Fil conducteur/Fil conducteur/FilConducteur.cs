using UnityEngine;
using TMPro;

public class FilConducteur : MonoBehaviour
{
    [Header("Courant")]

    // Indique le sens du courant dans le fil.
    // true signifie que le courant entre dans l'écran.
    // false signifie que le courant sort de l'écran.
    public bool courantEntreDansLEcran = true;

    [Header("Symbole")]

    // Texte affiché au centre du fil pour montrer le sens du courant.
    public TMP_Text symboleTexte;

    // Décalage local du symbole par rapport au centre du fil.
    public Vector3 decalageLocalSymbole = Vector3.zero;

    // Taille générale du symbole dans la scène.
    public float tailleSymbole = 0.7f;

    // Taille de police du symbole.
    public float fontSizeSymbole = 16f;

    void Start()
    {
        // On prépare le symbole au démarrage.
        InitialiserSymbole();

        // On affiche le bon symbole selon le sens initial du courant.
        MettreAJourSymbole();
    }

    void LateUpdate()
    {
        // On recentre le symbole à chaque frame.
        // LateUpdate est utilisé pour que le symbole reste bien placé après les autres mouvements.
        CentrerSymbole();
    }

    public void MettreCourantEntrant()
    {
        // Le courant entre maintenant dans l'écran.
        courantEntreDansLEcran = true;

        // On met à jour le symbole affiché.
        MettreAJourSymbole();
    }

    public void MettreCourantSortant()
    {
        // Le courant sort maintenant de l'écran.
        courantEntreDansLEcran = false;

        // On met à jour le symbole affiché.
        MettreAJourSymbole();
    }

    void InitialiserSymbole()
    {
        // Si aucun texte n'est assigné, on ne fait rien.
        if (symboleTexte == null)
            return;

        // Le texte devient un enfant du fil pour suivre ses déplacements.
        symboleTexte.transform.SetParent(transform, false);

        // On centre le texte horizontalement et verticalement.
        symboleTexte.alignment = TextAlignmentOptions.Center;

        // On applique la taille de police choisie.
        symboleTexte.fontSize = fontSizeSymbole;

        // On applique l'échelle du symbole.
        symboleTexte.transform.localScale = Vector3.one * tailleSymbole;

        // On place le symbole au bon endroit.
        CentrerSymbole();
    }

    void CentrerSymbole()
    {
        // Si le symbole n'existe pas, on arrête.
        if (symboleTexte == null)
            return;

        // On place le symbole au centre du fil avec un décalage optionnel.
        symboleTexte.transform.localPosition = decalageLocalSymbole;

        // On garde le symbole droit, sans rotation locale.
        symboleTexte.transform.localRotation = Quaternion.identity;
    }

    void MettreAJourSymbole()
    {
        // Si aucun texte n'est assigné, on ne fait rien.
        if (symboleTexte == null)
            return;

        // Convention physique :
        // × = courant entrant dans l'écran
        // • = courant sortant de l'écran
        symboleTexte.text = courantEntreDansLEcran ? "×" : "•";
    }

    public Vector2 GetDirectionChamp(Vector2 positionBoussole)
    {
        // On calcule le vecteur radial entre le fil et la boussole.
        // Ce vecteur part du fil vers la boussole.
        Vector2 radial = positionBoussole - (Vector2)transform.position;

        // Si la boussole est presque exactement au centre du fil,
        // la direction du champ est impossible à définir correctement.
        if (radial.sqrMagnitude < 0.0001f)
            return Vector2.zero;

        // Variable qui contiendra la direction du champ magnétique.
        Vector2 directionChamp;

        if (courantEntreDansLEcran)
        {
            // Si le courant entre dans l'écran,
            // le champ magnétique tourne dans le sens horaire.
            directionChamp = new Vector2(radial.y, -radial.x);
        }
        else
        {
            // Si le courant sort de l'écran,
            // le champ magnétique tourne dans le sens antihoraire.
            directionChamp = new Vector2(-radial.y, radial.x);
        }

        // On retourne seulement la direction, donc on normalise le vecteur.
        return directionChamp.normalized;
    }
}