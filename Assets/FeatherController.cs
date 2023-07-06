using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatherController : MonoBehaviour
{
    public float rotateRadius = 4f;
    public float rotateOmega = 0.0f;
    private float omegaRange = Mathf.PI * 0.08f;
    private float resistance = 0.0001f;

    private float r1 = 0.9f;
    private float r2 = 0.3f;

    public Vector3 pos;

    private bool omegaReflectFlag = false;
    private float omega;
    private float resistedSpeed;
    private float fallSpeed = 0.03f;
    private float fallResistance = 0.0005f;

    public GameObject director;
    public GameObject player;
    public GameObject arrowGenerator;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        resistedSpeed = resistance;
        omega = omegaRange;
        player = GameObject.Find("player");
        director = GameObject.Find("GameDirector");
        arrowGenerator = GameObject.Find("ArrowGenerator");
    }

    // Update is called once per frame
    void Update()
    {
        if (director.GetComponent<GameDirector>().isStopping) return;
        if (director.GetComponent<GameDirector>().isEnding || arrowGenerator.GetComponent<ArrowGenerator>().isDuring)
        {
            Destroy(gameObject);
            return;
        }
        {

        }
        if (omegaReflectFlag)
        {
            omega += resistedSpeed;
        }
        else
        {
            omega -= resistedSpeed;

        }
        // acceleration
        if (resistedSpeed > rotateOmega / 10.0f)
        {
            if (omegaReflectFlag)
            {
                if (omega >= 0)
                {
                    resistedSpeed -= resistance;
                    fallSpeed -= fallResistance;
                }
                else
                {
                    resistedSpeed += resistance;
                    fallSpeed += fallResistance;
                }
            } else
            {
                if (omega >= 0)
                {
                    resistedSpeed += resistance;
                    fallSpeed += fallResistance;
                }
                else
                {
                    resistedSpeed -= resistance;
                    fallSpeed -= fallResistance;
                }
            }
        }
        if (omega >= omegaRange)
        {
            omega = omegaRange;
            omegaReflectFlag = false;
        }
        else if (omega <= -omegaRange)
        {
            omega = -omegaRange;
            omegaReflectFlag = true;
        }

        float o = omega + Mathf.PI * 1f;
        setPosRelatively(Mathf.Sin(o) * rotateRadius, Mathf.Cos(o) * rotateRadius);
        transform.rotation = Quaternion.Euler(0, 0, -o * 180 / Mathf.PI + 180);
        moveAbsolutely(0, -fallSpeed);

        if (transform.position.y <= -3.9)
        {
            arrowGenerator.GetComponent<ArrowGenerator>().EnableBlack();
            Destroy(gameObject);
            Debug.Log("Broken");
            
        }

        Vector2 p1 = transform.position;
        Vector2 p2 = player.transform.position;
        Vector2 dir = p1 - p2;
        float d = dir.magnitude;

        if (d <= r1 + r2)
        {
            director.GetComponent<GameDirector>().GetFeather();

            arrowGenerator.GetComponent<ArrowGenerator>().EnableBlack();
            Destroy(gameObject);
        }
    }

    private void setPosRelatively(float x, float y)
    {
        transform.position = pos + new Vector3(x, y, 0);
    }

    private void moveAbsolutely(float x, float y)
    {
        pos += new Vector3(x, y, 0);
    }
}
