using UnityEngine;

public class SpawnBoussole : MonoBehaviour
{
    [SerializeField] private GameObject prefabBoussole;
    [SerializeField] private Transform pointDeSpawn;

    public void CreerBoussole()
    {
        if (prefabBoussole == null || pointDeSpawn == null)
            return;

        GameObject nouvelleBoussole = Instantiate(prefabBoussole, pointDeSpawn.position, Quaternion.identity);

        if (nouvelleBoussole.GetComponent<Collider2D>() == null)
            nouvelleBoussole.AddComponent<BoxCollider2D>();

        if (nouvelleBoussole.GetComponent<DragObject2D>() == null)
            nouvelleBoussole.AddComponent<DragObject2D>();
    }
}