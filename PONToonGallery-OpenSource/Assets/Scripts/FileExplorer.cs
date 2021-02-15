using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using System;
using UnityEngine.Android;

public class FileExplorer : MonoBehaviour
{
    public GameObject FolderPrefab;
    public GameObject FilePrefab;

    List<GameObject> CurrentButtons = new List<GameObject>();

    public List<Texture2D> LoadedTextures = new List<Texture2D>();
    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
            }
            string s = GetAndroidExternalStoragePath();
            Debug.Log(s);
            UpdateButtonList(s);
        }
        else
        {
            UpdateButtonList(@"C:\");
        }
        //DisableVR();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateButtonList(string path)
    {
        ClearButtons();
        DirectoryInfo di = Directory.GetParent(path);
        string[] FolderStrings = Directory.GetDirectories(path);
        string[] FileStrings = Directory.GetFiles(path);

        char slash = '\\';

        if (Application.platform == RuntimePlatform.Android)
        {
            slash = '/';
        }

        //Parent button
        if (di != null)
        {
            GameObject parentBtn = Instantiate(FolderPrefab, transform);
            parentBtn.GetComponent<ExplorerButton>().Init(this);
            parentBtn.GetComponent<ExplorerButton>().path = di.FullName;
            parentBtn.GetComponentInChildren<Text>().text += "...";
            CurrentButtons.Add(parentBtn);
        }

        foreach (string s in FolderStrings)
        {
           
            GameObject newBtn = Instantiate(FolderPrefab, transform);
            newBtn.GetComponent<ExplorerButton>().Init(this);
            newBtn.GetComponent<ExplorerButton>().path = s;
            string[] splitString = s.Split(slash);
            newBtn.GetComponentInChildren<Text>().text += splitString[splitString.Length-1];
            CurrentButtons.Add(newBtn);
        }

        foreach (string s in FileStrings)
        {
            Debug.Log(s);
            GameObject newBtn = Instantiate(FilePrefab, transform);
            newBtn.GetComponent<ExplorerButton>().Init(this);
            newBtn.GetComponent<ExplorerButton>().path = s;
            string[] splitString = s.Split(slash);
            newBtn.GetComponentInChildren<Text>().text += splitString[splitString.Length - 1];
            CurrentButtons.Add(newBtn);
        }
    }

    public void FolderClicked(string s)
    {
        UpdateButtonList(s);
    }

    public void FileClicked(string s)
    {
        Debug.Log("Clicked on file: " + s);


        string[] splitString = s.Split('.');
        
        //if (splitString[splitString.Length - 1] == "png")
        {
            Texture2D newTex = LoadPNG(s);

            if(newTex != null)
            {
                LoadedTextures.Add(newTex);
                Debug.Log("new texture loaded");
            }
        }
    }

    public void ClearButtons()
    {
        foreach (GameObject btn in CurrentButtons)
        {
            Destroy(btn);
        }
        CurrentButtons.Clear();
    }

    public static Texture2D LoadPNG(string filePath)
    {
        Texture2D tex = null;

        byte[] fileData;

        if(File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2, TextureFormat.BGRA32, false);
            if(!tex.LoadImage(fileData))
            {
                tex = null;
            }
        }

        return tex;
    }

    public void GoToGallery()
    {
        GameObject.Find("PersistObject").GetComponent<PersistData>().TextureList = LoadedTextures;
        //SceneManager.LoadScene("Gallery1");
        EnableVR();
    }

    public string GetAndroidExternalStoragePath()
    {
        string path = "";
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("android.os.Environment");
            path = jc.CallStatic<AndroidJavaObject>("getExternalStorageDirectory").Call<string>("getAbsolutePath");
            
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        return path;
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
