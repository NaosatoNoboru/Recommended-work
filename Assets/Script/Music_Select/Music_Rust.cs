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
        //�T�r�����I�������0.5�b�ҋ@
        if (end_flag == true)
        {
            count += Time.deltaTime;

            //�҂����̂ł܂��Đ�
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

        //�T�r�̎��Ԃ��I��肩�����特�ʂ��t�F�[�h�A�E�g
        if (audio_source.time + 1.0f >= rust_end)
        {
            if (audio_source.volume > 0.0f)
            {
                audio_source.volume -= Time.deltaTime;
            }
        }

        //�T�r���I�������߂�
        if (audio_source.time >= rust_end)
        {
            audio_source.time = rust_start;
            end_flag = true;
        }
    }
}
