using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Select : MonoBehaviour
{
    public AudioSource audio_source;//SE用
    public AudioClip[] SE = new AudioClip[3];//0=選択　1=決定　2=キャンセル

    public FadeController fade_controller;
    public RectTransform rt;
    public InputField input_field;
    public GameObject BGM;
    public GameObject music_add;
    public Text select_text;

    private string text_data;//テキストの内容代入用
    private string[] split_text;//テキストの内容代入用
    private string add_name;
    private int[] select_count = new int[2];//メニューの数 0=最新　1=古い
    private bool select;

    private static GameObject save_BGM;
    private static bool existence = false;//存在しているかどうか

    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        music_add.SetActive(false);

        text_data = select_text.text;

        // 改行で分割して配列に代入
        split_text = text_data.Split(char.Parse("\n"));

        select_text.text = "";

        //テキストに表示
        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                select_text.text += "<color=#323232>" + split_text[i] + "</color>\n";
            }
            else
            {
                select_text.text += "<color=#969696>" + split_text[i] + "</color>\n";
            }
        }

        //生成されてないなら生成する
        if (existence == false)
        {
            BGM = Instantiate(BGM);
            save_BGM = BGM;
        }

        select_count[0] = 0;
        select_count[1] = 0;
        select = false;
        existence = true;
    }

    // Update is called once per frame
    void Update()
    {
        //時間停止してないとき
        if (Time.timeScale == 1)
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
                    select_count[0] = 4;
                    pos.y = select_count[0] * 40;
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
                if (select_count[0] >= 5)
                {
                    select_count[0] = 0;
                    pos.y = -160;
                }
                else
                {
                    pos.y += 80;
                }
            }

            //項目を変えたら変更
            if (select_count[0] != select_count[1])
            {
                select_text.text = "";

                //テキストに表示
                for (int i = 0; i < 5; i++)
                {
                    if (i == select_count[0])
                    {
                        select_text.text += "<color=#323232>" + split_text[i] + "</color>\n";
                    }
                    else
                    {
                        select_text.text += "<color=#969696>" + split_text[i] + "</color>\n";
                    }
                }

                select_count[1] = select_count[0];
            }

            //エンターキーを押したら
            if (Input.GetKeyDown(KeyCode.Return) && select == false && fade_controller.isFadeIn == false)
            {
                audio_source.PlayOneShot(SE[1]);

                if (select_count[0] != 1)
                {
                    fade_controller.isFadeOut = true;
                    select = true;
                }
                else
                {
                    Time.timeScale = 0;//時間停止
                    music_add.SetActive(true);
                }
            }

            //更新
            rt.localPosition = pos;
        }
        else if (Time.timeScale == 0)
        {
            //時間停止してる時
            //ESCキーを押したら選択に戻る
            if (Input.GetKeyDown(KeyCode.Escape) && select == false)
            {
                audio_source.PlayOneShot(SE[2]);

                Time.timeScale = 1;
                input_field.text = "";
                music_add.SetActive(false);
            }
        }

        //シーン遷移
        if (select == true && fade_controller.isFadeOut == false)
        {
            switch (select_count[0])
            {
                case 0:
                    //BGMを消す
                    Destroy(save_BGM);
                    existence = false;

                    SceneManager.LoadScene("Music_Select");
                    break;
                case 1:
                    //BGMを消す
                    Destroy(save_BGM);
                    existence = false;

                    SceneManager.LoadScene("Sheet_Music");
                    break;
                case 2:
                    //BGMを消す
                    Destroy(save_BGM);
                    existence = false;

                    SceneManager.LoadScene("Training_Select");
                    break;
                case 3:
                    //BGMを残す
                    DontDestroyOnLoad(BGM);

                    SceneManager.LoadScene("Option"); 
                    break;
                case 4:
                    Application.Quit();//ゲーム終了
                    break;
                default:
                    break;
            }
        }
    }
    public void Input_Name()
    {
        add_name = input_field.text;
    }

    public void decision()
    {
        //フォルダーの作成
        Directory.CreateDirectory(Application.dataPath + "/Music/" + add_name);

        //JSONファイルの作成
        File.Create(Application.dataPath + "/Music/" + add_name + "/" + add_name + "_easy.json");
        File.Create(Application.dataPath + "/Music/" + add_name + "/" + add_name + ".json");
        File.Create(Application.dataPath + "/Music/" + add_name + "/" + add_name + "_hard.json");

        //曲リストに追加
        StreamWriter writer;
        Music_Select_Data MSD = Music_Select.Load_Select_Data();
        Select_Data select_data = new Select_Data();

        //代入
        select_data.music_name = add_name;
        MSD.data_count++;
        MSD.select_data.Add(select_data);

        string json = JsonUtility.ToJson(MSD, true);

        writer = new StreamWriter(Application.dataPath + "/Json/MusicList.json", false);
        writer.WriteLine(json);
        writer.Flush();
        writer.Close();

        Music_Data.music_json = add_name;

        Time.timeScale = 1;
        input_field.text = "";
        music_add.SetActive(false);
    }
}
