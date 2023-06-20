using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sheet_Music_UI : MonoBehaviour
{
    public AudioSource SE;
    public AudioClip[] Tap_SE = new AudioClip[3];//0=タンバリン　1=指パッチン　2=カード　3=なし

    public AudioSource audio_source;
    public KeyBoard_Operation KBO;
    public Mouse_Operation MO;
    public Lane_Controller lane_controller;

    public GameObject[] Notes_type = new GameObject[5];

    public Text timer_text;
    public Text section_text;
    public Text step_text;

    public float music_time;   //再生時間
    public int section;    //今いるセクション
    public int step;       //今いるステップ

    private float audio_time;
    private float sp_collar;//画像の色可変用
    private int old_section;//古いセクション
    private int old_gimmick_num;//古いギミックノーツ番号
    private int section_max;//セクションの最大値
    private bool fast;
    private bool music_flag;//再生時間の同期
    private bool collar_return;//0.5まで下がったかどうか

    // Start is called before the first frame update
    void Start()
    {
        section_text.text = "Section：0/0";
        step_text.text = "Step：0000";

        section = 0;
        audio_time = 0.0f;
        old_section = section;
        fast = false;
    }

    // Update is called once per frame
    void Update()
    {
        //最初だけ入る
        if (fast == false && Music_Json.input == true)
        {
            section_max = (int)(audio_source.clip.length / ((15.0f / Music_Data.bpm) * 16));

            old_gimmick_num = KeyBoard_Operation.gimmick_num;
            fast = true;
        }

        //マウスホイールを回したら時間を動かす
        if (MO.mouse_scroll != 0.0f && KBO.reproduction == false)
        {
            audio_time = (15.0f / Music_Data.bpm) * step;

            //-にならないようにする
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

        //セクションの計算
        {
            section = (int)Mathf.Round(step / 16);

            //セクションが変わったら
            if (section != old_section)
            {
                lane_controller.section_num = section;
                old_section = section;
            }
        }

        //再生フラグがtrueになった
        if (KBO.reproduction == true)
        {
            if (music_flag == false)
            {
                //ので再生時間と同期
                //music_time = 0.0f;
                music_flag = true;
            }

            //music_nowtime = audio_source.time;
            music_time += Time.deltaTime;

            //ステップ数の計算
            float step_calculation = 0.0f;

            step_calculation = (15.0f / Music_Data.bpm);

            //カウントアップ
            //if (music_time >= step_calculation)
            //{
            //    step++;
            //}

            while (music_time >= step_calculation)
            {
                step++;

                music_time -= step_calculation;
            }

            //音を鳴らしてないやつを探す
            for (int i = 0; i < Music_Json.notes_list.Count; i++)
            {
                //タイミングが来たら鳴らす
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

        //選択しているギミックノーツが変わったら
        if (old_gimmick_num != KeyBoard_Operation.gimmick_num)
        {
            for (int i = 0; i < 5; i++)
            {
                //色リセット
                Notes_type[i].GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }

        //選択しているノーツタイプを強調表示
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

            //色更新
            Notes_type[KeyBoard_Operation.gimmick_num].
                GetComponent<SpriteRenderer>().color = new Color(sp_collar, sp_collar, sp_collar, 1.0f);
        }


        //再生時間が曲の時間を超えたら止める
        if (Music_Json.input == true)
        {
            if (KBO.reproduction == true && audio_source.isPlaying == false)
            {
                audio_source.time = 0.0f;
                audio_source.Pause();

                KBO.reproduction = false;
                step = 0;
                section = 0;

                //初期化
                for (int i = 0; i < lane_controller.lane_list.Count; i++)
                {
                    //位置調整
                    Vector3 lane_pos = lane_controller.lane_list[i].transform.position;

                    lane_pos.x = -12;
                    lane_pos.y = 16 + (16 * (i * 2));

                    lane_controller.lane_list[i].transform.position = lane_pos;

                    //今いるセクションかその他か
                    if (i != lane_controller.section_num)
                    {
                        //その他
                        lane_controller.lane_list[i].GetComponent<MeshRenderer>().material = lane_controller.material[1];
                    }
                    else
                    {
                        //今ここ
                        lane_controller.lane_list[i].GetComponent<MeshRenderer>().material = lane_controller.material[0];
                    }

                    //最後にフィニッシュラインを置く
                    if (i == lane_controller.lane_list.Count - 1)
                    {
                        Vector3 line_intpos = lane_controller.Finish_Line.transform.position;

                        line_intpos.y = (16 * (i * 2));
                        line_intpos.y += lane_controller.step_add * 2;

                        lane_controller.Finish_Line.transform.position = line_intpos;
                    }
                }

                //初期化
                for (int i = 0; i < MO.notes_golist.Count; i++)
                {
                    //座標設定
                    Vector3 notes_obj = MO.notes_golist[i].transform.position;

                    //通常ノーツか否か
                    switch (Music_Json.notes_list[i].type)
                    {
                        case 0://通常ノーツなら普通に設置
                            notes_obj.y = 1 + (2 * Music_Json.notes_list[i].step);
                            break;
                        case 1://ロングノーツも普通に設置
                            notes_obj.y = -1 + (2 * Music_Json.notes_list[i].step);
                            break;
                        case 2://ロングノーツの根本なので少しずらす
                            notes_obj.y = (2 * Music_Json.notes_list[i].step);
                            break;
                        case 3://尾っぽ
                            //根元かどうか
                            if (Music_Json.notes_list[i - 1].long_num_max >= 1)
                            {
                                //根本なのでずらす
                                notes_obj.y = 1.0f + (2 * Music_Json.notes_list[i].step);
                            }
                            else
                            {
                                //通常配置
                                notes_obj.y = -1 + (2 * (Music_Json.notes_list[i].step + 1));
                            }
                            break;
                        default:
                            break;
                    }

                    MO.notes_golist[i].transform.position = notes_obj;
                }

                //初期化
                for (int i = 0; i < Music_Json.notes_list.Count; i++)
                {
                    Mouse_Operation.se_flag[i] = false;
                }
            }

            //テキストに反映
            {
                //再生時間
                timer_text.text = audio_source.time.ToString("000.00") + "/"
                + audio_source.clip.length.ToString("000.00");

                //セクション
                section_text.text = "Section:" + section.ToString("000") + "/" + section_max;

                //ステップ
                step_text.text = "Step:" + step.ToString("0000");
            }
        }
    }
}