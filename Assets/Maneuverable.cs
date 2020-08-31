using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Maneuverable : MonoBehaviour
{
    public float moveSpeed = 1f;
    public Grid mapGrid;
    public Transform movePoint;
    public Vector3Int direction = new Vector3Int(1, 0, 0);

    public Animator animPlayer;
    protected Tilemap tilemap;

    internal MovementLogic moveLogic;

    private Vector3 startingPosition;
    private Vector3Int startingDirection;

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


        startingPosition = transform.position;
        startingDirection = moveLogic.direction;
    }

    public void Reset()
    {
        transform.position = startingPosition;
        moveLogic.direction = startingDirection;
    }

    public Vector3Int CurrentCell => moveLogic.GetCurrentCell();

    public Vector3Int Direction => moveLogic.direction;
}
