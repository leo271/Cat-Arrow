using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastArrowController : MonoBehaviour
{
    [SerializeField, Range(0.001f, 0.01f)]
    private float gravity = 0.006f;
    [SerializeField, Range(4.0f, 7.0f)]
    private float yRange = 4.05f;

    float r1 = 0.5f; //arrow's radius
    float r2 = 0.9f; //player's radius
    private float speed = 0.0f;

    public GameObject player;
    public GameObject director;

    public float Gravity { get => gravity; set => gravity = value; }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("player");
        director = GameObject.Find("GameDirector");
    }

    // Update is called once per frame
    void Update()
    {
        if (director.GetComponent<GameDirector>().isStopping) return;
        if (director.GetComponent<GameDirector>().isEnding)
        {
            Destroy(gameObject);
            return;
        }
        speed += Gravity;
        transform.Translate(0, -speed, 0);
        if (transform.position.y < -yRange)
        {
            Destroy(gameObject);
        }

        Vector2 p1 = transform.position;
        Vector2 p2 = player.transform.position;
        Vector2 dir = p1 - p2;
        float d = dir.magnitude;

        if (d <= r1 + r2)
        {
            director.GetComponent<GameDirector>().DecreaseGauge();


            Destroy(gameObject);
        }

    }
}

