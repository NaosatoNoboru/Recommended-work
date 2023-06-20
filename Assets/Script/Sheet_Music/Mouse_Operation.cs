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
    public List<GameObject> notes_golist = new List<GameObject>();//ノーツ管理リスト
    public Notes notes_ori = new Notes();//リスト追加用

    public Sprite[] notes_collar = new Sprite[4];//ノーツの色
    public Sprite[] gimmick_notes_collar = new Sprite[4 * 4];//ギミックノーツの色
    public Sprite[] longnotes_collar = new Sprite[4];//ロングノーツの色
    public Sprite[] longnotes_end_collar = new Sprite[4];//ロングノーツ尾っぽの色

    public int lane_num;//レーン番号
    public int old_lane_num;//古いレーン番号
    public Vector3 mouse_pos;//マウス座標
    public Vector3 target;
    public float mouse_scroll;//スクロールした数値
    public bool recently;//ノーツが生成されたかどうか
    public static List<bool> se_flag = new List<bool>();//ノーツが来たどうか

    private Json_Data JD;

    private List<Vector3> lane_tp = new List<Vector3>();

    private float[] lpy = new float[2];//0=マイナスを整数にした座標 1=丸める前の座標
    private float sp_collar;//画像の色可変用
    private int section;
    private int step;
    private bool collar_return;//0.5まで下がったかどうか
    private bool existing_notes;//既存のノーツを置く
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
                //既存のノーツを設置する
                for (int i = 0, j = 1; i < JD.max_combo; i++)
                {
                    notes_golist.Add(Instantiate(Notes_Original));

                    //基の値が変わっちゃうから初期化
                    notes_ori = new Notes();

                    //座標設定
                    Vector3 notes_obj = notes_golist[i].transform.position;
                    Vector3 notes_obj_ls = notes_golist[i].transform.localScale;

                    notes_obj.x = -15 + (2 * Music_Json.notes_list[i].lane);

                    //通常ノーツか否か
                    switch (Music_Json.notes_list[i].type)
                    {
                        case 0://通常ノーツなら普通に設置
                            notes_obj.y = 1 + (2 * Music_Json.notes_list[i].step);
                            notes_obj.z = -2.0f;

                            //ギミックノーツかどうか
                            if (Music_Json.notes_list[i].gimmick != 0)
                            {
                                //ギミックノーツなので専用のにする
                                //ノーツの画像設定
                                notes_golist[i].GetComponent<SpriteRenderer>().sprite =
                                    gimmick_notes_collar[Music_Json.notes_list[i].lane +
                                    ((Music_Json.notes_list[i].gimmick - 1) * 4)];
                            }
                            else
                            {
                                //ノーツの画像設定
                                notes_golist[i].GetComponent<SpriteRenderer>().sprite
                                    = notes_collar[Music_Json.notes_list[i].lane];
                            }
                            break;
                        case 1://ロングノーツも普通に設置
                            notes_obj.y = -1 + (2 * (Music_Json.notes_list[i].step + 1));

                            notes_obj.z = -2.0f + (j / 10000.0f);
                            j++;

                            //ノーツの画像設定
                            notes_golist[i].GetComponent<SpriteRenderer>().sprite
                                = longnotes_collar[Music_Json.notes_list[i].lane];
                            break;
                        case 2://ロングノーツの根本なので少しずらす
                            notes_obj.y = 0.5f + (2 * Music_Json.notes_list[i].step);
                            notes_obj.z = -2.0f + (j / 10000.0f);

                            notes_obj_ls.y = 3;
                            j++;

                            //ノーツの画像設定
                            notes_golist[i].GetComponent<SpriteRenderer>().sprite
                                = longnotes_collar[Music_Json.notes_list[i].lane];
                            break;
                        case 3://尾っぽ
                            //根元かどうか
                            if (Music_Json.notes_list[i - 1].long_num_max >= 1)
                            {
                                //根本なのでずらす
                                notes_obj.y = 1.0f + (2 * Music_Json.notes_list[i].step);
                                notes_obj.z = -2.0f + (j / 10000.0f);

                                notes_obj_ls.y = 4;
                                j++;
                            }
                            else
                            {
                                //通常配置
                                notes_obj.y = -1 + (2 * (Music_Json.notes_list[i].step + 1));

                                notes_obj.z = -2.0f + (j / 10000.0f);
                                j++;
                            }

                            //ノーツの画像設定
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

            //マウスカーソルの座標とワールド座標変換
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

            //ステップ数を超えて移動ささせてたら戻す
            if (sm_ui.step < 0)
            {
                mouse_scroll = sm_ui.step * -1;
            }
            else if (sm_ui.step > lane_controller.step_max)
            {
                mouse_scroll = (sm_ui.step - lane_controller.step_max) * -1;
            }

            //マウスホイールを回転させたら動かす
            if (mouse_scroll != 0.0f && KBO.reproduction == false)
            {
                //座標移動
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

                    //下にスクロールしていたら
                    if (mouse_scroll < 0.0f && sm_ui.step != 0)
                    {
                        sm_ui.step += (int)mouse_scroll;

                        //中途半端な位置にあったら元に戻す
                        if (pos % 2 == 1)
                        {
                            sm_ui.step -= (int)mouse_scroll;
                        }
                    }
                    else if (mouse_scroll > 0.0f)
                    {
                        sm_ui.step += (int)mouse_scroll;
                    }

                    //レーンの移動
                    for (int i = 0; i < lane_controller.section_max; i++)
                    {
                        //位置調整
                        Vector3 lane_pos = lane_controller.lane_list[i].transform.position;

                        lane_pos.y = 16 + (16 * (i * 2));
                        lane_pos.y -= 2 * sm_ui.step;

                        lane_tp[i] = lane_pos;

                        lane_controller.lane_list[i].transform.position = lane_pos;


                        //最後にフィニッシュラインを置く
                        if (i == lane_controller.section_max - 1)
                        {
                            Vector3 line_intpos = lane_controller.Finish_Line.transform.position;

                            line_intpos.y = (16 * (i * 2));
                            line_intpos.y += lane_controller.step_add * 2;
                            line_intpos.y -= 2 * sm_ui.step;

                            lane_controller.Finish_Line.transform.position = line_intpos;
                        }
                    }

                    //ノーツの初期化
                    for (int i = 0; i < notes_golist.Count; i++)
                    {
                        //座標設定
                        Vector3 notes_obj = notes_golist[i].transform.position;

                        //通常ノーツか否か
                        switch (Music_Json.notes_list[i].type)
                        {
                            case 0://通常ノーツなら普通に設置
                                notes_obj.y = 1 + (2 * Music_Json.notes_list[i].step);
                                break;
                            case 1://ロングノーツも普通に設置
                                notes_obj.y = -1 + (2 * (Music_Json.notes_list[i].step + 1));
                                break;
                            case 2://ロングノーツの根本なので少しずらす
                                notes_obj.y = 0.5f + (2 * Music_Json.notes_list[i].step);
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

                        notes_obj.y -= 2 * sm_ui.step;
                        notes_golist[i].transform.position = notes_obj;
                    }
                }
            }

            //マウスカーソルのワールド座標からどのどこにいるか
            if ((target.x >= -16.0f && target.x <= -8.0f) &&
                (target.y >= lane_tp[lane_controller.section_num].y - 15 &&
                target.y <= lane_tp[lane_controller.section_num].y + 16))
            {
                lane_cursor.SetActive(true);

                Vector3 lane_cursor_pos = lane_tp[lane_controller.section_num];

                //y座標を求める
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

                //マウスカーソルのワールド座標からどのレーンにいるか検知する 4はレーン外
                if (target.x >= -16.0f && target.x <= -14.0f)
                {
                    //左
                    lane_num = 0;
                    lane_cursor_pos.x = -15;
                }
                else if (target.x > -14.0f && target.x <= -12.0f)
                {
                    //下
                    lane_num = 1;
                    lane_cursor_pos.x = -13;
                }
                else if (target.x > -12.0f && target.x <= -10.0f)
                {
                    //上
                    lane_num = 2;
                    lane_cursor_pos.x = -11;
                }
                else if (target.x > -10.0f && target.x <= -8.0f)
                {
                    //右
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

            //左クリックしたら
            if (Input.GetMouseButtonDown(0) && lane_num != 4)
            {
                int num = 0;        //同じ場所だった番号
                bool same = false;  //false = 増やす　true = 減らす 

                KeyBoard_Operation.long_num = 0;

                //ノーツの最大数だけ回す
                for (int i = 0; i < Music_Json.notes_list.Count; i++)
                {
                    //ノーツが置かれている場所をクリックした
                    if (Music_Json.notes_list[i].lane == lane_num &&
                        Music_Json.notes_list[i].step == step &&
                        Music_Json.notes_list[i].type == 0)
                    {
                        //消すフラグを建てる
                        same = true;
                        num = i;
                        break;
                    }
                }

                //レーン内にカーソルあって同じ場所をクリックしたかどうか
                if (same == false)
                {
                    //同じ場所ではないので
                    //オブジェクト生成
                    notes_golist.Add(Instantiate(Notes_Original));
                    Music_Json.notes_list.Add(notes_ori);

                    //値を代入
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].lane = lane_num;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].section = section;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].step = step;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].time =
                        ((15.0f / Music_Data.bpm) * (step + 1)) - ((15.0f / Music_Data.bpm) / 2);
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].type = 0;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].gimmick
                        = KeyBoard_Operation.gimmick_num;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].long_num_max = 0;

                    //基の値が変わっちゃうから初期化
                    notes_ori = new Notes();

                    notes_golist[Music_Json.notes_list.Count - 1].transform.position =
                        lane_cursor.transform.position;

                    //座標設定
                    Vector3 notes_obj = notes_golist[Music_Json.notes_list.Count - 1].transform.position;

                    notes_obj.x = lane_cursor.transform.position.x;
                    notes_obj.y = lane_cursor.transform.position.y;
                    notes_obj.z = -2.0f;

                    notes_golist[Music_Json.notes_list.Count - 1].transform.position = notes_obj;

                    //ギミックノーツかどうか
                    if (Music_Json.notes_list[Music_Json.notes_list.Count - 1].gimmick != 0)
                    {
                        //ギミックノーツなので専用のにする
                        //ノーツの画像設定
                        notes_golist[Music_Json.notes_list.Count - 1].GetComponent<SpriteRenderer>().sprite =
                            gimmick_notes_collar[lane_num +
                            ((Music_Json.notes_list[Music_Json.notes_list.Count - 1].gimmick - 1) * 4)];
                    }
                    else
                    {
                        //ノーツの画像設定
                        notes_golist[Music_Json.notes_list.Count - 1].GetComponent<SpriteRenderer>().sprite =
                            notes_collar[lane_num];
                    }

                    old_lane_num = lane_num;

                    //色を初期値に戻す
                    if (Music_Json.notes_list.Count != 0)
                    {
                        for (int i = 0; i < notes_golist.Count - 1; i++)
                        {
                            notes_golist[i].GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

                        }
                        sp_collar = 1.0f;
                    }

                    //タップ音判定の追加
                    se_flag.Add(Music_Json.input);
                    se_flag[Music_Json.notes_list.Count - 1] = false;

                    recently = true;
                }
                else if (lane_num != 4 && same == true)
                {
                    //ノーツの色を初期値に戻す
                    if (Music_Json.notes_list.Count != 0)
                    {
                        for (int i = 0; i < notes_golist.Count - 1; i++)
                        {
                            notes_golist[i].GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                        }
                        sp_collar = 1.0f;
                    }

                    //同じ場所なので
                    //オブジェクト削除
                    if (Music_Json.notes_list[num].long_num_max != 0)
                    {
                        int count = Music_Json.notes_list.Count;

                        //ロングノーツも削除
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

                        //押され場場所を削除
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

            //最新ノーツを強調表示
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

                //色更新
                notes_golist[(Music_Json.notes_list.Count - 1) - KeyBoard_Operation.long_num].
                    GetComponent<SpriteRenderer>().color = new Color(sp_collar, sp_collar, sp_collar, 1.0f);
            }


            //リストを初期化
            lane_tp.Clear();
        }
    }
}
