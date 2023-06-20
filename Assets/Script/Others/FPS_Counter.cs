using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS_Counter : MonoBehaviour
{
    public Text FPS_text;

    //FPS�v���p
    private float time_count;
    private float fps;
    private float prevTime;
    private float time;
    private int frame_count;

    // Start is called before the first frame update
    void Start()
    {
        frame_count = 0;
        prevTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.realtimeSinceStartup - prevTime;

        frame_count++;

        //0.5�b���ƂɌv��
        if (time >= 0.5f)
        {
            fps = frame_count / time;
            frame_count = 0;
            prevTime = Time.realtimeSinceStartup;

            FPS_text.text = "FPS�F" + fps.ToString("00");
        }
    }
}