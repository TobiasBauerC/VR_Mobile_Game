using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb = null;
    private WaitForSeconds _chaseModeTime = null;
    private WaitForSeconds _dashWaitTime = null;
    [SerializeField] private RailInfo _currentHorRail = null;
    [SerializeField] private RailInfo _currentVerRail = null;
    private int _blueGhostPoints = 200;
    private float _speed = 0.0f;
    private float _tapElapsedTime = 0.0f;
    private float _dashElapsedTime = 0.0f;
    private float _munchElapseTime = 0.0f;
    private bool _moving = true;
    private bool _dashing = false;
    private bool _tapped = false;

    [SerializeField] private float _normalSpeed = 5.0f;
    [SerializeField] private float _dashSpeed = 50.0f;
    [SerializeField] private float _tapTime = 0.1f;
    [SerializeField] private float _dashTime = 0.75f;
    [SerializeField] private Transform _cam = null;

    [Header("Audio")]
    [SerializeField]
    private AudioSource _munchSource;

    public bool chaseMode { private set; get; }
    public bool canDash { private set; get; }

    public WaitForSeconds ChaseModeTime
    {
        get
        {
            return _chaseModeTime;
        }

        set
        {
            _chaseModeTime = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if (!_cam)
            _cam = Camera.main.transform;

        ChaseModeTime = new WaitForSeconds(10.0f);
        _dashWaitTime = new WaitForSeconds(10.0f);
        chaseMode = false;
        canDash = true;
    }

    void OnEnable()
    {
        _moving = true;
    }

    // Update is called once per frame
    void Update()
    {
        GameManager.instance.paused = !_moving;

        PlayerMovement();
    }

    void PlayerMovement()
    {
        if (_tapped)
        {
            _tapElapsedTime += Time.deltaTime;
        }

        if (_moving == false)
            _munchSource.Stop();

        if (_munchSource.isPlaying && _munchElapseTime >= 0.75f)
            _munchSource.Stop();
        else
            _munchElapseTime += Time.deltaTime;

#if UNITY_ANDROID || UNITY_IOS
        for (int i = 0; i < Input.touchCount; ++i)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                _moving = false;
                _tapped = true;
            }
            else if (Input.GetTouch(i).phase == TouchPhase.Ended)
            {
                if (_tapElapsedTime <= _tapTime && _tapped == true)
                {
                    Dash();
                }

                _moving = true;
                _tapped = false;

                _tapElapsedTime = 0.0f;
            }
        }

#endif
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            _moving = false;
            _tapped = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (_tapElapsedTime <= _tapTime && _tapped == true)
            {
                Dash();
            }

            _moving = true;
            _tapped = false;

            _tapElapsedTime = 0.0f;
        }
#endif

        _speed = _normalSpeed;

        if (_dashing)
        {
            Dash();
        }

        if (_moving)
        {
            Vector3 lookDir = _cam.eulerAngles;

            if (_currentHorRail && _currentVerRail)
            {
                if (lookDir.y >= 45 && lookDir.y <= 135)
                {
                    _rb.velocity = Vector3.right * _speed;
                }
                else if (lookDir.y >= 225 && lookDir.y <= 315)
                {
                    _rb.velocity = Vector3.left * _speed;
                }
                else if (lookDir.y >= 135 && lookDir.y <= 225)
                {
                    _rb.velocity = Vector3.back * _speed;
                }
                else
                {
                    _rb.velocity = Vector3.forward * _speed;
                }
            }
            else if (_currentHorRail != null)
            {
                if (transform.position.z != _currentHorRail.zClamp)
                {
                    Vector3 pos = transform.position;

                    if (transform.position.z - _currentHorRail.zClamp > 0.08f)
                    {
                        pos.z -= 0.05f;
                    }
                    else if (transform.position.z - _currentHorRail.zClamp < -0.08f)
                    {
                        pos.z += 0.05f;
                    }
                    else
                    {
                        pos.z = _currentHorRail.zClamp;
                    }

                    transform.position = pos;
                }

                if (lookDir.y >= 0 && lookDir.y <= 180)
                {
                    _rb.velocity = Vector3.right * _speed;
                }
                else
                {
                    _rb.velocity = Vector3.left * _speed;
                }
            }
            else if (_currentVerRail != null)
            {
                if (transform.position.x != _currentVerRail.xClamp)
                {
                    Vector3 pos = transform.position;

                    if (transform.position.x - _currentVerRail.xClamp > 0.08f)
                    {
                        pos.x -= 0.05f;
                    }
                    else if (transform.position.x - _currentVerRail.xClamp < -0.08f)
                    {
                        pos.x += 0.05f;
                    }
                    else
                    {
                        pos.x = _currentVerRail.xClamp;
                    }

                    transform.position = pos;
                }

                if (lookDir.y >= 90 && lookDir.y <= 270)
                {
                    _rb.velocity = Vector3.back * _speed;
                }
                else
                {
                    _rb.velocity = Vector3.forward * _speed;
                }
            }
        }
    }

    private void Dash()
    {
        if (!canDash)
            return;

        if (_dashTime >= _dashElapsedTime)
        {
            GameManager.instance.JustDashed();
            _dashing = true;
            _speed = _dashSpeed;
            _moving = true;
            _dashElapsedTime += Time.deltaTime;
        }
        else
        {
            canDash = false;
            _moving = true;
            _dashing = false;
            _dashElapsedTime = 0.0f;
            StartCoroutine(CanDash());
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "PowerPellet")
        {
            if (!_munchSource.isPlaying)
            {
                _munchSource.Play();
            }
            _munchElapseTime = 0.0f;

            Destroy(c.gameObject);
            GameManager.instance.AddScore(50);
            StartCoroutine(ChaseMode());
        }
        else if (c.gameObject.tag == "Pellet")
        {
            if (!_munchSource.isPlaying)
            {
                _munchSource.Play();
            }
            _munchElapseTime = 0.0f;

            Destroy(c.gameObject);
            GameManager.instance.AddScore(10);
        }

        if (c.gameObject.tag == "HorizontalRail")
        {
            _currentHorRail = c.gameObject.GetComponent<RailInfo>();
        }
        if (c.gameObject.tag == "VerticalRail")
        {
            _currentVerRail = c.gameObject.GetComponent<RailInfo>();
        }

        if (c.gameObject.tag == "Portal")
        {
            Debug.Log("Hit");
            PortalTeleport portal = c.gameObject.GetComponent<PortalTeleport>();
            GetComponent<Collider>().enabled = false;
            StartCoroutine("TimeToTele");
            Vector3 newPos = transform.position;
            newPos.x = portal.teleDestination.x;
            newPos.z = portal.teleDestination.z;
            transform.position = newPos;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag == "HorizontalRail")
        {
            _currentHorRail = null;
        }
        if (c.gameObject.tag == "VerticalRail")
        {
            _currentVerRail = null;
        }
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.tag == "Ghost")
        {
            if (chaseMode && c.gameObject.GetComponent<GhostController>().isBlue)
            {
                c.gameObject.GetComponent<GhostController>().Respawn();
                GameManager.instance.AddScore(_blueGhostPoints);
                _blueGhostPoints *= 2;
            }
            else
            {
                _munchSource.Stop();
                _moving = false;
                GameManager.instance.PlayerHit();
            }
        }
    }

    private IEnumerator ChaseMode()
    {
        chaseMode = true;
        yield return ChaseModeTime;
        chaseMode = false;
        _blueGhostPoints = 200;
    }

    private IEnumerator CanDash()
    {
        GameManager.instance.DashRecover();
        yield return _dashWaitTime;
        canDash = true;
    }

    private IEnumerator TimeToTele()
    {
        yield return new WaitForSeconds(1.0f);
        GetComponent<Collider>().enabled = true;
    }
}
