using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Représente une résistance dans le circuit électrique.
/// Hérite de Composante et surcharge les propriétés ValeurOhms et Tension.
///
/// Responsable de :
/// - Fournir une résistance ajustable (en ohms) au circuit
/// - Afficher un slider UI pour modifier la valeur en mode slider
/// - Calculer sa tension aux bornes via la loi d'Ohm (V = R × I)
/// - Se détruire proprement en nettoyant le slider instancié
/// </summary>
public class Resistance : Composante
{
    [SerializeField] private float valeur = 100f; // Résistance en ohms
    [SerializeField] private Slider slider;        // Prefab du slider UI
    [SerializeField] private Canvas canvas;        // Canvas parent du slider instancié

    private Slider sliderR;     // Instance du slider créée en jeu
    private bool modeS = false; // Vrai si le slider est visible et actif

    /// <summary>
    /// Instancie le slider UI sur le canvas et initialise sa valeur à la résistance courante.
    /// </summary>
    private void Start()
    {
        sliderR = Instantiate(slider, canvas.transform);
        sliderR.transform.SetParent(canvas.transform, false);
        sliderR.value = valeur;
    }

    /// <summary>
    /// Chaque frame : si le mode slider est actif, positionne le slider
    /// au-dessus de la résistance en coordonnées écran et lit la valeur choisie.
    /// Sinon, masque le slider.
    /// 
    /// Note : la condition de rotation (z == 90 ET z == -90 simultanément) est
    /// toujours fausse — à corriger avec un OU (||) si un décalage différent
    /// selon l'orientation est souhaité.
    /// </summary>
    private void Update()
    {
        if (modeS)
        {
            sliderR.gameObject.SetActive(true);

            // TODO: la condition ci-dessous est toujours fausse (&&  au lieu de ||)
            // Remplacer par : transform.rotation.eulerAngles.z == 90 || transform.rotation.eulerAngles.z == 270
            if (transform.rotation.eulerAngles.z == 90 && transform.rotation.eulerAngles.z == -90)
                sliderR.transform.position = Camera.main.WorldToScreenPoint(
                    transform.position + new Vector3(0, 1f, 0));
            else
                sliderR.transform.position = Camera.main.WorldToScreenPoint(
                    transform.position + new Vector3(0, 0.6f, 0));

            // La valeur de résistance est pilotée directement par le slider
            valeur = sliderR.value;
        }
        else
        {
            sliderR.gameObject.SetActive(false);
        }
    }

    /// <summary>Valeur de la résistance en ohms. Utilisée par le CircuitManager pour la simulation.</summary>
    public override float ValeurOhms => valeur;

    /// <summary>
    /// Tension aux bornes de la résistance, calculée par la loi d'Ohm : V = R × I.
    /// Courant est mis à jour par le CircuitManager après chaque simulation.
    /// </summary>
    public override float Tension => valeur * Courant;

    /// <summary>
    /// Définit la valeur de la résistance par code.
    /// Clampée à 0.001 ohm minimum pour éviter une division par zéro dans la simulation.
    /// </summary>
    /// <param name="v">Nouvelle valeur en ohms.</param>
    public void SetValeur(float v) => valeur = Mathf.Max(0.001f, v);

    /// <summary>
    /// Empêche une résistance nulle ou négative lors de l'édition dans l'inspecteur Unity.
    /// </summary>
    private void OnValidate() => valeur = Mathf.Max(0.001f, valeur);

    /// <summary>
    /// Active ou désactive le mode slider.
    /// Appelé par le CircuitManager via ToggleModeSlider().
    /// </summary>
    /// <param name="modeSlider">Vrai pour afficher le slider, faux pour le masquer.</summary>
    public override void ToggleModeSlider(bool modeSlider) => modeS = modeSlider;

    /// <summary>
    /// Détruit le slider UI instancié quand la résistance est détruite.
    /// Évite de laisser des éléments UI orphelins dans la scène.
    /// </summary>
    private void OnDestroy()
    {
        if (sliderR != null)
            Destroy(sliderR.gameObject);
    }
}