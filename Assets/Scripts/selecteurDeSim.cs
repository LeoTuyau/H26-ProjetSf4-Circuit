using UnityEngine;
using UnityEngine.SceneManagement;

public class selecteurDeSim : MonoBehaviour
{
    // Cette fonction sera appelée par ton bouton
    public void selectSim(string nomDeLaScene)
    {
        SceneManager.LoadScene(nomDeLaScene);
    }
}