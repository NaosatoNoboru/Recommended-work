using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_Notes : MonoBehaviour
{
    public AudioSource audio_source;
    public AudioClip[] Tap_SE = new AudioClip[3];//0=タンバリン　1=指パッチン　2=カード　3=なし

    public GameObject notes;//ノーツ
    public GameObject particle_original;//生成元のパーティクル

    private Json_Data JD;
    private Key_Configu KC;

    private GameObject particle;//パーティクル

    private float speed;//移動速度
    private float ms;//判定数
    private bool reset_flag;//resetしたかどうか

    // Start is called before the first frame update
    void Start()
    {
        ms = 1.0f / 1000;
    }

    // Update is called once per frame
    void Update()
    {
        KC = Option.Load_Key_Configu();
        speed = 140 / 60.0f * (8 * KC.scroll_speed);

        //判定
        if (notes.transform.position.y <= -3.75f)
        {
            //音無し以外だったら鳴らす
            if (KC.tap_sound != 3)
            {
                audio_source.PlayOneShot(Tap_SE[KC.tap_sound]);
            }

            //パーティクルを出す
            {
                particle = Instantiate(particle_original);
                particle.transform.position = notes.transform.position;
            }

            reset_flag = true;
        }

        Vector3 notes_pos = notes.transform.position;

        //リセットするかどうか
        if (reset_flag == true)
        {
            notes_pos.y = 25.0f;
            reset_flag = false;
        }
        else
        {
            notes_pos.y -= speed * Time.deltaTime;
        }

        notes.transform.position = notes_pos;
    }
}
