using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_Notes : MonoBehaviour
{
    public AudioSource audio_source;
    public AudioClip[] Tap_SE = new AudioClip[3];//0=�^���o�����@1=�w�p�b�`���@2=�J�[�h�@3=�Ȃ�

    public GameObject notes;//�m�[�c
    public GameObject particle_original;//�������̃p�[�e�B�N��

    private Json_Data JD;
    private Key_Configu KC;

    private GameObject particle;//�p�[�e�B�N��

    private float speed;//�ړ����x
    private float ms;//���萔
    private bool reset_flag;//reset�������ǂ���

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

        //����
        if (notes.transform.position.y <= -3.75f)
        {
            //�������ȊO��������炷
            if (KC.tap_sound != 3)
            {
                audio_source.PlayOneShot(Tap_SE[KC.tap_sound]);
            }

            //�p�[�e�B�N�����o��
            {
                particle = Instantiate(particle_original);
                particle.transform.position = notes.transform.position;
            }

            reset_flag = true;
        }

        Vector3 notes_pos = notes.transform.position;

        //���Z�b�g���邩�ǂ���
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
