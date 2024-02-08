using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class UIFunctionHandler : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private ARAnchorPlacer anchorPlacer;
    [SerializeField] private TextMeshProUGUI sliderValTMP;

    // Start is called before the first frame update
    void Start()
    {
        settingsPanel.SetActive(false);
        StartCoroutine(LateStartCoroutine());
    }

    public void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void OnSliderValueChanged(float val)
    {
        anchorPlacer.SetForwardOffset(val);
        sliderValTMP.text = val.ToString("F2");
    }

    public void TogglePointCloudVisibility(bool val)
    {
        Debug.Log("Toggle Point Cloud: " + val);
        anchorPlacer.GetComponentInChildren<ARPointCloud>(true).gameObject.SetActive(val);
    }

    IEnumerator LateStartCoroutine()
    {
        yield return null;
        anchorPlacer.GetComponentInChildren<ARPointCloud>(true).gameObject.SetActive(false);
    }
}
