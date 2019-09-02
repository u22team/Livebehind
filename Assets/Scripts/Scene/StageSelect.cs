using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{

    // 定数 //
    public static readonly string[] StageName = new string[] // これの要素数でステージの数が決まる。
        {
            "ステージ1","ステージ2","ステージ3"
        };
    public static int[][,] Map_Front { get; private set; }
    public static int[][,] Map_Back { get; private set; }

    readonly int StageSelect_FontSize = 50; // ステージ名のフォントサイズ
    readonly float StageSelect_Space = 2; // 2つのステージ名の間の幅
    readonly Color
        StageSelectColor_Normal = Color.white,    // ステージ名の基本の色
        StageSelectColor_MouseOn = Color.yellow,  // ステージ名にマウスカーソルを合わせた時のステージ名の色
        StageSelectColor_MouseClick = Color.cyan; //ステージ名をクリックした時のステージ名の色

    readonly string GameSceneName = "Game(Waffuru)";

    AudioSource audioSource;
    ////////////////

    // 読み取り専用 //
    int StageLength { get { return StageName.Length; } } // StageNameのLengthを取得する。
    Vector2 MousePos // マウスカーソルのWorld座標を取得する。
    {
        get
        {
            Vector3 pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            return Camera.main.ScreenToWorldPoint(pos);
        }
    }

    Vector2 ToucchPos
    {
        get
        {
            Vector3 pos = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 10);
            return Camera.main.ScreenToWorldPoint(pos);
        }
    }
    ////////////////

    [SerializeField]
    GameObject[] Block; // ゲームに配置するGameObject



    [SerializeField]
    GameObject TextPrefab;
    [SerializeField]
    Transform Canvas;
    [SerializeField]
    Camera cam;
    public static GameObject[] Block_; // GameMapCreateが読み込む用。
    GameObject[] stage_names; // 生成したステージ名を全てこれに代入する。
    public static int stage_num { get; private set; } // 選択されたステージ番号

    // ここでマップの形を決める。好きに変えてくれ。
    void Assignment_Map()
    {
        // ID   Block
        int
            OO = 0, // 空気
            XX = 1  // ブロック
            ;

        // Map
        // 問題点:FrontとBackの要素数が違うとまずい
        Map_Front[0] = new int[,]
        {
            { XX,OO,OO,OO,OO,OO,OO,OO,OO,OO,OO,XX },
            { XX,OO,OO,OO,OO,OO,OO,OO,OO,OO,OO,XX },
            { XX,OO,OO,OO,OO,OO,OO,OO,OO,OO,OO,XX },
            { XX,OO,OO,OO,OO,OO,OO,OO,OO,OO,OO,XX },
            { XX,OO,OO,OO,OO,OO,OO,OO,OO,OO,OO,XX },
            { XX,OO,OO,OO,OO,OO,OO,OO,OO,OO,OO,XX },
            { XX,OO,OO,OO,XX,OO,OO,OO,OO,OO,OO,XX },
            { XX,XX,XX,XX,XX,XX,XX,XX,XX,XX,XX,XX },
        };
        Map_Back[0] = new int[,]
        {
            { XX,OO,OO,OO,OO,OO,OO,OO,OO,OO,OO,XX },
            { XX,OO,OO,OO,OO,OO,OO,OO,OO,OO,OO,XX },
            { XX,OO,OO,OO,OO,OO,OO,OO,OO,OO,OO,XX },
            { XX,OO,OO,OO,OO,OO,OO,OO,OO,OO,OO,XX },
            { XX,OO,OO,OO,XX,OO,OO,OO,OO,OO,OO,XX },
            { XX,OO,OO,OO,OO,OO,OO,OO,OO,OO,OO,XX },
            { XX,OO,OO,OO,OO,OO,OO,XX,OO,OO,OO,XX },
            { XX,XX,XX,XX,XX,XX,XX,XX,XX,XX,XX,XX },
        };
    }


    void Start()
    {
        Block_ = Block;
        Map_Front = new int[StageLength][,];
        Map_Back = new int[StageLength][,];
        stage_names = new GameObject[StageLength];
        Assignment_Map();

        StartCoroutine(Main_());
        LoadingUi.SetActive(false);
        audioSource = cam.gameObject.GetComponent<AudioSource>();
     }

    IEnumerator Main_()
    {
        CreateSelect();
        StartCoroutine(WheelScreen());

        RaycastHit2D tmp_hit = new RaycastHit2D();
        int index = -1;
        while (true)
        {
            yield return null;

            RaycastHit2D hit = MouseCursor(1 << TextPrefab.layer);
            if(hit != tmp_hit)
            {
                if (tmp_hit)
                {
                    if(ArrayContains(stage_names, tmp_hit.transform.gameObject, out int trash))
                        stage_names[index].GetComponent<Text>().color = StageSelectColor_Normal;
                }
                if (hit)
                {
                    if(ArrayContains(stage_names, hit.transform.gameObject, out index))
                        stage_names[index].GetComponent<Text>().color = StageSelectColor_MouseOn;
                }
                else
                    index = -1;
                tmp_hit = hit;
            }
            if (index >= 0 && Input.GetMouseButtonDown(0))
            {
                Text tmp_sr = stage_names[index].GetComponent<Text>();
                tmp_sr.color = StageSelectColor_MouseClick;
                while (Input.GetMouseButton(0))
                {
                    yield return null;
                    hit = MouseCursor(1 << TextPrefab.layer);
                    if(hit != tmp_hit)
                        break;
                    else if(Input.GetMouseButtonUp(0))
                    {
                        audioSource.Stop();
                        stage_num = index;
                        if (index == 0) LoadNextScene("Game");
                        else SceneManager.LoadScene("NowInDev");
                    }
                }
                tmp_sr.color = StageSelectColor_Normal;
            }
            if (index >= 0 && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Text tmp_sr = stage_names[index].GetComponent<Text>();
                tmp_sr.color = StageSelectColor_MouseClick;
                while (Input.GetMouseButton(0))
                {
                    yield return null;
                    hit = MouseCursor(1 << TextPrefab.layer);
                    if (hit != tmp_hit)
                        break;
                    else if (Input.GetMouseButtonUp(0))
                    {
                        audioSource.Stop();
                        stage_num = index;
                        if (index == 0) LoadNextScene("Game");
                        else SceneManager.LoadScene("NowInDev");
                    }
                }
                tmp_sr.color = StageSelectColor_Normal;
            }
        }
    }

    // ステージ名を羅列する。更にstage_namesに代入する。
    void CreateSelect()
    {
        for (int i = 0; i < StageLength; i++)
        {
            Vector2 pos = new Vector2(0, i * -StageSelect_Space);
            stage_names[i] = TextInstantiate(TextPrefab, Canvas, pos, StageName[i], StageSelect_FontSize, StageSelectColor_Normal).gameObject;
        }
    }

    // TextをInstantiateする。
    Text TextInstantiate(GameObject prefab, Transform parent, Vector2 pos, string text, int font_size, Color color)
    {
        Text tmp_txt = Instantiate(prefab, pos, Quaternion.identity, parent).GetComponent<Text>();
        tmp_txt.text = text;
        tmp_txt.fontSize = font_size;
        tmp_txt.color = color;
        return tmp_txt;
    }

    // マウスカーソルの下にRayを飛ばす。
    RaycastHit2D MouseCursor(int mask)
    {
        return Physics2D.Raycast(MousePos, Vector3.forward, 100, mask);
    }

    // arrayにvalueがあるかどうかを判定する。indexは何要素目にあるかをoutする。なかった場合は、index=-1。
    bool ArrayContains<T>(T[] array, T values, out int index)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(values))
            {
                index = i;
                return true;
            }
        }
        index = -1;
        return false;
    }

    IEnumerator WheelScreen()
    {
        // スクロールの速さ
        float v  = 4;

        while(true)
        {
            yield return null;
            cam.transform.position += Vector3.up * Input.GetAxis("Mouse ScrollWheel") * v;
        }
    }

    private AsyncOperation async;
    [SerializeField] GameObject LoadingUi;
    [SerializeField] Slider Slider;

    public void LoadNextScene(string sceneName)
    {
        LoadingUi.SetActive(true);
        StartCoroutine(LoadScene(sceneName));
    }

    IEnumerator LoadScene(string sceneName)
    {
        async = SceneManager.LoadSceneAsync(sceneName);

        while (!async.isDone)
        {
            Slider.value = async.progress;
            yield return null;
        }
    }
}
