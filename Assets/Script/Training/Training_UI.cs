using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Training_UI : MonoBehaviour
{
    public FadeController fade_controller;

    public Text[] judg_text = new Text[4];

    private int[] judg = new int[4];//�m�[�c�̔���
    private bool select;

    // Start is called before the first frame update
    void Start()
    {
        select = false;
    }

    // Update is called once per frame
    void Update()
    {
        //���萔�W��
        for (int i = 0; i < 4; i++)
        {
            judg_text[i].text = Notes_Controller.judg[i].ToString();
        }

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
            SceneManager.LoadScene("Training_Select");
        }
    }
}
