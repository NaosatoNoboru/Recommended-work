using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public class Music_Data : MonoBehaviour
{
    public InputField[] input_field = new InputField[4];
    public Text name_text;
    public Text difficulty_text;

    public static string music_name;
    public static string music_json;
    public static int bpm;
    public static string background;
    public static float rust_start;
    public static float rust_end;

    void Start()
    {

    }

    //public void Input_Music()
    //{
    //    music_name = input_field[0].text;
    //}

    //public void Input_Json()
    //{
    //    music_json = input_field.text;
    //}

    public void Input_Bpm()
    {
        bpm = int.Parse(input_field[0].text);
    }

    public void Input_Background()
    {
        background = input_field[1].text;
    }

    public void Input_Rust_Start()
    {
        rust_start = float.Parse(input_field[2].text);
        Save_Rust();
    }

    public void Input_Rust_End()
    {
        rust_end = float.Parse(input_field[3].text);
        Save_Rust();
    }

    public void Load_Value()
    {
        Music_Select_Data MSD = Music_Select.Load_Select_Data();

        for (int i = 0; i < MSD.data_count; i++)
        {
            //リストから同じ曲名を探す
            if (music_name == MSD.select_data[i].music_name)
            {
                //サビの時間を読み込む
                rust_start = MSD.select_data[i].rust[0];
                rust_end = MSD.select_data[i].rust[1];

                break;
            }
        }

        //input_field[0].text = music_name;
        input_field[0].text = bpm.ToString();
        input_field[1].text = background;
        input_field[2].text = rust_start.ToString();
        input_field[3].text = rust_end.ToString();

        name_text.text = music_name;

        //難易度の表示
        if (music_json.Contains("easy"))
        {
            difficulty_text.text = "<color=#00FF00>EASY</color>";
        }
        else if (music_json.Contains("hard"))
        {
            difficulty_text.text = "<color=#FF0000>HARD</color>";
        }
        else
        {
            difficulty_text.text = "<color=#FFFF00>NORMAL</color>";
        }
    }

    public static void Save_Rust()
    {
        StreamWriter writer;

        Music_Select_Data MSD = Music_Select.Load_Select_Data();

        //代入
        for (int i = 0; i < MSD.data_count; i++)
        {
            //リストから同じ曲名を探す
            if (music_name == MSD.select_data[i].music_name)
            {
                //サビの時間を書き込む
                MSD.select_data[i].rust[0] = rust_start;
                MSD.select_data[i].rust[1] = rust_end;
                break;
            }
        }

        string data = JsonUtility.ToJson(MSD, true);

        writer = new StreamWriter(Application.dataPath + "/Json/MusicList.json", false);
        writer.WriteLine(data);
        writer.Flush();
        writer.Close();
    }
}