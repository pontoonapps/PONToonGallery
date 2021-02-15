using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;


/// <summary>Controls interactable teleporting objects in the Demo scene.</summary>
[RequireComponent(typeof(Collider))]
public class VRMenuExitObject : MonoBehaviour
{
    /// <summary>
    /// The material to use when this object is inactive (not being gazed at).
    /// </summary>
    public Material inactiveMaterial;

    /// <summary>The material to use when this object is active (gazed at).</summary>
    public Material gazedAtMaterial;

    private Vector3 startingPosition;
    private Renderer myRenderer;

    public string SceneName = "WebMenu";

    /// <summary>Sets this instance's GazedAt state.</summary>
    /// <param name="gazedAt">
    /// Value `true` if this object is being gazed at, `false` otherwise.
    /// </param>
    public void SetGazedAt(bool gazedAt)
    {
        if (inactiveMaterial != null && gazedAtMaterial != null)
        {
            myRenderer.material = gazedAt ? gazedAtMaterial : inactiveMaterial;
        }

        if (gazedAt)
        {
            Invoke("LookAtFinish", 2.0f);
        }
        else
        {
            CancelInvoke("LookAtFinish");
        }
    }

    public void LookAtFinish()
    {
        GameObject po = GameObject.Find("PersistObject");

        if (po != null)
        {
            Destroy(po);
        }

        DisableVR();
        SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
    }

    void Teleport()
    {
        GameObject.FindGameObjectWithTag("Player").transform.position = transform.position;
        GameObject.Find("GameManager").GetComponent<GameManager>().UnhideTeleporters();
        gameObject.SetActive(false);
    }
    void Menu()
    {
        SceneManager.LoadScene("WebMenu", LoadSceneMode.Additive);
    }

    /// <summary>Resets this instance and its siblings to their starting positions.</summary>
    public void Reset()
    {
        int sibIdx = transform.GetSiblingIndex();
        int numSibs = transform.parent.childCount;
        for (int i = 0; i < numSibs; i++)
        {
            GameObject sib = transform.parent.GetChild(i).gameObject;
            sib.transform.localPosition = startingPosition;
            sib.SetActive(i == sibIdx);
        }
    }

    /// <summary>Calls the Recenter event.</summary>
    public void Recenter()
    {
#if !UNITY_EDITOR
        GvrCardboardHelpers.Recenter();
#else
        if (GvrEditorEmulator.Instance != null)
        {
            GvrEditorEmulator.Instance.Recenter();
        }
#endif  // !UNITY_EDITOR
    }

    /// <summary>Teleport this instance randomly when triggered by a pointer click.</summary>
    /// <param name="eventData">The pointer click event which triggered this call.</param>
    public void TeleportRandomly(BaseEventData eventData)
    {
        // Only trigger on left input button, which maps to
        // Daydream controller TouchPadButton and Trigger buttons.
        PointerEventData ped = eventData as PointerEventData;
        if (ped != null)
        {
            if (ped.button != PointerEventData.InputButton.Left)
            {
                return;
            }
        }

        // Pick a random sibling, move them somewhere random, activate them,
        // deactivate ourself.
        int sibIdx = transform.GetSiblingIndex();
        int numSibs = transform.parent.childCount;
        sibIdx = (sibIdx + Random.Range(1, numSibs)) % numSibs;
        GameObject randomSib = transform.parent.GetChild(sibIdx).gameObject;

        // Move to random new location ±90˚ horzontal.
        Vector3 direction = Quaternion.Euler(
            0,
            Random.Range(-90, 90),
            0) * Vector3.forward;

        // New location between 1.5m and 3.5m.
        float distance = (2 * Random.value) + 1.5f;
        Vector3 newPos = direction * distance;

        // Limit vertical position to be fully in the room.
        newPos.y = Mathf.Clamp(newPos.y, -1.2f, 4f);
        randomSib.transform.localPosition = newPos;

        randomSib.SetActive(true);
        gameObject.SetActive(false);
        SetGazedAt(false);
    }

    private void Start()
    {
        startingPosition = transform.localPosition;
        myRenderer = GetComponent<Renderer>();
        SetGazedAt(false);
    }

    IEnumerator LoadDevice(string newDevice, bool enable)
    {
        XRSettings.LoadDeviceByName(newDevice);
        yield return null;
        XRSettings.enabled = false;
        SceneManager.LoadScene("WebMenu");
    }

    void DisableVR()
    {
        StartCoroutine(LoadDevice("", true));
    }
}
