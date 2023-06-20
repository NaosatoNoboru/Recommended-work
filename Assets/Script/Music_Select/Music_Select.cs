using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

[System.Serializable]
public class Music_Select_Data
{
    public int data_count;//曲の数
    public List<Select_Data> select_data;
}

[System.Serializable]
public class Select_Data
{
    public string music_name;

    public float[] rust = new float[2];//サビ 0=サビ始め　1=サビ終わり
    public long[] score = new long[3];
    public string[] rank = new string[3];
}

public class Music_Select : MonoBehaviour
{
    public AudioSource[] audio_source = new AudioSource[2];//0=曲のサビ流す用 1=SE用
    public AudioClip[] SE = new AudioClip[3];//0=選択　1=決定　2=キャンセル

    public FadeController fade_controller;
    public RectTransform rt;
    public GameObject Pause;
    public Text music_list;//曲名表示
    public Text difficulty;//難易度表示
    public Text score;     //スコア表示
    public Text rank;      //ランク表示
    public Text auto;      //ランク表示
    public Text delete_music_name;//ポーズでの曲名表示
    public Text delete_selection;//確認

    public List<Select_Data> select_data_list = new List<Select_Data>();

    public static bool auto_flag = false;//false=オートOFF true=オートONN

    private Music_Select_Data MSD;

    private List<string> split_text = new List<string>();//曲名管理
    private float h;
    private float hs;
    private float start_pos;                    //初期y座標
    private int[] difficulty_count = new int[2];//0=今の難易度　1=古い難易度 [0=Easy 1=Normal 2=Hard]
    private int[] select_count = new int[2];//曲の数 0=最新　1=古い
    private int pause_select;//はい・いいえの選択
    private int select_scene;
    private bool select;
    private bool pause_flag;//true=ポーズ中

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        //曲データの読み込み
        MSD = Load_Select_Data();

        for (int i = 0; i < MSD.data_count; i++)
        {
            select_data_list.Add(MSD.select_data[i]);
            split_text.Add(MSD.select_data[i].music_name);
        }

        music_list.text = "";
        score.text = "BestScore：" + select_data_list[0].score[1].ToString("0000000");

        //テキストに曲名表示
        for (int i = 0; i < MSD.data_count; i++)
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

        difficulty.text = "＜ " + "<color=#FFFF00>NORMAL</color>" + " ＞";
        rank.text = MSD.select_data[0].rank[1];

        //ランクの表示
        if (MSD.select_data[0].rank[1] == "PFC")
        {
            rank.text = MSD.select_data[0].rank[1];
        }
        else if (MSD.select_data[0].rank[1] == "FC")
        {
            rank.text = "<color=#98BBDB>" + MSD.select_data[0].rank[1] + "</color>";
        }
        else
        {
            //スコアがあるときだけランク表示
            if (MSD.select_data[0].score[1] != 0)
            {
                rank.text = Result_UI.Rank_str(MSD.select_data[select_count[0]].score[difficulty_count[0]]);
            }
            else
            {
                rank.text = "";
            }
        }

        //オートかどうか
        if (auto_flag == false)
        {
            auto.text = "Auto OFF";
            auto.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
        }
        else
        {
            auto.text = "Auto ON";
            auto.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }

        //サビ流し
        Load_Rust();

        Pause.SetActive(false);

        //項目の数だけ大きさと座標を変える
        Vector3 pos = rt.localPosition;
        pos.y += (MSD.data_count - 1) * -40;
        rt.localPosition = pos;

        Vector2 size = rt.sizeDelta;
        size.y = 60 + ((MSD.data_count - 1) * 82.5f);
        rt.sizeDelta = size;

        //private変数の初期化
        start_pos = rt.localPosition.y;
        h = 0.0f;
        hs = 0.005f;
        difficulty_count[0] = 1;
        difficulty_count[1] = difficulty_count[0];
        select_count[0] = 0;
        select_count[1] = select_count[0];
        select_scene = 0;
        pause_select = 1;
        select = false;
        pause_flag = false;
    }

    // Update is called once per frame
    void Update()
    {
        //ポーズ中かどうか
        if (pause_flag == false)
        {
            Vector3 pos = rt.localPosition;

            //上に移動
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                audio_source[1].PlayOneShot(SE[0]);

                select_count[0]--;

                //一番上から一番下へ
                if (select_count[0] < 0)
                {
                    select_count[0] = MSD.data_count - 1;
                    pos.y = (MSD.data_count * -40) + ((MSD.data_count - 1) * 80);
                }
                else
                {
                    pos.y -= 80;
                }

                //サビ流しの更新
                Load_Rust();
            }

            //下に移動
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                audio_source[1].PlayOneShot(SE[0]);

                select_count[0]++;

                //一番下から一番上へ
                if (select_count[0] >= MSD.data_count)
                {
                    select_count[0] = 0;
                    pos.y = MSD.data_count * -40;
                }
                else
                {
                    pos.y += 80;
                }

                //サビ流しの更新
                Load_Rust();
            }

            //右に移動
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                audio_source[1].PlayOneShot(SE[0]);

                difficulty_count[0]++;

                if (difficulty_count[0] > 2)
                {
                    difficulty_count[0] = 0;
                }
            }

            //左に移動
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                audio_source[1].PlayOneShot(SE[0]);

                difficulty_count[0]--;

                if (difficulty_count[0] < 0)
                {
                    difficulty_count[0] = 2;
                }
            }

            //ランク表示
            if (MSD.select_data[select_count[0]].rank[difficulty_count[0]] == "PFC")
            {
                rank.text = MSD.select_data[select_count[0]].rank[difficulty_count[0]];

                //PFCなら虹色にする
                rank.color = Color.HSVToRGB(h % 1, 1, 1);

                h += hs;

                if (h > 360.0f || h < 0.0f)
                {
                    hs *= -1;
                }

            }
            else if (MSD.select_data[select_count[0]].rank[difficulty_count[0]] == "FC")
            {
                rank.text = "<color=#98BBDB>" + MSD.select_data[select_count[0]].rank[difficulty_count[0]] + "</color>";
            }
            else
            {
                //スコアがあるときだけランク表示
                if (MSD.select_data[select_count[0]].score[difficulty_count[0]] != 0)
                {
                    rank.text = Result_UI.Rank_str(MSD.select_data[select_count[0]].score[difficulty_count[0]]);
                }
                else
                {
                    rank.text = "";
                }
            }

            //ポーズ
            if (Input.GetKeyDown(KeyCode.D))
            {
                Pause.SetActive(true);

                delete_music_name.text = split_text[select_count[0]] + "を削除しますか？";
                delete_selection.text = "<color=#AAAAAA>はい　　　</color>" + "<color=#FFFFFF>いいえ</color>";
                pause_flag = true;
            }

            //オートモードの切り替え
            if (Input.GetKeyDown(KeyCode.A))
            {
                audio_source[1].PlayOneShot(SE[0]);

                if (auto_flag == false)
                {
                    auto.text = "Auto ON";
                    auto.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                    auto_flag = true;
                }
                else
                {
                    auto.text = "Auto OFF";
                    auto.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);

                    auto_flag = false;
                }
            }

            //エンターキーを押したらゲームへ
            if (Input.GetKeyDown(KeyCode.Return) && select == false && fade_controller.isFadeIn == false)
            {
                audio_source[1].PlayOneShot(SE[1]);

                fade_controller.isFadeOut = true;
                select_scene = 0;
                select = true;
            }

            //スペースキーを押したら譜面編集へ
            if (Input.GetKeyDown(KeyCode.Space) && select == false && fade_controller.isFadeIn == false)
            {
                audio_source[1].PlayOneShot(SE[1]);

                fade_controller.isFadeOut = true;
                select_scene = 1;
                select = true;
            }

            //エスケープキーを押したらセレクト画面へ
            if (Input.GetKeyDown(KeyCode.Escape) && select == false && fade_controller.isFadeIn == false)
            {
                audio_source[1].PlayOneShot(SE[2]);

                fade_controller.isFadeOut = true;
                select_scene = 2;
                select = true;
            }

            //曲を変えたら変更
            if (select_count[0] != select_count[1])
            {
                music_list.text = "";

                //スコアをテキストに表示
                score.text = "BestScore：" +
                    select_data_list[select_count[0]].score[difficulty_count[0]].ToString("0000000");

                //テキストに表示
                for (int i = 0; i < MSD.data_count; i++)
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

            //難易度を変えたら変更
            if (difficulty_count[0] != difficulty_count[1])
            {
                score.text = "BestScore：" +
                    select_data_list[select_count[0]].score[difficulty_count[0]].ToString("0000000");

                //テキストに表示
                switch (difficulty_count[0])
                {
                    case 0:
                        difficulty.text = "＜ " + "<color=#00FF00>EASY</color>" + " ＞";
                        break;
                    case 1:
                        difficulty.text = "＜ " + "<color=#FFFF00>NORMAL</color>" + " ＞";
                        break;
                    case 2:
                        difficulty.text = "＜ " + "<color=#FF0000>HARD</color>" + " ＞";
                        break;
                    default:
                        break;
                }

                //更新
                difficulty_count[1] = difficulty_count[0];
            }

            rt.localPosition = pos;

            //シーン遷移
            if (select == true && fade_controller.isFadeOut == false)
            {
                Music_Data.music_name = split_text[select_count[0]];
                Music_Data.music_json = split_text[select_count[0]];

                //Easy or Hardだったらさらに書き足す
                switch (difficulty_count[0])
                {
                    case 0:
                        Music_Data.music_json += "_easy";
                        break;
                    case 2:
                        Music_Data.music_json += "_hard";
                        break;
                    default:
                        break;
                }

                switch (select_scene)
                {
                    case 0:
                        //譜面データが存在するならGameへ
                        if (Music_Existence() == true)
                        {
                            SceneManager.LoadScene("Game");
                        }
                        else
                        {
                            SceneManager.LoadScene("Music_Select");
                        }
                        break;
                    case 1:
                        SceneManager.LoadScene("Sheet_Music");
                        break;
                    case 2:
                        SceneManager.LoadScene("Select");
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            //ポーズ中
            //右に移動
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                audio_source[1].PlayOneShot(SE[0]);

                pause_select++;

                if (pause_select > 1)
                {
                    pause_select = 0;
                }
            }

            //左に移動
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                audio_source[1].PlayOneShot(SE[0]);

                pause_select--;

                if (pause_select < 0)
                {
                    pause_select = 1;
                }
            }

            //選択の表示
            if (pause_select == 0)
            {
                delete_selection.text = "<color=#FFFFFF>はい　　　</color>" + "<color=#AAAAAA>いいえ</color>";
            }
            else
            {
                delete_selection.text = "<color=#AAAAAA>はい　　　</color>" + "<color=#FFFFFF>いいえ</color>";
            }

            //エンターキーを押したら
            if (Input.GetKeyDown(KeyCode.Return) && select == false && fade_controller.isFadeIn == false)
            {
                audio_source[1].PlayOneShot(SE[1]);

                if (pause_select == 0)
                {
                    //リロード
                    Delete_Music();
                    fade_controller.isFadeOut = true;
                    select = true;
                }
                else
                {
                    pause_flag = false;
                    Pause.SetActive(false);
                }
            }

            //エスケープキーを押した解除
            if (Input.GetKeyDown(KeyCode.Escape) && select == false && fade_controller.isFadeIn == false)
            {
                audio_source[1].PlayOneShot(SE[1]);
                pause_flag = false;
                Pause.SetActive(false);
            }

            //同じシーンを再読み込み
            if (select == true && fade_controller.isFadeOut == false)
            {
                SceneManager.LoadScene("Music_Select");
            }
        }
    }

    public static Music_Select_Data Load_Select_Data()
    {
        StreamReader reader;

        string data = "";

        reader = new StreamReader(Application.dataPath + "/Json/MusicList.json", false);
        data = reader.ReadToEnd();
        reader.Close();

        return JsonUtility.FromJson<Music_Select_Data>(data);
    }

    public void Load_Rust()
    {
        //曲が存在して、サビの終了時間が0.0秒ではないとき
        if (Resources.Load("Song/" + MSD.select_data[select_count[0]].music_name) as AudioClip == true &&
            MSD.select_data[select_count[0]].rust[1] > 0.0f)
        {
            audio_source[0].clip = Resources.Load("Song/" + MSD.select_data[select_count[0]].music_name) as AudioClip;

            //曲の開始時間が0.0秒以下なら0.0秒に戻す
            if (MSD.select_data[select_count[0]].rust[0] < 0.0f)
            {
                MSD.select_data[select_count[0]].rust[0] = 0.0f;
               Music_Data.Save_Rust();
            }

            //曲が存在して、曲の長さよりもサビ終わりの時間の方がでかいなら戻す
            if (Resources.Load("Song/" + MSD.select_data[select_count[0]].music_name) as AudioClip == true &&
               MSD.select_data[select_count[0]].rust[1] > audio_source[0].clip.length)
            {
                MSD.select_data[select_count[0]].rust[1] = audio_source[0].clip.length;
                Music_Data.Save_Rust();
            }

            audio_source[0].time = MSD.select_data[select_count[0]].rust[0];
            Music_Rust.rust_start = MSD.select_data[select_count[0]].rust[0];
            Music_Rust.rust_end = MSD.select_data[select_count[0]].rust[1];

            audio_source[0].Play();
        }
        else
        {
            //ないなら入れない
            audio_source[0].clip = null;
        }
    }

    public void Delete_Music()
    {
        //譜面データのファイルを削除
        Directory.Delete(Application.dataPath + "/Music/" + select_data_list[select_count[0]].music_name, true);

        //指定した曲の項目を削除
        MSD.select_data.RemoveAt(select_count[0]);

        MSD.data_count = MSD.select_data.Count;

        //曲リストに追加
        StreamWriter writer;
        Select_Data select_data = new Select_Data();

        string json = JsonUtility.ToJson(MSD, true);

        writer = new StreamWriter(Application.dataPath + "/Json/MusicList.json", false);
        writer.WriteLine(json);
        writer.Flush();
        writer.Close();
    }

    //譜面が存在するかどうか
    public bool Music_Existence()
    {
        bool existence = false;

        Json_Data JD = Music_Json.Load_Json();

        if (JD != null)
        {
            if (JD.max_combo != 0 && Resources.Load("Song/" + JD.music_name) as AudioClip == true)
            {
                existence = true;
            }
        }

        return existence;
    }
}
