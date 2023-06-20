using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

[System.Serializable]
public class Music_Select_Data
{
    public int data_count;//�Ȃ̐�
    public List<Select_Data> select_data;
}

[System.Serializable]
public class Select_Data
{
    public string music_name;

    public float[] rust = new float[2];//�T�r 0=�T�r�n�߁@1=�T�r�I���
    public long[] score = new long[3];
    public string[] rank = new string[3];
}

public class Music_Select : MonoBehaviour
{
    public AudioSource[] audio_source = new AudioSource[2];//0=�Ȃ̃T�r�����p 1=SE�p
    public AudioClip[] SE = new AudioClip[3];//0=�I���@1=����@2=�L�����Z��

    public FadeController fade_controller;
    public RectTransform rt;
    public GameObject Pause;
    public Text music_list;//�Ȗ��\��
    public Text difficulty;//��Փx�\��
    public Text score;     //�X�R�A�\��
    public Text rank;      //�����N�\��
    public Text auto;      //�����N�\��
    public Text delete_music_name;//�|�[�Y�ł̋Ȗ��\��
    public Text delete_selection;//�m�F

    public List<Select_Data> select_data_list = new List<Select_Data>();

    public static bool auto_flag = false;//false=�I�[�gOFF true=�I�[�gONN

    private Music_Select_Data MSD;

    private List<string> split_text = new List<string>();//�Ȗ��Ǘ�
    private float h;
    private float hs;
    private float start_pos;                    //����y���W
    private int[] difficulty_count = new int[2];//0=���̓�Փx�@1=�Â���Փx [0=Easy 1=Normal 2=Hard]
    private int[] select_count = new int[2];//�Ȃ̐� 0=�ŐV�@1=�Â�
    private int pause_select;//�͂��E�������̑I��
    private int select_scene;
    private bool select;
    private bool pause_flag;//true=�|�[�Y��

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        //�ȃf�[�^�̓ǂݍ���
        MSD = Load_Select_Data();

        for (int i = 0; i < MSD.data_count; i++)
        {
            select_data_list.Add(MSD.select_data[i]);
            split_text.Add(MSD.select_data[i].music_name);
        }

        music_list.text = "";
        score.text = "BestScore�F" + select_data_list[0].score[1].ToString("0000000");

        //�e�L�X�g�ɋȖ��\��
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

        difficulty.text = "�� " + "<color=#FFFF00>NORMAL</color>" + " ��";
        rank.text = MSD.select_data[0].rank[1];

        //�����N�̕\��
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
            //�X�R�A������Ƃ����������N�\��
            if (MSD.select_data[0].score[1] != 0)
            {
                rank.text = Result_UI.Rank_str(MSD.select_data[select_count[0]].score[difficulty_count[0]]);
            }
            else
            {
                rank.text = "";
            }
        }

        //�I�[�g���ǂ���
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

        //�T�r����
        Load_Rust();

        Pause.SetActive(false);

        //���ڂ̐������傫���ƍ��W��ς���
        Vector3 pos = rt.localPosition;
        pos.y += (MSD.data_count - 1) * -40;
        rt.localPosition = pos;

        Vector2 size = rt.sizeDelta;
        size.y = 60 + ((MSD.data_count - 1) * 82.5f);
        rt.sizeDelta = size;

        //private�ϐ��̏�����
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
        //�|�[�Y�����ǂ���
        if (pause_flag == false)
        {
            Vector3 pos = rt.localPosition;

            //��Ɉړ�
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                audio_source[1].PlayOneShot(SE[0]);

                select_count[0]--;

                //��ԏォ���ԉ���
                if (select_count[0] < 0)
                {
                    select_count[0] = MSD.data_count - 1;
                    pos.y = (MSD.data_count * -40) + ((MSD.data_count - 1) * 80);
                }
                else
                {
                    pos.y -= 80;
                }

                //�T�r�����̍X�V
                Load_Rust();
            }

            //���Ɉړ�
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                audio_source[1].PlayOneShot(SE[0]);

                select_count[0]++;

                //��ԉ������ԏ��
                if (select_count[0] >= MSD.data_count)
                {
                    select_count[0] = 0;
                    pos.y = MSD.data_count * -40;
                }
                else
                {
                    pos.y += 80;
                }

                //�T�r�����̍X�V
                Load_Rust();
            }

            //�E�Ɉړ�
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                audio_source[1].PlayOneShot(SE[0]);

                difficulty_count[0]++;

                if (difficulty_count[0] > 2)
                {
                    difficulty_count[0] = 0;
                }
            }

            //���Ɉړ�
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                audio_source[1].PlayOneShot(SE[0]);

                difficulty_count[0]--;

                if (difficulty_count[0] < 0)
                {
                    difficulty_count[0] = 2;
                }
            }

            //�����N�\��
            if (MSD.select_data[select_count[0]].rank[difficulty_count[0]] == "PFC")
            {
                rank.text = MSD.select_data[select_count[0]].rank[difficulty_count[0]];

                //PFC�Ȃ���F�ɂ���
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
                //�X�R�A������Ƃ����������N�\��
                if (MSD.select_data[select_count[0]].score[difficulty_count[0]] != 0)
                {
                    rank.text = Result_UI.Rank_str(MSD.select_data[select_count[0]].score[difficulty_count[0]]);
                }
                else
                {
                    rank.text = "";
                }
            }

            //�|�[�Y
            if (Input.GetKeyDown(KeyCode.D))
            {
                Pause.SetActive(true);

                delete_music_name.text = split_text[select_count[0]] + "���폜���܂����H";
                delete_selection.text = "<color=#AAAAAA>�͂��@�@�@</color>" + "<color=#FFFFFF>������</color>";
                pause_flag = true;
            }

            //�I�[�g���[�h�̐؂�ւ�
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

            //�G���^�[�L�[����������Q�[����
            if (Input.GetKeyDown(KeyCode.Return) && select == false && fade_controller.isFadeIn == false)
            {
                audio_source[1].PlayOneShot(SE[1]);

                fade_controller.isFadeOut = true;
                select_scene = 0;
                select = true;
            }

            //�X�y�[�X�L�[���������畈�ʕҏW��
            if (Input.GetKeyDown(KeyCode.Space) && select == false && fade_controller.isFadeIn == false)
            {
                audio_source[1].PlayOneShot(SE[1]);

                fade_controller.isFadeOut = true;
                select_scene = 1;
                select = true;
            }

            //�G�X�P�[�v�L�[����������Z���N�g��ʂ�
            if (Input.GetKeyDown(KeyCode.Escape) && select == false && fade_controller.isFadeIn == false)
            {
                audio_source[1].PlayOneShot(SE[2]);

                fade_controller.isFadeOut = true;
                select_scene = 2;
                select = true;
            }

            //�Ȃ�ς�����ύX
            if (select_count[0] != select_count[1])
            {
                music_list.text = "";

                //�X�R�A���e�L�X�g�ɕ\��
                score.text = "BestScore�F" +
                    select_data_list[select_count[0]].score[difficulty_count[0]].ToString("0000000");

                //�e�L�X�g�ɕ\��
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

            //��Փx��ς�����ύX
            if (difficulty_count[0] != difficulty_count[1])
            {
                score.text = "BestScore�F" +
                    select_data_list[select_count[0]].score[difficulty_count[0]].ToString("0000000");

                //�e�L�X�g�ɕ\��
                switch (difficulty_count[0])
                {
                    case 0:
                        difficulty.text = "�� " + "<color=#00FF00>EASY</color>" + " ��";
                        break;
                    case 1:
                        difficulty.text = "�� " + "<color=#FFFF00>NORMAL</color>" + " ��";
                        break;
                    case 2:
                        difficulty.text = "�� " + "<color=#FF0000>HARD</color>" + " ��";
                        break;
                    default:
                        break;
                }

                //�X�V
                difficulty_count[1] = difficulty_count[0];
            }

            rt.localPosition = pos;

            //�V�[���J��
            if (select == true && fade_controller.isFadeOut == false)
            {
                Music_Data.music_name = split_text[select_count[0]];
                Music_Data.music_json = split_text[select_count[0]];

                //Easy or Hard�������炳��ɏ�������
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
                        //���ʃf�[�^�����݂���Ȃ�Game��
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
            //�|�[�Y��
            //�E�Ɉړ�
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                audio_source[1].PlayOneShot(SE[0]);

                pause_select++;

                if (pause_select > 1)
                {
                    pause_select = 0;
                }
            }

            //���Ɉړ�
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                audio_source[1].PlayOneShot(SE[0]);

                pause_select--;

                if (pause_select < 0)
                {
                    pause_select = 1;
                }
            }

            //�I���̕\��
            if (pause_select == 0)
            {
                delete_selection.text = "<color=#FFFFFF>�͂��@�@�@</color>" + "<color=#AAAAAA>������</color>";
            }
            else
            {
                delete_selection.text = "<color=#AAAAAA>�͂��@�@�@</color>" + "<color=#FFFFFF>������</color>";
            }

            //�G���^�[�L�[����������
            if (Input.GetKeyDown(KeyCode.Return) && select == false && fade_controller.isFadeIn == false)
            {
                audio_source[1].PlayOneShot(SE[1]);

                if (pause_select == 0)
                {
                    //�����[�h
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

            //�G�X�P�[�v�L�[������������
            if (Input.GetKeyDown(KeyCode.Escape) && select == false && fade_controller.isFadeIn == false)
            {
                audio_source[1].PlayOneShot(SE[1]);
                pause_flag = false;
                Pause.SetActive(false);
            }

            //�����V�[�����ēǂݍ���
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
        //�Ȃ����݂��āA�T�r�̏I�����Ԃ�0.0�b�ł͂Ȃ��Ƃ�
        if (Resources.Load("Song/" + MSD.select_data[select_count[0]].music_name) as AudioClip == true &&
            MSD.select_data[select_count[0]].rust[1] > 0.0f)
        {
            audio_source[0].clip = Resources.Load("Song/" + MSD.select_data[select_count[0]].music_name) as AudioClip;

            //�Ȃ̊J�n���Ԃ�0.0�b�ȉ��Ȃ�0.0�b�ɖ߂�
            if (MSD.select_data[select_count[0]].rust[0] < 0.0f)
            {
                MSD.select_data[select_count[0]].rust[0] = 0.0f;
               Music_Data.Save_Rust();
            }

            //�Ȃ����݂��āA�Ȃ̒��������T�r�I���̎��Ԃ̕����ł����Ȃ�߂�
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
            //�Ȃ��Ȃ����Ȃ�
            audio_source[0].clip = null;
        }
    }

    public void Delete_Music()
    {
        //���ʃf�[�^�̃t�@�C�����폜
        Directory.Delete(Application.dataPath + "/Music/" + select_data_list[select_count[0]].music_name, true);

        //�w�肵���Ȃ̍��ڂ��폜
        MSD.select_data.RemoveAt(select_count[0]);

        MSD.data_count = MSD.select_data.Count;

        //�ȃ��X�g�ɒǉ�
        StreamWriter writer;
        Select_Data select_data = new Select_Data();

        string json = JsonUtility.ToJson(MSD, true);

        writer = new StreamWriter(Application.dataPath + "/Json/MusicList.json", false);
        writer.WriteLine(json);
        writer.Flush();
        writer.Close();
    }

    //���ʂ����݂��邩�ǂ���
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
