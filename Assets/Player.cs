using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 1f;
    public Grid mapGrid;
    public Transform movePoint;
    public Vector3Int direction = new Vector3Int(1, 0, 0);

    private Animator animPlayer;
    private MovementLogic moveLogic;


    // Start is called before the first frame update
    void Start()
    {
        animPlayer = GetComponent<Animator>();
        movePoint.parent = null;

        moveLogic = new MovementLogic(moveSpeed, mapGrid, movePoint, direction, animPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        moveLogic.ProcessMovement(this.gameObject, Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}