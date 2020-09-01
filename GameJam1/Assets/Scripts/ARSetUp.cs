using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARSetUp : MonoBehaviour
{
    public GameObject ARBoard;
    public GameObject canvas;
    private ARRaycastManager raycastManager;
    bool objPlaced = false;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    public NetworkGameManager networkManager;
    private ClientNetwork network;
    public bool in_editor = false;
    public GameObject grid;
    public ARPlaneManager plane_manager;
    public ARPointCloudManager particles;
    public GameObject environment;
    public GameObject youcanvas;
    private void Awake()
    {
        network = FindObjectOfType<ClientNetwork>();
    }
    // Start is called before the first frame update
    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 0)
        {
            return;
        }

        if (objPlaced == false)
        {
            var touch = Input.GetTouch(0);

            if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
            {
                grid.transform.position = hits[0].pose.position;
                //grid.transform.rotation = hits[0].pose.rotation;
                environment.SetActive(true);
                youcanvas.SetActive(true);
                networkManager.SetClientGrid(grid.GetComponent<ClientGrid>());
                networkManager.SetupScene();
                networkManager.AssignClientPlayer(network.player_id);
                networkManager.AssignNetworkPlayer(network.connected_user_count);
                objPlaced = true;
                plane_manager.enabled = false;
                particles.enabled = false;

            }
        }
    }
}
