using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody m_rb = null;
    private WaitForSeconds m_chaseModeTime = null;
    private Coroutine m_chaseModeCoroutine = null;
    public RailInfo m_currentHorRail = null;
    public RailInfo m_currentVerRail = null;
    private int m_blueGhostPoints = 200;
    private bool m_moving = false;
    private Vector3 m_upVector = new Vector3(0, 0, 1);
    private Vector3 m_downVector = new Vector3(0, 0, -1);

    [SerializeField] private float m_speed = 5.0f;
    [SerializeField] private Transform m_cam = null;

    public bool chaseMode
    {
        private set;
        get;
    }

    // Use this for initialization
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        if (!m_cam)
            m_cam = Camera.main.transform;

        m_chaseModeTime = new WaitForSeconds(10.0f);
        chaseMode = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.paused)
            return;
        PlayerMovement();
    }

    void PlayerMovement()
    {
#if UNITY_ANDROID || UNITY_IOS
        for (int i = 0; i < Input.touchCount; ++i)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
                m_moving = true;
            else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                m_moving = false;
        }
#endif
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
            m_moving = true;
        else
            m_moving = false;
#endif

        if (m_moving)
        {
            Vector3 lookDir = m_cam.eulerAngles;
            lookDir.y = Mathf.Round(lookDir.y / 90) * 90;

            if (m_currentHorRail && m_currentVerRail)
            {
                if (lookDir.y == 270)
                {
                    m_rb.velocity = Vector3.left * m_speed;
                }
                else if (lookDir.y == 90)
                {
                    m_rb.velocity = Vector3.right * m_speed;
                }
                else if (lookDir.y == 0 || lookDir.y == 360)
                {
                    m_rb.velocity = m_upVector * m_speed;
                }
                else if (lookDir.y == 180)
                {
                    m_rb.velocity = m_downVector * m_speed;
                }
            }
            else if (m_currentHorRail != null)
            {
                if (transform.position.z != m_currentHorRail.zClamp)
                {
                    Vector3 pos = transform.position;
                    pos.z = m_currentHorRail.zClamp;
                    transform.position = pos;
                }

                if (lookDir.y == 270)
                {
                    m_rb.velocity = Vector3.left * m_speed;
                }
                else if (lookDir.y == 90)
                {
                    m_rb.velocity = Vector3.right * m_speed;
                }
            }
            else if (m_currentVerRail != null)
            {
                if (transform.position.x != m_currentVerRail.xClamp)
                {
                    Vector3 pos = transform.position;
                    pos.x = m_currentVerRail.xClamp;
                    transform.position = pos;
                }

                if (lookDir.y == 0 || lookDir.y == 360)
                {
                    m_rb.velocity = m_upVector * m_speed;
                }
                else if (lookDir.y == 180)
                {
                    m_rb.velocity = m_downVector * m_speed;
                }
            }
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "PowerPellet")
        {
            Destroy(c.gameObject);
            GameManager.instance.AddScore(50);
            m_chaseModeCoroutine = StartCoroutine(ChaseMode());
        }
        else if (c.gameObject.tag == "Pellet")
        {
            Destroy(c.gameObject);
            GameManager.instance.AddScore(10);
        }

        if (c.gameObject.tag == "HorizontalRail")
        {
            m_currentHorRail = c.gameObject.GetComponent<RailInfo>();
        }
        if (c.gameObject.tag == "VerticalRail")
        {
            m_currentVerRail = c.gameObject.GetComponent<RailInfo>();
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag == "HorizontalRail")
        {
            m_currentHorRail = null;
        }
        if (c.gameObject.tag == "VerticalRail")
        {
            m_currentVerRail = null;
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
        m_blueGhostPoints = 200;
    }
}
