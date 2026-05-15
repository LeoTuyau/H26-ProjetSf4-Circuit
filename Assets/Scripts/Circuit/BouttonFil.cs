using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gère l'apparence visuelle du bouton de mode fil.
/// Bascule entre deux sprites (normal / sélectionné) pour indiquer
/// si le mode connexion par fil est actif ou non.
/// 
/// À placer sur un GameObject UI Image représentant le bouton fil.
/// Le <see cref="CircuitManager"/> appelle <see cref="SetSelected"/>
/// lors des changements de mode via <see cref="CircuitManager.ToggleFil()"/>.
/// </summary>
public class BouttonFil : MonoBehaviour
{
    [SerializeField] Sprite fil;         // Sprite affiché quand le mode fil est inactif
    [SerializeField] Sprite filSelected; // Sprite affiché quand le mode fil est actif

    private Image image; // Composant Image du bouton sur lequel les sprites sont appliqués

    /// <summary>
    /// Récupère le composant Image au démarrage.
    /// </summary>
    void Start()
    {
        image = GetComponent<Image>();
    }

    /// <summary>
    /// Bascule le sprite entre normal et sélectionné.
    /// Peut être appelé directement par un bouton Unity (OnClick).
    /// </summary>
    public void ToggleColor()
    {
        image.sprite = image.sprite == fil ? filSelected : fil;
    }

    /// <summary>
    /// Force l'état visuel du bouton selon la valeur donnée.
    /// Appelé par le <see cref="CircuitManager"/> pour synchroniser
    /// l'apparence du bouton avec l'état réel du mode fil.
    /// </summary>
    /// <param name="sel">Vrai pour afficher le sprite sélectionné, faux pour le sprite normal.</param>
    public void SetSelected(bool sel)
    {
        image.sprite = sel ? filSelected : fil;
    }
}