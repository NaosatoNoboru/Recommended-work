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

    public static long score_num;//スコアの値
    public static int hp;//体力
    public static int max_combo;//最大コンボ

    private Json_Data JD;

    private int combo;//コンボ数
    private int old_hp;//変更前の体力
    private int[] old_judg = new int[4];//ノーツの判定
    private float music_percent;//曲が何%進んだか
    private bool end_percent;//曲が終わったかどうか

    // Start is called before the first frame update
    void Start()
    {
        //最高コンボ取得用
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

        //最初は非表示
        Grade.SetActive(false);
        Combo.SetActive(false);
        Lost.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //ノーツ関連
        {
            for (int i = 0; i < 4; i++)
            {
                //値が変わったかどうか
                if (old_judg[i] != Notes_Controller.judg[i] && i != 3)
                {
                    Grade.SetActive(true);

                    //判定表示
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

                    //上下限を超えないようにする
                    if (hp > 100)
                    {
                        hp = 100;
                    }
                    else if (hp < 0)
                    {
                        hp = 0;
                    }

                    //コンボ値の計算
                    combo += Notes_Controller.judg[i] - old_judg[i];

                    //コンボ値が一定以上なら表示
                    if (combo >= 5)
                    {
                        Combo.SetActive(true);

                        if (Notes_Controller.judg[3] == 0)
                        {
                            //ミスがない
                            combo_text.text = combo.ToString();
                        }
                        else
                        {
                            //ミスあり
                            combo_text.text = "<color=#FFFFFF>" + combo.ToString() + "</color>";
                        }
                    }

                    //最大コンボ数の計算
                    if (max_combo < combo)
                    {
                        max_combo= combo;
                    }

                    //スコア計算
                    //((パーフェクト + (グッド / 2.0) + (バッド / 3.0)) / 最大コンボ) * 最大スコア
                    score_num = (long)((((Notes_Controller.judg[0]) + (Notes_Controller.judg[1] / 2.0) +
                        (Notes_Controller.judg[2] / 3.0)) / JD.max_combo) * 1000000.0);

                    //スコア表示
                    score_text.text = score_num.ToString("0000000");
                }

                //ミスしたかどうか
                if (old_judg[i] != Notes_Controller.judg[i] && i == 3)
                {
                    //ミスしたので初期化
                    Grade.SetActive(false);
                    Combo.SetActive(false);

                    hp -= 5;
                    combo = 0;

                    //上下限を超えないようにする
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

        //HP関連
        {
            //HPが変わったかどうか
            if (old_hp != hp)
            {
                //ゲージの大きさと座標を変える
                var gage_pos = HP_Gage.transform.localPosition;
                var gage_ls = HP_Gage.transform.localScale;

                gage_pos.y = -3 + (hp * 0.03f);
                gage_ls.y = 1 * (hp / 100.0f);

                HP_Gage.transform.localPosition = gage_pos;
                HP_Gage.transform.localScale = gage_ls;

                //残りHP表示
                hp_text.text = hp.ToString("000") + "%";

                //古い情報を最新に
                old_hp = hp;

                //HPがなくなったらLostパネルを表示
                if (hp == 0)
                {
                    Time.timeScale = 0;  //止める
                    Countdown.Music.Pause();
                    Lost.SetActive(true);
                }
            }
        }

        //曲の再生時間
        if (end_percent == false)
        {
            //ゲージを伸ばす
            music_percent = (Countdown.Music.time / Countdown.Music.clip.length) * 100.0f;

            //100%なら終わり
            if (music_percent >= 100.0f)
            {
                music_percent = 100.0f;
                end_percent = true;
            }

            //ゲージの大きさと座標を変える
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
