using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Notes_Controller : MonoBehaviour
{
    public AudioClip[] Tap_SE = new AudioClip[3];//0=タンバリン　1=指パッチン　2=カード　3=なし

    public static int[] judg = new int[4];//ノーツの判定
    public static int[] judg_lane = new int[4];//押せたレーンがどこか
    public static bool[] airtap_notes = new bool[4];//空打ちした時のノーツの色
    public static bool[] tap_notes = new bool[4];//ノーツを押した時の色

    public List<Notes> notes_list = new List<Notes>();

    private Json_Data JD;
    private Key_Configu KC;

    private float[] time = new float[2];//0=最初の時間 1=最後の時間
    private float[] pos = new float[2];//0=ロングノーツの下側の座標 1=ロングノーツの上側の座標
    private float start_length;//ロングノーツの長さの初期値
    private float speed;//移動速度
    private float ms;//判定数
    private bool start;//開始フラグ
    private bool[] hold_down = new bool[4];//押しっぱなしかどうか

    // Start is called before the first frame update
    void Start()
    {
        //ノーツデータ読み込み
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
            //キーコンフィグで割り当てたキーを押したかどうか
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

            //判定がまだあるなら
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

            //判定がまだあるなら
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
        //まだ判定があるか
        if (notes_list.Count != 0)
        {
            //ボタンを押したかどうか
            if (lane_type != 4)
            {
                //押し始めかどうか
                if (hold_down[lane_type] == false)
                {
                    hold_down[lane_type] = true;

                    //判定
                    if (notes_list[0].lane == lane_type &&
                        Mathf.Abs(notes_list[0].time - Countdown.Music.time) <= ms * 40)
                    {                    
                        //音無し以外だったら鳴らす
                        if (KC.tap_sound != 3)
                        {
                            Notes_Decision_Collar.Tap_audio_source.PlayOneShot(Tap_SE[KC.tap_sound]);
                        }

                        //通常ノーツだけ判定
                        if (notes_list[0].type == 0)
                        {
                            //パーフェクト
                            judg[0]++;

                            //光らせるレーンを取る
                            for (int i = 0; i < 4; i++)
                            {
                                if (judg_lane[i] == -1)
                                {
                                    judg_lane[i] = lane_type;
                                    break;
                                }
                            }
                        }

                        //押せたので判定を消す
                        notes_list.RemoveAt(0);

                        //押したレーンの判定を光らせる
                        tap_notes[lane_type] = true;

                        //判定が来たら消す
                        if (notes_list.Count == 0)
                        {
                            Destroy(this.gameObject);
                        }
                    }
                    else if (notes_list[0].lane == lane_type &&
                        Mathf.Abs(notes_list[0].time - Countdown.Music.time) <= ms * 80)
                    {
                        //通常ノーツだけ判定
                        if (notes_list[0].type == 0)
                        {                    
                            //音無し以外だったら鳴らす
                            if (KC.tap_sound != 3)
                            {
                                Notes_Decision_Collar.Tap_audio_source.PlayOneShot(Tap_SE[KC.tap_sound]);
                            }

                            //グッド
                            judg[1]++;

                            //光らせるレーンを取る
                            for (int i = 0; i < 4; i++)
                            {
                                if (judg_lane[i] == -1)
                                {
                                    judg_lane[i] = lane_type;
                                    break;
                                }
                            }
                        }

                        //押せたので判定を消す
                        notes_list.RemoveAt(0);

                        //押したレーンの判定を光らせる
                        tap_notes[lane_type] = true;

                        //判定が来たら消す
                        if (notes_list.Count == 0)
                        {
                            Destroy(this.gameObject);
                        }
                    }
                    else if (notes_list[0].lane == lane_type &&
                        Mathf.Abs(notes_list[0].time - Countdown.Music.time) <= ms * 120)
                    {
                        //通常ノーツだけ判定
                        if (notes_list[0].type == 0)
                        {
                            //音無し以外だったら鳴らす
                            if (KC.tap_sound != 3)
                            {
                                Notes_Decision_Collar.Tap_audio_source.PlayOneShot(Tap_SE[KC.tap_sound]);
                            }

                            //バッド
                            judg[2]++;

                            //光らせるレーンを取る
                            for (int i = 0; i < 4; i++)
                            {
                                if (judg_lane[i] == -1)
                                {
                                    judg_lane[i] = lane_type;
                                    break;
                                }
                            }
                        }

                        //押せたので判定を消す
                        notes_list.RemoveAt(0);

                        //押したレーンの判定を光らせる
                        tap_notes[lane_type] = true;

                        //判定が来たら消す
                        if (notes_list.Count == 0)
                        {
                            Destroy(this.gameObject);
                        }
                    }
                    else
                    {
                        //関係ないところで押したレーンの判定を空打ちで光らせる
                        airtap_notes[lane_type] = true;
                    }
                }
                else
                {
                    //ロングノーツだったら
                    if (notes_list[0].lane == lane_type &&
                       notes_list[0].type >= 1 &&
                        Mathf.Abs(notes_list[0].time - Countdown.Music.time) <= ms * 120)
                    {
                        //パーフェクト
                        judg[0]++;

                        //光らせるレーンを取る
                        for (int i = 0; i < 4; i++)
                        {
                            if (judg_lane[i] == -1)
                            {
                                judg_lane[i] = lane_type;
                                break;
                            }
                        }

                        //押せたので判定を消す
                        notes_list.RemoveAt(0);

                        //判定が来たら消す
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
                //ボタン押してない
                //ミス判定
                if (notes_list[0].time - Countdown.Music.time < -ms * 120)
                {
                    judg[3]++;

                    notes_list.RemoveAt(0);

                    //判定が来たら消す
                    if (notes_list.Count == 0)
                    {
                        Destroy(this.gameObject);
                    }
                }
            }

            //キーを押してなかったらデフォルトに戻す
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
        //通常ノーツだったら
        if (notes_list[0].type == 0 &&
           notes_list[0].time - Countdown.Music.time < ms * 5)
        {
            int lane = notes_list[0].lane;

            //音無し以外だったら鳴らす
            if (KC.tap_sound != 3)
            {
                Notes_Decision_Collar.Tap_audio_source.PlayOneShot(Tap_SE[KC.tap_sound]);
            }

            //パーフェクト
            judg[0]++;

            //光らせるレーンを取る
            for (int i = 0; i < 4; i++)
            {
                if (judg_lane[i] == -1)
                {
                    judg_lane[i] = lane;
                    break;
                }
            }

            //押したレーンの判定を光らせる
            //tap_notes[lane] = true;

            //押せたので判定を消す
            notes_list.RemoveAt(0);

            //判定が来たら消す
            if (notes_list.Count == 0)
            {
                Destroy(this.gameObject);
            }
        }
        else if (notes_list[0].type >= 1 &&
           notes_list[0].time - Countdown.Music.time <= ms * 5)
        {
            //ロングノーツだったら
            int lane = notes_list[0].lane;

            //パーフェクト
            judg[0]++;

            //光らせるレーンを取る
            for (int i = 0; i < 4; i++)
            {
                if (judg_lane[i] == -1)
                {
                    judg_lane[i] = lane;
                    break;
                }
            }

            //押したレーンの判定を光らせる
            tap_notes[lane] = true;

            //押せたので判定を消す
            notes_list.RemoveAt(0);

            //判定が来たら消す
            if (notes_list.Count == 0)
            {
                tap_notes[lane] = false;
                Destroy(this.gameObject);
            }
        }
    }
}
