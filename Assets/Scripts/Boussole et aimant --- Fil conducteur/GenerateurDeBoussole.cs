using UnityEngine;

public class SpawnBoussole : MonoBehaviour
{
    // Préfab de la boussole qui sera créée dans la scène.
    [SerializeField] private GameObject prefabBoussole;

    // Position où la nouvelle boussole doit apparaître.
    [SerializeField] private Transform pointDeSpawn;

    public void CreerBoussole()
    {
        // Si le préfab ou le point de spawn n'est pas assigné dans l'inspecteur,
        // on arrête la fonction pour éviter une erreur.
        if (prefabBoussole == null || pointDeSpawn == null)
            return;

        // Création d'une nouvelle boussole à la position du point de spawn.
        // Quaternion.identity signifie qu'elle est créée sans rotation particulière.
        GameObject nouvelleBoussole = Instantiate(prefabBoussole, pointDeSpawn.position, Quaternion.identity);

        // Si la boussole n'a pas de Collider2D, on lui ajoute un BoxCollider2D.
        // Cela permet à Unity de détecter les clics de souris sur l'objet.
        if (nouvelleBoussole.GetComponent<Collider2D>() == null)
            nouvelleBoussole.AddComponent<BoxCollider2D>();

        // Si la boussole n'a pas déjà le script DragObject2D,
        // on l'ajoute pour permettre de déplacer la boussole avec la souris.
        if (nouvelleBoussole.GetComponent<DragObject2D>() == null)
            nouvelleBoussole.AddComponent<DragObject2D>();
    }
}