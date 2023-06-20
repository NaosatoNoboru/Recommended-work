using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public AudioSource audio_source;//SE用
    public AudioClip SE;

    public FadeController fade_controller;

    private bool select;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        //エンターキーを押したら
        if (Input.GetKeyDown(KeyCode.Return) && select == false && fade_controller.isFadeIn == false)
        {
            audio_source.PlayOneShot(SE);

            fade_controller.isFadeOut = true;
            select = true;
        }

        //シーン遷移
        if (select == true && fade_controller.isFadeOut == false)
        {
            SceneManager.LoadScene("Select");
        }
    }
}
