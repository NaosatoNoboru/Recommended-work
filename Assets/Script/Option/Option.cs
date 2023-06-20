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
    public AudioSource audio_source;//SE�p
    public AudioClip[] SE = new AudioClip[3];//0=�I���@1=����@2=�L�����Z��
    public AudioClip[] Tap_SE = new AudioClip[3];//0=�^���o�����@1=�w�p�b�`���@2=�J�[�h�@3=�Ȃ�

    public FadeController fade_controller;
    public RectTransform rt;
    public RectTransform key_rt;
    public Text select_text;
    public Text key_text;
    public GameObject Panel;
    public GameObject demo_notes;

    public string[] tap_sound_name = new string[4];//�^�b�v���̖��O

    private string[] key_command = new string[4];//�L�[�R�}���h
    private string text_data;//�e�L�X�g�̓��e����p
    private string text_key_data;//�e�L�X�g�̓��e����p
    private string[] split_text;//�e�L�X�g�̓��e����p
    private string[] split_key_text;//�e�L�X�g�̓��e����p
    private float scroll_speed;//�v���C���[���I���������x
    private int tap_sound;//�^�b�v��
    private int[] select_count = new int[2];//���j���[�̐� 0=�ŐV�@1=�Â�
    private bool select;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        //Json�ǂݍ���
        {
            Key_Configu kc = Load_Key_Configu();

            //Json�œǂݍ��񂾒l����
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

        key_text.text += "�� " + tap_sound_name[tap_sound] + " ��\n";
        key_text.text += "�� " + scroll_speed.ToString("0.0") + " ��";

        text_data = select_text.text;
        text_key_data = key_text.text;

        // ���s�ŕ������Ĕz��ɑ��
        split_text = text_data.Split(char.Parse("\n"));
        split_key_text = text_key_data.Split(char.Parse("\n"));

        select_text.text = "";
        key_text.text = "";

        //�e�L�X�g�ɕ\��
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
        //���Ԓ�~���ĂȂ��Ƃ�
        if (Time.timeScale == 1)
        {
            Vector3[] pos = new Vector3[2];

            pos[0] = rt.localPosition;
            pos[1] = key_rt.localPosition;

            //��Ɉړ�
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                audio_source.PlayOneShot(SE[0]);

                select_count[0]--;

                //��ԏォ���ԉ���
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

            //���Ɉړ�
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                audio_source.PlayOneShot(SE[0]);

                select_count[0]++;

                //��ԉ������ԏ��
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

            //���ڂ�ς�����ύX
            if (select_count[0] != select_count[1])
            {
                //�m�[�c�������Ă���f���������邩�ǂ���
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

                //�e�L�X�g�ɕ\��
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

            //�G���^�[�L�[����������
            if (Input.GetKeyDown(KeyCode.Return) && select_count[0] < 4)
            {
                audio_source.PlayOneShot(SE[1]);

                Panel.SetActive(true);
                Time.timeScale = 0;
            }

            if (select_count[0] == 4)
            {
                //�^�b�v���̏ꏊ�ɗ��Ă鎞
                //��
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

                //�E
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

                //�^�b�v����炷
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
                //�X�N���[���X�s�[�h�̏ꏊ�ɗ��Ă鎞
                //��
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

                //�E
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

            //���j���[��ʂ�
            if (Input.GetKeyDown(KeyCode.Escape) && select == false && fade_controller.isFadeIn == false)
            {
                audio_source.PlayOneShot(SE[2]);

                fade_controller.isFadeOut = true;
                select = true;
            }

            //�X�V
            rt.localPosition = pos[0];
            key_rt.localPosition = pos[1];
        }
        else if (Time.timeScale == 0)
        {
            //���Ԓ�~���Ă鎞
            //ESC�L�[���������牽���ύX�����I���ɖ߂�
            if (Input.GetKeyDown(KeyCode.Escape) && select == false)
            {
                Time.timeScale = 1;
                Panel.SetActive(false);
            }
            else if (Input.anyKeyDown == true)
            {
                //��������L�[���͂�����
                foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(code))
                    {
                        //�L�[��ύX����
                        string str_copy = key_command[select_count[0]];
                        key_command[select_count[0]] = Input.inputString;

                        //���Ɋ��蓖�Ă��Ă���L�[���ǂ���
                        for (int i = 0; i < 4; i++)
                        {
                            //���蓖�Ă��Ă������������ւ���
                            if (i != select_count[0] && key_command[select_count[0]] == key_command[i])
                            {
                                key_command[select_count[0]] = key_command[i];
                                key_command[i] = str_copy;
                            }
                        }

                        //�Z�[�u�Ɣ��f
                        Save_Key_Configu();
                        ReLoad_Key_Configu();

                        Time.timeScale = 1;
                        Panel.SetActive(false);
                        break;
                    }
                }
            }
        }

        //�V�[���J��
        if (select == true && fade_controller.isFadeOut == false)
        {
            SceneManager.LoadScene("Select");
        }
    }

    private void Save_Key_Configu()
    {
        StreamWriter writer;
        Key_Configu kc = new Key_Configu();

        //���
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

    //�L�[�R���t�B�O�ǂݍ���
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

        //Json�œǂݍ��񂾒l����
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

        key_text.text += "�� " + tap_sound_name[tap_sound] + " ��\n";
        key_text.text += "�� " + scroll_speed.ToString("0.0") + " ��";

        text_key_data = key_text.text;

        // ���s�ŕ������Ĕz��ɑ��
        split_key_text = text_key_data.Split(char.Parse("\n"));

        key_text.text = "";

        //�e�L�X�g�ɕ\��
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
