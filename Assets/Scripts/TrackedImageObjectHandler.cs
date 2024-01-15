using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class TrackedImageObjectHandler : MonoBehaviour
{
    [SerializeField] private GameObject virtualObjPrefab;
    [SerializeField] private float yOffset = 0.05f;

    public void OnTrackedImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var image in eventArgs.added)
        {
            // Handle added event
            Debug.Log("Tracked new Image: " + image.referenceImage.name + " | Tracking State: " + image.trackingState);

            GameObject virtualObj = GameObject.Instantiate(virtualObjPrefab);
            virtualObj.transform.parent = image.gameObject.transform;
            virtualObj.transform.localPosition = new Vector3(0, yOffset, 0);
        }

        foreach (var image in eventArgs.updated)
        {
            // Handle updated event
            Debug.Log("Updated Image: " + image.referenceImage.name + " | Tracking State: " + image.trackingState);
        }

        foreach (var image in eventArgs.removed)
        {
            // Handle removed event
            Debug.Log("Removed Image: " + image.referenceImage.name + " | Tracking State: " + image.trackingState);
        }
    }
}
