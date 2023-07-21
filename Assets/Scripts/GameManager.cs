using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.UItext;
using System.IO;
//using Newtonsoft.Json;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private static object locker = new();

    private static bool shuttingDown = false;
    public enum Language
    {
        ENGLISH, KOREAN
    }


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
//    public static void GenerateLanguage()
//    { 
//        var content = string.Empty;
//#if UNITY_EDITOR
//        content = File.ReadAllText($"{Application.dataPath}\\3DT\\Assets\\Scripts\\3DT_Language");
//#else
//content = File.ReadAlltext($"{Application.streamingAssetsPath}\\Data\\3DT_Language");
//#endif
//        var texts = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(content);
//        var strings = string.Join(",\r\n", texts.Keys.ToList().Select(s => "\t\t" + s).ToList());
//        //File.WriteAllText($"{Application.dataPath}\\")
//    }

    private void OnApplicationQuit()
    {
        shuttingDown = true;
    }

    private void OnDestroy()
    {
        shuttingDown = true;
    }

    

    public static GameGrid Grid;
}
