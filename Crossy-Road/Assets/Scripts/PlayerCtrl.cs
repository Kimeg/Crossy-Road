using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public enum SOUNDS
    {
        BUTTON = 0,
        COIN,
        EXPLOSION,
        LOG,
        MOVE,
        WATER,
        GAMEOVER,
        GAME_BGM,
        TRAIN,
        LAVA
    }

    //< 사망시 생성시킬 파티클 효과 정보 가져오기
    public GameObject particle_Drown;
    public GameObject particle_Burn;
    public GameObject particle_RoadKill;

    public float OFFSET = 4f;
    public float JUMP_FORCE = 0.1f;
    public bool dead = false;

    private GameObject sounds;
    private Vector3 prev_pos;
    private Rigidbody rb;
    private Vector3 movement;
    private Sound _audio;
    private GameObject PauseUI;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        sounds = GameObject.Find("Sound").gameObject;
        _audio = sounds.GetComponent<Sound>();
        //audio.SoundPlay((int)SOUNDS.GAME_BGM);
        PauseUI = GameObject.Find("Canvas").gameObject.transform.Find("PauseUI").gameObject;
    }

    private void Update()
    {
        if (PauseUI != null)
        {
            if (!PauseUI.activeSelf && Mathf.Abs(rb.velocity.y) < 0.1 && !dead)
            {
                MoveControl();
            }
        }

        if (dead)
        {
            Kill();
        }

        GameObject.Find("Player_Coordinate").transform.position = transform.position;
    }

    private void MoveControl()
    {
        movement = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            movement = Vector3.forward * OFFSET;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            movement = Vector3.back * OFFSET;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            movement = Vector3.right * OFFSET;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            movement = Vector3.left * OFFSET;
        }

        if (!(movement == Vector3.zero))
        {
            _audio.SoundPlay((int)SOUNDS.MOVE);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 1f);
            transform.position += movement;
            prev_pos = movement;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Land")
        {
            transform.position = new Vector3(Mathf.Round(transform.position.x), 0, Mathf.Round(transform.position.z));
            if (transform.position.x % 2 != 0)
            {
                transform.position += Vector3.right;
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Coin")
        {
            _audio.SoundPlay((int)SOUNDS.COIN);
            Destroy(collider.gameObject);
            GameObject.Find("GM").GetComponent<BlockSetting>().score_value++;
        }

        if (collider.gameObject.tag == "Water")
        {
            _audio.SoundPlay((int)SOUNDS.WATER);
            if (movement.z > 0.1)
            {
                GameObject.Find("GM").GetComponent<BlockSetting>().score_value--;
            }

            //< 현 오브젝트 삭제시키며 파티클효과 생성
            //< 파티클 생성 효과 적용
            dead = true;
            Instantiate(particle_Drown, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
            Invoke("OnDeathUI", 2f);
        }

        if (collider.gameObject.tag == "Lava")
        {
            _audio.SoundPlay((int)SOUNDS.LAVA);
            if (movement.z > 0.1)
            {
                GameObject.Find("GM").GetComponent<BlockSetting>().score_value--;
            }

            //< 현 오브젝트 삭제시키며 파티클효과 생성
            //< 파티클 생성 효과 적용
            dead = true;
            Instantiate(particle_Burn, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
            Invoke("OnDeathUI", 2f);
        }

        if (collider.gameObject.tag == "Enemy")
        {
            _audio.SoundPlay((int)SOUNDS.EXPLOSION);
            if (movement.z > 0.1)
            {
                GameObject.Find("GM").GetComponent<BlockSetting>().score_value--;
            }

            //< 현 오브젝트 삭제시키며 파티클효과 생성
            //< 파티클 생성 효과 적용
            dead = true;
            Instantiate(particle_RoadKill, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
            Invoke("OnDeathUI", 2f);
        }

        if (collider.gameObject.tag == "Train")
        {
            _audio.SoundPlay((int)SOUNDS.TRAIN);
        }

        //< 제한 외벽의 경우 좌, 우 구분후 반대벽 방향으로 현 오브젝트 이동
        if (collider.gameObject.tag == "LeftBoundary" || collider.gameObject.tag == "RightBoundary")
        {
            _audio.SoundPlay((int)SOUNDS.EXPLOSION);

            dead = true;
            Instantiate(particle_RoadKill, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
            Invoke("OnDeathUI", 2f);
        }

        if (collider.gameObject.tag == "Obstacle")
        {
            transform.position -= prev_pos;
        }
    }

    private void Kill()
    {
        transform.position = new Vector3(transform.position.x, -20f, transform.position.z);
        //Destroy(gameObject);
    }

    private void OnDeathUI()
    {
        _audio.StopPlay();
        _audio.SoundPlay((int)SOUNDS.GAMEOVER);
        GameObject.Find("Canvas").transform.Find("DeathUI").gameObject.SetActive(true);
    }
}