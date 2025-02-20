using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateNode : MonoBehaviour
{
    [SerializeField] private List<GameObject> nodePrefabs;
    public GameObject newNode;
    public List<GameObject> nodeList;
    private int nodeIndex = 0;
    public Transform edgeExit;
    public bool isColliding = false;
    private bool isActive = false;
    public bool allowCollisionCheck = true;
    public int portIndex = 0;
    [SerializeField] private List<int> spawnChance;
    private bool resetNode = false;

    //note: reference to the port at which the edge spawned      
    public GenerateEdge port;

    private NodeData nodeDataScript;
    public int count;


    public bool tracePathFromPillarNode;
    public bool tracePathFromPlayerNode;

    void Start()
    {
        nodeDataScript = GameObject.FindWithTag("NodeData").GetComponent<NodeData>();

        //initialize list of nodes
        foreach (GameObject node in nodePrefabs)
        {
            nodeList.Add(node);
        }

        // add pillar node to list of nodes to generate after certain number of nodes have been generated
        if (nodeDataScript.count >= nodeDataScript.countThreshold)
        {
            nodePrefabs.Add(nodeDataScript.pillarEdge);
            nodeList.Add(nodeDataScript.pillarEdge);
            spawnChance.Add(nodeDataScript.pillarSpawnChance);

        }

        StartCoroutine(CheckCollision());

    }

    // Update is called once per frame
    void Update()
    {
        // //respawn node after despawn
        // if (Vector3.Distance(transform.position, nodeDataScript.player.transform.position) < nodeDataScript.spawnThreshold && resetNode)
        // {
        //     //clear pre-existing node list
        //     while (nodeList.Count != 0)
        //     {
        //         nodeList.RemoveAt(0);
        //     }

        //     //initialize list of nodes
        //     foreach (GameObject node in nodePrefabs)
        //     {
        //         nodeList.Add(node);
        //     }
        //     StartCoroutine(CheckCollision());
        //     resetNode = false;
        // }

        if (newNode != null)
        {
            if (newNode.GetComponent<Node>().isColliding && newNode.GetComponent<Node>().portList.Count == 1)
            {
                //no ports left for which the node will be non-colliding, destroy and remove node from node list
                Destroy(newNode);
                nodeList.RemoveAt(nodeIndex);
                spawnChance.RemoveAt(nodeIndex);


                if (nodeList.Count != 0)
                {
                    //regenerate different node from node list
                    newNode = GenerateRandomNode();
                }
            }
        }


    }

    IEnumerator CheckCollision()
    {
        //wait for OnTrigger callbacks
        yield return new WaitForFixedUpdate(); ;

        //check if not colliding, set mesh active and disable collision checking
        if (!isColliding && !isActive)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            isActive = true;
            allowCollisionCheck = false;

            //start generate node
            if (nodeList.Count != 0)
            {
                newNode = GenerateRandomNode();
            }
        }
        else
        {
            //if colliding, loop
            StartCoroutine(CheckCollision());
        }
    }

    private GameObject GenerateRandomNode()
    {
        //store node selection
        nodeIndex = RandomWeightedGenerator.GenerateRandomIndex(spawnChance.ToArray());

        //generate node
        GameObject node = nodeList[nodeIndex];

        //store port selection
        portIndex = Random.Range(0, node.GetComponent<Node>().portList.Count);

        Vector3 nodeEntranceOffsetPos = CalculateNodeEntranceOffset(node, portIndex, edgeExit);
        GameObject newNode = Instantiate(node, edgeExit.position + nodeEntranceOffsetPos, node.transform.rotation);

        newNode.GetComponent<Node>().edge = this;

        return newNode;
    }

    public static Vector3 CalculateNodeEntranceOffset(GameObject node, int portIndex, Transform edgeExit)
    {

        //rotate node to point in direction of edge exit, and add opposite local angle of port as global angle to node angle 
        Quaternion mirrorRotation = edgeExit.rotation * Quaternion.Euler(0, 180, 0);
        node.transform.rotation = mirrorRotation * Quaternion.Inverse(node.GetComponent<Node>().portList[portIndex].localRotation);

        //calculate offset position 
        return node.transform.position - node.GetComponent<Node>().portList[portIndex].position;
    }

    private void OnTriggerStay(Collider other)
    {
        if (allowCollisionCheck && other.gameObject.CompareTag("Node"))
        {
            isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (allowCollisionCheck && other.gameObject.CompareTag("Node"))
        {
            isColliding = false;
        }
    }
}
