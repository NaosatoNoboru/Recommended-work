using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;

[System.Serializable]
class Json_Data_Rapper
{
    //これで配列も書き込めるようにする
    public Json_Data json_data;
}

[System.Serializable]
public class Json_Data
{
    public string music_name;
    public int bpm;
    public string background;
    public int max_combo;
    public List<Notes> notes;
}

[System.Serializable]
public class Notes
{
    public int lane;
    public int section;
    public int step;
    public int type;
    public int gimmick;
    public int long_num_max;
    public float time;
}

public class Music_Json : MonoBehaviour
{
    public AudioSource audio_source;

    public FadeController fade_controller;
    public Music_Data music_data;

    public static List<Notes> notes_list = notes_list = new List<Notes>();

    public static bool input;//データロードをしているかどうか

    private AudioClip audio_clip;
    private static string file_name;
    private bool select;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        Json_Data JD = Load_Json();
        input = false;

        //編集したことのある譜面データかどうか　ないならスルー
        if (JD != null)
        {
            //BGMの入力
            audio_clip = Resources.Load("Song/" + file_name) as AudioClip;
            audio_source.clip = audio_clip;

            Music_Data.music_name = JD.music_name;
            Music_Data.bpm = JD.bpm;
            Music_Data.background = JD.background;
            //long_num = JD.long_num;

            for (int i = 0; i < JD.max_combo; i++)
            {
                notes_list.Add(JD.notes[i]);
            }

            if (audio_clip != null && JD.bpm != 0)
            {
                input = true;
            }
        }
        else
        {
            Music_Data.bpm = 0;
            Music_Data.background = null;

            input = false;
        }

        music_data.Load_Value();

        Save_Json();
        select = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (select == true && fade_controller.isFadeOut == false)
        {
            Save_Json();
            SceneManager.LoadScene("Sheet_Music");
        }
    }

    public static void Save_Json()
    {
        StreamWriter writer;

        Json_Data_Rapper JDR = new Json_Data_Rapper();
        JDR.json_data = new Json_Data();

        //notes_list.Sort((a, b) => a.time.CompareTo(b.time));

        //代入
        JDR.json_data.music_name = Music_Data.music_name;
        JDR.json_data.bpm = Music_Data.bpm;
        JDR.json_data.background = Music_Data.background;
        JDR.json_data.max_combo = notes_list.Count;
        JDR.json_data.notes = notes_list;

        string json = JsonUtility.ToJson(JDR.json_data, true);

        writer = new StreamWriter(Application.dataPath +
            "/Music/" + file_name + "/" + Music_Data.music_json + ".json", false);
        writer.WriteLine(json);
        writer.Flush();
        writer.Close();
    }

    public static Json_Data Load_Json()
    {
        StreamReader reader;

        string data = "";

        file_name = Music_Data.music_name;

        //初期化
        notes_list.Clear();

        reader = new StreamReader(Application.dataPath +
            "/Music/" + file_name + "/" + Music_Data.music_json + ".json");
        data = reader.ReadToEnd();
        reader.Close();

        return JsonUtility.FromJson<Json_Data>(data);
    }

    public void Reload()
    {
        fade_controller.isFadeOut = true;
        select = true;
    }
}