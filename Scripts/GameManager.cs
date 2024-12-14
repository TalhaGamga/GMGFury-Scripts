using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Action OnFlamePlaced;
    public int placedFlameCount = 0;
    public Action OnGameRestart;

    [SerializeField] private GameObject winObj;

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }

            return instance;
        }
    }

    private void OnEnable()
    {
        OnFlamePlaced += incrementPlacedFlameCountAndCheckGamePhase;
        OnGameRestart += resetFlames;
    }

    private void OnDisable()
    {
        OnFlamePlaced -= incrementPlacedFlameCountAndCheckGamePhase;
        OnGameRestart -= resetFlames;
    }

    private void OnDestroy()
    {
        GameManager[] managers = FindObjectsOfType<GameManager>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        { 
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void incrementPlacedFlameCountAndCheckGamePhase()
    {
        placedFlameCount++;

        if (placedFlameCount == 2)
        {
            winObj.SetActive(true);
        }
    }

    private void resetFlames()
    {
        placedFlameCount = 0;
        winObj.SetActive(false);
    }
}
