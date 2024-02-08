using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;

public class ARAnchorPlacer : MonoBehaviour
{
    [SerializeField] private GameObject contentPrefab;
    [SerializeField] private float forwardOffset = 0.8f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            Vector3 spawnPos = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(forwardOffset);

            AnchorObject(spawnPos);
        }
    }

    public void SetForwardOffset(float val)
    {
        forwardOffset = val;
    }

    public void AnchorObject(Vector3 position)
    {
        GameObject newAnchor = new GameObject("New Anchor");
        newAnchor.transform.parent = null;
        newAnchor.transform.position = position;
        newAnchor.AddComponent<ARAnchor>();

        GameObject content = Instantiate(contentPrefab, newAnchor.transform);
        content.transform.localPosition = Vector3.zero;
    }

    public void OnTrackablesAdded(ARTrackablesChangedEventArgs<ARAnchor> args)
    {
        foreach (ARAnchor anchor in args.added)
        {
            Debug.Log("New anchor added - " + " State: " + anchor.trackingState);
        }

        foreach (ARAnchor anchor in args.updated)
        {
            Debug.Log("Anchor updated - " + " State: " + anchor.trackingState);
        }

        foreach (ARAnchor anchor in args.removed)
        {
            Debug.Log("Anchor removed");
        }
    }

    /**
     * Code Taken from: https://discussions.unity.com/t/detect-if-pointer-is-over-any-ui-element/138619
     */
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
