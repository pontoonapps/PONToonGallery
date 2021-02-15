using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.XR;

public class PrebuiltGalleries : MonoBehaviour
{
    public DropdownData[] Prebuilt;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Dropdown dd = GetComponent<Dropdown>();
        List<Dropdown.OptionData> od = new List<Dropdown.OptionData>();
        for(int i = 0; i < Prebuilt.Length; i++)
        {
            od.Add(new Dropdown.OptionData(Prebuilt[i].GalleryName));
        }
        dd.AddOptions(od);
    }

    public void DropdownValueChanged(int index)
    {
        GameObject.Find("PersistObject").GetComponent<PersistData>().TextureList = new List<Texture2D>(Prebuilt[index-1].ImageList);
        EnableVR();
        
    }

    IEnumerator LoadDevice(string newDevice, bool enable)
    {
        XRSettings.LoadDeviceByName(newDevice);
        yield return null;
        XRSettings.enabled = enable;
        SceneManager.LoadScene("Gallery1");
    }

    void EnableVR()
    {
        StartCoroutine(LoadDevice("Cardboard", true));
    }

    void DisableVR()
    {
        StartCoroutine(LoadDevice("", false));
    }
}
