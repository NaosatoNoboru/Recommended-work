using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notes_Decision_Collar : MonoBehaviour
{
    public static AudioSource Tap_audio_source;//SE用

    public GameObject[] notes = new GameObject[4];//ノーツ
    public GameObject[] mask_notes = new GameObject[4];//マスクノーツ
    public GameObject[] particle_original = new GameObject[4];//パーティクル

    public Sprite[] neutral_notes_collar = new Sprite[4];//通常時のノーツの色
    public Sprite[] airtap_notes_collar = new Sprite[4];//空打ちした時のノーツの色
    public Sprite[] tap_notes_collar = new Sprite[4];//ノーツを押した時の色

    private List<GameObject> particle_list = new List<GameObject>();//パーティクル
    private int[] judg = new int[3];//判定数

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
            //どの判定にいるか
            if (Notes_Controller.tap_notes[i] == true)
            {
                //ノーツを押せている
                notes[i].GetComponent<SpriteRenderer>().sprite = tap_notes_collar[i];
                mask_notes[i].SetActive(true);
            }
            else if (Notes_Controller.airtap_notes[i] == true)
            {
                //空打ち
                notes[i].GetComponent<SpriteRenderer>().sprite = airtap_notes_collar[i];
            }
            else
            {
                //通常
                notes[i].GetComponent<SpriteRenderer>().sprite = neutral_notes_collar[i];
                mask_notes[i].SetActive(false);
            }
        }

        int nc_judg = Notes_Controller.judg[0] + Notes_Controller.judg[1] + Notes_Controller.judg[2];

        //bad以上の判定だったらパーティクルを出す
        if (judg[0] + judg[1] + judg[2] != nc_judg)
        {
            var judg_all = nc_judg - (judg[0] + judg[1] + judg[2]);

            if (judg_all > 4)
            {
                judg_all = 4;
            }

            //判定した回数だけ回す
            for (int i = 0; i < judg_all; i++)
            {
                particle_list.Add(Instantiate(particle_original[Notes_Controller.judg_lane[i]]));

                particle_list[particle_list.Count - 1].transform.position =
                    notes[Notes_Controller.judg_lane[i]].transform.position;

                //初期化
                Notes_Controller.judg_lane[i] = -1;
            }

            //変化した分を反映
            for (int i = 0; i < 3; i++)
            {
                judg[i] = Notes_Controller.judg[i];
            }
        }
    }
}
