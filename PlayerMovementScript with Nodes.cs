﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour {

    [SerializeField]
    private float speed = 3.0f;
    [SerializeField]
    private Sprite idleSprite;

    private Vector2 direction = Vector2.zero;
    private Vector2 nextDirection;

    [SerializeField]
   private Node currentNode;
    private Node previousNode, targetNode;


    // Use this for initialization
    void Start() {

        Node node = GetNodeAtPosition (transform.localPosition);
        if (node != null) {
            currentNode = node;
            Debug.Log (currentNode);
        }

        direction = Vector2.left;
        ChangePosition(direction);
    }

    // Update is called once per frame
    void Update() {

        CheckInput();

        Move ();

        UpdateOrientation();

        UpdateAnimationState();
    }

    void CheckInput(){
        if (Input.GetKeyDown(KeyCode.A)){
            ChangePosition(Vector2.left);

        } else if (Input.GetKeyDown(KeyCode.D)){
            ChangePosition(Vector2.right);

        } else if (Input.GetKeyDown(KeyCode.W)){
            ChangePosition(Vector2.up);

        } else if (Input.GetKeyDown(KeyCode.S)){
            ChangePosition(Vector2.down);

        }
    }

    void ChangePosition (Vector2 d)
    {
        if (d != direction)
            nextDirection = d;

        if(currentNode != null)
        {
            Node moveToNode = CanMove(d);

            if(moveToNode != null)
            {
                direction = d;
                targetNode = moveToNode;
                previousNode = currentNode;
                currentNode = null;
            }
        }
    }

    void Move(){
        if (targetNode != currentNode && targetNode != null)
        {
            if (nextDirection == direction * -1)
            {
                direction *= -1;

                Node tempNode = targetNode;
                targetNode = previousNode;
                previousNode = tempNode;
            }
            if (OverShotTarget())
            {
                currentNode = targetNode;

                transform.localPosition = currentNode.transform.position;
                Node moveToNode = CanMove(nextDirection);
                if (moveToNode != null)
                    direction = nextDirection;
                if (moveToNode != null)
                    moveToNode = CanMove(direction);
                if (moveToNode != null)
                {
                    targetNode = moveToNode;
                    previousNode = currentNode;
                    currentNode = null;
                }
                else { direction = Vector2.zero; }

            }
            else { transform.localPosition += (Vector3)(direction * speed) * Time.deltaTime; }
        }
        
    }

    void MoveToNode (Vector2 d) {

        Node moveToNode = CanMove(d);

        if (moveToNode != null) {

            transform.localPosition = moveToNode.transform.position;
            currentNode = moveToNode;
        }
    }

    void UpdateOrientation()
    {
        if (direction == Vector2.left)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction == Vector2.right)
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction == Vector2.up)
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (direction == Vector2.down)
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 270);
        }
    }

    void UpdateAnimationState()
    {
        if(direction == Vector2.zero)
        {
            GetComponent<Animator>().enabled = false;
            GetComponent<SpriteRenderer>().sprite = idleSprite;
        }
        else
        {
            GetComponent<Animator>().enabled = true;
        }
    }

    Node CanMove(Vector2 d)
    {
        Node moveToNode = null;

        for (int i = 0; i < currentNode.neighbors.Length; i++)
        {
            if (currentNode.validDirections[i] == d)
            {
                moveToNode = currentNode.neighbors[i];
                break;
            }
           
        }
        return moveToNode;
    }

    Node GetNodeAtPosition(Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];
        if (tile != null)
        {
            return tile.GetComponent<Node>();
        }
        return null;
    }

    bool OverShotTarget()
    {
        float nodeToTarget = LengthFromNode(targetNode.transform.position);
        float nodeToSelf = LengthFromNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }
    float LengthFromNode (Vector2 targetPosition)
    {
        Vector2 vec = targetPosition - (Vector2)previousNode.transform.position;
        return vec.sqrMagnitude;
    }
}