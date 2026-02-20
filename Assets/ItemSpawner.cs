using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] GameObject pile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void spawnPile()
    {
        Instantiate(pile, new Vector3(-1.233f, 3.83f, -0.245f),Quaternion.Euler(0,0,0));
    }
}
