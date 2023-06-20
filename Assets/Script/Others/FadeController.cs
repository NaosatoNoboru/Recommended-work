


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class FadeController : MonoBehaviour
{
    public float fadeSpeed;        //���l�ω��X�s�[�h
    public float red, green, blue;   //�p�l���̐F
    public bool isFadeOut = false;  //�t�F�[�h�A�E�g�t���O
    public bool isFadeIn = false;   //�t�F�[�h�C���t���O

    private Image fadeImage;        //�p�l���̃C���[�W

    private float alfa;
    private bool fadeFlage = false; //�t�F�[�h�J�n�E�I���t���O

    void Start()
    {
        //������
        fadeImage = GetComponent<Image>();
        alfa = fadeImage.color.a;

        fadeImage.color = new Color(red, green, blue, alfa);
    }

    void Update()
    {
        if (isFadeIn==true)
        {
            FadeIn();
        }

        if (isFadeOut == true)
        {
            FadeOut();
        }
    }

    //�t�F�[�h�C��
    public void FadeIn()
    {
        //�t�F�[�h�J�n
        if (!fadeFlage)
        {
            fadeFlage = true;
            alfa = 1.0f;

            if (!fadeImage.enabled)
            {
                fadeImage.enabled = true;  //�p�l���\���I��
            }
        }
            
        SetAlpha();         //�J���[�ύX���p�l���ɔ��f
        alfa -= fadeSpeed;  //���l���Z
        
        //�t�F�[�h�C������
        if (alfa <= 0)
        {
            fadeFlage = false;
            isFadeIn = false;
            fadeImage.enabled = false;    //�p�l���\���I�t        
        }
    }

    //�t�F�[�h�A�E�g
    public void FadeOut()
    {
        //�t�F�[�h�J�n
        if (fadeFlage == false)
        {
            fadeFlage = true;
            alfa = 0.0f;

            if (fadeImage.enabled == false)
            {
                fadeImage.enabled = true;  //�p�l���\���I��
            }
                
        }

        alfa += fadeSpeed;  //���l���Z
        SetAlpha();         //�J���[�ύX���p�l���ɔ��f

        //�t�F�[�h�A�E�g����
        if (alfa >= 1)
        {
            fadeFlage = false;
            isFadeOut = false;
        }
    }

    //�C���[�W�̃J���[�Z�b�g
    void SetAlpha()
    {
        fadeImage.color = new Color(red, green, blue, alfa);
    }
}
