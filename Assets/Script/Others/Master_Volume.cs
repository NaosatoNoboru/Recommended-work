using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class Master_Volume : MonoBehaviour
{
    public Sprite[] volume_sprite = new Sprite[5];
    public Text volume_text;
    public Image icon;

    public static float volume;//�}�X�^�[�{�����[��

    private Key_Configu kc;

    private float count;
    private float alpha;
    private float old_volume;
    private float silence_volume;//�����ɂ���O�̉��ʕۑ�
    private bool silence;
    private bool change_volume;

    // Start is called before the first frame update
    void Start()
    {
        kc = Option.Load_Key_Configu();

        volume = kc.master_volume;
        AudioListener.volume = volume;

        count = 0;
        alpha = 0.0f;
        old_volume = volume;
        silence = false;
        change_volume = false;

        volume_text.color = new Color(0.0f, 0.0f, 0.0f, alpha);
        icon.color = new Color(1.0f, 1.0f, 1.0f, alpha);
    }

    // Update is called once per frame
    void Update()
    {
        //�����������牟���Ȃ�
        if (silence == false)
        {
            //���ʂ�������
            if (Input.GetKeyDown(KeyCode.F1))
            {
                volume -= 0.25f;

                if (volume < 0.0f)
                {
                    volume = 0.0f;
                }

                AudioListener.volume = volume;
                Save_Volume();
            }

            //���ʂ��グ��
            if (Input.GetKeyDown(KeyCode.F2))
            {
                volume += 0.25f;

                if (volume > 1.0f)
                {
                    volume = 1.0f;
                }

                AudioListener.volume = volume;
                Save_Volume();
            }
        }

        //����
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (silence == false)
            {
                //��������
                silence_volume = volume;
                volume = 0.0f;
                silence = true;
            }
            else
            {
                //����߂�
                volume = silence_volume;

                silence = false;
            }

            AudioListener.volume = volume;
            Save_Volume();
        }

        //���ʂ��ς������
        if (old_volume != volume)
        {
            this.GetComponent<Image>().sprite = volume_sprite[(int)(volume * 4)];

            old_volume = volume;
            alpha = 1.0f;
            change_volume = true;
        }

        //���ʕύX������alpha��ύX���ăt�F�[�h�A�E�g������
        if (change_volume == true)
        {
            if (alpha <= 0.8f)
            {
                alpha -= Time.deltaTime;
            }
            else
            {
                alpha -= Time.deltaTime / 5;
            }

            //�I���
            if (alpha < 0.0f)
            {
                alpha = 0.0f;
                change_volume = false;
            }

            volume_text.color = new Color(0.0f, 0.0f, 0.0f, alpha);
            icon.color = new Color(1.0f, 1.0f, 1.0f, alpha);
        }
    }

    private void Save_Volume()
    {
        StreamWriter writer;

        //���
        kc.master_volume = volume;

        string json = JsonUtility.ToJson(kc, true);

        writer = new StreamWriter(Application.dataPath + "/Json/Setting.json", false);
        writer.WriteLine(json);
        writer.Flush();
        writer.Close();
    }
}
