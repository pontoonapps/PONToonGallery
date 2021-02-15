using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistData : MonoBehaviour
{
    public List<Texture2D> TextureList;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
