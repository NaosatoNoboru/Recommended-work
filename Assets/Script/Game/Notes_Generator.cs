using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Notes_Generator : MonoBehaviour
{
    public GameObject Notes_Original;
    public GameObject Long_End_Original;
    public List<GameObject> notes_golist = new List<GameObject>();//ノーツ管理リスト
    public static List<Notes> notes_list = new List<Notes>();

    public Sprite[] notes_collar = new Sprite[4];//ノーツの色
    public Sprite[] gimmick_notes_collar = new Sprite[4 * 4];//ギミックノーツの色
    public Sprite[] longnotes_collar = new Sprite[4];//ロングノーツの色
    public Sprite[] longnotes_end_collar = new Sprite[4];//ロングノーツの尾っぽの色

    public float speed;//移動速度

    private static int[] count = new int[2];//ノーツカウント数

    private Json_Data JD;
    private Key_Configu KC;
    private Notes notes_ori = new Notes();//リスト追加用

    // Start is called before the first frame update
    void Start()
    {
        //初期化
        notes_list.Clear();

        //曲データ読み込み
        {
            JD = Music_Json.Load_Json();
            KC = Option.Load_Key_Configu();

            for (int i = 0; i < JD.max_combo; i++)
            {
                notes_list.Add(JD.notes[i]);
            }
        }

        //既存のノーツを設置する
        for (int i = 0, j = 0; i < notes_list.Count; i++)
        {
            if (notes_list[i].type != 1)
            {
                //尾っぽかどうか
                if (notes_list[i].type != 3)
                {
                    notes_golist.Add(Instantiate(Notes_Original));
                }
                else
                {
                    //尾っぽだけNotes_Controllerがないオブジェクトにする
                    notes_golist.Add(Instantiate(Long_End_Original));
                }

                //基の値が変わっちゃうから初期化
                notes_ori = new Notes();

                //座標設定
                Vector3 notes_obj_tp = notes_golist[j].transform.position;
                Vector3 notes_obj_ls = notes_golist[j].transform.localScale;

                notes_obj_tp.x = -7.5f + (5 * notes_list[i].lane);

                //通常ノーツか否か
                switch (notes_list[i].type)
                {
                    case 0://通常ノーツなら普通に設置
                        notes_obj_tp.y = -12.5f + (2 * notes_list[i].step * KC.scroll_speed);
                        notes_obj_tp.y += KC.scroll_speed / 2;//位置調整
                        notes_obj_tp.z = -2.0f;

                        //ギミックノーツかどうか
                        if (notes_list[i].gimmick != 0)
                        {
                            //ギミックノーツなので専用のにする
                            //ノーツの画像設定
                            notes_golist[j].GetComponent<SpriteRenderer>().sprite =
                                gimmick_notes_collar[notes_list[i].lane +
                                ((notes_list[i].gimmick - 1) * 4)];
                        }
                        else
                        {
                            //ノーツの画像設定
                            notes_golist[j].GetComponent<SpriteRenderer>().sprite =
                                notes_collar[notes_list[i].lane];
                        }
                        break;
                    case 2://ロングノーツの根本なので少しずらす
                        int root = 0;//根元の番号

                        for (int k = 0; k < notes_list.Count; k++)
                        {
                            //同じレーンで曲が始まってからの時間からどこのレーンから伸ばしているかを調べる
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
                        //notes_obj_tp.y -= (KC.scroll_speed * 3)- 2;//位置調整
                        notes_obj_tp.y -= (2.5f * KC.scroll_speed) - 1.5f;//位置調整
                        notes_obj_tp.z = -1.9f;

                        notes_obj_ls.y = KC.scroll_speed * (notes_list[root].long_num_max) * 2;

                        //尾っぽ用調整
                        {
                            notes_obj_tp.y -= 1.0f - (KC.scroll_speed / 10);
                            notes_obj_ls.y -= 1.0f - (KC.scroll_speed / 10);
                        }

                        //ノーツの画像設定
                        notes_golist[j].GetComponent<SpriteRenderer>().sprite =
                            longnotes_collar[notes_list[i].lane];
                        break;
                    case 3://尾っぽ
                        notes_obj_tp.y = -12.5f + (2 * notes_list[i].step * KC.scroll_speed);
                        //notes_obj_tp.y += KC.scroll_speed / 2;//位置調整
                        notes_obj_tp.z = -2.0f;

                        //ノーツの画像設定
                        notes_golist[j].GetComponent<SpriteRenderer>().sprite =
                           longnotes_end_collar[notes_list[i].lane];

                        notes_golist[j].transform.parent = notes_golist[j - 1].transform;

                        notes_obj_ls.x = 1.0f;
                        notes_obj_ls.y /= notes_golist[j - 1].transform.lossyScale.y;

                        break;
                    default:
                        break;
                }

                //反映
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

    //ノーツの情報を渡す
    public static List<Notes> Notes_Information()
    {
        var notes = new List<Notes>();

        if (count[0] != 0)
        {
            //ロングノーツかどうか
            if (notes_list[count[0] - 1].long_num_max > 1)
            {
                int num = count[0];

                //ロングノーツの長さだけ情報を渡す
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

                //長さが1のときforだと通常ノーツ分しか渡さないからロングノーツ分を渡す
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
