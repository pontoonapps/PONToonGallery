using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FadeScript : MonoBehaviour
{

    public float FadeOutTime = 0.5f;
    public float FadeInTime = 0.5f;

    private bool FadingOut = false;
    private bool FadingIn = false;
    private float timer = 0.0f;
    Image imageRef;
    private Texture2D _texture;
    private Color theColour = new Color();
    void Start()
    {
        imageRef = GetComponent<Image>();
    }

    void Update()
    {
        if (FadingOut)
        {
            imageRef.color = new Color(0.0f, 0.0f, 0.0f, Mathf.Lerp(0.0f, 1.0f, timer / FadeOutTime));
            timer += Time.deltaTime;
        }
        else if (FadingIn)
        {
            imageRef.color = new Color(0.0f, 0.0f, 0.0f, Mathf.Lerp(1.0f, 0.0f, timer / FadeInTime));
            timer += Time.deltaTime;
        }
    }

    /*void OnGUI()
    {
        if (_texture == null) _texture = new Texture2D(1, 1);

        _texture.SetPixel(0, 0, theColour);
        _texture.Apply();

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _texture);
    }*/

    public void StartFade()
    {
        FadingOut = true;
        Invoke("StartFadeIn", FadeOutTime);
        timer = 0.0f;
    }

    void StartFadeIn()
    {
        FadingOut = false;
        FadingIn = true;
        timer = 0.0f;
        Invoke("FinishFade", FadeInTime);
    }

    void FinishFade()
    {
        FadingIn = false;
    }
}
