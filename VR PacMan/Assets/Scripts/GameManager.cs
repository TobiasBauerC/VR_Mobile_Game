using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const string READY_MSG = "READY!";

    public static GameManager instance = null; // creating single refrenceable instance

    [SerializeField] private Text m_scoreTxt;
    [SerializeField] private Text m_livesTxt;
    [SerializeField] private Text m_readyTxt;
    [SerializeField] private int m_oneUpGoal = 10000;

    private int m_lives = 2;
    private int m_oneUpScore = 0;
    private bool m_lifeRecived = false;
    private Vector3 m_spawnLocation = Vector3.zero;
    private GameObject m_playerObject = null;
    private PlayerController m_playerController = null;
    private WaitForSeconds m_respawnWait = new WaitForSeconds(2.0f);
    private WaitForSeconds m_startWait = new WaitForSeconds(3.0f);
    private Coroutine m_respawnCor = null;
    private Coroutine m_startCor = null;

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
        m_playerObject = GameObject.FindGameObjectWithTag("Player");
        m_playerController = m_playerObject.GetComponent<PlayerController>();
        m_playerController.enabled = false;
        m_spawnLocation = m_playerObject.transform.position;
        paused = false;
        m_scoreTxt.text = string.Format("Score: {0}", score.ToString());
        m_livesTxt.text = string.Format("Lives: {0}", m_lives.ToString());
        m_readyTxt.text = READY_MSG;

        m_startCor = StartCoroutine(StartPlayer());
    }

    // used to add score as long as the given score is more than 0.
    public void AddScore(int _amount)
    {
        if (_amount > 0)
        {
            m_oneUpScore = score += _amount; // set OneUpScore and score at the ame time
            if (!m_lifeRecived)
                OneUpCheck(); // call function that handles one up
            m_scoreTxt.text = string.Format("Score: {0}", score.ToString());
        }
    }

    // this checks to see if the player has enough score for an extra life
    private void OneUpCheck()
    {
        if (m_oneUpScore >= m_oneUpGoal)
        {
            m_lives++;
            m_lifeRecived = true;
            m_livesTxt.text = string.Format("Lives: {0}", m_lives.ToString());
        }
    }

    public void PlayerHit()
    {
        if (m_lives > 0)
            m_lives--;

        m_livesTxt.text = string.Format("Lives: {0}", m_lives.ToString());
        m_respawnCor = StartCoroutine(RespawnPlayer());
    }

    private IEnumerator RespawnPlayer()
    {
        m_playerController.enabled = false;
        yield return m_respawnWait;
        m_readyTxt.text = READY_MSG;
        m_playerObject.transform.position = m_spawnLocation;
        m_startCor = StartCoroutine(StartPlayer());
    }

    private IEnumerator StartPlayer()
    {
        yield return m_startWait;
        m_readyTxt.text = "";
        m_playerController.enabled = true;
    }
}
