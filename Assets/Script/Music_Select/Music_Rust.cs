using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music_Rust : MonoBehaviour
{
    public AudioSource audio_source;

    public static float rust_start;
    public static float rust_end;

    private float count;
    private bool end_flag;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        end_flag = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //サビ流し終わったら0.5秒待機
        if (end_flag == true)
        {
            count += Time.deltaTime;

            //待ったのでまた再生
            if (count >= 0.5f)
            {
                end_flag = false;
                count = 0.0f;
            }
        }

        if (audio_source.time <= rust_start + 1.0f)
        {
            if (audio_source.volume < 1.0f)
            {
                audio_source.volume += Time.deltaTime;
            }
        }

        //サビの時間が終わりかけたら音量をフェードアウト
        if (audio_source.time + 1.0f >= rust_end)
        {
            if (audio_source.volume > 0.0f)
            {
                audio_source.volume -= Time.deltaTime;
            }
        }

        //サビが終わったら戻す
        if (audio_source.time >= rust_end)
        {
            audio_source.time = rust_start;
            end_flag = true;
        }
    }
}
