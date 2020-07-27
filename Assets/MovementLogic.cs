using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovementLogic
{
    public float moveSpeed = 1f;
    public Grid mapGrid;
    public Transform movePoint;
    public Vector3Int direction = new Vector3Int(1, 0, 0);
    public Animator animPlayer;
    public GameObject target;
    public Tilemap tilemap;
    public MovementLogic(float moveSpeed, Grid mapGrid, Transform movePoint, Vector3Int direction, Animator animPlayer, GameObject target)
    {
        this.moveSpeed = moveSpeed;
        this.mapGrid = mapGrid ?? throw new ArgumentNullException(nameof(mapGrid));
        this.movePoint = movePoint ?? throw new ArgumentNullException(nameof(movePoint));
        this.direction = direction;
        this.animPlayer = animPlayer ?? throw new ArgumentNullException(nameof(animPlayer));
        this.target = target ?? throw new ArgumentNullException(nameof(target));

        tilemap = mapGrid.GetComponentInChildren<Tilemap>();
    }

    public void ProcessMovement(float hInput, float vInput)
    {
        Vector3Int currCell = GetCurrentCell(); // i don't know why but it works only like this

        tilemap = mapGrid.GetComponentInChildren<Tilemap>();

        Vector3Int targetCell = currCell + direction;

        if (Vector3.Distance(target.transform.position, movePoint.position) <= 0.5f)
        {
            if (Mathf.Abs(hInput) == 1f)
            {
                Vector3Int dir = new Vector3Int((int)hInput, 0, 0);
                if (CheckCell(currCell + dir, tilemap))
                {
                    targetCell = currCell + dir;
                    direction = dir;
                }
            }
            else if (Mathf.Abs(vInput) == 1f)
            {
                Vector3Int dir = new Vector3Int(0, (int)vInput, 0);
                if (CheckCell(currCell + dir, tilemap))
                {
                    targetCell = currCell + dir;
                    direction = dir;
                }
            }
        }

        tilemap.SetTile(new Vector3Int(0, 0, 0), tilemap.GetTile(targetCell));


        Vector3 cellMiddleTranslation = new Vector3((tilemap.cellSize / 2f).x, (-tilemap.cellSize / 2f).y, 0);

        if (CheckCell(targetCell, tilemap))
        {
            movePoint.position = mapGrid.CellToWorld(targetCell) + cellMiddleTranslation;
        }

        // wrap around the screen
        // the portal across the screen is at y = -17
        if (currCell.x <= tilemap.cellBounds.xMin - 1)
        {
            target.transform.position = mapGrid.CellToWorld(new Vector3Int(tilemap.cellBounds.xMax - 1, -17, 0)) + cellMiddleTranslation;
            targetCell = new Vector3Int(tilemap.cellBounds.xMax - 2, -17, 0);
            direction = new Vector3Int(-1, 0, 0); // to the left

        }
        else if (currCell.x >= tilemap.cellBounds.xMax)
        {
            target.transform.position = mapGrid.CellToWorld(new Vector3Int(tilemap.cellBounds.xMin, -17, 0)) + cellMiddleTranslation;
            targetCell = new Vector3Int(tilemap.cellBounds.xMin + 1, -17, 0);
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

        target.transform.position = Vector3.MoveTowards(target.transform.position, movePoint.position, moveSpeed * Time.deltaTime);
    }

    public Vector3Int GetCurrentCell()
    {
        return mapGrid.WorldToCell(target.transform.position) + new Vector3Int(0, 1, 0);
    }
    public Vector3Int GetTargetCell()
    {
        return GetCurrentCell() + direction;
    }

    public bool CheckCell(Vector3Int targetCell, Tilemap tilemap)
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
