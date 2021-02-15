using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] Canvases;

    public float MaxWidth = 100.0f;
    public float MaxHeight = 100.0f;

    PersistData persistData = null;

    public GameObject[] Teleporters;

    public bool ShouldEnableVR = false;
    public GoogleVR.Demos.DemoInputManager InputManagerRef;

    private void Awake()
    {
        if (ShouldEnableVR)
            EnableVR();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject go = GameObject.Find("PersistObject");
        if (go != null)
            persistData = go.GetComponent<PersistData>();

        if (persistData == null)
        {
            Debug.Log("No persistance data found");
        }
        else
        {
            for(int i = 0; i < persistData.TextureList.Count; i++)
            {
                if (i >= Canvases.Length)
                {
                    break;
                }

                float aspect = (float)persistData.TextureList[i].width / (float)persistData.TextureList[i].height;

                if (aspect > 1.0f) // widescreen
                {
                    Canvases[i].transform.localScale = new Vector3(MaxWidth, 100.0f, MaxWidth / aspect);
                }
                else // long screen?
                {
                    Canvases[i].transform.localScale = new Vector3(MaxHeight * aspect, 100.0f, MaxHeight);
                }

                Canvases[i].GetComponentInChildren<Renderer>().material.mainTexture = persistData.TextureList[i];
            }
        }

        Teleporters = GameObject.FindGameObjectsWithTag("Teleporter");
        Cursor.visible = false;
    }

    IEnumerator LoadDevice(string newDevice, bool enable)
    {
        XRSettings.LoadDeviceByName(newDevice);
        yield return null;
        XRSettings.enabled = enable;
        //InputManagerRef.Init();
    }

    void EnableVR()
    {
        StartCoroutine(LoadDevice("Cardboard", true));
    }

    void DisableVR()
    {
        StartCoroutine(LoadDevice("", false));
    }

    public void UnhideTeleporters()
    {
        foreach(GameObject tp in Teleporters)
        {
            tp.SetActive(true);
        }
    }
}
