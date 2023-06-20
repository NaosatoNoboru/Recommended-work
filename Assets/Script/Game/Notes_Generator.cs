using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Notes_Generator : MonoBehaviour
{
    public GameObject Notes_Original;
    public GameObject Long_End_Original;
    public List<GameObject> notes_golist = new List<GameObject>();//�m�[�c�Ǘ����X�g
    public static List<Notes> notes_list = new List<Notes>();

    public Sprite[] notes_collar = new Sprite[4];//�m�[�c�̐F
    public Sprite[] gimmick_notes_collar = new Sprite[4 * 4];//�M�~�b�N�m�[�c�̐F
    public Sprite[] longnotes_collar = new Sprite[4];//�����O�m�[�c�̐F
    public Sprite[] longnotes_end_collar = new Sprite[4];//�����O�m�[�c�̔����ۂ̐F

    public float speed;//�ړ����x

    private static int[] count = new int[2];//�m�[�c�J�E���g��

    private Json_Data JD;
    private Key_Configu KC;
    private Notes notes_ori = new Notes();//���X�g�ǉ��p

    // Start is called before the first frame update
    void Start()
    {
        //������
        notes_list.Clear();

        //�ȃf�[�^�ǂݍ���
        {
            JD = Music_Json.Load_Json();
            KC = Option.Load_Key_Configu();

            for (int i = 0; i < JD.max_combo; i++)
            {
                notes_list.Add(JD.notes[i]);
            }
        }

        //�����̃m�[�c��ݒu����
        for (int i = 0, j = 0; i < notes_list.Count; i++)
        {
            if (notes_list[i].type != 1)
            {
                //�����ۂ��ǂ���
                if (notes_list[i].type != 3)
                {
                    notes_golist.Add(Instantiate(Notes_Original));
                }
                else
                {
                    //�����ۂ���Notes_Controller���Ȃ��I�u�W�F�N�g�ɂ���
                    notes_golist.Add(Instantiate(Long_End_Original));
                }

                //��̒l���ς�����Ⴄ���珉����
                notes_ori = new Notes();

                //���W�ݒ�
                Vector3 notes_obj_tp = notes_golist[j].transform.position;
                Vector3 notes_obj_ls = notes_golist[j].transform.localScale;

                notes_obj_tp.x = -7.5f + (5 * notes_list[i].lane);

                //�ʏ�m�[�c���ۂ�
                switch (notes_list[i].type)
                {
                    case 0://�ʏ�m�[�c�Ȃ畁�ʂɐݒu
                        notes_obj_tp.y = -12.5f + (2 * notes_list[i].step * KC.scroll_speed);
                        notes_obj_tp.y += KC.scroll_speed / 2;//�ʒu����
                        notes_obj_tp.z = -2.0f;

                        //�M�~�b�N�m�[�c���ǂ���
                        if (notes_list[i].gimmick != 0)
                        {
                            //�M�~�b�N�m�[�c�Ȃ̂Ő�p�̂ɂ���
                            //�m�[�c�̉摜�ݒ�
                            notes_golist[j].GetComponent<SpriteRenderer>().sprite =
                                gimmick_notes_collar[notes_list[i].lane +
                                ((notes_list[i].gimmick - 1) * 4)];
                        }
                        else
                        {
                            //�m�[�c�̉摜�ݒ�
                            notes_golist[j].GetComponent<SpriteRenderer>().sprite =
                                notes_collar[notes_list[i].lane];
                        }
                        break;
                    case 2://�����O�m�[�c�̍��{�Ȃ̂ŏ������炷
                        int root = 0;//�����̔ԍ�

                        for (int k = 0; k < notes_list.Count; k++)
                        {
                            //�������[���ŋȂ��n�܂��Ă���̎��Ԃ���ǂ��̃��[������L�΂��Ă��邩�𒲂ׂ�
                            if (i != k && notes_list[i].lane == notes_list[k].lane &&
                                notes_list[i].step == notes_list[k].step + 1 &&
                                notes_list[k].long_num_max != 0)
                            {
                                root = k;
                                break;
                            }
                        }

                        notes_obj_tp.y = (-14.0f + KC.scroll_speed) +
                            (notes_list[root].long_num_max * KC.scroll_speed) +
                            (2 * notes_list[i].step * KC.scroll_speed);
                        //notes_obj_tp.y -= (KC.scroll_speed * 3)- 2;//�ʒu����
                        notes_obj_tp.y -= (2.5f * KC.scroll_speed) - 1.5f;//�ʒu����
                        notes_obj_tp.z = -1.9f;

                        notes_obj_ls.y = KC.scroll_speed * (notes_list[root].long_num_max) * 2;

                        //�����ۗp����
                        {
                            notes_obj_tp.y -= 1.0f - (KC.scroll_speed / 10);
                            notes_obj_ls.y -= 1.0f - (KC.scroll_speed / 10);
                        }

                        //�m�[�c�̉摜�ݒ�
                        notes_golist[j].GetComponent<SpriteRenderer>().sprite =
                            longnotes_collar[notes_list[i].lane];
                        break;
                    case 3://������
                        notes_obj_tp.y = -12.5f + (2 * notes_list[i].step * KC.scroll_speed);
                        //notes_obj_tp.y += KC.scroll_speed / 2;//�ʒu����
                        notes_obj_tp.z = -2.0f;

                        //�m�[�c�̉摜�ݒ�
                        notes_golist[j].GetComponent<SpriteRenderer>().sprite =
                           longnotes_end_collar[notes_list[i].lane];

                        notes_golist[j].transform.parent = notes_golist[j - 1].transform;

                        notes_obj_ls.x = 1.0f;
                        notes_obj_ls.y /= notes_golist[j - 1].transform.lossyScale.y;

                        break;
                    default:
                        break;
                }

                //���f
                notes_golist[j].transform.position = notes_obj_tp;
                notes_golist[j].transform.localScale = notes_obj_ls;

                j++;
            }
        }

        speed = JD.bpm / 60.0f * (8 * KC.scroll_speed);

        count[0] = 0;
        count[1] = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //�m�[�c�̏���n��
    public static List<Notes> Notes_Information()
    {
        var notes = new List<Notes>();

        if (count[0] != 0)
        {
            //�����O�m�[�c���ǂ���
            if (notes_list[count[0] - 1].long_num_max > 1)
            {
                int num = count[0];

                //�����O�m�[�c�̒�����������n��
                for (int i = 0; i < notes_list[num - 1].long_num_max; i++)
                {
                    notes.Add(notes_list[count[0]]);
                    count[0]++;
                }
            }
            else
            {
                notes.Add(notes_list[count[0]]);
                count[0]++;

                //������1�̂Ƃ�for���ƒʏ�m�[�c�������n���Ȃ����烍���O�m�[�c����n��
                if (notes_list[count[0] - 1].long_num_max == 1)
                {
                    notes.Add(notes_list[count[0]]);
                    count[0]++;
                }
            }
        }
        else
        {
            notes.Add(notes_list[count[0]]);
            count[0]++;
        }

        return notes;
    }
}
