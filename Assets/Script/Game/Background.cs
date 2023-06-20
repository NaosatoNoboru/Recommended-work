using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Background : MonoBehaviour
{
    public GameObject sub_background;
    public string background_name;

    private Json_Data JD;

    private Texture2D texture;
    private string extension;
    private Sprite sprite;
    private byte[] background_data;

    // Start is called before the first frame update
    void Start()
    {
        //îwåiÇÃñºëOì«Ç›çûÇ›
        JD = Music_Json.Load_Json();

        if (JD.background != null)
        {
            try
            {
                //ÉoÉCÉgÇ…ÇµÇƒì«Ç›çûÇﬁ
                background_name = Application.dataPath + "/Background/" + JD.background;
                texture = new Texture2D(2, 2);
                background_data = File.ReadAllBytes(background_name);
                texture.LoadImage(background_data);

                //îΩâf
                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                this.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;

            }
            catch
            {
                sub_background.SetActive(true);
            }
        }
        else
        {
            sub_background.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
