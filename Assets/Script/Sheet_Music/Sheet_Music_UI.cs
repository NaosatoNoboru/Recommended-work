using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sheet_Music_UI : MonoBehaviour
{
    public AudioSource SE;
    public AudioClip[] Tap_SE = new AudioClip[3];//0=�^���o�����@1=�w�p�b�`���@2=�J�[�h�@3=�Ȃ�

    public AudioSource audio_source;
    public KeyBoard_Operation KBO;
    public Mouse_Operation MO;
    public Lane_Controller lane_controller;

    public GameObject[] Notes_type = new GameObject[5];

    public Text timer_text;
    public Text section_text;
    public Text step_text;

    public float music_time;   //�Đ�����
    public int section;    //������Z�N�V����
    public int step;       //������X�e�b�v

    private float audio_time;
    private float sp_collar;//�摜�̐F�ϗp
    private int old_section;//�Â��Z�N�V����
    private int old_gimmick_num;//�Â��M�~�b�N�m�[�c�ԍ�
    private int section_max;//�Z�N�V�����̍ő�l
    private bool fast;
    private bool music_flag;//�Đ����Ԃ̓���
    private bool collar_return;//0.5�܂ŉ����������ǂ���

    // Start is called before the first frame update
    void Start()
    {
        section_text.text = "Section�F0/0";
        step_text.text = "Step�F0000";

        section = 0;
        audio_time = 0.0f;
        old_section = section;
        fast = false;
    }

    // Update is called once per frame
    void Update()
    {
        //�ŏ���������
        if (fast == false && Music_Json.input == true)
        {
            section_max = (int)(audio_source.clip.length / ((15.0f / Music_Data.bpm) * 16));

            old_gimmick_num = KeyBoard_Operation.gimmick_num;
            fast = true;
        }

        //�}�E�X�z�C�[�����񂵂��玞�Ԃ𓮂���
        if (MO.mouse_scroll != 0.0f && KBO.reproduction == false)
        {
            audio_time = (15.0f / Music_Data.bpm) * step;

            //-�ɂȂ�Ȃ��悤�ɂ���
            if (audio_time <= 0.0f)
            {
                music_time = 0.0f;
            }

            if (audio_time <= 0.0f)
            {
                audio_time = 0.0f;
            }

            audio_source.time = audio_time;
        }

        //�Z�N�V�����̌v�Z
        {
            section = (int)Mathf.Round(step / 16);

            //�Z�N�V�������ς������
            if (section != old_section)
            {
                lane_controller.section_num = section;
                old_section = section;
            }
        }

        //�Đ��t���O��true�ɂȂ���
        if (KBO.reproduction == true)
        {
            if (music_flag == false)
            {
                //�̂ōĐ����ԂƓ���
                //music_time = 0.0f;
                music_flag = true;
            }

            //music_nowtime = audio_source.time;
            music_time += Time.deltaTime;

            //�X�e�b�v���̌v�Z
            float step_calculation = 0.0f;

            step_calculation = (15.0f / Music_Data.bpm);

            //�J�E���g�A�b�v
            //if (music_time >= step_calculation)
            //{
            //    step++;
            //}

            while (music_time >= step_calculation)
            {
                step++;

                music_time -= step_calculation;
            }

            //����炵�ĂȂ����T��
            for (int i = 0; i < Music_Json.notes_list.Count; i++)
            {
                //�^�C�~���O��������炷
                if (Mathf.Abs(Music_Json.notes_list[i].time - audio_source.time) <= 0.01f * 5.0f &&
                   Music_Json.notes_list[i].type == 0 &&
                   Mouse_Operation.se_flag[i] == false)
                {
                    var KC= Option.Load_Key_Configu();

                    SE.PlayOneShot(Tap_SE[KC.tap_sound]);
                    Mouse_Operation.se_flag[i] = true;
                    break;
                }
            }
        }
        else
        {
            music_flag = false;
        }

        //�I�����Ă���M�~�b�N�m�[�c���ς������
        if (old_gimmick_num != KeyBoard_Operation.gimmick_num)
        {
            for (int i = 0; i < 5; i++)
            {
                //�F���Z�b�g
                Notes_type[i].GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }

        //�I�����Ă���m�[�c�^�C�v�������\��
        {
            if (collar_return == false)
            {
                sp_collar -= 0.005f;

                if (sp_collar <= 0.5f)
                {
                    collar_return = true;
                }
            }
            else
            {
                sp_collar += 0.005f;

                if (sp_collar >= 1.0f)
                {
                    collar_return = false;
                }
            }

            //�F�X�V
            Notes_type[KeyBoard_Operation.gimmick_num].
                GetComponent<SpriteRenderer>().color = new Color(sp_collar, sp_collar, sp_collar, 1.0f);
        }


        //�Đ����Ԃ��Ȃ̎��Ԃ𒴂�����~�߂�
        if (Music_Json.input == true)
        {
            if (KBO.reproduction == true && audio_source.isPlaying == false)
            {
                audio_source.time = 0.0f;
                audio_source.Pause();

                KBO.reproduction = false;
                step = 0;
                section = 0;

                //������
                for (int i = 0; i < lane_controller.lane_list.Count; i++)
                {
                    //�ʒu����
                    Vector3 lane_pos = lane_controller.lane_list[i].transform.position;

                    lane_pos.x = -12;
                    lane_pos.y = 16 + (16 * (i * 2));

                    lane_controller.lane_list[i].transform.position = lane_pos;

                    //������Z�N�V���������̑���
                    if (i != lane_controller.section_num)
                    {
                        //���̑�
                        lane_controller.lane_list[i].GetComponent<MeshRenderer>().material = lane_controller.material[1];
                    }
                    else
                    {
                        //������
                        lane_controller.lane_list[i].GetComponent<MeshRenderer>().material = lane_controller.material[0];
                    }

                    //�Ō�Ƀt�B�j�b�V�����C����u��
                    if (i == lane_controller.lane_list.Count - 1)
                    {
                        Vector3 line_intpos = lane_controller.Finish_Line.transform.position;

                        line_intpos.y = (16 * (i * 2));
                        line_intpos.y += lane_controller.step_add * 2;

                        lane_controller.Finish_Line.transform.position = line_intpos;
                    }
                }

                //������
                for (int i = 0; i < MO.notes_golist.Count; i++)
                {
                    //���W�ݒ�
                    Vector3 notes_obj = MO.notes_golist[i].transform.position;

                    //�ʏ�m�[�c���ۂ�
                    switch (Music_Json.notes_list[i].type)
                    {
                        case 0://�ʏ�m�[�c�Ȃ畁�ʂɐݒu
                            notes_obj.y = 1 + (2 * Music_Json.notes_list[i].step);
                            break;
                        case 1://�����O�m�[�c�����ʂɐݒu
                            notes_obj.y = -1 + (2 * Music_Json.notes_list[i].step);
                            break;
                        case 2://�����O�m�[�c�̍��{�Ȃ̂ŏ������炷
                            notes_obj.y = (2 * Music_Json.notes_list[i].step);
                            break;
                        case 3://������
                            //�������ǂ���
                            if (Music_Json.notes_list[i - 1].long_num_max >= 1)
                            {
                                //���{�Ȃ̂ł��炷
                                notes_obj.y = 1.0f + (2 * Music_Json.notes_list[i].step);
                            }
                            else
                            {
                                //�ʏ�z�u
                                notes_obj.y = -1 + (2 * (Music_Json.notes_list[i].step + 1));
                            }
                            break;
                        default:
                            break;
                    }

                    MO.notes_golist[i].transform.position = notes_obj;
                }

                //������
                for (int i = 0; i < Music_Json.notes_list.Count; i++)
                {
                    Mouse_Operation.se_flag[i] = false;
                }
            }

            //�e�L�X�g�ɔ��f
            {
                //�Đ�����
                timer_text.text = audio_source.time.ToString("000.00") + "/"
                + audio_source.clip.length.ToString("000.00");

                //�Z�N�V����
                section_text.text = "Section:" + section.ToString("000") + "/" + section_max;

                //�X�e�b�v
                step_text.text = "Step:" + step.ToString("0000");
            }
        }
    }
}