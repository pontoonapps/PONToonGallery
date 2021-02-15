using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonType
{
    Folder,
    File
};

public class ExplorerButton : MonoBehaviour
{
    public ButtonType theButtonType;

    public string path;

    private FileExplorer feRef = null;

    public void Init(FileExplorer feRef)
    {
        this.feRef = feRef;
    }
    public void OnClickedFolderBtn()
    {
        feRef.FolderClicked(path);
    }

    public void OnClickedFileBtn()
    {
        feRef.FileClicked(path);
    }
}
