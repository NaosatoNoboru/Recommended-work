using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Pause : MonoBehaviour
{
    public AudioSource audio_source;//SE�p
    public AudioClip[] SE = new AudioClip[3];//0=�I���@1=����@2=�L�����Z��
    public VideoPlayer video_player;

    public FadeController fade_controller;
    public Text menu_text;

    public GameObject pause_panel;
    public GameObject movie;

    private float time;//�Ȃ̎���
    private int select;
    private int old_select;
    private int click_num;
    private bool key;

    // Start is called before the first frame update
    void Start()
    {
        menu_text.text =
            "<color=#FFFFFF>�ĊJ</color>\n" +
            "<color=#AAAAAA>���Ȃ���</color>\n" +
            "<color=#AAAAAA>�Ȃ�I��</color>";

        pause_panel.SetActive(false);
        movie.SetActive(false);

        click_num = -1;
        time = 0.0f;
        key = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Countdown.count_end == true && Game_UI.hp > 0)
        {
            //�|�[�Y�̐؂�ւ�
            if (key == false)
            {
                Time.timeScale = 0;  //���Ԓ�~
                time = Countdown.Music.time;

                Countdown.Music.Pause();
                pause_panel.SetActive(true);

                key = true;
            }
            else
            {
                Countdown.Music.time = time;

                movie.SetActive(true);
                pause_panel.SetActive(false);

                key = false;
            }
        }

        //�J�E���g�_�E�����悪�I�������ĊJ
        if (video_player.clip.length == video_player.time)
        {
            Time.timeScale = 1;  //�ĊJ
            video_player.time = 0.0f;
            movie.SetActive(false);

            Countdown.Music.Play();
        }

        //�|�[�Y��ʂ̎�
        if (Time.timeScale == 0 && key == true)
        {
            //��Ɉړ�
            if (Input.GetKeyDown(KeyCode.UpArrow) && fade_controller.isFadeIn == false)
            {
                audio_source.PlayOneShot(SE[0]);
                select--;

                //��ԏ�Ȃ��ԉ��Ɉړ�
                if (select <= -1)
                {
                    select = 2;
                }
            }

            //���Ɉړ�
            if (Input.GetKeyDown(KeyCode.DownArrow) && fade_controller.isFadeIn == false)
            {
                audio_source.PlayOneShot(SE[0]);
                select++;

                //��ԉ��Ȃ��ԏ�Ɉړ�
                if (select >= 3)
                {
                    select = 0;
                }
            }

            //����
            if (Input.GetKeyDown(KeyCode.Return) && click_num == -1 && fade_controller.isFadeIn == false)
            {
                audio_source.PlayOneShot(SE[1]);

                switch (select)
                {
                    case 0:
                        Countdown.Music.time = time;
                        key = false;

                        pause_panel.SetActive(false);
                        movie.SetActive(true);
                        break;
                    case 1:
                        fade_controller.isFadeOut = true;
                        click_num = 0;
                        break;
                    case 2:
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
                        menu_text.text =
                            "<color=#FFFFFF>�ĊJ</color>\n" +         //������
                            "<color=#AAAAAA>���Ȃ���</color>\n" +
                            "<color=#AAAAAA>�Ȃ�I��</color>";
                        break;
                    case 1:
                        menu_text.text =
                            "<color=#AAAAAA>�ĊJ</color>\n" +
                            "<color=#FFFFFF>���Ȃ���</color>\n" +   //������
                            "<color=#AAAAAA>�Ȃ�I��</color>";
                        break;
                    case 2:
                        menu_text.text =
                            "<color=#AAAAAA>�ĊJ</color>\n" +
                            "<color=#AAAAAA>���Ȃ���</color>\n" +
                            "<color=#FFFFFF>�Ȃ�I��</color>";          //������
                        break;
                    default:
                        break;
                }

                //�Â��̂��X�V
                old_select = select;
            }

            //�V�[���J��
            if (fade_controller.isFadeOut == false)
            {
                switch (click_num)
                {
                    case 0:
                        Time.timeScale = 1;  //�ĊJ
                        SceneManager.LoadScene("Game");
                        break;
                    case 1:
                        Time.timeScale = 1;  //�ĊJ
                        SceneManager.LoadScene("Music_Select");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
