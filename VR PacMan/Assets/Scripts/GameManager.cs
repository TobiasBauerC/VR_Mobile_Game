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
	[SerializeField] private int _oneUpGoal = 10000;
    [SerializeField] private GhostObserver _ghostObserver;

    [Header("Audio")]
    [SerializeField] private AudioSource _playerAudioSource;
    [SerializeField] private AudioClip _readyClip;
    [SerializeField] private AudioClip _deathClip;
    [SerializeField] private AudioClip _eatGhostClip;

	private int _lives = 2;
	private int _oneUpScore = 0;
    private int _pelletsRemaining;
	private bool _lifeRecived = false;
	private bool _dashRecovering = false;
	private bool _playing = false;
	private Vector3 _spawnLocation = Vector3.zero;
	private GameObject _playerObject = null;
	private PlayerController _playerController = null;
	private WaitForSeconds _respawnWait = new WaitForSeconds(2.0f);
	private WaitForSeconds _startWait = new WaitForSeconds(4.0f);

    public int score
    {
        private set;
        get;
    }

    public Transform pacManTransform
    {
        get { return _playerObject.transform; }
    }

    public bool paused { get; set; }

    // initial setup
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        // all the following will be moved into an "OnSceneLoad" function
		Physics.IgnoreLayerCollision(11, 11);
		Physics.IgnoreLayerCollision(10, 11);
		Physics.IgnoreLayerCollision(8, 11);
        if (!_ghostObserver)
            _ghostObserver = GetComponent<GhostObserver>();
        _playerObject = GameObject.FindGameObjectWithTag("Player");
        _playerController = _playerObject.GetComponent<PlayerController>();
        _playerController.enabled = false;
        _spawnLocation = _playerObject.transform.position;
		paused = true;

        GameObject[] pellets = GameObject.FindGameObjectsWithTag("Pellet");
        _pelletsRemaining += pellets.Length;
        GameObject[] powerPellets = GameObject.FindGameObjectsWithTag("PowerPellet");
        _pelletsRemaining += powerPellets.Length;
        _pelletsRemaining++;

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

        if (_pelletsRemaining <= 0)
        {
            StartCoroutine(KillPlayer(3));
        }
	}

    // used to add score as long as the given score is more than 0.
    public void AddScore(int amount)
    {
        if(amount == 200)
            _playerAudioSource.PlayOneShot(_eatGhostClip, 1.0f);
        else
        {
            _pelletsRemaining--;
        }

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
        _playerAudioSource.PlayOneShot(_deathClip, 1.0f);

        if (_lives > 0)
        {
            _lives--;
            paused = true;
            _livesTxt.text = string.Format("Lives: {0}", _lives.ToString());
            StartCoroutine(RespawnPlayer());
        }
        else
        {
            StartCoroutine(KillPlayer(2));
        }
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

        _ghostObserver.ResetGhosts();
        _readyTxt.text = READY_MSG;
        _playerObject.transform.position = _spawnLocation;
		StartCoroutine(StartPlayer());
    }

    private IEnumerator StartPlayer()
    {
        _playerAudioSource.PlayOneShot(_readyClip, 1.0f);
        yield return _startWait;
        _readyTxt.text = "";
        _playerController.enabled = true;
    }

    private IEnumerator KillPlayer(int winLoss)
    {
        _playerController.enabled = false;
        yield return _startWait;
        SceneManager.instance.GoToGameOver(winLoss);
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
