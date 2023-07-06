using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using System;


public class GameDirector : MonoBehaviour
{
    
    public bool isEnding = false;
    public float score = 0.0f;
    public float startTime = 0.0f;
    public float nowTime = 0.0f;
    public bool isStopping = false;
    //private bool startScFlag = false;
    private float endTimFlag = 0.0f;
    private int hp = 20;
    public bool isDark = false;
    private float darkTimer = 0.0f;
    private float bonusPoint = 0.0f;
    const float ONE_FRAME = 0.0166667f;
    bool isDesp = false;
    public float intoDesTime;
    private int resetCount = 0;



    public GameObject AG;
    public GameObject PL;
    public GameObject hpGauge;
    public GameObject yHpGauge;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI endMessage;
    public TextMeshProUGUI scoresText;
    public TextMeshProUGUI darkDescription;
    public AudioClip damage;
    public AudioClip heal;
    public AudioClip gameOver;
    public AudioClip levelUp;
    public AudioClip newRecord;
    public AudioClip rankIN;
    public AudioClip SwitchActiveSound;
    public AudioClip turnDark;
    public AudioClip intoDesparate;
    public AudioClip resetSE;
    public AudioSource audioSource;
    public AudioSource bgm;
    public AudioSource bgmDark;
    public AudioSource bgmDesparate;
    public GameObject RButton, LButton;
    public GameObject RetryButton;
    public GameObject PlayButton, StopButton;
    public AnnounceText announce;



    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        startTime = Time.realtimeSinceStartup;
        nowTime = startTime;
        audioSource = GetComponent<AudioSource>();
        //PlayerPrefs.DeleteAll();
        SaveDataInit();
        RetryButton.SetActive(false);
        PlayButton.SetActive(false); StopButton.SetActive(true);
        announce = new AnnounceText(darkDescription, 5.0f);
        intoDesTime = (intoDesparate.samples / intoDesparate.frequency);
    }

    // Update is called once per frame
    void Update()
    {
        if (isStopping) return;
        if (Input.GetKey(KeyCode.T))
        {
            nowTime += 0.1f;
            hp = 40;
        }
        if (!isEnding)
        {
            nowTime += ONE_FRAME;
            if (isDark)
            {
                darkTimer -= ONE_FRAME;
                bonusPoint += ONE_FRAME * 1.5f;
                if(darkTimer <= 0.0f)
                {
                    EndDark();
                    isDark = false;
                    darkTimer = 0.0f;
                }
            }

            score = nowTime - startTime + bonusPoint;
        } else if((Time.realtimeSinceStartup - endTimFlag) >= 2.0f)
        {
            endMessage.enabled = true;
            RetryButton.SetActive(true);
        }
        if (announce == null)
        {
            Debug.Log("Announce is null");
        }
        else
        {
            announce.Update();
        }
        scoreText.text = "Score:" + score.ToString("F1");
        if(!isDark && score> 155)
        {
            bgm.volume -= 0.005f;
        }
    }

    public void DecreaseGauge()
    {
        if (isEnding) return;
        hp -= 2;
        if (isDark) hp -= 2;
        
        if(hp <= 0)
        {
            EndGame();
        }
        ShowHP();
        audioSource.PlayOneShot(damage);
    }

    public void YArrow()
    {
        if (isEnding) return;
        if(hp < 40)
        { 
            hp++;

            audioSource.PlayOneShot(heal);
        }
        ShowHP();
    }

    private void ShowHP()
    {
        //Debug.Log(hp);
        if(hp > 20)
        {
            hpGauge.GetComponent<Image>().fillAmount = 1.0f;
            yHpGauge.GetComponent<Image>().fillAmount = (hp - 20) / 20.0f;
        } else
        {
            hpGauge.GetComponent<Image>().fillAmount = hp / 20.0f;
            yHpGauge.GetComponent<Image>().fillAmount = 0;
        }
    }


    public void EndGame()
    {
        isEnding = true;
        AG.GetComponent<ArrowGenerator>().End();
        PL.GetComponent<PlayerController>().End();
        //endMessage.text = "You Died.";
        endMessage.enabled = false;
        bgm.Stop();
        bgmDark.Stop();
        bgmDesparate.Stop();
        //audioSource.PlayOneShot(gameOver);
        Add(score);
        scoresText.text = GetFull();
        RButton.SetActive(false); LButton.SetActive(false);
        endTimFlag = Time.realtimeSinceStartup;
        PlayButton.SetActive(false); StopButton.SetActive(false);
        bonusPoint = 0.0f;
        AG.GetComponent<ArrowGenerator>().background.color = Color.white;
        PL.GetComponent<PlayerController>().turnLook(false);
        isDark = false;
        darkDescription.text = "";
        isDesp = false;
        resetCount = 0;
    }

    public void Restart()
    {
        Debug.Log("Restart");
        isEnding = false;
        AG.GetComponent<ArrowGenerator>().Reset();
        PL.GetComponent<PlayerController>().Reset();
        endMessage.text = "";
        scoresText.text = "";
        startTime = 0.0f;
        nowTime = 0.0f;
        hpGauge.GetComponent<Image>().fillAmount = 1.0f;
        bgm.Play();
        hp = 20;
        RButton.SetActive(true); LButton.SetActive(true);
        RetryButton.SetActive(false);
        PlayButton.SetActive(false); StopButton.SetActive(true);
        Debug.Log("Restart");
        bgm.volume = 0.45f;
    }

    public void SwitchActive()
    {
        resetCount++;
        if (resetCount >= 10)
        {
            resetCount = -1;
            PlayerPrefs.DeleteAll();
            audioSource.PlayOneShot(resetSE);
        }
        if (AG.GetComponent<ArrowGenerator>().intoDesTimer != 0) return;
        if (isStopping)
        {
            isStopping = false;
            PL.GetComponent<PlayerController>().SetActive(isStopping);
            AG.GetComponent<ArrowGenerator>().SetActive(isStopping);
            PlayButton.SetActive(false); StopButton.SetActive(true);
            if (isDark) bgmDark.Play();
            else bgm.Play();
        } else
        {
            isStopping = true;
            PL.GetComponent<PlayerController>().SetActive(isStopping);
            AG.GetComponent<ArrowGenerator>().SetActive(isStopping);
            PlayButton.SetActive(true); StopButton.SetActive(false);
            bgm.Stop();
            bgmDark.Stop();
        }
        audioSource.PlayOneShot(SwitchActiveSound);
    }

    public void LevelUpSound()
    {
        //audioSource.PlayOneShot(levelUp);
    }

    public void ChangePitch(float pitch)
    {
        bgm.pitch = pitch;
    }

    public void ChangeLevel(int level)
    {
        if (level < 4) return; 
        announce.SetColor(1, 1, 1);
        switch (level)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                //announce.Show("Stage" + (level + 1));
                break;
            case 4:
                announce.Show("Extreme");
                break;
            case 5:
                announce.Show("Hell");
                break;
            case 6:
                announce.Show("DESPARATE");
                if (isDark) return;
                audioSource.PlayOneShot(intoDesparate);
                bgmDesparate.PlayScheduled(AudioSettings.dspTime + intoDesTime);
                isDesp = true;
                bgm.Stop();
                bgmDark.Stop();
                break;
        }

    }

    public void GetFeather()
    {
        if (isDark) return;
        PL.GetComponent<PlayerController>().turnLook(true);
        bgm.Stop();
        bgmDesparate.Stop();
        bgmDark.Play();
        darkTimer = (nowTime - startTime) <= 50 ? 15.0f : UnityEngine.Random.Range((nowTime - startTime) / 5.0f, (nowTime - startTime) / 3.0f);
        isDark = true;
        AG.GetComponent<ArrowGenerator>().SetDark(true);
        announce.SetColor(0.8f, 0, 0);
        announce.Show("Damage & Score *2\n" + (int)darkTimer + "s");
    }

    public void EndDark()
    {
        PL.GetComponent<PlayerController>().turnLook(false);
        if (isDesp) bgmDesparate.Play();
        else bgm.Play();
        bgmDark.Stop();
        AG.GetComponent<ArrowGenerator>().SetDark(false);
        darkDescription.text = "";
    }


    public const int DataNumber = 5;
    public float[] Sscore = new float[DataNumber];
    public string[] date = new string[DataNumber];

    private static string[] Num_Text =
    {
        "1st", "2nd", "3rd", "4th", "5th", "6th", "7th"
    };

    public void SaveDataInit()
    {
        for (int i = 0; i < DataNumber; i++)
        {
            if (PlayerPrefs.HasKey(i + "score"))
            {
                Sscore[i] = PlayerPrefs.GetFloat(i + "score");
            }
            else
            {
                Sscore[i] = 0.0f;
            }
            if (PlayerPrefs.HasKey(i + "date"))
            {
                date[i] = PlayerPrefs.GetString(i + "date");
            }
            else
            {
                date[i] = "";
            }
        }
    }

    public string GetFull()
    {
        string str = "";
        for (int i = 0; i < DataNumber; i++)
        {
            str += Num_Text[i] + ". " + Get(i) + "\n";
        }
        return str;
    }

    public string Get(int num)
    {
        if (num >= 0 && num < DataNumber)
        {
            return "Score:<size=55>" + Sscore[num].ToString("F1") + "</size> " + date[num];
        }
        return "";
    }

    public void Save()
    {
        for (int i = 0; i < DataNumber; i++)
        {
            PlayerPrefs.SetFloat(i + "score", Sscore[i]);
            PlayerPrefs.SetString(i + "date", date[i]);
        }

    }

    public void Add(float score)
    {
        int rank = -1;
        for (int i = 0; i < DataNumber; i++)
        {
            if (score >= this.Sscore[i])
            {
                rank = i;
                break;
            }
        }
        if (rank == -1)
        {
            audioSource.PlayOneShot(gameOver);
            return;
        }
        for (int i = DataNumber - 2; i >= rank; i--)
        {
            this.Sscore[i + 1] = this.Sscore[i];
            this.date[i + 1] = this.date[i];
        }
        this.Sscore[rank] = score;
        DateTime dt = DateTime.Now;
        this.date[rank] = dt.ToString("d");
        Save();
        if(rank == 0)
        {
            audioSource.PlayOneShot(newRecord);
        } else
        {
            audioSource.PlayOneShot(rankIN);
        }
        
    }
}


public class AnnounceText
{
    TextMeshProUGUI tmp;
    private float fadeTimer;
    private float fadeMax;
    private Color color;
    private const float ONE_FRAME = 0.0166667f;
    private bool isAvailable;
    private float fadeMaxDefault;

    public AnnounceText(TextMeshProUGUI tmp)
    {
        this.tmp = tmp;
        fadeTimer = 0.0f;
        fadeMax = 0.0f;
        color = tmp.color;
        isAvailable = false;
        fadeMaxDefault = 0;
    }

    public AnnounceText(TextMeshProUGUI tmp, float fadeMaxDefault)
    {
        this.tmp = tmp;
        fadeTimer = 0.0f;
        fadeMax = 0.0f;
        color = tmp.color;
        isAvailable = false;
        this.fadeMaxDefault = fadeMaxDefault;
    }

    public void Update()
    {
        if(tmp == null)
        {
            Debug.Log("tmp = null");
            return;
        }
        if (!isAvailable) return;
        tmp.color = new Color(color.r, color.g, color.b, fadeTimer / fadeMax);
        fadeTimer -= ONE_FRAME;
        if(fadeTimer <= 0)
        {
            fadeTimer = 0;
            fadeMax = 0;
            isAvailable = false;
        }
    }

    public void Show(string text, float second)
    {
        tmp.text = text;
        fadeMax = second;
        fadeTimer = second;
        isAvailable = true;
    }

    public void Show(string text)
    {
        Show(text, fadeMaxDefault);
    }

    public void SetColor(float r, float g, float b)
    {
        tmp.color = new Color(r, g, b, 1.0f);
        color = tmp.color;
    }

    public void Disable()
    {
        tmp.text = "";
        isAvailable = false;
        fadeMax = 0;
        fadeTimer = 0;
    }

    public bool IsAvailable() { return isAvailable; }
}
