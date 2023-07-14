using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static readonly object locker = new();

    private static bool shuttingDown;

    private static GameManager instance;

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
                if (instance != null) return instance;
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    instance = new GameObject("GameManager").AddComponent<GameManager>();
                }

                DontDestroyOnLoad(instance);
            }

            return instance;
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

    [SerializeField] private int gridSizeX = 10;
    [SerializeField] private int gridSizeY = 22;
    [SerializeField] private int gridSizeZ = 10;

    public static  GameGrid   Grid;
    private static BlockQueue blockQueue;

    [SerializeField] private float blockSize = 1.0f;
    [SerializeField] private float interval  = 1.0f;

    private Coroutine func;

    private void Awake()
    {
        Grid       = new GameGrid(gridSizeX, gridSizeY, gridSizeZ, blockSize);
        blockQueue = new BlockQueue();

        func = StartCoroutine(BlockDown());
    }

    private IEnumerator BlockDown()
    {
        while (true)
        {
            if (!Grid.IsPlaneEmpty(0))
            {
                GameOver();
                yield break;
            }

            blockQueue.Current.Move(-Vector3.down);

            yield return new WaitForSeconds(interval);
        }
    }

    private void GameOver()
    {
        StopCoroutine(func);
    }
}