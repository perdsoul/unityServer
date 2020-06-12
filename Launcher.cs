using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Net;
using Resources;
using Resources.Scripts.DBscan;
using Resources.Scripts.History;
using Siccity.GLTFUtility;

public enum eGiMode
{
    Enlighten , 
    RSM , 
    LPV , 
    Visible,
}

public enum eConsoleResolution
{
    W800H600 , 
}

public class Launcher : MonoBehaviour 
{
    static Launcher mInstance = null;

    public static Launcher instance
    {
        get
        {
            return mInstance;
        }
    }

    public string SceneName = "Main";

    public Stats StatComp = null;

    public eGiMode GIMode = eGiMode.Enlighten;

    public eConsoleResolution ConsoleResolution = eConsoleResolution.W800H600;

    public string serverAddress = "127.0.0.1";

    public int serverPort = 8081;

    int mConsoleWidth = 800;
    int mConsoleHeight = 600;

    int IdCounter = 5;
    int IdStep = 3;
    int ChildConponentCount = 0;

    ConnectionMgr mConnectionMgr = null;

    RadianceManager mRadianceMgr = null;

    private History mHistory = null;

    Camera mSceneCamera = null;

    List<Light> mDynLights = new List<Light>();

    public UnityEngine.UI.Dropdown SceneDropdown = null;

    public List<Light> dynLights
    {
        get
        {
            return mDynLights;
        }
    }

    public Camera sceneCamera
    {
        get
        {
            return mSceneCamera;
        }
    }

    public ConnectionMgr connectionMgr
    {
        get
        {
            return mConnectionMgr;
        }
    }

    public RadianceManager radianceMgr
    {
        get
        {
            return mRadianceMgr;
        }
    }

    //1751578
    public History history
    {
        get
        {
            return mHistory;
        }
    }
    //1751578

    public int consoleWidth
    {
        get
        {
            return mConsoleWidth;
        }
    }

    public int consoleHeight
    {
        get
        {
            return mConsoleHeight;
        }
    }

    public Stats stats
    {
        get
        {
            return StatComp;
        }
    }

    public string GetSceneName {
        get {
            return SceneName;
        }
    }

    void Awake()
    {
        mInstance = this;

        Initialize();

        DontDestroyOnLoad(this.gameObject);
    }

    void Initialize()
    {
        switch (ConsoleResolution)
        {
            case eConsoleResolution.W800H600:
                mConsoleWidth = 800;
                mConsoleHeight = 600;
                break;

            default:
                break;
        }
    }

	void Start () 
    {
        Screen.fullScreen = false;
        GameObject go = new GameObject("ConnectionMgr");
        go.transform.parent = this.transform;
        mConnectionMgr = go.AddComponent<ConnectionMgr>();

        mRadianceMgr = new RadianceManager();

        //StartCoroutine(SetupAsync());

        ConsoleUI.instance.SetIpAddress(serverAddress);
        //StartCoroutine(CheckInternetIpAddressAsync());

        //1751578
        mHistory = go.AddComponent<History>();
        //1751578
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != SceneManager.GetActiveScene().name) {
            GameObject[] gb = scene.GetRootGameObjects();
            for (int i = 0; i < gb.Length; i++)
            {
                if (gb[i].GetComponent<VisibleRoot>())
                {
                    SetupFOI(gb[i], scene.name);
                    SceneManager.MoveGameObjectToScene(gb[i], SceneManager.GetActiveScene());
                }
            }
            SceneManager.UnloadSceneAsync(scene);
        }
    }

    IEnumerator SetupAsync(int _port)
    {
        Debug.Log("===SETUP ASYNC");
        SceneName = SceneDropdown.options[SceneDropdown.value].text;
        AsyncOperation loadAsync = SceneManager.LoadSceneAsync(SceneName);

        yield return loadAsync;
        
        while(!loadAsync.isDone)
        {
            yield return null;
        }

        SetupTartgeScene();

        //SceneManager.LoadSceneAsync("Demo2", LoadSceneMode.Additive);

        //yield return new AsyncOperation();

        GameObject go = new GameObject("NetworkServer");
        NetworkServer networkServer = go.AddComponent<NetworkServer>();

        if (_port == 0)
        {
            networkServer.Startup(serverAddress, serverPort);
        }
        else
        {
            serverPort = _port;
            networkServer.Startup(serverAddress, _port);
        }
    }

    void SetupTartgeScene()
    {
        Debug.Log("Setup target scene");

        // Setup camera
        mSceneCamera = Camera.main;
        //mSceneCamera.enabled = false;
        mSceneCamera.clearFlags = CameraClearFlags.Color;
        mSceneCamera.backgroundColor = Color.black;

        // Setup all light
        //if (GameObject.FindGameObjectWithTag("LCRS_Light0") != null)
        //{
        //    mDynLights.Add(GameObject.FindGameObjectWithTag("LCRS_Light0").gameObject.GetComponent<Light>());
        //}

        //if (GameObject.FindGameObjectWithTag("LCRS_Light1") != null)
        //{
        //    mDynLights.Add(GameObject.FindGameObjectWithTag("LCRS_Light1").gameObject.GetComponent<Light>());
        //}

        //SetupLCRS();

        Scene thisScene = SceneManager.GetActiveScene();
        GameObject[] go = thisScene.GetRootGameObjects();
        for (int i = 0; i < go.Length; i++) {
            if(go[i].GetComponent<VisibleRoot>())
                SetupFOI(go[i], thisScene.name);
        }
    }

    void SetupLCRS()
    {
        List<GameObject> rootGameObjects = new List<GameObject>();
        SceneManager.GetActiveScene().GetRootGameObjects(rootGameObjects);

        Shader lcrsShader = UnityEngine.Resources.Load("Shader/CustomStandardShader/CustomStandardSpecular", typeof(Shader)) as Shader;

        for (int i = 0; i < rootGameObjects.Count; ++i)
        {
            SetupSpecificMaterial(rootGameObjects[i].transform, lcrsShader);
        }
    }

    void SetupSpecificMaterial(Transform goTrans, Shader lcrsShader)
    {
        MeshRenderer[] mrs = goTrans.gameObject.GetComponents<MeshRenderer>();
        if (mrs != null)
        {
            for (int i = 0; i < mrs.Length; ++i)
            {
                for (int j = 0; j < mrs[i].materials.Length; ++j)
                {
                    mrs[i].materials[j].shader = lcrsShader;
                }

            }
        }

        for (int i = 0; i < goTrans.childCount; ++i)
        {
            SetupSpecificMaterial(goTrans.GetChild(i), lcrsShader);
        }
    }

    public Color32 ToColor(int HexVal)
    {
        byte R = (byte)((HexVal >> 16) & 0xFF);
        byte G = (byte)((HexVal >> 8) & 0xFF);
        byte B = (byte)((HexVal) & 0xFF);
        return new Color32(R, G, B, 255);
    }

    void SetupFOI()
    {
        List<GameObject> rootGameObjects = new List<GameObject>();
        UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects(rootGameObjects);
        Shader lcrsShader = UnityEngine.Resources.Load("Shader/VisibleID", typeof(Shader)) as Shader;

        for (int i = 0; i < rootGameObjects.Count; ++i)
        {
            if (rootGameObjects[i].GetComponent<VisibleRoot>())
            {
                int idCounter = 5;
                int idStep = 3;

                //mRadianceMgr.SetupAccumulator(rootGameObjects[i].transform.childCount);

                for (int j = 0; j < rootGameObjects[i].transform.childCount; ++j)
                {
                    idCounter += idStep;
                    Color32 foiColor = ToColor(idCounter);

                    mRadianceMgr.mVisibleGrid[foiColor.r, foiColor.g, foiColor.b] = (j + 1);
                    mRadianceMgr.mVisibleNames[foiColor.r, foiColor.g, foiColor.b] = rootGameObjects[i].transform.GetChild(j).name;

                    SetupFoiMaterial(rootGameObjects[i].transform.GetChild(j), lcrsShader, foiColor);

                    //mRadianceMgr.mVisibleAccumulator[j] = new PartInfo(rootGameObjects[i].transform.GetChild(j).transform);
                }
            }
        }
        mRadianceMgr.mIsPrepaed = true;
    }

    void SetupFOI(GameObject go, String sceneName)
    {
        Shader lcrsShader = UnityEngine.Resources.Load("Shader/VisibleID", typeof(Shader)) as Shader;

        //mRadianceMgr.SetupAccumulator(go.transform.childCount);

        for (int j = ChildConponentCount; j < go.transform.childCount + ChildConponentCount; ++j)
        {
            IdCounter += IdStep;
            Color32 foiColor = ToColor(IdCounter);

            mRadianceMgr.mVisibleGrid[foiColor.r, foiColor.g, foiColor.b] = (j + 1);
            mRadianceMgr.mVisibleNames[foiColor.r, foiColor.g, foiColor.b] = sceneName + "/" + go.name + "/" + go.transform.GetChild(j - ChildConponentCount).name;

            SetupFoiMaterial(go.transform.GetChild(j - ChildConponentCount), lcrsShader, foiColor);

            //mRadianceMgr.mVisibleAccumulator[j] = new PartInfo(rootGameObjects[i].transform.GetChild(j).transform);
        }
        ChildConponentCount += go.transform.childCount;
        mRadianceMgr.mIsPrepaed = true;
    }


    void SetupFoiMaterial(Transform goTrans, Shader lcrsShader, Color32  foiColor)
    {
        
        MeshRenderer[] mrs = goTrans.gameObject.GetComponents<MeshRenderer>();
        if (mrs != null)
        {
            for (int i = 0; i < mrs.Length; ++i)
            {
                for (int j = 0; j < mrs[i].materials.Length; ++j)
                {
                    mrs[i].materials[j].shader = lcrsShader;

                    mrs[i].materials[j].color = foiColor;
                }

            }
        }

        for (int i = 0; i < goTrans.childCount; ++i)
        {
            SetupFoiMaterial(goTrans.GetChild(i), lcrsShader , foiColor);
        }
    }

    public void RunServer(int _port = 0)
    {
        StartCoroutine(SetupAsync(_port));
    }

    public void RunClient()
    {
        GameObject go = new GameObject("NetworkClient");
        NetworkClient networkClient = go.AddComponent<NetworkClient>();
        networkClient.Startup(serverAddress, serverPort);

        // Lock framerate of client
        Application.targetFrameRate = 27;

        // Enable client camera
        sceneCamera.enabled = true;
    }

    public IEnumerator CheckInternetIpAddressAsync()
    {
        string httpRequest = "http://pv.sohu.com/cityjson?ie=utf-8";

        WWW ret = new WWW(httpRequest);
        yield return ret;
        if (ret.error != null)
        {
            Debug.LogError("error:" + ret.error);
            yield break;
        }

        if (string.IsNullOrEmpty(ret.text))
        {
            yield break;
        }

        string prefix = "\"cip\": \"";
        int sIdx = ret.text.LastIndexOf(prefix);
        int eIdx = ret.text.LastIndexOf("\", \"cid\": \"");

        serverAddress = ret.text.Substring(sIdx + prefix.Length, eIdx - sIdx - prefix.Length);

        //ConsoleUI.instance.SetIpAddress(serverAddress);
    }

    public string ipAddress
    {
        get
        {
            return serverAddress;
        }

        set
        {
            serverAddress = value;
        }
    }

    bool bGiKeywordState = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bGiKeywordState = !bGiKeywordState;
            if (bGiKeywordState == false)
            {
                Shader.EnableKeyword("UNITY_ONLY_OUTPUT_GI");
            }
            else
            {
                Shader.DisableKeyword("UNITY_ONLY_OUTPUT_GI");
            }
        }
    }
}
