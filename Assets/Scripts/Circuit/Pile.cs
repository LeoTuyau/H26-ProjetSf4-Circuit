using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Représente une pile (source de tension) dans le circuit électrique.
/// Hérite de Composante et surcharge les propriétés Tension et ValeurOhms.
/// 
/// Responsable de :
/// - Fournir une tension constante (ou ajustable via slider) au circuit
/// - Afficher un slider UI pour modifier la tension en mode slider
/// - Se détruire proprement en nettoyant le slider instancié
/// </summary>
public class Pile : Composante
{
    [SerializeField] private float tension = 9f;  // Tension de la pile en volts
    [SerializeField] private Slider slider;        // Prefab du slider UI
    [SerializeField] private Canvas canvas;        // Canvas parent du slider instancié

    private Slider sliderP;          // Instance du slider créée en jeu
    private bool modeSlider = false; // Vrai si le slider est visible et actif

    /// <summary>
    /// Instancie le slider UI sur le canvas et initialise sa valeur à la tension courante.
    /// </summary>
    private void Start()
    {
        sliderP = Instantiate(slider, canvas.transform);
        sliderP.transform.SetParent(canvas.transform, false);
        sliderP.value = tension;
    }

    /// <summary>
    /// Chaque frame : si le mode slider est actif, positionne le slider
    /// au-dessus de la pile en coordonnées écran et lit la valeur choisie.
    /// Sinon, masque le slider.
    /// </summary>
    private void Update()
    {
        if (modeSlider)
        {
            sliderP.gameObject.SetActive(true);

            // Suit la position mondiale de la pile, décalé légèrement vers le haut
            sliderP.transform.position = Camera.main.WorldToScreenPoint(
                transform.position + new Vector3(0, 0.7f, 0));

            // La tension est pilotée directement par la valeur du slider
            tension = sliderP.value;
        }
        else
        {
            sliderP.gameObject.SetActive(false);
        }
    }

    /// <summary>Tension de la pile en volts. Utilisée par le CircuitManager pour la simulation.</summary>
    public override float Tension => tension;

    /// <summary>
    /// Une pile idéale n'a pas de résistance interne.
    /// Retourne toujours 0 — le CircuitManager la traite séparément via un stamp de tension.
    /// </summary>
    public override float ValeurOhms => 0f;

    /// <summary>
    /// Définit la tension de la pile par code.
    /// La valeur est clampée à 0 pour éviter une tension négative.
    /// </summary>
    /// <param name="v">Nouvelle tension en volts.</param>
    public void SetTension(float v) => tension = Mathf.Max(0f, v);

    /// <summary>
    /// Empêche une tension négative lors de l'édition dans l'inspecteur Unity.
    /// </summary>
    private void OnValidate() => tension = Mathf.Max(0f, tension);

    /// <summary>
    /// Active ou désactive le mode slider.
    /// Appelé par le CircuitManager via ToggleModeSlider().
    /// </summary>
    /// <param name="modeSlider">Vrai pour afficher le slider, faux pour le masquer.</param>
    public override void ToggleModeSlider(bool modeSlider) => this.modeSlider = modeSlider;

    /// <summary>
    /// Détruit le slider UI instancié quand la pile est détruite.
    /// Évite de laisser des éléments UI orphelins dans la scène.
    /// </summary>
    private void OnDestroy()
    {
        if (sliderP != null)
            Destroy(sliderP.gameObject);
    }
}