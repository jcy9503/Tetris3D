using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static readonly object locker = new();
    
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

    public static GameGrid Grid = new(10, 22, 10);
    public static BlockQueue BlockQueue = new();
    private GameObject[] Blocks;
    [SerializeField] private const float blockSize = 1.0f;
    public static bool GameOver { get; private set; }

    private void Awake()
    {
        GameOver = false;
    }

    private void GridRender()
    {
        int sizeX = Grid.SizeX;
        int sizeY = Grid.SizeY;
        int sizeZ = Grid.SizeZ;
        
        

        for (int i = 0; i < sizeY; ++i)
        {
            Vector3 curPosition = new(-blockSize * sizeX / 2, blockSize * (sizeY / 2f - i), -blockSize * sizeZ / 2);
            for (int j = 0; j < sizeX; ++j)
            {
                for (int k = 0; k < sizeZ; ++k)
                {
                    
                }
            }
        }
    }
}
