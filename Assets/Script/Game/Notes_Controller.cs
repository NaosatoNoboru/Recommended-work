using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Notes_Controller : MonoBehaviour
{
    public AudioClip[] Tap_SE = new AudioClip[3];//0=�^���o�����@1=�w�p�b�`���@2=�J�[�h�@3=�Ȃ�

    public static int[] judg = new int[4];//�m�[�c�̔���
    public static int[] judg_lane = new int[4];//���������[�����ǂ���
    public static bool[] airtap_notes = new bool[4];//��ł��������̃m�[�c�̐F
    public static bool[] tap_notes = new bool[4];//�m�[�c�����������̐F

    public List<Notes> notes_list = new List<Notes>();

    private Json_Data JD;
    private Key_Configu KC;

    private float[] time = new float[2];//0=�ŏ��̎��� 1=�Ō�̎���
    private float[] pos = new float[2];//0=�����O�m�[�c�̉����̍��W 1=�����O�m�[�c�̏㑤�̍��W
    private float start_length;//�����O�m�[�c�̒����̏����l
    private float speed;//�ړ����x
    private float ms;//���萔
    private bool start;//�J�n�t���O
    private bool[] hold_down = new bool[4];//�������ςȂ����ǂ���

    // Start is called before the first frame update
    void Start()
    {
        //�m�[�c�f�[�^�ǂݍ���
        {
            JD = Music_Json.Load_Json();
            KC = Option.Load_Key_Configu();

            if (notes_list.Count == 0)
            {
                notes_list = Notes_Generator.Notes_Information();
            }
        }

        time[0] = notes_list[0].time;
        time[1] = notes_list[notes_list.Count - 1].time;
        pos[0] = this.gameObject.transform.position.y - (this.gameObject.transform.localScale.y / 2);
        pos[1] = this.gameObject.transform.position.y + (this.gameObject.transform.localScale.y / 2);
        ms = 1.0f / 1000;
        speed = JD.bpm / 60.0f * (8 * KC.scroll_speed);
        start_length = this.gameObject.transform.localScale.y;

        for (int i = 0; i < 4; i++)
        {
            judg[i] = 0;
            judg_lane[i] = -1;
        }

        start = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Countdown.count_end == true)
        {
            start = true;
        }
        else
        {
            start = false;
        }

        if (start == true && Music_Select.auto_flag == false)
        {
            //�L�[�R���t�B�O�Ŋ��蓖�Ă��L�[�����������ǂ���
            if (Input.GetKey(KC.Left) || Input.GetKey(KC.Down) ||
                Input.GetKey(KC.Up) || Input.GetKey(KC.Right))
            {
                if (Input.GetKey(KC.Right))
                {
                    Hit_Check(3);
                }

                if (Input.GetKey(KC.Up))
                {
                    Hit_Check(2);
                }

                if (Input.GetKey(KC.Down))
                {
                    Hit_Check(1);
                }

                if (Input.GetKey(KC.Left))
                {
                    Hit_Check(0);
                }
            }

            //���肪�܂�����Ȃ�
            if (notes_list.Count != 0)
            {
                Hit_Check(4);

                Vector3 notes_pos = this.transform.position;

                notes_pos.y -= speed * Time.deltaTime;

                this.transform.position = notes_pos;
            }
        }
        else if (start == true && Music_Select.auto_flag == true)
        {
            Auto();

            //���肪�܂�����Ȃ�
            if (notes_list.Count != 0)
            {
                Vector3 notes_pos = this.transform.position;

                notes_pos.y -= speed * Time.deltaTime;

                this.transform.position = notes_pos;
            }
        }
    }

    void Hit_Check(int lane_type)
    {
        //�܂����肪���邩
        if (notes_list.Count != 0)
        {
            //�{�^�������������ǂ���
            if (lane_type != 4)
            {
                //�����n�߂��ǂ���
                if (hold_down[lane_type] == false)
                {
                    hold_down[lane_type] = true;

                    //����
                    if (notes_list[0].lane == lane_type &&
                        Mathf.Abs(notes_list[0].time - Countdown.Music.time) <= ms * 40)
                    {                    
                        //�������ȊO��������炷
                        if (KC.tap_sound != 3)
                        {
                            Notes_Decision_Collar.Tap_audio_source.PlayOneShot(Tap_SE[KC.tap_sound]);
                        }

                        //�ʏ�m�[�c��������
                        if (notes_list[0].type == 0)
                        {
                            //�p�[�t�F�N�g
                            judg[0]++;

                            //���点�郌�[�������
                            for (int i = 0; i < 4; i++)
                            {
                                if (judg_lane[i] == -1)
                                {
                                    judg_lane[i] = lane_type;
                                    break;
                                }
                            }
                        }

                        //�������̂Ŕ��������
                        notes_list.RemoveAt(0);

                        //���������[���̔�������点��
                        tap_notes[lane_type] = true;

                        //���肪���������
                        if (notes_list.Count == 0)
                        {
                            Destroy(this.gameObject);
                        }
                    }
                    else if (notes_list[0].lane == lane_type &&
                        Mathf.Abs(notes_list[0].time - Countdown.Music.time) <= ms * 80)
                    {
                        //�ʏ�m�[�c��������
                        if (notes_list[0].type == 0)
                        {                    
                            //�������ȊO��������炷
                            if (KC.tap_sound != 3)
                            {
                                Notes_Decision_Collar.Tap_audio_source.PlayOneShot(Tap_SE[KC.tap_sound]);
                            }

                            //�O�b�h
                            judg[1]++;

                            //���点�郌�[�������
                            for (int i = 0; i < 4; i++)
                            {
                                if (judg_lane[i] == -1)
                                {
                                    judg_lane[i] = lane_type;
                                    break;
                                }
                            }
                        }

                        //�������̂Ŕ��������
                        notes_list.RemoveAt(0);

                        //���������[���̔�������点��
                        tap_notes[lane_type] = true;

                        //���肪���������
                        if (notes_list.Count == 0)
                        {
                            Destroy(this.gameObject);
                        }
                    }
                    else if (notes_list[0].lane == lane_type &&
                        Mathf.Abs(notes_list[0].time - Countdown.Music.time) <= ms * 120)
                    {
                        //�ʏ�m�[�c��������
                        if (notes_list[0].type == 0)
                        {
                            //�������ȊO��������炷
                            if (KC.tap_sound != 3)
                            {
                                Notes_Decision_Collar.Tap_audio_source.PlayOneShot(Tap_SE[KC.tap_sound]);
                            }

                            //�o�b�h
                            judg[2]++;

                            //���点�郌�[�������
                            for (int i = 0; i < 4; i++)
                            {
                                if (judg_lane[i] == -1)
                                {
                                    judg_lane[i] = lane_type;
                                    break;
                                }
                            }
                        }

                        //�������̂Ŕ��������
                        notes_list.RemoveAt(0);

                        //���������[���̔�������点��
                        tap_notes[lane_type] = true;

                        //���肪���������
                        if (notes_list.Count == 0)
                        {
                            Destroy(this.gameObject);
                        }
                    }
                    else
                    {
                        //�֌W�Ȃ��Ƃ���ŉ��������[���̔������ł��Ō��点��
                        airtap_notes[lane_type] = true;
                    }
                }
                else
                {
                    //�����O�m�[�c��������
                    if (notes_list[0].lane == lane_type &&
                       notes_list[0].type >= 1 &&
                        Mathf.Abs(notes_list[0].time - Countdown.Music.time) <= ms * 120)
                    {
                        //�p�[�t�F�N�g
                        judg[0]++;

                        //���点�郌�[�������
                        for (int i = 0; i < 4; i++)
                        {
                            if (judg_lane[i] == -1)
                            {
                                judg_lane[i] = lane_type;
                                break;
                            }
                        }

                        //�������̂Ŕ��������
                        notes_list.RemoveAt(0);

                        //���肪���������
                        if (notes_list.Count == 0)
                        {
                            tap_notes[lane_type] = false;
                            airtap_notes[lane_type] = false;

                            Destroy(this.gameObject);
                        }
                    }
                }
            }
            else
            {
                //�{�^�������ĂȂ�
                //�~�X����
                if (notes_list[0].time - Countdown.Music.time < -ms * 120)
                {
                    judg[3]++;

                    notes_list.RemoveAt(0);

                    //���肪���������
                    if (notes_list.Count == 0)
                    {
                        Destroy(this.gameObject);
                    }
                }
            }

            //�L�[�������ĂȂ�������f�t�H���g�ɖ߂�
            {
                if (Input.GetKey(KC.Left) == false)
                {
                    hold_down[0] = false;
                    tap_notes[0] = false;
                    airtap_notes[0] = false;
                }

                if (Input.GetKey(KC.Down) == false)
                {
                    hold_down[1] = false;
                    tap_notes[1] = false;
                    airtap_notes[1] = false;
                }

                if (Input.GetKey(KC.Up) == false)
                {
                    hold_down[2] = false;
                    tap_notes[2] = false;
                    airtap_notes[2] = false;
                }

                if (Input.GetKey(KC.Right) == false)
                {
                    hold_down[3] = false;
                    tap_notes[3] = false;
                    airtap_notes[3] = false;
                }
            }
        }
    }

    void Auto()
    {
        //�ʏ�m�[�c��������
        if (notes_list[0].type == 0 &&
           notes_list[0].time - Countdown.Music.time < ms * 5)
        {
            int lane = notes_list[0].lane;

            //�������ȊO��������炷
            if (KC.tap_sound != 3)
            {
                Notes_Decision_Collar.Tap_audio_source.PlayOneShot(Tap_SE[KC.tap_sound]);
            }

            //�p�[�t�F�N�g
            judg[0]++;

            //���点�郌�[�������
            for (int i = 0; i < 4; i++)
            {
                if (judg_lane[i] == -1)
                {
                    judg_lane[i] = lane;
                    break;
                }
            }

            //���������[���̔�������点��
            //tap_notes[lane] = true;

            //�������̂Ŕ��������
            notes_list.RemoveAt(0);

            //���肪���������
            if (notes_list.Count == 0)
            {
                Destroy(this.gameObject);
            }
        }
        else if (notes_list[0].type >= 1 &&
           notes_list[0].time - Countdown.Music.time <= ms * 5)
        {
            //�����O�m�[�c��������
            int lane = notes_list[0].lane;

            //�p�[�t�F�N�g
            judg[0]++;

            //���点�郌�[�������
            for (int i = 0; i < 4; i++)
            {
                if (judg_lane[i] == -1)
                {
                    judg_lane[i] = lane;
                    break;
                }
            }

            //���������[���̔�������点��
            tap_notes[lane] = true;

            //�������̂Ŕ��������
            notes_list.RemoveAt(0);

            //���肪���������
            if (notes_list.Count == 0)
            {
                tap_notes[lane] = false;
                Destroy(this.gameObject);
            }
        }
    }
}
