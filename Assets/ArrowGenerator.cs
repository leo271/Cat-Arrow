using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArrowGenerator : MonoBehaviour
{
    public GameObject arrowPrefab;
    public GameObject yarrowPrefab;
    public GameObject fastArrowPrefab;
    public GameObject gameDirector;
    public GameObject featherPrefab;
    public GameObject player;
    public SpriteRenderer background;
    public Sprite normalArrow;
    public Sprite blackArrow;
    
    float span = 1.0f;
    float delta = 0.0f;
    int gameLevel = -1;
    public TextMeshProUGUI stageText;
    public bool isEnding = false;
    public bool isStopping = false;
    public float nowT = 0.0f;
    public float startT = 0.0f;
    public bool isDark = false;
    public bool canMakeFeather = true;
    public float intoDesTimer = 0.0f;
    public bool isDuring = false;

    //GameLevelRange
    public const float ONE     = 10.0f;   public const float spanOne       = 0.8f;
    public const float TWO     = 25.0f;   public const float spanTwo       = 0.6f;
    public const float THREE   = 40.0f;   public const float spanThree     = 0.4f;
    public const float FOUR    = 60.0f;   public const float spanFour      = 0.2f;
    public const float EXTREME = 100.0f;  public const float spanExtreme   = 0.1f;
    public const float HELL    = 160.0f;  public const float spanHell      = 0.08f;
                                          public const float spanDespatare = 0.075f;

    private const float ONE_FRAME = 0.0166667f;

    void Start()
    {
        startT = Time.realtimeSinceStartup;
        nowT = startT;
        background = GameObject.Find("background").GetComponent<SpriteRenderer>();
        arrowPrefab.GetComponent<SpriteRenderer>().sprite = normalArrow;
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnding || isStopping) return;
        if(intoDesTimer > 0)
        {
            isDuring = true;
            intoDesTimer -= ONE_FRAME;
            if(intoDesTimer <= 0)
            {
                isDuring = false;
            }
            return;
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            GameObject y = Instantiate(featherPrefab);
            y.transform.position = new Vector3(Random.Range(-6.0f, 6.0f), 20 + Random.Range(0, 10.0f), 0);
            canMakeFeather = false;
        }
        if (Input.GetKey(KeyCode.T))
        {
            nowT += 0.1f;
        }
        this.delta += ONE_FRAME;
        nowT += ONE_FRAME;
        if (isDark) nowT += ONE_FRAME * 1.5f;
        if(this.delta >= span)
        {
            this.delta -= span;
            if(Random.Range(0, 19) == 0)
            {
                GameObject y = Instantiate(yarrowPrefab);
                y.transform.position = new Vector3(Random.Range(-10.0f, 10.0f), 7, 0);
            } 
            if(Random.Range(0, 299) == 0 && !isDark && canMakeFeather)
            {
                Debug.Log("Made Feather");
                GameObject f = Instantiate(featherPrefab);
                f.transform.position = new Vector3(Random.Range(-6.0f, 6.0f), 20 + Random.Range(0, 10.0f), 0);
                canMakeFeather = false;
            }
            if (Random.Range(0, 30) == 0 && (nowT - startT) >= HELL)
            {
                Debug.Log("Made Fast Arrow");
                GameObject f = Instantiate(fastArrowPrefab);
                f.transform.position = new Vector3(player.transform.position.x, 7, 0);
            }

            GameObject go = Instantiate(arrowPrefab);
            
            
            float px = Random.Range(-10.0f, 10.0f);
            go.transform.position = new Vector3(px, 7, 0);
        }
        if(gameLevel != UpdateGameLevel())
        {
            gameDirector.GetComponent<GameDirector>().ChangeLevel(gameLevel);
            gameDirector.GetComponent<GameDirector>().LevelUpSound();
            if(gameLevel <= 3)
            {
                stageText.text = "Stage" + (gameLevel + 1);
            } else if(gameLevel == 4)
            {
                stageText.text = "Extreme";
            } else if(gameLevel == 5)
            {
                stageText.text = "Hell";
            } else if(gameLevel == 6)
            {
                stageText.text = "DESPARATE";
                intoDesTimer = gameDirector.GetComponent<GameDirector>().intoDesTime;
            }
        }
    }

    private int UpdateGameLevel()
    {
        float t = nowT - startT;
        if(t <= ONE)
        {
            span = spanOne;
            gameLevel = 0;
            gameDirector.GetComponent<GameDirector>().ChangePitch(1.0f);
            background.color = new Color(1.0f, 1.0f, 1.0f);
        }
        else if(t <= TWO)
        {
            span = spanTwo;
            gameLevel = 1;
            background.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else if(t <= THREE)
        {
            span = spanThree;
            gameLevel = 2;
            background.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else if(t <= FOUR)
        {
            span = spanFour;
            gameLevel = 3;
            background.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
        else if(t <= EXTREME)
        {
            span = spanExtreme;
            gameLevel = 4;
            background.color = new Color(1.0f, 0.70f, 0.6f, 1.0f);
            gameDirector.GetComponent<GameDirector>().ChangePitch(1.05f);
        } else if(t<= HELL)
        {
            span = spanHell;
            gameLevel = 5;
            background.color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
            gameDirector.GetComponent<GameDirector>().ChangePitch(1.1f);
        } else
        {
            span = spanDespatare;
            gameLevel = 6;
            background.color = new Color(0.5471698f, 0.03871484f, 0.135052f, 1.0f);
        }
        if (isDark) background.color = new Color(0.2f, 0.2f, 0.2f, 1.0f);
        return gameLevel;

    }

    public void End()
    {
        isEnding = true;
        SetDark(false);
        intoDesTimer = 0.0f;
    }

    public void SetActive(bool isStopping)
    {
        this.isStopping = isStopping;
    }

    public void SetDark(bool isDark)
    {
        this.isDark = isDark;
        if (isDark)
        {
            Debug.Log("Black");
            arrowPrefab.GetComponent<SpriteRenderer>().sprite = blackArrow;
        } else
        {
            arrowPrefab.GetComponent<SpriteRenderer>().sprite = normalArrow;
        }
    }

    public void Reset()
    { 
        startT = 0; nowT = 0;
        isEnding = false;
        canMakeFeather = true;
    }

    public void EnableBlack()
    {
        canMakeFeather = true;
    }
}


