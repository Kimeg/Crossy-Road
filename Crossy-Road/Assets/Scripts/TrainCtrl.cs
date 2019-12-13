using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCtrl : MonoBehaviour
{
    public int dir;

    private float MoveSpeed;
    private float MAP_WIDTH = 52f;

    // Start is called before the first frame update
    void Start()
    {
        MoveSpeed = Random.Range(0.1f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameObject.Find("Canvas").gameObject.transform.Find("PauseUI").gameObject.activeSelf)
        {
            transform.position += Vector3.right * MoveSpeed * dir;

            if (transform.position.x > MAP_WIDTH || transform.position.x < -MAP_WIDTH)
            {
                OppositeSide();
            }
        }
    }

    void OppositeSide() 
    {
        transform.position += Vector3.right * 2 * MAP_WIDTH * -dir;
    }
}
