using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lost : MonoBehaviour
{
    public AudioSource audio_source;//SE用
    public AudioClip[] SE = new AudioClip[3];//0=選択　1=決定　2=キャンセル

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
            "<color=#FFFFFF>リトライ</color>\n" +
            "<color=#AAAAAA>曲選択へ</color>";

        click_num = -1;
        decision = false;
    }

    // Update is called once per frame
    void Update()
    {
        //上に移動
        if (Input.GetKeyDown(KeyCode.UpArrow) && fade_controller.isFadeIn == false)
        {
            audio_source.PlayOneShot(SE[0]);

            select--;

            //一番上なら一番下に移動
            if (select <= -1)
            {
                select = 1;
            }
        }

        //下に移動
        if (Input.GetKeyDown(KeyCode.DownArrow) && fade_controller.isFadeIn == false)
        {
            audio_source.PlayOneShot(SE[0]);

            select++;

            //一番下なら一番上に移動
            if (select >= 2)
            {
                select = 0;
            }
        }

        //決定
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

        //移動した時だけ更新
        if (select != old_select)
        {
            switch (select)
            {
                case 0:
                    lost_text.text =
                        "<color=#FFFFFF>リトライ</color>\n" +   //今ここ
                        "<color=#AAAAAA>曲選択へ</color>";
                    break;
                case 1:
                    lost_text.text =
                        "<color=#AAAAAA>リトライ</color>\n" +
                        "<color=#FFFFFF>曲選択へ</color>";      //今ここ
                    break;
                default:
                    break;
            }

            //古いのを更新
            old_select = select;
        }

        //シーン遷移
        if (decision ==true&& fade_controller.isFadeOut == false)
        {
            Time.timeScale = 1;  //再開

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
