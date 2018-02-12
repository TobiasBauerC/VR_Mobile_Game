using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb = null;
	private WaitForSeconds _chaseModeTime = null;
	private WaitForSeconds _dashCoolDown = null;
	private Coroutine _chaseModeCoroutine = null;
	private RailInfo _currentHorRail = null;
	private RailInfo _currentVerRail = null;
	private Vector3 _upVector = new Vector3(0, 0, 1);
	private Vector3 _downVector = new Vector3(0, 0, -1);
	private int _blueGhostPoints = 200;
	private float _speed = 0.0f;
	private float _tapElapsedTime = 0.0f;
	private float _dashElapsedTime = 0.0f;
	private bool _moving = false;
	private bool _dashing = false;
	private bool _tapped = false;

	[SerializeField] private float _normalSpeed = 5.0f;
	[SerializeField] private float _dashSpeed = 50.0f;
	[SerializeField] private float _tapTime = 0.1f;
	[SerializeField] private float _dashTime = 0.75f;
	[SerializeField] private Transform _cam = null;

    public bool chaseMode { private set; get; }
	public bool canDash { private set; get; }


    // Use this for initialization
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if (!_cam)
            _cam = Camera.main.transform;

        _chaseModeTime = new WaitForSeconds(10.0f);
		_dashCoolDown = new WaitForSeconds(10.0f);
        chaseMode = false;
		canDash = true;
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
		if(_tapped)
		{
			_tapElapsedTime += Time.deltaTime;
		}

		#if UNITY_ANDROID || UNITY_IOS
        for (int i = 0; i < Input.touchCount; ++i)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
			{
                _moving = true;
				_tapped = true;
			}
            else if (Input.GetTouch(i).phase == TouchPhase.Ended)
			{
                _moving = false;
				_tapped = false;

				if(_tapElapsedTime <= _tapTime)
				{
					Dash();
				}

				_tapElapsedTime = 0.0f;
			}
        }
			
		#endif
		#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
		{
            _moving = true;
			_tapped = true;
		}
		else if(Input.GetMouseButtonUp(0))
		{
            _moving = false;
			_tapped = false;

			if(_tapElapsedTime <= _tapTime)
			{
				Dash();
			}

			_tapElapsedTime = 0.0f;
		}
		#endif

		_speed = _normalSpeed;

		if(_dashing)
		{
			Dash();
		}

        if (_moving)
        {
            Vector3 lookDir = _cam.eulerAngles;
            lookDir.y = Mathf.Round(lookDir.y / 90) * 90;

            if (_currentHorRail && _currentVerRail)
            {
                if (lookDir.y == 270)
                {
                    _rb.velocity = Vector3.left * _speed;
                }
                else if (lookDir.y == 90)
                {
                    _rb.velocity = Vector3.right * _speed;
                }
                else if (lookDir.y == 0 || lookDir.y == 360)
                {
                    _rb.velocity = _upVector * _speed;
                }
                else if (lookDir.y == 180)
                {
                    _rb.velocity = _downVector * _speed;
                }
            }
            else if (_currentHorRail != null)
            {
				if (transform.position.z != _currentHorRail.zClamp)
                {
					Vector3 pos = transform.position;

					if(transform.position.z - _currentHorRail.zClamp > 0.08f)
					{
						pos.z -= 0.05f;
					}
					else if(transform.position.z - _currentHorRail.zClamp < -0.08f)
					{
						pos.z += 0.05f;
					}
					else
					{
	                    pos.z = _currentHorRail.zClamp;
					}

					transform.position = pos;
                }

                if (lookDir.y == 270)
                {
                    _rb.velocity = Vector3.left * _speed;
                }
                else if (lookDir.y == 90)
                {
                    _rb.velocity = Vector3.right * _speed;
                }
            }
            else if (_currentVerRail != null)
            {
                if (transform.position.x != _currentVerRail.xClamp)
                {
                    Vector3 pos = transform.position;

					if(transform.position.x - _currentVerRail.xClamp > 0.08f)
					{
						pos.x -= 0.05f;
					}
					else if(transform.position.x - _currentVerRail.xClamp < -0.08f)
					{
						pos.x += 0.05f;
					}
					else
					{
                    	pos.x = _currentVerRail.xClamp;
					}

					transform.position = pos;
                }

                if (lookDir.y == 0 || lookDir.y == 360)
                {
                    _rb.velocity = _upVector * _speed;
                }
                else if (lookDir.y == 180)
                {
                    _rb.velocity = _downVector * _speed;
                }
            }
        }
    }

	private void Dash()
	{
		if(!canDash)
			return;

		if(_dashTime >= _dashElapsedTime)
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
			_moving = false;
			_dashing = false;
			_dashElapsedTime = 0.0f;
			StartCoroutine(CanDash());
		}
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "PowerPellet")
        {
            Destroy(c.gameObject);
            GameManager.instance.AddScore(50);
            _chaseModeCoroutine = StartCoroutine(ChaseMode());
        }
        else if (c.gameObject.tag == "Pellet")
        {
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
            if (chaseMode)
            {
                c.gameObject.GetComponent<GhostController>().Respawn();
                GameManager.instance.AddScore(_blueGhostPoints);
                _blueGhostPoints *= 2;
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
        yield return _chaseModeTime;
        chaseMode = false;
        _blueGhostPoints = 200;
    }

	private IEnumerator CanDash()
	{
		GameManager.instance.DashRecover();
		yield return _chaseModeTime;
		canDash = true;
	}
}
