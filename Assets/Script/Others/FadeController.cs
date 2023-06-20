


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class FadeController : MonoBehaviour
{
    public float fadeSpeed;        //α値変化スピード
    public float red, green, blue;   //パネルの色
    public bool isFadeOut = false;  //フェードアウトフラグ
    public bool isFadeIn = false;   //フェードインフラグ

    private Image fadeImage;        //パネルのイメージ

    private float alfa;
    private bool fadeFlage = false; //フェード開始・終了フラグ

    void Start()
    {
        //初期化
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

    //フェードイン
    public void FadeIn()
    {
        //フェード開始
        if (!fadeFlage)
        {
            fadeFlage = true;
            alfa = 1.0f;

            if (!fadeImage.enabled)
            {
                fadeImage.enabled = true;  //パネル表示オン
            }
        }
            
        SetAlpha();         //カラー変更をパネルに反映
        alfa -= fadeSpeed;  //α値減算
        
        //フェードイン完了
        if (alfa <= 0)
        {
            fadeFlage = false;
            isFadeIn = false;
            fadeImage.enabled = false;    //パネル表示オフ        
        }
    }

    //フェードアウト
    public void FadeOut()
    {
        //フェード開始
        if (fadeFlage == false)
        {
            fadeFlage = true;
            alfa = 0.0f;

            if (fadeImage.enabled == false)
            {
                fadeImage.enabled = true;  //パネル表示オン
            }
                
        }

        alfa += fadeSpeed;  //α値加算
        SetAlpha();         //カラー変更をパネルに反映

        //フェードアウト完了
        if (alfa >= 1)
        {
            fadeFlage = false;
            isFadeOut = false;
        }
    }

    //イメージのカラーセット
    void SetAlpha()
    {
        fadeImage.color = new Color(red, green, blue, alfa);
    }
}
