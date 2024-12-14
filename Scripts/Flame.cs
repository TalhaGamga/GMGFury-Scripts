using UnityEngine;

public class Flame : MonoBehaviour, ICollectable
{
    Transform collectPoint;
    [SerializeField] FlamePoint flamePoint;
    Vector2 initialPoint;

    bool isCollected = false;
    bool isPlaced = false;
    bool isTaken = false;

    [SerializeField] float lerpSpeed;

    private void Start()
    {
        initialPoint = transform.position;
        GameManager.Instance.OnGameRestart += restartFlame;

    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameRestart -= restartFlame;
        }
    }

    public void Collect(ICollector collector)
    {
        if (!isTaken)
        {
            collectPoint = collector.CollectPoint;
            isCollected = true;
        }
    }

    private void Update()
    {
        if (isCollected && !isTaken)
        {
            transform.position = Vector3.Lerp(transform.position, collectPoint.position, lerpSpeed);
        }

        if (!isPlaced && Vector3.Distance(transform.position, flamePoint.transform.position) < 3)
        {
            isTaken = true;

            transform.position = Vector3.Lerp(transform.position, flamePoint.transform.position, lerpSpeed);

            if (Vector3.Distance(transform.position, flamePoint.transform.position) < 0.01f)
            {
                isPlaced = true;
                GameManager.Instance.OnFlamePlaced?.Invoke();
                Debug.Log("Placed");
            }
        }
    }

    private void restartFlame()
    {
        transform.position = initialPoint;

        bool isCollected = false;
        bool isPlaced = false;
        bool isTaken = false;

        collectPoint = null;

    }
}