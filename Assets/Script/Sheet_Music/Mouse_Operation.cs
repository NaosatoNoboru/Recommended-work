using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Mouse_Operation : MonoBehaviour
{
    public Lane_Controller lane_controller;
    public KeyBoard_Operation KBO;
    public Sheet_Music_UI sm_ui;

    public GameObject lane_cursor;
    public GameObject Notes_Original;
    public List<GameObject> notes_golist = new List<GameObject>();//�m�[�c�Ǘ����X�g
    public Notes notes_ori = new Notes();//���X�g�ǉ��p

    public Sprite[] notes_collar = new Sprite[4];//�m�[�c�̐F
    public Sprite[] gimmick_notes_collar = new Sprite[4 * 4];//�M�~�b�N�m�[�c�̐F
    public Sprite[] longnotes_collar = new Sprite[4];//�����O�m�[�c�̐F
    public Sprite[] longnotes_end_collar = new Sprite[4];//�����O�m�[�c�����ۂ̐F

    public int lane_num;//���[���ԍ�
    public int old_lane_num;//�Â����[���ԍ�
    public Vector3 mouse_pos;//�}�E�X���W
    public Vector3 target;
    public float mouse_scroll;//�X�N���[���������l
    public bool recently;//�m�[�c���������ꂽ���ǂ���
    public static List<bool> se_flag = new List<bool>();//�m�[�c�������ǂ���

    private Json_Data JD;

    private List<Vector3> lane_tp = new List<Vector3>();

    private float[] lpy = new float[2];//0=�}�C�i�X�𐮐��ɂ������W 1=�ۂ߂�O�̍��W
    private float sp_collar;//�摜�̐F�ϗp
    private int section;
    private int step;
    private bool collar_return;//0.5�܂ŉ����������ǂ���
    private bool existing_notes;//�����̃m�[�c��u��
    private bool stop;

    // Start is called before the first frame update
    void Start()
    {
        JD = Music_Json.Load_Json();

        mouse_scroll = 0.0f;
        sp_collar = 1.0f;
        section = 0;
        step = 0;
        recently = false;
        collar_return = false;
        stop = false;
    }

    //Update is called once per frame
    void Update()
    {
        if (Music_Json.input == true)
        {
            if (existing_notes == false)
            {
                //�����̃m�[�c��ݒu����
                for (int i = 0, j = 1; i < JD.max_combo; i++)
                {
                    notes_golist.Add(Instantiate(Notes_Original));

                    //��̒l���ς�����Ⴄ���珉����
                    notes_ori = new Notes();

                    //���W�ݒ�
                    Vector3 notes_obj = notes_golist[i].transform.position;
                    Vector3 notes_obj_ls = notes_golist[i].transform.localScale;

                    notes_obj.x = -15 + (2 * Music_Json.notes_list[i].lane);

                    //�ʏ�m�[�c���ۂ�
                    switch (Music_Json.notes_list[i].type)
                    {
                        case 0://�ʏ�m�[�c�Ȃ畁�ʂɐݒu
                            notes_obj.y = 1 + (2 * Music_Json.notes_list[i].step);
                            notes_obj.z = -2.0f;

                            //�M�~�b�N�m�[�c���ǂ���
                            if (Music_Json.notes_list[i].gimmick != 0)
                            {
                                //�M�~�b�N�m�[�c�Ȃ̂Ő�p�̂ɂ���
                                //�m�[�c�̉摜�ݒ�
                                notes_golist[i].GetComponent<SpriteRenderer>().sprite =
                                    gimmick_notes_collar[Music_Json.notes_list[i].lane +
                                    ((Music_Json.notes_list[i].gimmick - 1) * 4)];
                            }
                            else
                            {
                                //�m�[�c�̉摜�ݒ�
                                notes_golist[i].GetComponent<SpriteRenderer>().sprite
                                    = notes_collar[Music_Json.notes_list[i].lane];
                            }
                            break;
                        case 1://�����O�m�[�c�����ʂɐݒu
                            notes_obj.y = -1 + (2 * (Music_Json.notes_list[i].step + 1));

                            notes_obj.z = -2.0f + (j / 10000.0f);
                            j++;

                            //�m�[�c�̉摜�ݒ�
                            notes_golist[i].GetComponent<SpriteRenderer>().sprite
                                = longnotes_collar[Music_Json.notes_list[i].lane];
                            break;
                        case 2://�����O�m�[�c�̍��{�Ȃ̂ŏ������炷
                            notes_obj.y = 0.5f + (2 * Music_Json.notes_list[i].step);
                            notes_obj.z = -2.0f + (j / 10000.0f);

                            notes_obj_ls.y = 3;
                            j++;

                            //�m�[�c�̉摜�ݒ�
                            notes_golist[i].GetComponent<SpriteRenderer>().sprite
                                = longnotes_collar[Music_Json.notes_list[i].lane];
                            break;
                        case 3://������
                            //�������ǂ���
                            if (Music_Json.notes_list[i - 1].long_num_max >= 1)
                            {
                                //���{�Ȃ̂ł��炷
                                notes_obj.y = 1.0f + (2 * Music_Json.notes_list[i].step);
                                notes_obj.z = -2.0f + (j / 10000.0f);

                                notes_obj_ls.y = 4;
                                j++;
                            }
                            else
                            {
                                //�ʏ�z�u
                                notes_obj.y = -1 + (2 * (Music_Json.notes_list[i].step + 1));

                                notes_obj.z = -2.0f + (j / 10000.0f);
                                j++;
                            }

                            //�m�[�c�̉摜�ݒ�
                            notes_golist[i].GetComponent<SpriteRenderer>().sprite
                                = longnotes_end_collar[Music_Json.notes_list[i].lane];
                            break;
                        default:
                            break;
                    }

                    notes_golist[i].transform.position = notes_obj;
                    notes_golist[i].transform.localScale = notes_obj_ls;

                    se_flag.Add(Music_Json.input);
                    se_flag[i] = false;
                }

                existing_notes = true;
            }

            //�}�E�X�J�[�\���̍��W�ƃ��[���h���W�ϊ�
            mouse_pos = Input.mousePosition;
            target = Camera.main.ScreenToWorldPoint(mouse_pos);
            target.x += 0.5f;
            target.y += 0.5f;
            mouse_scroll = 0.0f;

            section = lane_controller.section_num;

            for (int i = 0; i < lane_controller.section_max; i++)
            {
                lane_tp.Add(lane_controller.lane_list[i].transform.position);
            }

            mouse_scroll = Input.GetAxisRaw("Mouse ScrollWheel") * 10;

            //�X�e�b�v���𒴂��Ĉړ��������Ă���߂�
            if (sm_ui.step < 0)
            {
                mouse_scroll = sm_ui.step * -1;
            }
            else if (sm_ui.step > lane_controller.step_max)
            {
                mouse_scroll = (sm_ui.step - lane_controller.step_max) * -1;
            }

            //�}�E�X�z�C�[������]�������瓮����
            if (mouse_scroll != 0.0f && KBO.reproduction == false)
            {
                //���W�ړ�
                {
                    float pos = lane_tp[0].y;

                    if (pos < 0.0f)
                    {
                        pos *= -1;
                    }

                    while (pos >= 2)
                    {
                        pos -= 2;
                    }

                    //���ɃX�N���[�����Ă�����
                    if (mouse_scroll < 0.0f && sm_ui.step != 0)
                    {
                        sm_ui.step += (int)mouse_scroll;

                        //���r���[�Ȉʒu�ɂ������猳�ɖ߂�
                        if (pos % 2 == 1)
                        {
                            sm_ui.step -= (int)mouse_scroll;
                        }
                    }
                    else if (mouse_scroll > 0.0f)
                    {
                        sm_ui.step += (int)mouse_scroll;
                    }

                    //���[���̈ړ�
                    for (int i = 0; i < lane_controller.section_max; i++)
                    {
                        //�ʒu����
                        Vector3 lane_pos = lane_controller.lane_list[i].transform.position;

                        lane_pos.y = 16 + (16 * (i * 2));
                        lane_pos.y -= 2 * sm_ui.step;

                        lane_tp[i] = lane_pos;

                        lane_controller.lane_list[i].transform.position = lane_pos;


                        //�Ō�Ƀt�B�j�b�V�����C����u��
                        if (i == lane_controller.section_max - 1)
                        {
                            Vector3 line_intpos = lane_controller.Finish_Line.transform.position;

                            line_intpos.y = (16 * (i * 2));
                            line_intpos.y += lane_controller.step_add * 2;
                            line_intpos.y -= 2 * sm_ui.step;

                            lane_controller.Finish_Line.transform.position = line_intpos;
                        }
                    }

                    //�m�[�c�̏�����
                    for (int i = 0; i < notes_golist.Count; i++)
                    {
                        //���W�ݒ�
                        Vector3 notes_obj = notes_golist[i].transform.position;

                        //�ʏ�m�[�c���ۂ�
                        switch (Music_Json.notes_list[i].type)
                        {
                            case 0://�ʏ�m�[�c�Ȃ畁�ʂɐݒu
                                notes_obj.y = 1 + (2 * Music_Json.notes_list[i].step);
                                break;
                            case 1://�����O�m�[�c�����ʂɐݒu
                                notes_obj.y = -1 + (2 * (Music_Json.notes_list[i].step + 1));
                                break;
                            case 2://�����O�m�[�c�̍��{�Ȃ̂ŏ������炷
                                notes_obj.y = 0.5f + (2 * Music_Json.notes_list[i].step);
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

                        notes_obj.y -= 2 * sm_ui.step;
                        notes_golist[i].transform.position = notes_obj;
                    }
                }
            }

            //�}�E�X�J�[�\���̃��[���h���W����ǂ̂ǂ��ɂ��邩
            if ((target.x >= -16.0f && target.x <= -8.0f) &&
                (target.y >= lane_tp[lane_controller.section_num].y - 15 &&
                target.y <= lane_tp[lane_controller.section_num].y + 16))
            {
                lane_cursor.SetActive(true);

                Vector3 lane_cursor_pos = lane_tp[lane_controller.section_num];

                //y���W�����߂�
                for (int j = 0; j < 16; j++)
                {
                    if (target.y >= (lane_tp[lane_controller.section_num].y - 15) + (j * 2) &&
                        target.y <= (lane_tp[lane_controller.section_num].y - 13) + (j * 2))
                    {
                        lane_cursor_pos.y = (lane_tp[lane_controller.section_num].y - 15) + (j * 2);
                        step = (section * 16) + j;
                        break;
                    }
                }

                //�}�E�X�J�[�\���̃��[���h���W����ǂ̃��[���ɂ��邩���m���� 4�̓��[���O
                if (target.x >= -16.0f && target.x <= -14.0f)
                {
                    //��
                    lane_num = 0;
                    lane_cursor_pos.x = -15;
                }
                else if (target.x > -14.0f && target.x <= -12.0f)
                {
                    //��
                    lane_num = 1;
                    lane_cursor_pos.x = -13;
                }
                else if (target.x > -12.0f && target.x <= -10.0f)
                {
                    //��
                    lane_num = 2;
                    lane_cursor_pos.x = -11;
                }
                else if (target.x > -10.0f && target.x <= -8.0f)
                {
                    //�E
                    lane_num = 3;
                    lane_cursor_pos.x = -9;
                }
                lane_cursor.transform.position = lane_cursor_pos;
            }
            else
            {
                lane_num = 4;
                lane_cursor.SetActive(false);
            }

            //���N���b�N������
            if (Input.GetMouseButtonDown(0) && lane_num != 4)
            {
                int num = 0;        //�����ꏊ�������ԍ�
                bool same = false;  //false = ���₷�@true = ���炷 

                KeyBoard_Operation.long_num = 0;

                //�m�[�c�̍ő吔������
                for (int i = 0; i < Music_Json.notes_list.Count; i++)
                {
                    //�m�[�c���u����Ă���ꏊ���N���b�N����
                    if (Music_Json.notes_list[i].lane == lane_num &&
                        Music_Json.notes_list[i].step == step &&
                        Music_Json.notes_list[i].type == 0)
                    {
                        //�����t���O�����Ă�
                        same = true;
                        num = i;
                        break;
                    }
                }

                //���[�����ɃJ�[�\�������ē����ꏊ���N���b�N�������ǂ���
                if (same == false)
                {
                    //�����ꏊ�ł͂Ȃ��̂�
                    //�I�u�W�F�N�g����
                    notes_golist.Add(Instantiate(Notes_Original));
                    Music_Json.notes_list.Add(notes_ori);

                    //�l����
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].lane = lane_num;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].section = section;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].step = step;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].time =
                        ((15.0f / Music_Data.bpm) * (step + 1)) - ((15.0f / Music_Data.bpm) / 2);
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].type = 0;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].gimmick
                        = KeyBoard_Operation.gimmick_num;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].long_num_max = 0;

                    //��̒l���ς�����Ⴄ���珉����
                    notes_ori = new Notes();

                    notes_golist[Music_Json.notes_list.Count - 1].transform.position =
                        lane_cursor.transform.position;

                    //���W�ݒ�
                    Vector3 notes_obj = notes_golist[Music_Json.notes_list.Count - 1].transform.position;

                    notes_obj.x = lane_cursor.transform.position.x;
                    notes_obj.y = lane_cursor.transform.position.y;
                    notes_obj.z = -2.0f;

                    notes_golist[Music_Json.notes_list.Count - 1].transform.position = notes_obj;

                    //�M�~�b�N�m�[�c���ǂ���
                    if (Music_Json.notes_list[Music_Json.notes_list.Count - 1].gimmick != 0)
                    {
                        //�M�~�b�N�m�[�c�Ȃ̂Ő�p�̂ɂ���
                        //�m�[�c�̉摜�ݒ�
                        notes_golist[Music_Json.notes_list.Count - 1].GetComponent<SpriteRenderer>().sprite =
                            gimmick_notes_collar[lane_num +
                            ((Music_Json.notes_list[Music_Json.notes_list.Count - 1].gimmick - 1) * 4)];
                    }
                    else
                    {
                        //�m�[�c�̉摜�ݒ�
                        notes_golist[Music_Json.notes_list.Count - 1].GetComponent<SpriteRenderer>().sprite =
                            notes_collar[lane_num];
                    }

                    old_lane_num = lane_num;

                    //�F�������l�ɖ߂�
                    if (Music_Json.notes_list.Count != 0)
                    {
                        for (int i = 0; i < notes_golist.Count - 1; i++)
                        {
                            notes_golist[i].GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

                        }
                        sp_collar = 1.0f;
                    }

                    //�^�b�v������̒ǉ�
                    se_flag.Add(Music_Json.input);
                    se_flag[Music_Json.notes_list.Count - 1] = false;

                    recently = true;
                }
                else if (lane_num != 4 && same == true)
                {
                    //�m�[�c�̐F�������l�ɖ߂�
                    if (Music_Json.notes_list.Count != 0)
                    {
                        for (int i = 0; i < notes_golist.Count - 1; i++)
                        {
                            notes_golist[i].GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                        }
                        sp_collar = 1.0f;
                    }

                    //�����ꏊ�Ȃ̂�
                    //�I�u�W�F�N�g�폜
                    if (Music_Json.notes_list[num].long_num_max != 0)
                    {
                        int count = Music_Json.notes_list.Count;

                        //�����O�m�[�c���폜
                        for (int i = 0, j = 0; i < count; i++)
                        {
                            if (num != i &&
                                Music_Json.notes_list[num].lane == Music_Json.notes_list[i - j].lane &&
                                Music_Json.notes_list[num].step == Music_Json.notes_list[i - j].step - (j + 1))
                            {
                                Destroy(notes_golist[i - j]);
                                notes_golist.RemoveAt(i - j);
                                Music_Json.notes_list.RemoveAt(i - j);
                                se_flag.RemoveAt(i - j);

                                j++;
                            }

                            if (Music_Json.notes_list[num].long_num_max == j)
                            {
                                break;
                            }
                        }

                        //�������ꏊ���폜
                        Destroy(notes_golist[num]);
                        notes_golist.RemoveAt(num);
                        Music_Json.notes_list.RemoveAt(num);
                        se_flag.RemoveAt(num);
                    }
                    else
                    {
                        Destroy(notes_golist[num]);
                        notes_golist.RemoveAt(num);
                        Music_Json.notes_list.RemoveAt(num);
                        se_flag.RemoveAt(num);
                    }

                    recently = false;
                }
            }

            //�ŐV�m�[�c�������\��
            if (recently == true)
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
                notes_golist[(Music_Json.notes_list.Count - 1) - KeyBoard_Operation.long_num].
                    GetComponent<SpriteRenderer>().color = new Color(sp_collar, sp_collar, sp_collar, 1.0f);
            }


            //���X�g��������
            lane_tp.Clear();
        }
    }
}
