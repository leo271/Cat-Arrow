using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 0.1f;
    private float speed = 0.0f;
    private bool isEnding = false;
    private bool isStopping = false;
    private bool isBlack = false;

    public Sprite normal;
    public Sprite black;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = normal;
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnding || isStopping) return;
        //player movement
        if (Input.GetKey(KeyCode.LeftArrow) && transform.position.x >= -9.9f)
        {
            transform.Translate(-playerSpeed, 0, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow) && transform.position.x <= 9.9f)
        {
            transform.Translate(playerSpeed, 0, 0);
        }
        if (speed > 0 && transform.position.x <= 9.9f || speed < 0 && transform.position.x >= -9.9f)
        {
            transform.Translate(speed, 0, 0);
        }
    }
    public void LButton()
    {
        speed = -playerSpeed;
        Debug.Log("LButton");
    }
    public void RButton()
    {
        speed = playerSpeed;
        Debug.Log("RButton");
    }
    public void PStop()
    {
        speed = 0.0f;
        Debug.Log("Stop");
    }

    public void End()
    {
        isEnding = true;
    }

    public void SetActive(bool isStopping)
    {
        this.isStopping = isStopping;
    }

    public void Reset()
    {
        isEnding = false;
        transform.Translate(-transform.position.x, 0, 0);
        speed = 0;
        isBlack = false;
    }

    public void turnLook(bool isBlack)
    {
        if (isBlack)
        {
            GetComponent<SpriteRenderer>().sprite = black;
            this.isBlack = false;
        } else
        {
            GetComponent<SpriteRenderer>().sprite = normal;
        }
        this.isBlack = isBlack;
    }

}
