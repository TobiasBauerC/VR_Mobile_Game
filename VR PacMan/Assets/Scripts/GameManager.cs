using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField] private Text _scoreTxt;

    public int score
    {
        private set;
        get;
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        _scoreTxt.text = score.ToString();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddScore(int pAmount)
    {
        if (pAmount > 0)
        {
            score += pAmount;
            _scoreTxt.text = score.ToString();
        }
    }
}
