using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Select : MonoBehaviour
{
    public AudioSource audio_source;//SE�p
    public AudioClip[] SE = new AudioClip[3];//0=�I���@1=����@2=�L�����Z��

    public FadeController fade_controller;
    public RectTransform rt;
    public InputField input_field;
    public GameObject BGM;
    public GameObject music_add;
    public Text select_text;

    private string text_data;//�e�L�X�g�̓��e����p
    private string[] split_text;//�e�L�X�g�̓��e����p
    private string add_name;
    private int[] select_count = new int[2];//���j���[�̐� 0=�ŐV�@1=�Â�
    private bool select;

    private static GameObject save_BGM;
    private static bool existence = false;//���݂��Ă��邩�ǂ���

    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        music_add.SetActive(false);

        text_data = select_text.text;

        // ���s�ŕ������Ĕz��ɑ��
        split_text = text_data.Split(char.Parse("\n"));

        select_text.text = "";

        //�e�L�X�g�ɕ\��
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

        //��������ĂȂ��Ȃ琶������
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
        //���Ԓ�~���ĂȂ��Ƃ�
        if (Time.timeScale == 1)
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
                    select_count[0] = 4;
                    pos.y = select_count[0] * 40;
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

            //���ڂ�ς�����ύX
            if (select_count[0] != select_count[1])
            {
                select_text.text = "";

                //�e�L�X�g�ɕ\��
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

            //�G���^�[�L�[����������
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
                    Time.timeScale = 0;//���Ԓ�~
                    music_add.SetActive(true);
                }
            }

            //�X�V
            rt.localPosition = pos;
        }
        else if (Time.timeScale == 0)
        {
            //���Ԓ�~���Ă鎞
            //ESC�L�[����������I���ɖ߂�
            if (Input.GetKeyDown(KeyCode.Escape) && select == false)
            {
                audio_source.PlayOneShot(SE[2]);

                Time.timeScale = 1;
                input_field.text = "";
                music_add.SetActive(false);
            }
        }

        //�V�[���J��
        if (select == true && fade_controller.isFadeOut == false)
        {
            switch (select_count[0])
            {
                case 0:
                    //BGM������
                    Destroy(save_BGM);
                    existence = false;

                    SceneManager.LoadScene("Music_Select");
                    break;
                case 1:
                    //BGM������
                    Destroy(save_BGM);
                    existence = false;

                    SceneManager.LoadScene("Sheet_Music");
                    break;
                case 2:
                    //BGM������
                    Destroy(save_BGM);
                    existence = false;

                    SceneManager.LoadScene("Training_Select");
                    break;
                case 3:
                    //BGM���c��
                    DontDestroyOnLoad(BGM);

                    SceneManager.LoadScene("Option"); 
                    break;
                case 4:
                    Application.Quit();//�Q�[���I��
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
        //�t�H���_�[�̍쐬
        Directory.CreateDirectory(Application.dataPath + "/Music/" + add_name);

        //JSON�t�@�C���̍쐬
        File.Create(Application.dataPath + "/Music/" + add_name + "/" + add_name + "_easy.json");
        File.Create(Application.dataPath + "/Music/" + add_name + "/" + add_name + ".json");
        File.Create(Application.dataPath + "/Music/" + add_name + "/" + add_name + "_hard.json");

        //�ȃ��X�g�ɒǉ�
        StreamWriter writer;
        Music_Select_Data MSD = Music_Select.Load_Select_Data();
        Select_Data select_data = new Select_Data();

        //���
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
