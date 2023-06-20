using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Sheet_Music_Test_UI : MonoBehaviour
{
    public FadeController fade_controller;

    public Text hit_text;
    public Text miss_text;

    public int[] judg = new int[4];//ノーツの判定

    private bool select;

    // Start is called before the first frame update
    void Start()
    {
        select = false;
    }

    // Update is called once per frame
    void Update()
    {
        hit_text.text = "Hit：" + (Notes_Controller.judg[0] + Notes_Controller.judg[1] + Notes_Controller.judg[2]);
        miss_text.text = "Miss：" + Notes_Controller.judg[3];

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
            //SceneManager.LoadScene("Sheet_Music");
            SceneManager.LoadScene("Sheet_Music");
        }
    }
}
