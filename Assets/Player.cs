using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public float moveSpeed = 1f;
    public Grid mapGrid;
    public Transform movePoint;
    public Vector3Int direction = new Vector3Int(1, 0, 0);

    private Animator animPlayer;

    // Start is called before the first frame update
    void Start()
    {
        animPlayer = GetComponent<Animator>();
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int currCell = mapGrid.WorldToCell(transform.position) + new Vector3Int(0, 1, 0);

        var tilemap = mapGrid.GetComponentInChildren<Tilemap>();

        Vector3Int targetCell = currCell + direction;

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.5f)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                Vector3Int dir = new Vector3Int((int)Input.GetAxisRaw("Horizontal"), 0, 0);
                if (CheckCell(currCell + dir, tilemap))
                {
                    targetCell = currCell + dir;
                    direction = dir;
                }
            }
            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                Vector3Int dir = new Vector3Int(0, (int)Input.GetAxisRaw("Vertical"), 0);
                if (CheckCell(currCell + dir, tilemap))
                {
                    targetCell = currCell + dir;
                    direction = dir;
                }
            }
        }

        tilemap.SetTile(new Vector3Int(0, 0, 0), tilemap.GetTile(targetCell));

        if (CheckCell(targetCell, tilemap))
        {
            movePoint.position = mapGrid.CellToWorld(targetCell) + new Vector3(0.25f, -0.25f, 0);
        }

        // wrap around the screen
        if (currCell.x <= -1)
        {
            transform.position = mapGrid.CellToWorld(new Vector3Int(27, -17, 0)) + new Vector3(0.25f, -0.25f, 0);
            targetCell = new Vector3Int(26, -17, 0);
            direction = new Vector3Int(-1, 0, 0); // to the left

        }
        else if (currCell.x >= 28)
        {
            transform.position = mapGrid.CellToWorld(new Vector3Int(0, -17, 0)) + new Vector3(0.25f, -0.25f, 0);
            targetCell = new Vector3Int(1, -17, 0);
            direction = new Vector3Int(1, 0, 0); // to the right
        }

        // switch didn't work unfortunately
        if (direction == new Vector3Int(1, 0, 0))
        {
            animPlayer.Play("Base Layer.Right");
        }
        if (direction == new Vector3Int(-1, 0, 0))
        {
            animPlayer.Play("Base Layer.Left");
        }
        if (direction == new Vector3Int(0, 1, 0))
        {
            animPlayer.Play("Base Layer.Up");
        }
        if (direction == new Vector3Int(0, -1, 0))
        {
            animPlayer.Play("Base Layer.Down");
        }

        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
    }

    private bool CheckCell(Vector3Int targetCell, Tilemap tilemap)
    {
        if (tilemap.GetTile(targetCell) != null)
        {
            if (tilemap.GetTile(targetCell).name != "Tile_coltiles_1") // if not wall
            {
                return true;
            }
        }
        else
        {
            return true; // if empty
        }

        return false; // if wall
    }
}