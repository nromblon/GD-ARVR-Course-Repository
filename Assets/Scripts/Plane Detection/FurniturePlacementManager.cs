using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;

public class FurniturePlacementManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> contentPrefabs;
    [SerializeField] private Vector3 contentOffset = new Vector3(0, .05f, 0);
    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits;

    private int selectedContent = 0;
    private GameObject draggingObject = null;
    private Vector3 mousePositionOnDrag = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        hits = new List<ARRaycastHit>();
        raycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPointerOverUIObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleTap();
        }

        if (draggingObject != null)
        {
            Debug.Log("dragging...");
            Ray r = Camera.main.ScreenPointToRay(mousePositionOnDrag);
            Debug.DrawRay(r.origin, r.direction, Color.red, 3f);
            // If raycast is currently hitting a horizontal upright plane
            //if (activeRaycast.plane != null && activeRaycast.plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp)
            //{
            //    draggingObject.transform.position = activeRaycast.pose.position;
            //}
            if (raycastManager.Raycast(r, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                foreach (ARRaycastHit hit in hits)
                {
                    // Check if plane hit is aligned horizontally (up)
                    if (hit.trackable is ARPlane plane && plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp)
                    {
                        draggingObject.transform.position = hit.pose.position;
                    }
                }
            }
        }
    }

    private void HandleTap()
    {
        // If dragging, stop dragging and remove persistent raycast.
        if (draggingObject != null)
        {
            Debug.Log("Remove Drag");
            //raycastManager.RemoveRaycast(activeRaycast);
            //activeRaycast = null;
            DetachAnchor(draggingObject);
            AnchorContent(draggingObject.transform.position, draggingObject);
            mousePositionOnDrag = Vector3.zero;
            draggingObject = null;
            return; // exit out of HandleTap
        }

        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(r.origin, r.direction, Color.red, 1.5f);

        // Physics Raycast to check if tapping on an existing virtual object
        RaycastHit hitInfo;
        if (Physics.Raycast(r, out hitInfo))
        {
            if (hitInfo.collider.gameObject.CompareTag("Furniture"))
            {

                Debug.Log("Hit Furniture");
                draggingObject = hitInfo.collider.gameObject;
                mousePositionOnDrag = Input.mousePosition;
                //activeRaycast = raycastManager.AddRaycast(Vector2.zero, 1f);
                //Debug.Log("Creating raycast: " + activeRaycast);
                return; // exit out of HandleTap
            }

        }
        // Raycast only checks against plane within polygon trackables
        if (raycastManager.Raycast(r, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            Debug.Log($"Hit Count: {hits.Count}");
            foreach (ARRaycastHit hit in hits)
            {
                // Check if plane hit is aligned horizontally (up)
                if (hit.trackable is ARPlane plane && plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp)
                {
                    AnchorContent(hit.pose.position);
                    break;  // break, so only checks up until the first valid plane
                }
            }
        }
    }

    private void AnchorContent(Vector3 worldPos, GameObject contentObj=null)
    {
        GameObject newAnchor = new GameObject("Anchor");
        newAnchor.transform.parent = null;
        newAnchor.transform.position = worldPos;
        newAnchor.AddComponent<ARAnchor>();

        if (contentObj != null)
            contentObj.transform.SetParent(newAnchor.transform, true);
        else
        {
            GameObject content = Instantiate(contentPrefabs[selectedContent], newAnchor.transform);
            content.transform.localPosition = contentOffset;
        }
    }

    private void DetachAnchor(GameObject contentObject)
    {
        GameObject anchor = contentObject.transform.parent.gameObject;

        contentObject.transform.parent = null;

        Destroy(anchor);
    }

    public void SetSelectedFurniture(int idx)
    {
        selectedContent = idx;
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
