using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl : MonoBehaviour
{
    public float MoveSpeed;

    private int dir;
    private float MAP_WIDTH = 52f;

    // Start is called before the first frame update
    private void Start()
    {
        dir = -1;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!GameObject.Find("Canvas").gameObject.transform.Find("PauseUI").gameObject.activeSelf)
        {
            transform.Translate(Vector3.right * MoveSpeed * dir);

            if (transform.position.x > 26f)
            {
                transform.position += Vector3.left * MAP_WIDTH;
            }
            if (transform.position.x < -26f)
            {
                transform.position += Vector3.right * MAP_WIDTH;
            }
        }
    }
}