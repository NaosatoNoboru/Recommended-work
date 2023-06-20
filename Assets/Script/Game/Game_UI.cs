using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game_UI : MonoBehaviour
{
    public GameObject Grade;
    public GameObject Combo;
    public GameObject HP_Gage;
    public GameObject Music_Gage;
    public GameObject Lost;

    public Text name_text;
    public Text score_text;
    public Text grade_text;
    public Text combo_text;
    public Text hp_text;

    public static long score_num;//�X�R�A�̒l
    public static int hp;//�̗�
    public static int max_combo;//�ő�R���{

    private Json_Data JD;

    private int combo;//�R���{��
    private int old_hp;//�ύX�O�̗̑�
    private int[] old_judg = new int[4];//�m�[�c�̔���
    private float music_percent;//�Ȃ���%�i�񂾂�
    private bool end_percent;//�Ȃ��I��������ǂ���

    // Start is called before the first frame update
    void Start()
    {
        //�ō��R���{�擾�p
        JD = Music_Json.Load_Json();

        name_text.text = JD.music_name;

        for (int i = 0; i < 4; i++)
        {
            old_judg[i] = 0;
        }

        score_num = 0;
        max_combo = 0;
        hp = 100;
        old_hp = hp;
        music_percent = 0.0f;

        //�ŏ��͔�\��
        Grade.SetActive(false);
        Combo.SetActive(false);
        Lost.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //�m�[�c�֘A
        {
            for (int i = 0; i < 4; i++)
            {
                //�l���ς�������ǂ���
                if (old_judg[i] != Notes_Controller.judg[i] && i != 3)
                {
                    Grade.SetActive(true);

                    //����\��
                    switch (i)
                    {
                        case 0:
                            hp += 2;
                            grade_text.text = "<color=#FFA800>PERFECT</color>\n";
                            break;
                        case 1:
                            hp += 1;
                            grade_text.text = "<color=#40FF00>GOOD</color>\n";
                            break;
                        case 2:
                            hp -= 1;
                            grade_text.text = "<color=#000A91>BAD</color>\n";
                            break;
                        default:
                            break;
                    }

                    //�㉺���𒴂��Ȃ��悤�ɂ���
                    if (hp > 100)
                    {
                        hp = 100;
                    }
                    else if (hp < 0)
                    {
                        hp = 0;
                    }

                    //�R���{�l�̌v�Z
                    combo += Notes_Controller.judg[i] - old_judg[i];

                    //�R���{�l�����ȏ�Ȃ�\��
                    if (combo >= 5)
                    {
                        Combo.SetActive(true);

                        if (Notes_Controller.judg[3] == 0)
                        {
                            //�~�X���Ȃ�
                            combo_text.text = combo.ToString();
                        }
                        else
                        {
                            //�~�X����
                            combo_text.text = "<color=#FFFFFF>" + combo.ToString() + "</color>";
                        }
                    }

                    //�ő�R���{���̌v�Z
                    if (max_combo < combo)
                    {
                        max_combo= combo;
                    }

                    //�X�R�A�v�Z
                    //((�p�[�t�F�N�g + (�O�b�h / 2.0) + (�o�b�h / 3.0)) / �ő�R���{) * �ő�X�R�A
                    score_num = (long)((((Notes_Controller.judg[0]) + (Notes_Controller.judg[1] / 2.0) +
                        (Notes_Controller.judg[2] / 3.0)) / JD.max_combo) * 1000000.0);

                    //�X�R�A�\��
                    score_text.text = score_num.ToString("0000000");
                }

                //�~�X�������ǂ���
                if (old_judg[i] != Notes_Controller.judg[i] && i == 3)
                {
                    //�~�X�����̂ŏ�����
                    Grade.SetActive(false);
                    Combo.SetActive(false);

                    hp -= 5;
                    combo = 0;

                    //�㉺���𒴂��Ȃ��悤�ɂ���
                    if (hp > 100)
                    {
                        hp = 100;
                    }
                    else if (hp < 0)
                    {
                        hp = 0;
                    }
                }

                old_judg[i] = Notes_Controller.judg[i];
            }
        }

        //HP�֘A
        {
            //HP���ς�������ǂ���
            if (old_hp != hp)
            {
                //�Q�[�W�̑傫���ƍ��W��ς���
                var gage_pos = HP_Gage.transform.localPosition;
                var gage_ls = HP_Gage.transform.localScale;

                gage_pos.y = -3 + (hp * 0.03f);
                gage_ls.y = 1 * (hp / 100.0f);

                HP_Gage.transform.localPosition = gage_pos;
                HP_Gage.transform.localScale = gage_ls;

                //�c��HP�\��
                hp_text.text = hp.ToString("000") + "%";

                //�Â������ŐV��
                old_hp = hp;

                //HP���Ȃ��Ȃ�����Lost�p�l����\��
                if (hp == 0)
                {
                    Time.timeScale = 0;  //�~�߂�
                    Countdown.Music.Pause();
                    Lost.SetActive(true);
                }
            }
        }

        //�Ȃ̍Đ�����
        if (end_percent == false)
        {
            //�Q�[�W��L�΂�
            music_percent = (Countdown.Music.time / Countdown.Music.clip.length) * 100.0f;

            //100%�Ȃ�I���
            if (music_percent >= 100.0f)
            {
                music_percent = 100.0f;
                end_percent = true;
            }

            //�Q�[�W�̑傫���ƍ��W��ς���
            var gage_pos = Music_Gage.transform.localPosition;
            var gage_ls = Music_Gage.transform.localScale;

            gage_pos.y = -3 + (music_percent * 0.03f);
            gage_ls.y = 1 * (music_percent / 100.0f);

            Music_Gage.transform.localPosition = gage_pos;
            Music_Gage.transform.localScale = gage_ls;
        }

        if (end_percent == true)
        {
            SceneManager.LoadScene("Result");
        }
    }
}
