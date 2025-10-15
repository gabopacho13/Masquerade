using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PositionNode : MonoBehaviour
{
    public List<GameObject> positionNodes = new List<GameObject>();
    public float problemAngle = 180.0f;
    private List<bool> isProblem = new List<bool>();
    public GameObject BestNode { get; private set; } = null;
    private Transform player;
    public bool isFinalNode = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        BestNode = positionNodes[0];
        for (int i = 0; i < positionNodes.Count; i++)
        {
            isProblem.Add(false);
        }
    }

    private void Update()
    {
        List<float> angles = new List<float>();
        for (int i = 0; i < positionNodes.Count; i++)
        {
            GameObject node = positionNodes[i];
            Vector3 dirToNode = (node.transform.position - transform.position).normalized;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(dirToNode, dirToPlayer);
            angles.Add(angleToPlayer);
            isProblem[i] = angleToPlayer <= problemAngle*0.5;
        }
        if (!isProblem.Contains(false))
        {
            int leastInfluencedIndex = 0;
            float maxAngle = angles[0];

            for (int i = 1; i < angles.Count; i++)
            {
                if (angles[i] > maxAngle)
                {
                    maxAngle = angles[i];
                    leastInfluencedIndex = i;
                }
            }

            // Resaltar ese nodo como "especial"
            isProblem[leastInfluencedIndex] = false;
        }
        BestNode = positionNodes[isProblem.IndexOf(false)];
    }
}
