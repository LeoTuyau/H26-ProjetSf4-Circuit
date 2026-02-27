using UnityEngine;

public class QuitterApplication : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();

        Debug.Log("Quit game button works!"); 
    }
}
