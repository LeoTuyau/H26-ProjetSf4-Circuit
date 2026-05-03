using Unity.Properties;
using UnityEngine;

public class FollowBall : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = ball.transform.position+ new Vector3(0.25f,1.7f,0);
    }
    public void SetBall(GameObject ball)
    {
        this.ball = ball;
    }
}
