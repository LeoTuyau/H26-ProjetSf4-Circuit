using UnityEngine;
using UnityEngine.UI;

public class GraphToggleController : MonoBehaviour
{
    public Toggle graphToggle;
    public GameObject graph; // objet contenant le graphique

    void Start()
    {
        // Assurer que le graphique suit l'état du toggle au départ
        graph.SetActive(graphToggle.isOn);

        // Ajouter un listener au toggle
        graphToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        // Activer ou désactiver le graphique selon le toggle
        graph.SetActive(isOn);
    }
}