using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] GameObject textPrefab;
    [SerializeField] GameObject manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CreateBall()
    {
        GameObject gameObj = Instantiate(prefab, new Vector3(5, 5, 0), Quaternion.Euler(0, 0, 0));
        manager.GetComponent<ForceManager>().AddBall(gameObj);
        GameObject textObj = Instantiate(textPrefab);
        textObj.GetComponent<FollowBall>().SetBall(gameObj);
    }
}
