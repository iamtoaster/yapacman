using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Maneuverable : MonoBehaviour
{
    public float moveSpeed = 1f;
    public Grid mapGrid;
    public Transform movePoint;
    public Vector3Int direction = new Vector3Int(1, 0, 0);

    protected Animator animPlayer;
    protected MovementLogic moveLogic;
    protected Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        if (gameObject.HasComponent<Animator>())
        {
            animPlayer = GetComponent<Animator>();
        } 
        else
        {
            throw new NotImplementedException("Generating Animator dynamically not supported. Add Animator with animations first.");
        }

        tilemap = mapGrid.GetComponentInChildren<Tilemap>();

        movePoint.parent = null;

        moveLogic = new MovementLogic(moveSpeed, mapGrid, movePoint, direction, animPlayer, gameObject);
    }
}
