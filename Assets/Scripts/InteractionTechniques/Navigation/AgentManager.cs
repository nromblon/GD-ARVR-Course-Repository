using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    List<ARAgent> agents;

    // Start is called before the first frame update
    void Start()
    {
        agents = new List<ARAgent>(GetComponentsInChildren<ARAgent>());
    }

    // Update is called once per frame
    void Update()
    {
        if (ARNavigationManager.Instance.HasLevelPlaced)
        {
            Debug.Log("level has been placed");
            //Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            Ray r = new Ray(Camera.main.transform.position, (ARNavigationManager.Instance.GetBeaconPosition() - Camera.main.transform.position).normalized);
            Debug.DrawRay(r.origin, r.direction, Color.cyan, 1.5f);
            RaycastHit hitInfo;
            if (Physics.Raycast(r, out hitInfo))
            {
                if (hitInfo.collider.CompareTag("Plane"))
                    MoveAllAgents(hitInfo.point);
            }
        }
    }

    public void MoveAllAgents(Vector3 position)
    {
        foreach (ARAgent agent in agents)
            agent.MoveAgent(position);
    }

    public void StopAllAgents()
    {
        foreach (ARAgent agent in agents)
            agent.StopAgent();
    }
}
