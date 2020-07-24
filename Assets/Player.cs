using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Grid mapGrid;
    public Transform movePoint;
    public Transform tarCell;

    public Vector3Int direction = new Vector3Int(1, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        tarCell.parent = null;
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int currCell = mapGrid.WorldToCell(transform.position) + new Vector3Int(0,1,0);

        Vector3Int targetCell = currCell + direction;
        tarCell.position = mapGrid.CellToWorld(targetCell);

        var tilemap = mapGrid.GetComponentInChildren<Tilemap>();

        tilemap.SetTile(new Vector3Int(0, 0, 0), tilemap.GetTile(targetCell));
        if (tilemap.GetTile(targetCell) != null)
        {
            if (tilemap.GetTile(targetCell).name != "Tile_coltiles_1")
            {
                movePoint.position = mapGrid.CellToWorld(targetCell) + new Vector3(0.25f, -0.25f, 0);
            }
        } else
        {
            movePoint.position = mapGrid.CellToWorld(targetCell) + new Vector3(0.25f, -0.25f, 0);
        }

        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        /*if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                movePoint.position += Input.GetAxisRaw("Horizontal") * new Vector3(0.5f, 0f, 0f);
            }
            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                movePoint.position += Input.GetAxisRaw("Vertical") * new Vector3(0f, 0.5f, 0f);
            }
        }*/
    }
}