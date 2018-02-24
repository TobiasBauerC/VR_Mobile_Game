using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const string READY_MSG = "READY!";

    public static GameManager instance = null; // creating single refrenceable instance

	[SerializeField] private Text _scoreTxt;
	[SerializeField] private Text _livesTxt;
	[SerializeField] private Text _readyTxt;
	[SerializeField] private Text _dashTxt;
	[SerializeField] private Text _startText;
	[SerializeField] private int _oneUpGoal = 10000;

	private int _lives = 2;
	private int _oneUpScore = 0;
	private bool _lifeRecived = false;
	private bool _dashRecovering = false;
	private bool _playing = false;
	private Vector3 _spawnLocation = Vector3.zero;
	private GameObject _playerObject = null;
	private PlayerController _playerController = null;
	private WaitForSeconds _respawnWait = new WaitForSeconds(2.0f);
	private WaitForSeconds _startWait = new WaitForSeconds(3.0f);

    public int score
    {
        private set;
        get;
    }

    public bool paused { get; set; }

    // initial setup
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        // all the following will be moved into an "OnSceneLoad" function
        _playerObject = GameObject.FindGameObjectWithTag("Player");
        _playerController = _playerObject.GetComponent<PlayerController>();
        _playerController.enabled = false;
        _spawnLocation = _playerObject.transform.position;
        paused = false;
        _scoreTxt.text = string.Format("Score: {0}", score.ToString());
        _livesTxt.text = string.Format("Lives: {0}", _lives.ToString());
        _readyTxt.text = READY_MSG;
		StartCoroutine(StartPlayer());
    }

	void Update()
	{
		if(_dashRecovering)
		{
			Color newColor = _dashTxt.color;
			newColor.g += 1.0f * 0.1f * Time.deltaTime;
			_dashTxt.color = newColor;
			if(_playerController.canDash == true)
			{
				StartCoroutine(DashRecovered());
			}
		}
	}

    // used to add score as long as the given score is more than 0.
    public void AddScore(int amount)
    {
        if (amount > 0)
        {
            _oneUpScore = score += amount; // set OneUpScore and score at the ame time
            if (!_lifeRecived)
                OneUpCheck(); // call function that handles one up
            _scoreTxt.text = string.Format("Score: {0}", score.ToString());
        }
    }

    // this checks to see if the player has enough score for an extra life
    private void OneUpCheck()
    {
        if (_oneUpScore >= _oneUpGoal)
        {
            _lives++;
            _lifeRecived = true;
            _livesTxt.text = string.Format("Lives: {0}", _lives.ToString());
        }
    }

    public void PlayerHit()
    {
        if (_lives > 0)
            _lives--;

        _livesTxt.text = string.Format("Lives: {0}", _lives.ToString());
        StartCoroutine(RespawnPlayer());
    }

	public void JustDashed()
	{
		Color newColor = _dashTxt.color;
		newColor.g = 0.0f;
		_dashTxt.color = newColor;
	}

	public void DashRecover()
	{
		_dashRecovering = true;
	}
		
    private IEnumerator RespawnPlayer()
    {
        _playerController.enabled = false;
        yield return _respawnWait;
        _readyTxt.text = READY_MSG;
        _playerObject.transform.position = _spawnLocation;
		StartCoroutine(StartPlayer());
    }

    private IEnumerator StartPlayer()
    {
        yield return _startWait;
        _readyTxt.text = "";
        _playerController.enabled = true;
    }
	private IEnumerator DashRecovered()
	{
		Color newColor = _dashTxt.color;
		newColor.g = 1.0f;
		_dashTxt.color = newColor;
		_dashTxt.resizeTextMaxSize = 40;
		_dashRecovering = false;
		yield return new WaitForSeconds(0.1f);
		_dashTxt.resizeTextMaxSize = 35;
	}
}
