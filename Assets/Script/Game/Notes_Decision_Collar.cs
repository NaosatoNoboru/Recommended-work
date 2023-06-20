using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notes_Decision_Collar : MonoBehaviour
{
    public static AudioSource Tap_audio_source;//SE�p

    public GameObject[] notes = new GameObject[4];//�m�[�c
    public GameObject[] mask_notes = new GameObject[4];//�}�X�N�m�[�c
    public GameObject[] particle_original = new GameObject[4];//�p�[�e�B�N��

    public Sprite[] neutral_notes_collar = new Sprite[4];//�ʏ펞�̃m�[�c�̐F
    public Sprite[] airtap_notes_collar = new Sprite[4];//��ł��������̃m�[�c�̐F
    public Sprite[] tap_notes_collar = new Sprite[4];//�m�[�c�����������̐F

    private List<GameObject> particle_list = new List<GameObject>();//�p�[�e�B�N��
    private int[] judg = new int[3];//���萔

    // Start is called before the first frame update
    void Start()
    {
        Tap_audio_source = this.GetComponent<AudioSource>();

        for (int i = 0; i < 3; i++)
        {
            judg[i] = Notes_Controller.judg[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            //�ǂ̔���ɂ��邩
            if (Notes_Controller.tap_notes[i] == true)
            {
                //�m�[�c�������Ă���
                notes[i].GetComponent<SpriteRenderer>().sprite = tap_notes_collar[i];
                mask_notes[i].SetActive(true);
            }
            else if (Notes_Controller.airtap_notes[i] == true)
            {
                //��ł�
                notes[i].GetComponent<SpriteRenderer>().sprite = airtap_notes_collar[i];
            }
            else
            {
                //�ʏ�
                notes[i].GetComponent<SpriteRenderer>().sprite = neutral_notes_collar[i];
                mask_notes[i].SetActive(false);
            }
        }

        int nc_judg = Notes_Controller.judg[0] + Notes_Controller.judg[1] + Notes_Controller.judg[2];

        //bad�ȏ�̔��肾������p�[�e�B�N�����o��
        if (judg[0] + judg[1] + judg[2] != nc_judg)
        {
            var judg_all = nc_judg - (judg[0] + judg[1] + judg[2]);

            if (judg_all > 4)
            {
                judg_all = 4;
            }

            //���肵���񐔂�����
            for (int i = 0; i < judg_all; i++)
            {
                particle_list.Add(Instantiate(particle_original[Notes_Controller.judg_lane[i]]));

                particle_list[particle_list.Count - 1].transform.position =
                    notes[Notes_Controller.judg_lane[i]].transform.position;

                //������
                Notes_Controller.judg_lane[i] = -1;
            }

            //�ω��������𔽉f
            for (int i = 0; i < 3; i++)
            {
                judg[i] = Notes_Controller.judg[i];
            }
        }
    }
}
