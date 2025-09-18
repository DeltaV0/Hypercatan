using UnityEngine;

public class CamMover : MonoBehaviour
{
    public Transform trans;

    public Transform trans2;

    public Camera cam;

    public bool Targetable;
    private void FixedUpdate()
    {
        if(!Targetable){
        trans2.position = new Vector3(cam.ScreenToWorldPoint (Input.mousePosition).x, cam.ScreenToWorldPoint (Input.mousePosition).y, 0);
        }
        if (!Targetable)
        {
            transform.position = Vector3.Lerp(transform.position, Vector3.Lerp(new Vector3(trans.position.x, trans.position.y, transform.position.z), new Vector3(trans2.position.x, trans2.position.y, transform.position.z), 0.25f), 0.1f);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, Vector3.Lerp(new Vector3(trans.position.x, trans.position.y, transform.position.z), new Vector3(trans2.position.x, trans2.position.y, transform.position.z), 0.25f), 0.8f);


        }

    }
}
