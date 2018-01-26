using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody m_rb = null;
    private WaitForSeconds m_chaseModeTime = null;
    private Coroutine m_chaseModeCoroutine = null;
    private int m_blueGhostPoints = 200;

    [SerializeField] private float m_speed = 5.0f;
    [SerializeField] private Transform m_mainCam = null;

    public bool chaseMode
    {
        private set;
        get;
    }

    // Use this for initialization
    void Start()
    {
        if (!m_mainCam)
            m_mainCam = Camera.main.transform;

        m_rb = GetComponent<Rigidbody>();

        m_chaseModeTime = new WaitForSeconds(10.0f);
        chaseMode = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.paused)
            return;

        m_rb.velocity = m_mainCam.forward * m_speed;
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "PowerPellet")
        {
            Destroy(c.gameObject);
            GameManager.instance.AddScore(50);
            m_chaseModeCoroutine = StartCoroutine(ChaseMode());
        }
        else if (c.gameObject.tag == "Pelletl")
        {
            Destroy(c.gameObject);
            GameManager.instance.AddScore(10);
        }
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag == "Ghost")
        {
            if (chaseMode)
            {
                c.gameObject.GetComponent<GhostController>().Respawn();
                GameManager.instance.AddScore(m_blueGhostPoints);
                m_blueGhostPoints *= 2;
            }
            else
            {
                GameManager.instance.PlayerHit();
            }
        }
    }

    private IEnumerator ChaseMode()
    {
        chaseMode = true;
        yield return m_chaseModeTime;
        chaseMode = false;
        int m_blueGhostPoints = 200;
    }
}
