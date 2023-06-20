using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    public static AudioSource Music;//曲 

    public AudioSource audio_source;//SE
    public AudioClip[] SE = new AudioClip[4];//0=3 1=2 2=1 3=GO(カウントダウン) 

    public FadeController fade_controller;
    public Sprite[] countdown = new Sprite[3];//ノーツの色

    public static bool count_end;

    private AudioClip Song;

    private string file_name;//曲の名前
    private string count_str;//四捨五入用
    private float count;//カウントダウン

    // Start is called before the first frame update
    void Start()
    {
        file_name = Music_Data.music_name;

        //BGMの入力
        Song = Resources.Load("Song/" + file_name) as AudioClip;
        Music = GetComponent<AudioSource>();
        Music.clip = Song;

        count = 0;
        count_end = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //カウントダウンが終わってないなら
        if (count_end == false && fade_controller.isFadeIn == false)
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            count += Time.deltaTime;
            count_str = count.ToString("0.00");
            count = float.Parse(count_str);

            if (count <= 0.02f)
            {
                //3
                audio_source.PlayOneShot(SE[0]);
            }
            else if (count == 0.36f)
            {
                //2
                audio_source.PlayOneShot(SE[1]);
                this.gameObject.GetComponent<SpriteRenderer>().sprite = countdown[0];

            }
            else if (count == 0.72f)
            {
                //1
                audio_source.PlayOneShot(SE[2]);
                this.gameObject.GetComponent<SpriteRenderer>().sprite = countdown[1];
            }
            else if (count >= 1.08f)
            {
                //GO
                audio_source.PlayOneShot(SE[3]);
                Music.Play();
                this.gameObject.GetComponent<SpriteRenderer>().sprite = countdown[2];

                count = 0.0f;
                count_end = true;
            }
        }
        else
        {
            Color color = this.gameObject.GetComponent<SpriteRenderer>().color;
            color.a -= 0.05f;
            this.gameObject.GetComponent<SpriteRenderer>().color = color;

            if (color.a <= 0.0f && audio_source.isPlaying == false)
            {
                color.a = 0.0f;
                //this.gameObject.SetActive(false);
            }
        }
    }
}
