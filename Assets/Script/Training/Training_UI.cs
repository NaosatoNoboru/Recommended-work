using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Training_UI : MonoBehaviour
{
    public FadeController fade_controller;

    public Text[] judg_text = new Text[4];

    private int[] judg = new int[4];//ノーツの判定
    private bool select;

    // Start is called before the first frame update
    void Start()
    {
        select = false;
    }

    // Update is called once per frame
    void Update()
    {
        //判定数標示
        for (int i = 0; i < 4; i++)
        {
            judg_text[i].text = Notes_Controller.judg[i].ToString();
        }

        //曲が終了したので編集画面へ
        if (Countdown.Music.isPlaying == false && Countdown.count_end == true && select == false)
        {
            fade_controller.isFadeOut = true;
            select = true;
        }

        //メニュー画面へ
        if (Input.GetKeyDown(KeyCode.Escape) && select == false && fade_controller.isFadeIn == false)
        {
            fade_controller.isFadeOut = true;
            select = true;
        }

        //シーン遷移
        if (select == true && fade_controller.isFadeOut == false)
        {
            SceneManager.LoadScene("Training_Select");
        }
    }
}
