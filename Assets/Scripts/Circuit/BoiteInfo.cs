using UnityEngine;
using TMPro;

/// <summary>
/// Tooltip UI qui suit la souris et affiche les informations d'une composante survolée.
/// 
/// Fonctionne comme un singleton accessible via <see cref="Instance"/> depuis
/// n'importe quelle composante du circuit.
/// 
/// Usage :
/// <code>
/// BoiteInfo.Instance.Afficher($"R = {valeur} Ω\nI = {Courant} A");
/// BoiteInfo.Instance.Cacher();
/// </code>
/// 
/// À placer sur un GameObject UI (enfant d'un Canvas) contenant un panel
/// avec un <see cref="TMP_Text"/> enfant.
/// </summary>
public class BoiteInfo : MonoBehaviour
{
    /// <summary>Instance singleton accessible globalement.</summary>
    public static BoiteInfo Instance;

    [SerializeField] private GameObject panel;  // Panneau UI affiché/masqué
    [SerializeField] private TMP_Text texte;    // Texte affiché dans le tooltip

    /// <summary>
    /// Décalage en pixels entre la position de la souris et le coin du tooltip.
    /// Ajuster pour éviter que le tooltip ne recouvre le curseur.
    /// </summary>
    private Vector2 offset = new Vector2(170, -80);

    private RectTransform rectTransform; // RectTransform du tooltip pour le positionnement

    /// <summary>
    /// Initialise le singleton, récupère le RectTransform et masque le panel au démarrage.
    /// </summary>
    void Awake()
    {
        Instance = this;
        rectTransform = GetComponent<RectTransform>();
        panel.SetActive(false);
    }

    /// <summary>
    /// Chaque frame : si le tooltip est visible, le repositionne à la position
    /// de la souris avec le décalage défini.
    /// </summary>
    void Update()
    {
        if (panel.activeSelf)
            rectTransform.position = Input.mousePosition + (Vector3)offset;
    }

    /// <summary>
    /// Affiche le tooltip avec le contenu donné.
    /// Appelé par une composante quand la souris la survole (OnMouseEnter).
    /// </summary>
    /// <param name="contenu">Texte à afficher dans le tooltip (supporte les sauts de ligne \n).</param>
    public void Afficher(string contenu)
    {
        texte.text = contenu;
        panel.SetActive(true);
    }

    /// <summary>
    /// Masque le tooltip.
    /// Appelé par une composante quand la souris la quitte (OnMouseExit).
    /// </summary>
    public void Cacher()
    {
        panel.SetActive(false);
    }
}