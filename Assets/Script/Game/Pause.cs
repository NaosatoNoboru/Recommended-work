using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Pause : MonoBehaviour
{
    public AudioSource audio_source;//SE用
    public AudioClip[] SE = new AudioClip[3];//0=選択　1=決定　2=キャンセル
    public VideoPlayer video_player;

    public FadeController fade_controller;
    public Text menu_text;

    public GameObject pause_panel;
    public GameObject movie;

    private float time;//曲の時間
    private int select;
    private int old_select;
    private int click_num;
    private bool key;

    // Start is called before the first frame update
    void Start()
    {
        menu_text.text =
            "<color=#FFFFFF>再開</color>\n" +
            "<color=#AAAAAA>やりなおす</color>\n" +
            "<color=#AAAAAA>曲を選ぶ</color>";

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
            //ポーズの切り替え
            if (key == false)
            {
                Time.timeScale = 0;  //時間停止
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

        //カウントダウン動画が終わったら再開
        if (video_player.clip.length == video_player.time)
        {
            Time.timeScale = 1;  //再開
            video_player.time = 0.0f;
            movie.SetActive(false);

            Countdown.Music.Play();
        }

        //ポーズ画面の時
        if (Time.timeScale == 0 && key == true)
        {
            //上に移動
            if (Input.GetKeyDown(KeyCode.UpArrow) && fade_controller.isFadeIn == false)
            {
                audio_source.PlayOneShot(SE[0]);
                select--;

                //一番上なら一番下に移動
                if (select <= -1)
                {
                    select = 2;
                }
            }

            //下に移動
            if (Input.GetKeyDown(KeyCode.DownArrow) && fade_controller.isFadeIn == false)
            {
                audio_source.PlayOneShot(SE[0]);
                select++;

                //一番下なら一番上に移動
                if (select >= 3)
                {
                    select = 0;
                }
            }

            //決定
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

            //移動した時だけ更新
            if (select != old_select)
            {
                switch (select)
                {
                    case 0:
                        menu_text.text =
                            "<color=#FFFFFF>再開</color>\n" +         //今ここ
                            "<color=#AAAAAA>やりなおす</color>\n" +
                            "<color=#AAAAAA>曲を選ぶ</color>";
                        break;
                    case 1:
                        menu_text.text =
                            "<color=#AAAAAA>再開</color>\n" +
                            "<color=#FFFFFF>やりなおす</color>\n" +   //今ここ
                            "<color=#AAAAAA>曲を選ぶ</color>";
                        break;
                    case 2:
                        menu_text.text =
                            "<color=#AAAAAA>再開</color>\n" +
                            "<color=#AAAAAA>やりなおす</color>\n" +
                            "<color=#FFFFFF>曲を選ぶ</color>";          //今ここ
                        break;
                    default:
                        break;
                }

                //古いのを更新
                old_select = select;
            }

            //シーン遷移
            if (fade_controller.isFadeOut == false)
            {
                switch (click_num)
                {
                    case 0:
                        Time.timeScale = 1;  //再開
                        SceneManager.LoadScene("Game");
                        break;
                    case 1:
                        Time.timeScale = 1;  //再開
                        SceneManager.LoadScene("Music_Select");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
