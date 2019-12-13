using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCtrl : MonoBehaviour
{
    public enum SOUNDS
    {
        BUTTON = 0,
        COIN,
        EXPLOSION,
        LOG,
        MOVE,
        WATER,
        GAMEOVER
    }

    public int dir = 1;
    public float MoveSpeed;
    public float MAP_WIDTH;
    public bool isLog;
    public bool onLog = false;

    private GameObject player;
    private GameObject sounds;
    private Sound _audio;

    // Start is called before the first frame update
    private void Start()
    {
        //player = GameObject.Find("Chicken").gameObject;
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        sounds = GameObject.Find("Sound").gameObject;
        _audio = sounds.GetComponent<Sound>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!GameObject.Find("Canvas").gameObject.transform.Find("PauseUI").gameObject.activeSelf)
        {
            if (onLog)
            {
                //< ActiveSelf같은 경우 오브젝트의 값이 null인 경우 오류 발생
                if (player != null)
                {
                    player.transform.position += Vector3.right * MoveSpeed * dir;
                }
                else
                {
                    onLog = false;
                }
            }

            transform.position += Vector3.right * MoveSpeed * dir;

            if (transform.position.x > 30f)
            {
                transform.position += Vector3.left * MAP_WIDTH;
            }
            if (transform.position.x < -30f)
            {
                transform.position += Vector3.right * MAP_WIDTH;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _audio.SoundPlay((int)SOUNDS.LOG);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!onLog)
            {
                onLog = true;
            }
        }
        else
        {
            onLog = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            onLog = false;
        }
    }
}