using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EdgeCollection
{
    public List<GenerateNode> edges;
}

public class NodeData : MonoBehaviour
{
    // variables for spawning nodes
    public GameObject player;
    public float spawnThreshold = 400f;

    // variables for spawning pillar node
    public int count;
    public int countThreshold;
    public GameObject pillarEdge;
    public int pillarSpawnChance = 100;

    // variables for map pathfinding
    public List<GenerateNode> edgesFromPlayer;
    public List<EdgeCollection> branch;
    public int pillarIndex = 0;
    public bool drawPath;

    [SerializeField] private LineRenderer lr;
    public List<Transform> points;
    [SerializeField] private Vector3 offsetPosition;
    private bool isPastIntersectionNode;
    public bool isFirstIntersectionNode;
    [SerializeField] private Node intersectionNode;
    public bool istracePathFromPlayerNodeComplete;


    void Start()
    {
        branch = new List<EdgeCollection>();
    }

    public void SetIntersectionNode(Node intersectionNode)
    {
        if (!isFirstIntersectionNode)
        {
            this.intersectionNode = intersectionNode;
            isFirstIntersectionNode = true;
        }
    }

    void Update()
    {
        if (drawPath && branch.Count > 0 && istracePathFromPlayerNodeComplete)
        {
            SetPaths();
            drawPath = false;
            istracePathFromPlayerNodeComplete = false;
        }
    }

    private void SetPaths()
    {
        // assign points from player to the intersecting node
        for (int i = 0; i < edgesFromPlayer.Count; i++)
        {
            // check if past the intersection node 
            if (!isPastIntersectionNode)
            {
                points.Add(edgesFromPlayer[i].transform.GetChild(2).transform); // exit
                points.Add(edgesFromPlayer[i].transform.GetChild(1).transform); // entry

                // note: perform intersection node check after assigning edge points, as the last edge of this trace has reference to the intersection node, 
                //       doing the check before would prevent the last edge points of this trace from being added
                if (edgesFromPlayer[i].port.GetComponentInParent<Node>().isIntersectionNodeSet)
                {
                    isPastIntersectionNode = true;
                }
            }
        }
        isPastIntersectionNode = false; // reset flag

        List<GenerateNode> edgesFromPillar = GetShortestIntersectingListOfEdges();

        // assign points from pillar to the intersecting node
        for (int i = edgesFromPillar.Count - 1; i >= 0; i--)
        {
            points.Add(edgesFromPillar[i].transform.GetChild(1).transform); // entry
            points.Add(edgesFromPillar[i].transform.GetChild(2).transform); // exit
        }

        // set points for line renderer from points list 
        lr.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            lr.SetPosition(i, points[i].position + offsetPosition);
        }
    }

    private List<GenerateNode> GetShortestIntersectingListOfEdges()
    {
        // sub branch is a list of sublists of edges starting at the intersection node 
        List<EdgeCollection> subBranch = new List<EdgeCollection>();

        // build subset lists
        for (int i = 0; i < branch.Count; i++)
        {
            subBranch.Add(new EdgeCollection());
            subBranch[i].edges = new List<GenerateNode>();

            for (int j = 0; j < branch[i].edges.Count; j++)
            {
                // add edges to sublist
                subBranch[i].edges.Add(branch[i].edges[j]);
                                
                // check if at interstection node
                if (branch[i].edges[j].port.GetComponentInParent<Node>() == intersectionNode)
                {
                    break;
                }
            }
        }

        // initialize shortestListOfEdges
        List<GenerateNode> shortestListOfEdges = subBranch[0].edges;

        // get shortest sublist
        for (int i = 0; i < branch.Count; i++)
        {
            if (subBranch[i].edges.Count < shortestListOfEdges.Count)
            {
                shortestListOfEdges = subBranch[i].edges;
            }
        }

        return shortestListOfEdges;
    }
}