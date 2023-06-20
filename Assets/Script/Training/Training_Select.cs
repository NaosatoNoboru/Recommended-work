using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Video;

[System.Serializable]
public class Training_Select_Data
{
    public int data_count;
    public List<string> training_name;
}

public class Training_Select : MonoBehaviour
{
    public AudioSource audio_source;
    public AudioClip[] SE = new AudioClip[3];//0=選択　1=決定　2=キャンセル
    public VideoPlayer Movie;
    public VideoClip[] Training = new VideoClip[17];//練習譜面の動画

    public FadeController fade_controller;
    public RectTransform rt;
    public Text music_list;//曲名表示

    public List<string> training_data_list = new List<string>();

    private Training_Select_Data TSD;

    private List<string> split_text = new List<string>();//曲名管理
    private float start_pos;                    //初期y座標
    private int[] select_count = new int[2];//曲の数 0=最新　1=古い
    private int select_scene;
    private bool select;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        //データの読み込み
        TSD = Load_Training_Data();

        for (int i = 0; i < TSD.data_count; i++)
        {
            training_data_list.Add(TSD.training_name[i]);
            split_text.Add(TSD.training_name[i]);
        }

        music_list.text = "";

        //テキストに曲名表示
        for (int i = 0; i < TSD.data_count; i++)
        {
            if (i == 0)
            {
                music_list.text += "<color=#323232>" + split_text[i] + "</color>\n";
            }
            else
            {
                music_list.text += "<color=#969696>" + split_text[i] + "</color>\n";
            }
        }

        //動画を挿入
        Movie.clip = Training[0];

        //項目の数だけ大きさと座標を変える
        Vector3 pos = rt.localPosition;
        pos.y += (TSD.data_count - 1) * -40;
        rt.localPosition = pos;

        Vector2 size = rt.sizeDelta;
        size.y = 60 + ((TSD.data_count - 1) * 82.5f);
        rt.sizeDelta = size;

        //private変数の初期化
        start_pos = rt.localPosition.y;
        select_count[0] = 0;
        select_count[1] = select_count[0];
        select_scene = 0;
        select = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = rt.localPosition;

        //上に移動
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            audio_source.PlayOneShot(SE[0]);

            select_count[0]--;

            //一番上から一番下へ
            if (select_count[0] < 0)
            {
                select_count[0] = TSD.data_count - 1;
                pos.y = (TSD.data_count * -40) + ((TSD.data_count - 1) * 80);
            }
            else
            {
                pos.y -= 80;
            }
        }

        //下に移動
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            audio_source.PlayOneShot(SE[0]);

            select_count[0]++;

            //一番下から一番上へ
            if (select_count[0] >= TSD.data_count)
            {
                select_count[0] = 0;
                pos.y = TSD.data_count * -40;
            }
            else
            {
                pos.y += 80;
            }
        }

        //エンターキーを押したらゲームへ
        if (Input.GetKeyDown(KeyCode.Return) && select == false && fade_controller.isFadeIn == false)
        {
            audio_source.PlayOneShot(SE[1]);

            fade_controller.isFadeOut = true;
            select_scene = 0;
            select = true;
        }

        //エスケープキーを押したらセレクト画面へ
        if (Input.GetKeyDown(KeyCode.Escape) && select == false && fade_controller.isFadeIn == false)
        {
            audio_source.PlayOneShot(SE[2]);

            fade_controller.isFadeOut = true;
            select_scene = 1;
            select = true;
        }

        //曲を変えたら変更
        if (select_count[0] != select_count[1])
        {
            Movie.clip = Training[select_count[0]];
            Movie.time = 0.0f;

            music_list.text = "";

            //テキストに表示
            for (int i = 0; i < TSD.data_count; i++)
            {
                if (i == select_count[0])
                {
                    music_list.text += "<color=#323232>" + split_text[i] + "</color>\n";
                }
                else
                {
                    music_list.text += "<color=#969696>" + split_text[i] + "</color>\n";
                }
            }

            select_count[1] = select_count[0];
        }

        rt.localPosition = pos;

        //シーン遷移
        if (select == true && fade_controller.isFadeOut == false)
        {
            Music_Data.music_name = "Training";
            Music_Data.music_json = "Training_" + split_text[select_count[0]];

            if (select_scene == 0)
            {
                SceneManager.LoadScene("Training");
            }
            else
            {
                SceneManager.LoadScene("Select");
            }
        }
    }

    public Training_Select_Data Load_Training_Data()
    {
        StreamReader reader;

        string data = "";

        reader = new StreamReader(Application.dataPath + "/Json/Training_List.json", false);
        data = reader.ReadToEnd();
        reader.Close();

        return JsonUtility.FromJson<Training_Select_Data>(data);
    }
}
