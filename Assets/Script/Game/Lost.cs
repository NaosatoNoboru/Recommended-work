using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lost : MonoBehaviour
{
    public AudioSource audio_source;//SE�p
    public AudioClip[] SE = new AudioClip[3];//0=�I���@1=����@2=�L�����Z��

    public FadeController fade_controller;
    public Text lost_text;

    private int select;
    private int old_select;
    private int click_num;
    private bool decision;

    // Start is called before the first frame update
    void Start()
    {
        lost_text.text =
            "<color=#FFFFFF>���g���C</color>\n" +
            "<color=#AAAAAA>�ȑI����</color>";

        click_num = -1;
        decision = false;
    }

    // Update is called once per frame
    void Update()
    {
        //��Ɉړ�
        if (Input.GetKeyDown(KeyCode.UpArrow) && fade_controller.isFadeIn == false)
        {
            audio_source.PlayOneShot(SE[0]);

            select--;

            //��ԏ�Ȃ��ԉ��Ɉړ�
            if (select <= -1)
            {
                select = 1;
            }
        }

        //���Ɉړ�
        if (Input.GetKeyDown(KeyCode.DownArrow) && fade_controller.isFadeIn == false)
        {
            audio_source.PlayOneShot(SE[0]);

            select++;

            //��ԉ��Ȃ��ԏ�Ɉړ�
            if (select >= 2)
            {
                select = 0;
            }
        }

        //����
        if (Input.GetKeyDown(KeyCode.Return) && click_num == -1 && fade_controller.isFadeIn == false)
        {
            audio_source.PlayOneShot(SE[1]);

            decision = true;

            switch (select)
            {
                case 0:
                    fade_controller.isFadeOut = true;
                    click_num = 0;
                    break;
                case 1:
                    fade_controller.isFadeOut = true;
                    click_num = 1;
                    break;
                default:
                    break;
            }
        }

        //�ړ������������X�V
        if (select != old_select)
        {
            switch (select)
            {
                case 0:
                    lost_text.text =
                        "<color=#FFFFFF>���g���C</color>\n" +   //������
                        "<color=#AAAAAA>�ȑI����</color>";
                    break;
                case 1:
                    lost_text.text =
                        "<color=#AAAAAA>���g���C</color>\n" +
                        "<color=#FFFFFF>�ȑI����</color>";      //������
                    break;
                default:
                    break;
            }

            //�Â��̂��X�V
            old_select = select;
        }

        //�V�[���J��
        if (decision ==true&& fade_controller.isFadeOut == false)
        {
            Time.timeScale = 1;  //�ĊJ

            switch (click_num)
            {
                case 0:
                    SceneManager.LoadScene("Game");
                    break;
                case 1:
                    SceneManager.LoadScene("Music_Select");
                    break;
                default:
                    break;
            }
        }
    }
}
