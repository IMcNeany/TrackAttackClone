using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartMovement : MonoBehaviour
{
  
    public List<GameObject> placedTrackPieces;
    [SerializeField]
    private NodeList current_nodes;
    [SerializeField]
    Vector3 currentNodePos;
  public  List<Transform> nodesOnPiece;
    bool NextMovement = true;
    bool should_move = true;
    int track_index = 0;
    int node_index = 0;

    private NetworkGameManager _networkGameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        _networkGameManager = FindObjectOfType<NetworkGameManager>();
        nodesOnPiece = new List<Transform>();
        //  placedTrackPieces = new List<GameObject>();
        GetNextNodeList();
    }

    // Update is called once per frame
    void Update()
    {
        if (placedTrackPieces.Count > 0 && placedTrackPieces[0] == null)
        {
            _networkGameManager.ChangeControlsState(false);
            var temp = new List<GameObject>();
            var temp2 = new List<Transform>();
            nodesOnPiece = temp2;
            placedTrackPieces = temp;
            _networkGameManager.client_network.SendInfoCartOffTracks();
        }
        else if (placedTrackPieces.Count > 0)
        {
            MoveCart();


            //   if(should_move)
            //  {
            //     ElliotMoveCart();
            // }
        }
    }

    public void AddTrack(GameObject track)
    {
        if (track.GetComponent<NodeList>())
        {
            placedTrackPieces.Add(track);
        }
    }

    private void SetPlacedTracks()
    {

        current_nodes = placedTrackPieces[0].GetComponent<NodeList>();


    }

    private void GetNextNodeList()
    {
        if (placedTrackPieces.Count < 1)
        {
            return;
        }
        current_nodes = placedTrackPieces[track_index].GetComponent<NodeList>();
        node_index = 0;
    }

    private void ElliotMoveCart()
    {
        if(track_index > placedTrackPieces.Count)
        {
            //need next track nodes
            Debug.Log("Got here");
        }
        else
        {

            if(node_index >= current_nodes.nodes.Count)
            {
                placedTrackPieces.RemoveAt(0);
                GetNextNodeList();
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, current_nodes.nodes[node_index].position, 0.5f * Time.deltaTime);
                //transform.position = Vector3.Lerp(transform.position, current_nodes.nodes[node_index].position, 1.0f * Time.deltaTime);
                if (Vector3.Distance(transform.position, current_nodes.nodes[node_index].position) < 0.1f)
                {
                    node_index++;
                }
            }
            

        }
    }

    void MoveCart()
    {
       // NextMovement = false;
        //only need first one as it will get delected onced finished with
        NodeList nodeAccess = placedTrackPieces[0].GetComponent<NodeList>();
   
        if(nodeAccess.NodeCount() != 0)
        {
            if (nodesOnPiece.Count == 0)
            {
                for (int i = 0; i < nodeAccess.NodeCount(); i++)
                {
                    Transform Node = nodeAccess.GetNode(i);
                    nodesOnPiece.Add(Node);
                }
            }
            else
            {
                gameObject.transform.LookAt(nodesOnPiece[0]);
                currentNodePos = nodesOnPiece[0].transform.position;
                if (MoveToTarget(nodesOnPiece[0]))
                {
                    nodesOnPiece.RemoveAt(0);
                    if(nodesOnPiece.Count <= 0)
                    {
                        placedTrackPieces.RemoveAt(0);
                    }
                }
            }
                
           // placedTrackPieces.Remove(nodeAccess.GetNode(0).gameObject.transform.parent.gameObject);
          
            //get first node lerp car pos to that
            //get next noe, do same etc..
        }
       // NextMovement = true;
        //yield return null;
    }

    bool MoveToTarget(Transform node)
    {
        if (Vector3.Distance(transform.position, node.position) > 0.1f)
        {
            this.transform.position = Vector3.MoveTowards(transform.position, node.position, 3.0f * Time.deltaTime);
           
            return false;
        }

         return true;
    }
}
