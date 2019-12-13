using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    public Vector3 offset = new Vector3(4.5f, 9, -9);
    private GameObject player;

    int dir = 1;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                dir *= -1;

                Vector3 angles = transform.eulerAngles;
                angles.y *= -1;
                transform.eulerAngles = angles;
            }

            Vector3 pp = player.transform.position;
            transform.position = Vector3.Lerp(transform.position, new Vector3(pp.x + (offset.x * dir), transform.position.y, pp.z + offset.z), 0.1f);
        }

        float h = Input.GetAxisRaw("Mouse X");

        transform.Rotate(Vector3.up * h);
    }
}
