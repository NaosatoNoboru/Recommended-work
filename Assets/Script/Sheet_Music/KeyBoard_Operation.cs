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

    public float start_time;//再生開始時間
    public static int long_num;//ロングノーツを増やした量
    public static int gimmick_num;//ギミック番号　0=通常　1=スピード上昇　2=スピード減少　3=割合　4=毒
    public int combo;//コンボ
    public int section;//セクション
    public int step;//ステップ
    public bool reproduction;//音楽再生フラグ

    private Json_Data JD;

    private bool finish;//譜面制作終了フラグ
    private bool button_select;//true = 曲選択画面へ false = 譜面確認へ

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
        //譜面情報があるなら
        if (Music_Json.input == true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (reproduction == true)
                {
                    //再生停止
                    audio_source.Pause();
                    reproduction = false;

                    //初期化
                    for (int i = 0; i < Music_Json.notes_list.Count; i++)
                    {
                        Mouse_Operation.se_flag[i] = false;
                    }
                }
                else
                {
                    //再生開始
                    start_time = Time.timeSinceLevelLoad;

                    audio_source.Play();
                    reproduction = true;
                }
            }
        }

        //ESCキー押したら譜面制作終了
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

        //最新ノーツが更新されているとき
        if (MO.recently == true && Music_Json.notes_list[Music_Json.notes_list.Count - 1].gimmick == 0)
        {
            //ロングノーツを増やす
            if (Input.GetKeyDown(KeyCode.E))
            {
                //最初のロングノーツかどうか
                if (long_num > 1)
                {
                    MO.notes_golist.Add(Instantiate(MO.Notes_Original));
                    Music_Json.notes_list.Add(MO.notes_ori);

                    //尾っぽと新しいのを入れ替える
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

                    //値を代入
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].lane = MO.old_lane_num;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].section = section;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].step = step;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].time =
                        ((15.0f / Music_Data.bpm) * (step + 1)) - ((15.0f / Music_Data.bpm) / 2);
                    Music_Json.notes_list[Music_Json.notes_list.Count - 2].type = 1;
                    Music_Json.notes_list[Music_Json.notes_list.Count - (2 + long_num)].long_num_max++;

                    //基の値が変わっちゃうから初期化
                    MO.notes_ori = new Notes();

                    //座標設定
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

                    //ノーツの画像設定
                    MO.notes_golist[Music_Json.notes_list.Count - 1].GetComponent<SpriteRenderer>().sprite =
                        MO.longnotes_end_collar[MO.old_lane_num];
                    MO.notes_golist[Music_Json.notes_list.Count - 2].GetComponent<SpriteRenderer>().sprite =
                        MO.longnotes_collar[MO.old_lane_num];

                    long_num++;
                }
                else
                {
                    //オブジェクト生成
                    MO.notes_golist.Add(Instantiate(MO.Notes_Original));
                    Music_Json.notes_list.Add(MO.notes_ori);

                    //尾っぽを最後尾に移動
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

                    //値を代入
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].lane = MO.old_lane_num;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].section = section;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].step = step;
                    Music_Json.notes_list[Music_Json.notes_list.Count - 1].time =
                        ((15.0f / Music_Data.bpm) * (step + 1)) - ((15.0f / Music_Data.bpm) / 2);
                    Music_Json.notes_list[Music_Json.notes_list.Count - long_num - 2].long_num_max++;

                    //基の値が変わっちゃうから初期化
                    MO.notes_ori = new Notes();

                    //座標設定
                    Vector3 notes_obj =
                        MO.notes_golist[Music_Json.notes_list.Count - 1].transform.position;
                    Vector3 notes_obj_ls =
                        MO.notes_golist[Music_Json.notes_list.Count - 1].transform.localScale;

                    notes_obj.x = MO.notes_golist[Music_Json.notes_list.Count - (2 + long_num)].transform.position.x;
                    notes_obj.y = MO.notes_golist[Music_Json.notes_list.Count - (2 + long_num)].transform.position.y +
                        (long_num * 2);
                    notes_obj.y += 1.5f;//微調整
                    notes_obj.z = -1.9f + (long_num / 100);

                    //0=尾っぽ　else=尾っぽの調整と根本
                    if (long_num == 0)
                    {
                        Music_Json.notes_list[Music_Json.notes_list.Count - 1].type = 3;
                        notes_obj.y += 0.5f;
                        notes_obj_ls.y = 4;

                        //ノーツの画像設定
                        MO.notes_golist[Music_Json.notes_list.Count - 1].GetComponent<SpriteRenderer>().sprite
                            = MO.longnotes_end_collar[MO.old_lane_num];
                    }
                    else
                    {
                        //尾っぽの座標とサイズ
                        Vector3 notes_obj_before =
                            MO.notes_golist[Music_Json.notes_list.Count - 2].transform.position;
                        Vector3 notes_obj_ls_before =
                            MO.notes_golist[Music_Json.notes_list.Count - 2].transform.localScale;

                        notes_obj.y += 0.5f;
                        notes_obj_before.y -= 0.5f;
                        notes_obj_ls_before.y = 3;

                        //尾っぽの反映
                        MO.notes_golist[Music_Json.notes_list.Count - 2].transform.position = notes_obj_before;
                        MO.notes_golist[Music_Json.notes_list.Count - 2].transform.localScale = notes_obj_ls_before;

                        Music_Json.notes_list[Music_Json.notes_list.Count - 2].type = 2;

                        notes_obj_ls.y = 2;

                        //ノーツの画像設定
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

            //ロングノーツを減らす
            if (Input.GetKeyDown(KeyCode.Q) && long_num != 0)
            {
                //最後のロングノーツかどうか
                if (long_num <= 2)
                {
                    //オブジェクト削除
                    Destroy(MO.notes_golist[Music_Json.notes_list.Count - 1]);
                    MO.notes_golist.RemoveAt(Music_Json.notes_list.Count - 1);
                    Music_Json.notes_list.RemoveAt(Music_Json.notes_list.Count - 1);
                    Music_Json.notes_list[Music_Json.notes_list.Count - long_num].long_num_max--;

                    //尾っぽの調整
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

                        //ノーツの画像設定
                        MO.notes_golist[Music_Json.notes_list.Count - 1].GetComponent<SpriteRenderer>().sprite
                            = MO.longnotes_end_collar[MO.old_lane_num];
                    }

                    long_num--;
                }
                else
                {
                    //オブジェクト削除
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

        //セクションをカウントダウン
        if (Input.GetKeyDown(KeyCode.LeftArrow) && reproduction == false &&
            lane_controller.section_num - 1 >= 0)
        {
            lane_controller.section_num--;
            sm_ui.section--;

            Set_Position();
        }

        //セクションをカウントアップ
        if (Input.GetKeyDown(KeyCode.RightArrow) && reproduction == false &&
            lane_controller.section_num + 1 < lane_controller.section_max)
        {
            lane_controller.section_num++;
            sm_ui.section++;

            Set_Position();
        }

        //セクションを5カウントダウン
        if (Input.GetKeyDown(KeyCode.DownArrow) && reproduction == false &&
            lane_controller.section_num - 5 >= 0)
        {
            lane_controller.section_num-=5;
            sm_ui.section-=5;

            Set_Position();
        }

        //セクションを5カウントアップ
        if (Input.GetKeyDown(KeyCode.UpArrow) && reproduction == false &&
            lane_controller.section_num + 5 < lane_controller.section_max)
        {
            lane_controller.section_num += 5;
            sm_ui.section += 5;

            Set_Position();
        }

        ////ノーツタイプ選択
        //{
        //    //通常
        //    if (Input.GetKeyDown(KeyCode.Alpha1))
        //    {
        //        gimmick_num = 0;
        //    }

        //    //スピードアップ
        //    if (Input.GetKeyDown(KeyCode.Alpha2))
        //    {
        //        gimmick_num = 1;
        //    }

        //    //スピードダウン
        //    if (Input.GetKeyDown(KeyCode.Alpha3))
        //    {
        //        gimmick_num = 2;
        //    }

        //    //割合
        //    if (Input.GetKeyDown(KeyCode.Alpha4))
        //    {
        //        gimmick_num = 3;
        //    }

        //    //毒
        //    if (Input.GetKeyDown(KeyCode.Alpha5))
        //    {
        //        gimmick_num = 4;
        //    }
        //}

        if (fade_controller.isFadeOut == false && finish == true)
        {
            if (button_select == true)
            {
                //曲選択画面へ
                SceneManager.LoadScene("Music_Select");
            }
            else
            {
                //譜面確認画面へ
                SceneManager.LoadScene("Sheet_Music_Test");
            }
        }

        void Set_Position()
        {
            sm_ui.step = sm_ui.section * 16;
            sm_ui.audio_source.time = ((15.0f / Music_Data.bpm) * 16) * sm_ui.section;

            for (int i = 0; i < lane_controller.section_max; i++)
            {
                //位置調整
                Vector3 lane_pos = lane_controller.lane_list[i].transform.position;

                lane_pos.y = 16 + (16 * (i * 2));//初期値
                lane_pos.y -= 16 * (lane_controller.section_num * 2);//移動

                lane_controller.lane_list[i].transform.position = lane_pos;

                //最後にフィニッシュラインを移動
                if (i == lane_controller.section_max - 1)
                {
                    Vector3 line_pos = lane_controller.Finish_Line.transform.position;

                    line_pos.y = (16 * (i * 2));
                    line_pos.y += lane_controller.step_add * 2;//初期値
                    line_pos.y -= 16 * (lane_controller.section_num * 2);//移動

                    lane_controller.Finish_Line.transform.position = line_pos;
                }
            }

            for (int i = 0; i < MO.notes_golist.Count; i++)
            {
                Vector3 notes_pos = MO.notes_golist[i].transform.position;

                //通常ノーツか否か
                switch (Music_Json.notes_list[i].type)
                {
                    case 0://通常ノーツなら普通に設置
                        notes_pos.y = 1 + (2 * Music_Json.notes_list[i].step);
                        break;
                    case 1://ロングノーツも普通に設置
                        notes_pos.y = -1 + (2 * (Music_Json.notes_list[i].step + 1));
                        break;
                    case 2://ロングノーツの根本なので少しずらす
                        notes_pos.y = 0.5f + (2 * Music_Json.notes_list[i].step);
                        break;
                    case 3://尾っぽ
                           //根元かどうか
                        if (Music_Json.notes_list[i - 1].long_num_max >= 1)
                        {
                            //根本なのでずらす
                            notes_pos.y = 1.0f + (2 * Music_Json.notes_list[i].step);
                        }
                        else
                        {
                            //通常配置
                            notes_pos.y = -1 + (2 * (Music_Json.notes_list[i].step + 1));
                        }
                        break;
                    default:
                        break;
                }

                notes_pos.y -= 16 * (lane_controller.section_num * 2);//移動

                MO.notes_golist[i].transform.position = notes_pos;
            }
        }
    }
}