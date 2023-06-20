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
    public AudioClip[] SE = new AudioClip[3];//0=�I���@1=����@2=�L�����Z��
    public VideoPlayer Movie;
    public VideoClip[] Training = new VideoClip[17];//���K���ʂ̓���

    public FadeController fade_controller;
    public RectTransform rt;
    public Text music_list;//�Ȗ��\��

    public List<string> training_data_list = new List<string>();

    private Training_Select_Data TSD;

    private List<string> split_text = new List<string>();//�Ȗ��Ǘ�
    private float start_pos;                    //����y���W
    private int[] select_count = new int[2];//�Ȃ̐� 0=�ŐV�@1=�Â�
    private int select_scene;
    private bool select;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        //�f�[�^�̓ǂݍ���
        TSD = Load_Training_Data();

        for (int i = 0; i < TSD.data_count; i++)
        {
            training_data_list.Add(TSD.training_name[i]);
            split_text.Add(TSD.training_name[i]);
        }

        music_list.text = "";

        //�e�L�X�g�ɋȖ��\��
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

        //�����}��
        Movie.clip = Training[0];

        //���ڂ̐������傫���ƍ��W��ς���
        Vector3 pos = rt.localPosition;
        pos.y += (TSD.data_count - 1) * -40;
        rt.localPosition = pos;

        Vector2 size = rt.sizeDelta;
        size.y = 60 + ((TSD.data_count - 1) * 82.5f);
        rt.sizeDelta = size;

        //private�ϐ��̏�����
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

        //��Ɉړ�
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            audio_source.PlayOneShot(SE[0]);

            select_count[0]--;

            //��ԏォ���ԉ���
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

        //���Ɉړ�
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            audio_source.PlayOneShot(SE[0]);

            select_count[0]++;

            //��ԉ������ԏ��
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

        //�G���^�[�L�[����������Q�[����
        if (Input.GetKeyDown(KeyCode.Return) && select == false && fade_controller.isFadeIn == false)
        {
            audio_source.PlayOneShot(SE[1]);

            fade_controller.isFadeOut = true;
            select_scene = 0;
            select = true;
        }

        //�G�X�P�[�v�L�[����������Z���N�g��ʂ�
        if (Input.GetKeyDown(KeyCode.Escape) && select == false && fade_controller.isFadeIn == false)
        {
            audio_source.PlayOneShot(SE[2]);

            fade_controller.isFadeOut = true;
            select_scene = 1;
            select = true;
        }

        //�Ȃ�ς�����ύX
        if (select_count[0] != select_count[1])
        {
            Movie.clip = Training[select_count[0]];
            Movie.time = 0.0f;

            music_list.text = "";

            //�e�L�X�g�ɕ\��
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

        //�V�[���J��
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
