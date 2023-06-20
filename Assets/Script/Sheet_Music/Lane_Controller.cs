using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Lane_Controller : MonoBehaviour
{
    public AudioSource audio_source;
    public KeyBoard_Operation KBO;
    public Mouse_Operation MO;

    public List<GameObject> lane_list = new List<GameObject>();//���[���̔z��
    public GameObject lane_Original;
    public GameObject Finish_Line;
    public Material[] material = new Material[2];//�F�����p 0=������Z�N�V�����@1=���̑�

    public int section_num;//������Z�N�V����
    public int section_max;//�Z�N�V�����̍ő�l
    public int step_add;
    public int step_max;

    private float speed;//�ړ����x
    private int old_section_num;//�ύX�O�̃Z�N�V����
    private bool start;//�ŏ�����

    // Start is called before the first frame update
    void Start()
    {
        section_num = 0;
        step_add = 0;
        old_section_num = section_num;
        start = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (start == false && Music_Json.input == true)
        {
            speed = Music_Data.bpm / 60.0f * 8.0f;

            section_max = (int)(audio_source.clip.length / ((15.0f / Music_Data.bpm) * 16));

            //�����ɒl������Ȃ�+����
            if (UnityEngine.Mathf.FloorToInt((int)(audio_source.clip.length / ((15.0f / Music_Data.bpm) * 16))) != 0.0f)
            {
                section_max++;
            }

            float time_calculation = audio_source.clip.length - ((section_max - 1) * ((15.0f / Music_Data.bpm) * 16));

            //�X�e�b�v�̍ő吔���o��
            step_max = (section_max - 1) * 16;

            while (time_calculation >= 15.0f / Music_Data.bpm)
            {
                time_calculation -= 15.0f / Music_Data.bpm;
                step_max++;
                step_add++;
            }

            for (int i = 0; i < section_max; i++)
            {
                //�I�u�W�F�N�g����
                lane_list.Add(Instantiate(lane_Original));

                //�ʒu����
                Vector3 lane_pos = lane_list[i].transform.position;

                lane_pos.x = -12;
                lane_pos.y = 16 + (16 * (i * 2));

                lane_list[i].transform.position = lane_pos;

                //������Z�N�V���������̑���
                if (i != section_num)
                {
                    //���̑�
                    lane_list[i].GetComponent<MeshRenderer>().material = material[1];
                }
                else
                {
                    //������
                    lane_list[i].GetComponent<MeshRenderer>().material = material[0];
                }

                //�Ō�Ƀt�B�j�b�V�����C����u��
                if (i == section_max - 1)
                {
                    Finish_Line = Instantiate(Finish_Line);

                    Vector3 line_pos = Finish_Line.transform.position;

                    line_pos.y = (16 * (i * 2));
                    line_pos.y += step_add * 2;

                    Finish_Line.transform.position = line_pos;
                }
            }

            start = true;
        }

        //�Z�N�V�������ς������}�e���A���̕ύX
        if (section_num != old_section_num)
        {
            for (int i = 0; i < section_max; i++)
            {
                //������Z�N�V���������̑���
                if (i != section_num)
                {
                    //���̑�
                    lane_list[i].GetComponent<MeshRenderer>().material = material[1];
                }
                else
                {
                    //������
                    lane_list[i].GetComponent<MeshRenderer>().material = material[0];
                }
            }

            old_section_num = section_num;
        }

        //�ĊJ�n�����̂œ�����
        if (KBO.reproduction == true)
        {
            for (int i = 0; i < section_max; i++)
            {
                Vector3 lane_pos = lane_list[i].transform.position;

                lane_pos.y -= speed * Time.deltaTime;

                lane_list[i].transform.position = lane_pos;
            }

            for (int i = 0; i < MO.notes_golist.Count; i++)
            {
                Vector3 notes_pos = MO.notes_golist[i].transform.position;

                notes_pos.y -= speed * Time.deltaTime;

                MO.notes_golist[i].transform.position = notes_pos;
            }

            Vector3 line_pos = Finish_Line.transform.position;

            line_pos.y -= speed * Time.deltaTime;

            Finish_Line.transform.position = line_pos;
        }
    }
}
