using UnityEngine;

public class DeplacementSouris2D : MonoBehaviour
{
    Camera cam;
    float zOffset;

    void Start()
    {
        cam = Camera.main;
        zOffset = transform.position.z;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mouseScreen = Input.mousePosition;
            mouseScreen.z = Mathf.Abs(cam.transform.position.z - zOffset);

            Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);

            transform.position = new Vector3(mouseWorld.x, mouseWorld.y, zOffset);
        }
    }
}