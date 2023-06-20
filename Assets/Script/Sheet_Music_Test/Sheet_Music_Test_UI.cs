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

    public int[] judg = new int[4];//�m�[�c�̔���

    private bool select;

    // Start is called before the first frame update
    void Start()
    {
        select = false;
    }

    // Update is called once per frame
    void Update()
    {
        hit_text.text = "Hit�F" + (Notes_Controller.judg[0] + Notes_Controller.judg[1] + Notes_Controller.judg[2]);
        miss_text.text = "Miss�F" + Notes_Controller.judg[3];

        //�Ȃ��I�������̂ŕҏW��ʂ�
        if (Countdown.Music.isPlaying == false && Countdown.count_end == true && select == false)
        {
            fade_controller.isFadeOut = true;
            select = true;
        }

        //���j���[��ʂ�
        if (Input.GetKeyDown(KeyCode.Escape) && select == false && fade_controller.isFadeIn == false)
        {
            fade_controller.isFadeOut = true;
            select = true;
        }

        //�V�[���J��
        if (select == true && fade_controller.isFadeOut == false)
        {
            //SceneManager.LoadScene("Sheet_Music");
            SceneManager.LoadScene("Sheet_Music");
        }
    }
}
