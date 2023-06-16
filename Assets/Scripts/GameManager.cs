using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static object locker = new();
    
    private static bool shuttingDown = false;
    
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (shuttingDown)
            {
                Debug.Log("[Singleton] instance GameManager already choked. Returning null.");
                return null;
            }

            lock (locker)
            {
                if (_instance != null) return _instance;
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    _instance = new GameObject("GameManager").AddComponent<GameManager>();
                }
                
                DontDestroyOnLoad(_instance);
            }

            return _instance;
        }
    }

    private void OnApplicationQuit()
    {
        shuttingDown = true;
    }

    private void OnDestroy()
    {
        shuttingDown = true;
    }

    public static CGameGrid Grid = new(10, 22, 10);
    public static CBlockQueue BlockQueue = new();
    public static bool GameOver { get; private set; }

    private void Awake()
    {
        GameOver = false;
    }
}
