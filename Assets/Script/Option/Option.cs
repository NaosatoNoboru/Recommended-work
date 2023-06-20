using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;

[System.Serializable]
public class Key_Configu
{
    public string Left;
    public string Down;
    public string Up;
    public string Right;
    public int tap_sound;
    public float scroll_speed;
    public float master_volume;
}
public class Option : MonoBehaviour
{
    public AudioSource audio_source;//SE用
    public AudioClip[] SE = new AudioClip[3];//0=選択　1=決定　2=キャンセル
    public AudioClip[] Tap_SE = new AudioClip[3];//0=タンバリン　1=指パッチン　2=カード　3=なし

    public FadeController fade_controller;
    public RectTransform rt;
    public RectTransform key_rt;
    public Text select_text;
    public Text key_text;
    public GameObject Panel;
    public GameObject demo_notes;

    public string[] tap_sound_name = new string[4];//タップ音の名前

    private string[] key_command = new string[4];//キーコマンド
    private string text_data;//テキストの内容代入用
    private string text_key_data;//テキストの内容代入用
    private string[] split_text;//テキストの内容代入用
    private string[] split_key_text;//テキストの内容代入用
    private float scroll_speed;//プレイヤーが選択した速度
    private int tap_sound;//タップ音
    private int[] select_count = new int[2];//メニューの数 0=最新　1=古い
    private bool select;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        //Json読み込み
        {
            Key_Configu kc = Load_Key_Configu();

            //Jsonで読み込んだ値を代入
            key_command[0] = kc.Left;
            key_command[1] = kc.Down;
            key_command[2] = kc.Up;
            key_command[3] = kc.Right;
            tap_sound = kc.tap_sound;
            scroll_speed = kc.scroll_speed;
        }

        for (int i = 0; i < 4; i++)
        {
            key_text.text += key_command[i] + "\n";
        }

        key_text.text += "＜ " + tap_sound_name[tap_sound] + " ＞\n";
        key_text.text += "＜ " + scroll_speed.ToString("0.0") + " ＞";

        text_data = select_text.text;
        text_key_data = key_text.text;

        // 改行で分割して配列に代入
        split_text = text_data.Split(char.Parse("\n"));
        split_key_text = text_key_data.Split(char.Parse("\n"));

        select_text.text = "";
        key_text.text = "";

        //テキストに表示
        for (int i = 0; i < 6; i++)
        {
            if (i == select_count[0])
            {
                select_text.text += "<color=#323232>" + split_text[i] + "</color>\n";
                key_text.text += "<color=#323232>" + split_key_text[i] + "</color>\n";
            }
            else
            {
                select_text.text += "<color=#969696>" + split_text[i] + "</color>\n";
                key_text.text += "<color=#969696>" + split_key_text[i] + "</color>\n";
            }
        }

        Panel.SetActive(false);
        demo_notes.SetActive(false);

        select_count[0] = 0;
        select_count[1] = 0;
        select = false;
    }

    // Update is called once per frame
    void Update()
    {
        //時間停止してないとき
        if (Time.timeScale == 1)
        {
            Vector3[] pos = new Vector3[2];

            pos[0] = rt.localPosition;
            pos[1] = key_rt.localPosition;

            //上に移動
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                audio_source.PlayOneShot(SE[0]);

                select_count[0]--;

                //一番上から一番下へ
                if (select_count[0] < 0)
                {
                    select_count[0] = 5;
                    pos[0].y = select_count[0] * 50;
                    pos[1].y = select_count[0] * 50;
                }
                else
                {
                    pos[0].y -= 80;
                    pos[1].y -= 80;
                }
            }

            //下に移動
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                audio_source.PlayOneShot(SE[0]);

                select_count[0]++;

                //一番下から一番上へ
                if (select_count[0] >= 6)
                {
                    select_count[0] = 0;
                    pos[0].y = -200;
                    pos[1].y = -200;
                }
                else
                {
                    pos[0].y += 80;
                    pos[1].y += 80;
                }
            }

            //項目を変えたら変更
            if (select_count[0] != select_count[1])
            {
                //ノーツが落ちてくるデモを見せるかどうか
                if (select_count[0] >= 4)
                {
                    demo_notes.SetActive(true);
                }
                else
                {
                    demo_notes.SetActive(false);
                }

                select_text.text = "";
                key_text.text = "";

                //テキストに表示
                for (int i = 0; i < 6; i++)
                {
                    if (i == select_count[0])
                    {
                        select_text.text += "<color=#323232>" + split_text[i] + "</color>\n";
                        key_text.text += "<color=#323232>" + split_key_text[i] + "</color>\n";
                    }
                    else
                    {
                        select_text.text += "<color=#969696>" + split_text[i] + "</color>\n";
                        key_text.text += "<color=#969696>" + split_key_text[i] + "</color>\n";
                    }
                }

                select_count[1] = select_count[0];
            }

            //エンターキーを押したら
            if (Input.GetKeyDown(KeyCode.Return) && select_count[0] < 4)
            {
                audio_source.PlayOneShot(SE[1]);

                Panel.SetActive(true);
                Time.timeScale = 0;
            }

            if (select_count[0] == 4)
            {
                //タップ音の場所に来てる時
                //左
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    audio_source.PlayOneShot(SE[0]);

                    tap_sound--;

                    if (tap_sound < 0)
                    {
                        tap_sound = 3;
                    }

                    Save_Key_Configu();
                    ReLoad_Key_Configu();
                }

                //右
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    audio_source.PlayOneShot(SE[0]);

                    tap_sound++;

                    if (tap_sound > 3)
                    {
                        tap_sound = 0;
                    }

                    Save_Key_Configu();
                    ReLoad_Key_Configu();
                }

                //タップ音を鳴らす
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (tap_sound != 3)
                    {
                        audio_source.PlayOneShot(Tap_SE[tap_sound]);
                    }
                }
            }
            else if (select_count[0] == 5)
            {
                //スクロールスピードの場所に来てる時
                //左
                if (Input.GetKeyDown(KeyCode.LeftArrow) && scroll_speed > 1.0f)
                {
                    audio_source.PlayOneShot(SE[0]);

                    scroll_speed -= 0.1f;

                    if (scroll_speed < 1.0f)
                    {
                        scroll_speed = 1.0f;
                    }

                    Save_Key_Configu();
                    ReLoad_Key_Configu();
                }

                //右
                if (Input.GetKeyDown(KeyCode.RightArrow) && scroll_speed < 8.0f)
                {
                    audio_source.PlayOneShot(SE[0]);

                    scroll_speed += 0.1f;

                    if (scroll_speed > 8.0f)
                    {
                        scroll_speed = 8.0f;
                    }

                    Save_Key_Configu();
                    ReLoad_Key_Configu();
                }
            }

            //メニュー画面へ
            if (Input.GetKeyDown(KeyCode.Escape) && select == false && fade_controller.isFadeIn == false)
            {
                audio_source.PlayOneShot(SE[2]);

                fade_controller.isFadeOut = true;
                select = true;
            }

            //更新
            rt.localPosition = pos[0];
            key_rt.localPosition = pos[1];
        }
        else if (Time.timeScale == 0)
        {
            //時間停止してる時
            //ESCキーを押したら何も変更せず選択に戻る
            if (Input.GetKeyDown(KeyCode.Escape) && select == false)
            {
                Time.timeScale = 1;
                Panel.SetActive(false);
            }
            else if (Input.anyKeyDown == true)
            {
                //何かしらキー入力した時
                foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(code))
                    {
                        //キーを変更した
                        string str_copy = key_command[select_count[0]];
                        key_command[select_count[0]] = Input.inputString;

                        //既に割り当てられているキーかどうか
                        for (int i = 0; i < 4; i++)
                        {
                            //割り当てられてるやつだったら入れ替える
                            if (i != select_count[0] && key_command[select_count[0]] == key_command[i])
                            {
                                key_command[select_count[0]] = key_command[i];
                                key_command[i] = str_copy;
                            }
                        }

                        //セーブと反映
                        Save_Key_Configu();
                        ReLoad_Key_Configu();

                        Time.timeScale = 1;
                        Panel.SetActive(false);
                        break;
                    }
                }
            }
        }

        //シーン遷移
        if (select == true && fade_controller.isFadeOut == false)
        {
            SceneManager.LoadScene("Select");
        }
    }

    private void Save_Key_Configu()
    {
        StreamWriter writer;
        Key_Configu kc = new Key_Configu();

        //代入
        kc.Left = key_command[0];
        kc.Down = key_command[1];
        kc.Up = key_command[2];
        kc.Right = key_command[3];
        kc.tap_sound = tap_sound;
        kc.scroll_speed = scroll_speed;
        kc.master_volume = Master_Volume.volume;

        string json = JsonUtility.ToJson(kc, true);

        writer = new StreamWriter(Application.dataPath + "/Json/Setting.json", false);
        writer.WriteLine(json);
        writer.Flush();
        writer.Close();
    }

    //キーコンフィグ読み込み
    public static Key_Configu Load_Key_Configu()
    {
        StreamReader reader;

        string data = "";

        reader = new StreamReader(Application.dataPath + "/Json/Setting.json");
        data = reader.ReadToEnd();
        reader.Close();

        return JsonUtility.FromJson<Key_Configu>(data);
    }

    private void ReLoad_Key_Configu()
    {
        Key_Configu kc = Load_Key_Configu();

        //Jsonで読み込んだ値を代入
        key_command[0] = kc.Left;
        key_command[1] = kc.Down;
        key_command[2] = kc.Up;
        key_command[3] = kc.Right;
        scroll_speed = kc.scroll_speed;

        key_text.text = "";

        for (int i = 0; i < 4; i++)
        {
            key_text.text += key_command[i] + "\n";
        }

        key_text.text += "＜ " + tap_sound_name[tap_sound] + " ＞\n";
        key_text.text += "＜ " + scroll_speed.ToString("0.0") + " ＞";

        text_key_data = key_text.text;

        // 改行で分割して配列に代入
        split_key_text = text_key_data.Split(char.Parse("\n"));

        key_text.text = "";

        //テキストに表示
        for (int i = 0; i < 6; i++)
        {
            if (i == select_count[0])
            {
                key_text.text += "<color=#323232>" + split_key_text[i] + "</color>\n";
            }
            else
            {
                key_text.text += "<color=#969696>" + split_key_text[i] + "</color>\n";
            }
        }
    }
}
