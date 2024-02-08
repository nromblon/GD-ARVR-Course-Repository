using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARNavigationManager : MonoBehaviour
{
    // Singleton
    public static ARNavigationManager Instance;

    [SerializeField] private GameObject levelPrefab;
    [SerializeField] private Vector3 levelOffset = new Vector3(0, .05f, 0);
    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits;

    public bool HasLevelPlaced { get; private set; }
    public bool IsBeaconVisible { get; private set; }

    private GameObject beaconObject;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        hits = new List<ARRaycastHit>();
        HasLevelPlaced = false;
        IsBeaconVisible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!HasLevelPlaced)
            CheckPlanePlacement();
    }

    public void CheckPlanePlacement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(r.origin, r.direction, Color.red, 1.5f);

            // Physics Raycast first to check if tapping on an existing virtual object
            RaycastHit hitInfo;
            if (Physics.Raycast(r, out hitInfo))
            {
                if (hitInfo.collider.gameObject.CompareTag("Removable"))
                {
                    Destroy(hitInfo.collider.gameObject.GetComponentInParent<ARAnchor>().gameObject);
                    return; // exit out of update
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
                        AnchorLevel(hit.pose.position);
                        break;  // break, so only checks up until the first valid plane
                    }
                }
            }
        }
    }

    private void AnchorLevel(Vector3 worldPos)
    {
        GameObject newAnchor = new GameObject("Anchor");
        newAnchor.transform.parent = null;
        newAnchor.transform.position = worldPos;
        newAnchor.AddComponent<ARAnchor>();

        GameObject content = Instantiate(levelPrefab, newAnchor.transform);
        content.transform.localPosition = levelOffset;

        HasLevelPlaced = true;
    }

    public Vector3 GetBeaconPosition()
    {
        if (beaconObject != null)
            return beaconObject.transform.position;
        else
            return Vector3.zero;
    }

    public void SetBeaconObject(GameObject obj)
    {
        this.beaconObject = obj;
        IsBeaconVisible = true;
    }
}
