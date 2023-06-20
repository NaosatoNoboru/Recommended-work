using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyBoard_Operation : MonoBehaviour
{
    public AudioSource audio_source;
    public FadeController fade_controller;

    public Sheet_Music_UI sm_ui;
    public Lane_Controller lane_controller;
    public Mouse_Operation MO;

    public float start_time;//�Đ��J�n����
    public static int long_num;//�����O�m�[�c�𑝂₵����
    public static int gimmick_num;//�M�~�b�N�ԍ��@0=�ʏ�@1=�X�s�[�h�㏸�@2=�X�s�[�h�����@3=�����@4=��
    public int combo;//�R���{
    public int section;//�Z�N�V����
    public int step;//�X�e�b�v
    public bool reproduction;//���y�Đ��t���O

    private Json_Data JD;

    private bool finish;//���ʐ���I���t���O
    private bool button_select;//true = �ȑI����ʂ� false = ���ʊm�F��

    // Start is called before the first frame update
    void Start()
    {
        JD = Music_Json.Load_Json();

        combo = 0;
        long_num = 0;
        reproduction = false;
        finish = false;
        button_select = false;
    }

    // Update is called once per frame
    void Update()
    {
        //���ʏ�񂪂���Ȃ�
        if (Music_Json.input == true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (reproduction == true)
                {
                    //�Đ���~
                    audio_source.Pause();
                    reproduction = false;

                    //������
                    for (int i = 0; i < Music_Json.notes_list.Count; i++)
                    {
                        Mouse_Operation.se_flag[i] = false;
                    }
                }
                else
                {
                    //�Đ��J�n
                    start_time = Time.timeSinceLevelLoad;

                    audio_source.Play();
                    reproduction = true;
                }
            }
        }

        //ESC�L�[�������畈�ʐ���I��
        if (Input.GetKeyDown(KeyCode.Escape) && finish == false && fade_controller.isFadeIn == false)
        {
            fade_controller.isFadeOut = true;
            finish = true;
            button_select = true;
        }

        if (Input.GetKeyDown(KeyCode.Return) && finish == false && fade_controller.isFadeIn == false)
        {
            Music_Json.Save_Json();
            fade_controller.isFadeOut = true;
            finish = true;
            button_select = false;
        }

        //�ŐV�m�[�c���X�V����Ă���Ƃ�
        if (MO.recently == true && Music_Json.notes_list[Music_Json.notes_list.Count - 1].gimmick == 0)
        {
            //�����O�m�[�c�𑝂₷
            if (Input.GetKeyDown(KeyCode.E))
            {
                //�ŏ��̃����O�m�[�c���ǂ���
                if (long_num > 1)
                {
                    MO.notes_golist.Add(Instantiate(MO.Notes_Original));
                    Music_Json.notes_list.Add(MO.notes_ori);

                    //�����ۂƐV�����̂����ւ���
                    {
                        MO.notes_golist[Music_Json.notes_list.Count - 1].transform.position =
                            MO.notes_golist[Music_Json.notes_list.Count - 2].transform.position;

                        var end_notes = Music_Json.notes_list[Music_Json.notes_list.Count - 2];

                        Music_Json.notes_list[Music_Json.notes_list.Count - 1].lane = end_notes.lane;
                        Music_Json.notes_list[Music_Json.notes_list.Count - 1].section = end_notes.section;
                        Music_Json.notes_list[Music_Json.notes_list.Count - 1].step = end_notes.step;
                        Music_Json.notes_list[Music_Json.notes_list.Count - 1].time = end_notes.time;
                        Music_Json.notes_list[Music_Json.notes_list.Count - 1].type = end_notes.type;
                        Music_Json.notes_list[Music_Json.notes_list.Count - 1].long_num_max = 0;
                    }

                    step = Music_Json.notes_list[Music_Json.notes_list.Count - (2 + long_num)].step + long_num + 1;
                    section = (int)Mathf.Round(step / 16);

                    //�l����
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].lane = MO.old_lane_num;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].section = section;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].step = step;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].time =
                        ((15.0f / Music_Data.bpm) * (step + 1)) - ((15.0f / Music_Data.bpm) / 2);
                    Music_Json.notes_list[Music_Json.notes_list.Count - 2].type = 1;
                    Music_Json.notes_list[Music_Json.notes_list.Count - (2 + long_num)].long_num_max++;

                    //��̒l���ς�����Ⴄ���珉����
                    MO.notes_ori = new Notes();

                    //���W�ݒ�
                    Vector3 notes_obj =
                        MO.notes_golist[Music_Json.notes_list.Count - (2 + long_num)].transform.position;

                    notes_obj.x = MO.notes_golist[Music_Json.notes_list.Count - (2 + long_num)].transform.position.x;
                    notes_obj.y = MO.notes_golist[Music_Json.notes_list.Count - (2 + long_num)].transform.position.y +
                        (long_num * 2);
                    notes_obj.z = -1.9f + (long_num / 100);

                    MO.notes_golist[Music_Json.notes_list.Count - 1].transform.position = notes_obj;

                    Vector3 notes_obj_before =
                        MO.notes_golist[Music_Json.notes_list.Count - 1].transform.position;

                    notes_obj_before.y += 2;

                    MO.notes_golist[Music_Json.notes_list.Count - 1].transform.position = notes_obj_before;

                    //�m�[�c�̉摜�ݒ�
                    MO.notes_golist[Music_Json.notes_list.Count - 1].GetComponent<SpriteRenderer>().sprite =
                        MO.longnotes_end_collar[MO.old_lane_num];
                    MO.notes_golist[Music_Json.notes_list.Count - 2].GetComponent<SpriteRenderer>().sprite =
                        MO.longnotes_collar[MO.old_lane_num];

                    long_num++;
                }
                else
                {
                    //�I�u�W�F�N�g����
                    MO.notes_golist.Add(Instantiate(MO.Notes_Original));
                    Music_Json.notes_list.Add(MO.notes_ori);

                    //�����ۂ��Ō���Ɉړ�
                    if (long_num == 1)
                    {
                        MO.notes_golist[Music_Json.notes_list.Count - 1].transform.position =
                            MO.notes_golist[Music_Json.notes_list.Count - 2].transform.position;

                        var end_notes = Music_Json.notes_list[Music_Json.notes_list.Count - 2];

                        Music_Json.notes_list[Music_Json.notes_list.Count - 1].lane = end_notes.lane;
                        Music_Json.notes_list[Music_Json.notes_list.Count - 1].section = end_notes.section;
                        Music_Json.notes_list[Music_Json.notes_list.Count - 1].step = end_notes.step;
                        Music_Json.notes_list[Music_Json.notes_list.Count - 1].time = end_notes.time;
                        Music_Json.notes_list[Music_Json.notes_list.Count - 1].type = end_notes.type;
                        Music_Json.notes_list[Music_Json.notes_list.Count - long_num - 1].long_num_max = 0;
                    }

                    step = Music_Json.notes_list[Music_Json.notes_list.Count - 2].step + 1;
                    section = (int)Mathf.Round(step / 16);

                    //�l����
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].lane = MO.old_lane_num;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].section = section;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].step = step;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].time =
                        ((15.0f / Music_Data.bpm) * (step + 1)) - ((15.0f / Music_Data.bpm) / 2);
                    Music_Json.notes_list[Music_Json.notes_list.Count - long_num - 2].long_num_max++;

                    //��̒l���ς�����Ⴄ���珉����
                    MO.notes_ori = new Notes();

                    //���W�ݒ�
                    Vector3 notes_obj =
                        MO.notes_golist[Music_Json.notes_list.Count - 1].transform.position;
                    Vector3 notes_obj_ls =
                        MO.notes_golist[Music_Json.notes_list.Count - 1].transform.localScale;

                    notes_obj.x = MO.notes_golist[Music_Json.notes_list.Count - (2 + long_num)].transform.position.x;
                    notes_obj.y = MO.notes_golist[Music_Json.notes_list.Count - (2 + long_num)].transform.position.y +
                        (long_num * 2);
                    notes_obj.y += 1.5f;//������
                    notes_obj.z = -1.9f + (long_num / 100);

                    //0=�����ہ@else=�����ۂ̒����ƍ��{
                    if (long_num == 0)
                    {
                        Music_Json.notes_list[Music_Json.notes_list.Count - 1].type = 3;
                        notes_obj.y += 0.5f;
                        notes_obj_ls.y = 4;

                        //�m�[�c�̉摜�ݒ�
                        MO.notes_golist[Music_Json.notes_list.Count - 1].GetComponent<SpriteRenderer>().sprite
                            = MO.longnotes_end_collar[MO.old_lane_num];
                    }
                    else
                    {
                        //�����ۂ̍��W�ƃT�C�Y
                        Vector3 notes_obj_before =
                            MO.notes_golist[Music_Json.notes_list.Count - 2].transform.position;
                        Vector3 notes_obj_ls_before =
                            MO.notes_golist[Music_Json.notes_list.Count - 2].transform.localScale;

                        notes_obj.y += 0.5f;
                        notes_obj_before.y -= 0.5f;
                        notes_obj_ls_before.y = 3;

                        //�����ۂ̔��f
                        MO.notes_golist[Music_Json.notes_list.Count - 2].transform.position = notes_obj_before;
                        MO.notes_golist[Music_Json.notes_list.Count - 2].transform.localScale = notes_obj_ls_before;

                        Music_Json.notes_list[Music_Json.notes_list.Count - 2].type = 2;

                        notes_obj_ls.y = 2;

                        //�m�[�c�̉摜�ݒ�
                        MO.notes_golist[Music_Json.notes_list.Count - 2].GetComponent<SpriteRenderer>().sprite =
                            MO.longnotes_collar[MO.old_lane_num];
                        MO.notes_golist[Music_Json.notes_list.Count - 1].GetComponent<SpriteRenderer>().sprite =
                            MO.longnotes_end_collar[MO.old_lane_num];
                    }

                    MO.notes_golist[Music_Json.notes_list.Count - 1].transform.position = notes_obj;
                    MO.notes_golist[Music_Json.notes_list.Count - 1].transform.localScale = notes_obj_ls;

                    long_num++;
                }
            }

            //�����O�m�[�c�����炷
            if (Input.GetKeyDown(KeyCode.Q) && long_num != 0)
            {
                //�Ō�̃����O�m�[�c���ǂ���
                if (long_num <= 2)
                {
                    //�I�u�W�F�N�g�폜
                    Destroy(MO.notes_golist[Music_Json.notes_list.Count - 1]);
                    MO.notes_golist.RemoveAt(Music_Json.notes_list.Count - 1);
                    Music_Json.notes_list.RemoveAt(Music_Json.notes_list.Count - 1);
                    Music_Json.notes_list[Music_Json.notes_list.Count - long_num].long_num_max--;

                    //�����ۂ̒���
                    if (long_num == 2)
                    {
                        Music_Json.notes_list[Music_Json.notes_list.Count - 1].type = 3;

                        Vector3 notes_obj =
                            MO.notes_golist[Music_Json.notes_list.Count - 1].transform.position;
                        Vector3 notes_obj_ls =
                            MO.notes_golist[Music_Json.notes_list.Count - 1].transform.localScale;

                        notes_obj.y += 0.5f;
                        notes_obj_ls.y = 4;

                        MO.notes_golist[Music_Json.notes_list.Count - 1].transform.position = notes_obj;
                        MO.notes_golist[Music_Json.notes_list.Count - 1].transform.localScale = notes_obj_ls;

                        //�m�[�c�̉摜�ݒ�
                        MO.notes_golist[Music_Json.notes_list.Count - 1].GetComponent<SpriteRenderer>().sprite
                            = MO.longnotes_end_collar[MO.old_lane_num];
                    }

                    long_num--;
                }
                else
                {
                    //�I�u�W�F�N�g�폜
                    Destroy(MO.notes_golist[Music_Json.notes_list.Count - 1]);
                    MO.notes_golist.RemoveAt(Music_Json.notes_list.Count - 1);
                    Music_Json.notes_list.RemoveAt(Music_Json.notes_list.Count - 1);
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].type = 3;
                    Music_Json.notes_list[Music_Json.notes_list.Count - long_num].long_num_max--;

                    MO.notes_golist[Music_Json.notes_list.Count - 1].GetComponent<SpriteRenderer>().sprite =
                        MO.longnotes_end_collar[MO.old_lane_num];

                    long_num--;
                }
            }
        }

        //�Z�N�V�������J�E���g�_�E��
        if (Input.GetKeyDown(KeyCode.LeftArrow) && reproduction == false &&
            lane_controller.section_num - 1 >= 0)
        {
            lane_controller.section_num--;
            sm_ui.section--;

            Set_Position();
        }

        //�Z�N�V�������J�E���g�A�b�v
        if (Input.GetKeyDown(KeyCode.RightArrow) && reproduction == false &&
            lane_controller.section_num + 1 < lane_controller.section_max)
        {
            lane_controller.section_num++;
            sm_ui.section++;

            Set_Position();
        }

        //�Z�N�V������5�J�E���g�_�E��
        if (Input.GetKeyDown(KeyCode.DownArrow) && reproduction == false &&
            lane_controller.section_num - 5 >= 0)
        {
            lane_controller.section_num-=5;
            sm_ui.section-=5;

            Set_Position();
        }

        //�Z�N�V������5�J�E���g�A�b�v
        if (Input.GetKeyDown(KeyCode.UpArrow) && reproduction == false &&
            lane_controller.section_num + 5 < lane_controller.section_max)
        {
            lane_controller.section_num += 5;
            sm_ui.section += 5;

            Set_Position();
        }

        ////�m�[�c�^�C�v�I��
        //{
        //    //�ʏ�
        //    if (Input.GetKeyDown(KeyCode.Alpha1))
        //    {
        //        gimmick_num = 0;
        //    }

        //    //�X�s�[�h�A�b�v
        //    if (Input.GetKeyDown(KeyCode.Alpha2))
        //    {
        //        gimmick_num = 1;
        //    }

        //    //�X�s�[�h�_�E��
        //    if (Input.GetKeyDown(KeyCode.Alpha3))
        //    {
        //        gimmick_num = 2;
        //    }

        //    //����
        //    if (Input.GetKeyDown(KeyCode.Alpha4))
        //    {
        //        gimmick_num = 3;
        //    }

        //    //��
        //    if (Input.GetKeyDown(KeyCode.Alpha5))
        //    {
        //        gimmick_num = 4;
        //    }
        //}

        if (fade_controller.isFadeOut == false && finish == true)
        {
            if (button_select == true)
            {
                //�ȑI����ʂ�
                SceneManager.LoadScene("Music_Select");
            }
            else
            {
                //���ʊm�F��ʂ�
                SceneManager.LoadScene("Sheet_Music_Test");
            }
        }

        void Set_Position()
        {
            sm_ui.step = sm_ui.section * 16;
            sm_ui.audio_source.time = ((15.0f / Music_Data.bpm) * 16) * sm_ui.section;

            for (int i = 0; i < lane_controller.section_max; i++)
            {
                //�ʒu����
                Vector3 lane_pos = lane_controller.lane_list[i].transform.position;

                lane_pos.y = 16 + (16 * (i * 2));//�����l
                lane_pos.y -= 16 * (lane_controller.section_num * 2);//�ړ�

                lane_controller.lane_list[i].transform.position = lane_pos;

                //�Ō�Ƀt�B�j�b�V�����C�����ړ�
                if (i == lane_controller.section_max - 1)
                {
                    Vector3 line_pos = lane_controller.Finish_Line.transform.position;

                    line_pos.y = (16 * (i * 2));
                    line_pos.y += lane_controller.step_add * 2;//�����l
                    line_pos.y -= 16 * (lane_controller.section_num * 2);//�ړ�

                    lane_controller.Finish_Line.transform.position = line_pos;
                }
            }

            for (int i = 0; i < MO.notes_golist.Count; i++)
            {
                Vector3 notes_pos = MO.notes_golist[i].transform.position;

                //�ʏ�m�[�c���ۂ�
                switch (Music_Json.notes_list[i].type)
                {
                    case 0://�ʏ�m�[�c�Ȃ畁�ʂɐݒu
                        notes_pos.y = 1 + (2 * Music_Json.notes_list[i].step);
                        break;
                    case 1://�����O�m�[�c�����ʂɐݒu
                        notes_pos.y = -1 + (2 * (Music_Json.notes_list[i].step + 1));
                        break;
                    case 2://�����O�m�[�c�̍��{�Ȃ̂ŏ������炷
                        notes_pos.y = 0.5f + (2 * Music_Json.notes_list[i].step);
                        break;
                    case 3://������
                           //�������ǂ���
                        if (Music_Json.notes_list[i - 1].long_num_max >= 1)
                        {
                            //���{�Ȃ̂ł��炷
                            notes_pos.y = 1.0f + (2 * Music_Json.notes_list[i].step);
                        }
                        else
                        {
                            //�ʏ�z�u
                            notes_pos.y = -1 + (2 * (Music_Json.notes_list[i].step + 1));
                        }
                        break;
                    default:
                        break;
                }

                notes_pos.y -= 16 * (lane_controller.section_num * 2);//�ړ�

                MO.notes_golist[i].transform.position = notes_pos;
            }
        }
    }
}