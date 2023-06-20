using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Result_UI : MonoBehaviour
{
    public FadeController fade_controller;

    public GameObject HP_Gage;

    public Text[] decision_text = new Text[4];
    public Text music_name_text;
    public Text difficulty_text;
    public Text score_text;
    public Text max_combo_text;
    public Text rank_text;
    public Text hp_text;

    private Json_Data JD;

    private static string rank;
    private float h;
    private float hs;
    private int select;
    private bool select_flag;//�I���������ǂ���

    // Start is called before the first frame update
    void Start()
    {        
		Application.targetFrameRate = 60;

        JD = Music_Json.Load_Json();

        //���萔�̕\��
        for (int i = 0; i < 4; i++)
        {
            decision_text[i].text = Notes_Controller.judg[i].ToString();
        }

        music_name_text.text = JD.music_name;//�Ȗ�
        score_text.text = Game_UI.score_num.ToString("0000000");//�X�R�A
        max_combo_text.text = Game_UI.max_combo.ToString();

        //��Փx�̕\��
        if (Music_Data.music_json.Contains("easy"))
        {
            difficulty_text.text = "<color=#00FF00>EASY</color>";
        }
        else if (Music_Data.music_json.Contains("hard"))
        {
            difficulty_text.text = "<color=#FF0000>HARD</color>";
        }
        else
        {
            difficulty_text.text = "<color=#FFFF00>NORMAL</color>";
        }

        //�����N
        if (Notes_Controller.judg[0] == JD.max_combo)
        {
            //ALL PERFECT
            rank_text.text = "PFC";
            rank = "PFC";
        }
        else if (Notes_Controller.judg[3] == 0)
        {
            //�t���R���{
            rank_text.text = "<color=#98BBDB>FC</color>";
            rank = "FC";
        }
        else
        {
            //�X�R�A�ł̃����N����
            rank_text.text = Rank_str(Game_UI.score_num);
        }

        //�I�[�g����Ȃ���
        if (Music_Select.auto_flag == false)
        {
            //�X�R�A�ƃ����N��ۑ�����
            Save_Score();
        }

        //HP�֘A
        {
            //�Q�[�W�̑傫���ƍ��W��ς���
            var gage_pos = HP_Gage.transform.localPosition;
            var gage_ls = HP_Gage.transform.localScale;

            gage_pos.y = -3 + (Game_UI.hp * 0.03f);
            gage_ls.y = 1 * (Game_UI.hp / 100.0f);

            HP_Gage.transform.localPosition = gage_pos;
            HP_Gage.transform.localScale = gage_ls;

            //�c��HP�\��
            hp_text.text = Game_UI.hp.ToString("000") + "%";
        }

        h = 0.0f;
        hs = 0.005f;
        select = 0;
        select_flag = false;
    }

    // Update is called once per frame
    void Update()
    {
        //���g���C
        if (Input.GetKeyDown(KeyCode.R) && select_flag == false)
        {
            fade_controller.isFadeOut = true;
            select = 0;
            select_flag = true;
        }

        //�ȑI����
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape)) && select_flag == false)
        {
            fade_controller.isFadeOut = true;
            select = 1;
            select_flag = true;
        }

        if (select_flag == true && fade_controller.isFadeOut == false)
        {
            if (select == 0)
            {
                //���g���C
                SceneManager.LoadScene("Game");
            }
            else
            {
                //�ȑI����
                SceneManager.LoadScene("Music_Select");
            }
        }

        //PFC�Ȃ���F�ɂ���
        if (Notes_Controller.judg[0] == JD.max_combo)
        {
            rank_text.color = Color.HSVToRGB(h % 1, 1, 1);

            h += hs;

            if (h > 360.0f || h < 0.0f)
            {
                hs *= -1;
            }
        }
    }
    public static void Save_Score()
    {
        StreamWriter writer;

        Music_Select_Data MSD = Music_Select.Load_Select_Data();

        //���
        for (int i = 0; i < MSD.data_count; i++)
        {
            //���X�g���瓯���Ȗ���T��
            if (Music_Data.music_name == MSD.select_data[i].music_name)
            {
                //��Փx
                if (Music_Data.music_json.Contains("easy"))
                {
                    //�x�X�g�X�R�A����Ȃ�X�V
                    if (MSD.select_data[i].score[0] < Game_UI.score_num)
                    {
                        MSD.select_data[i].score[0] = Game_UI.score_num;
                        MSD.select_data[i].rank[0] = rank;
                    }
                }
                else if (Music_Data.music_json.Contains("hard"))
                {
                    //�x�X�g�X�R�A����Ȃ�X�V
                    if (MSD.select_data[i].score[2] < Game_UI.score_num)
                    {
                        MSD.select_data[i].score[2] = Game_UI.score_num;
                        MSD.select_data[i].rank[2] = rank;
                    }
                }
                else
                {
                    //�x�X�g�X�R�A����Ȃ�X�V
                    if (MSD.select_data[i].score[1] < Game_UI.score_num)
                    {
                        MSD.select_data[i].score[1] = Game_UI.score_num;
                        MSD.select_data[i].rank[1] = rank;
                    }
                }
            }
        }

        string data = JsonUtility.ToJson(MSD, true);

        writer = new StreamWriter(Application.dataPath + "/Json/MusicList.json", false);
        writer.WriteLine(data);
        writer.Flush();
        writer.Close();
    }


    public static string Rank_str(long score)
    {
        string score_rank;

        if (score >= 900000.0f)
        {
            //S
            score_rank = "<color=#FFD700>S</color>";
            rank = "S";
        }
        else if (score >= 800000.0f)
        {
            //A
            score_rank = "<color=#C0C0C0>A</color>";
            rank = "A";
        }
        else if (score >= 700000.0f)
        {
            //B
            score_rank = "<color=#C47222>B</color>";
            rank = "B";
        }
        else if (score >= 600000.0f)
        {
            //C
            score_rank = "<color=#FF0000>C</color>";
            rank = "C";
        }
        else
        {
            //D
            score_rank = "<color=#000A91>D</color>";
            rank = "D";
        }

        return score_rank;
    }
}
