using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMGR : MonoBehaviour
{
    private AudioSource BGM;
    private GameObject player;
    public GameObject animals;
    public GameObject game_sounds;
    private Sound _audio;
    public Texture2D cursor;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    private bool paused = false;

    // Start is called before the first frame update
    private void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            player = players[0];
        }

        BGM = GetComponent<AudioSource>();
        BGM.Play();
        if (game_sounds != null)
        {
            _audio = game_sounds.GetComponent<Sound>();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if ((player != null) && player.GetComponent<PlayerCtrl>().dead)
        {
            BGM.Stop();
        }

        Cursor.SetCursor(cursor, hotSpot, cursorMode);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                UnPauseBTN();
                paused = false;
            }
            else
            {
                PauseBTN();
                paused = true;
            }
        }
    }

    public void StartBTN()
    {
        _audio.SoundPlay(0);
        BGM.Stop();
        //DontDestroyOnLoad(game_sounds);
        SceneManager.LoadScene("GameScene");
    }

    public void OptionBTN()
    {
        _audio.SoundPlay(0);
        UIwrapper("HomeUI", "OptionUI");
        //GameObject.Find("BackGround").gameObject.SetActive(false);
    }

    public void UnOptionBTN()
    {
        _audio.SoundPlay(0);
        UIwrapper("OptionUI", "HomeUI");
        //GameObject.Find("BackGround").gameObject.SetActive(true);
    }

    public void CreditBTN()
    {
        _audio.SoundPlay(0);
        UIwrapper("HomeUI", "CreditUI");
        //GameObject.Find("BackGround").gameObject.SetActive(false);
    }

    public void UnCreditBTN()
    {
        _audio.SoundPlay(0);
        UIwrapper("CreditUI", "HomeUI");
        //GameObject.Find("BackGround").gameObject.SetActive(true);
    }

    public void RuleBTN()
    {
        _audio.SoundPlay(0);
        if (animals != null)
        {
            animals.SetActive(true);
        }
        UIwrapper("HomeUI", "RuleUI");
        //GameObject.Find("BackGround").gameObject.SetActive(false);
    }

    public void UnRuleBTN()
    {
        _audio.SoundPlay(0);
        if (animals != null)
        {
            animals.SetActive(false);
        }
        UIwrapper("RuleUI", "HomeUI");
        //GameObject.Find("BackGround").gameObject.SetActive(true);
    }

    public void PauseBTN()
    {
        _audio.SoundPlay(0);
        TimeZero();
        GameObject.Find("Canvas").transform.Find("PauseUI").gameObject.SetActive(true);
        
    }

    public void UnPauseBTN()
    {
        _audio.SoundPlay(0);
        TimeResume();
        GameObject.Find("Canvas").transform.Find("PauseUI").gameObject.SetActive(false);
    }

    public void RestartBTN()
    {
        _audio.SoundPlay(0);
        TimeResume();
        SceneManager.LoadScene("GameScene");
    }

    public void HomeBTN()
    {
        _audio.SoundPlay(0);
        TimeResume();
        BGM.Stop();
        SceneManager.LoadScene("HomeScene");
    }

    public void ResetBTN()
    {
        _audio.SoundPlay(0);
        GameObject.Find("GM").GetComponent<BlockSetting>().SaveFile(0);
        GameObject.Find("GM").GetComponent<BlockSetting>().max_score_value = 0;
    }

    public void ExitBTN()
    {
        _audio.SoundPlay(0);
        Application.Quit();
    }

    private void UIwrapper(string ui_1, string ui_2)
    {
        GameObject.Find("Canvas").transform.Find(ui_1).gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find(ui_2).gameObject.SetActive(true);
    }

    private void TimeZero()
    {
        Time.timeScale = 0;
    }

    private void TimeResume()
    {
        Time.timeScale = 1;
    }
}