using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostAI : Maneuverable
{
    public int inverseTurnChance = 32; // the bigger, the less chance to turn there is

    private Vector3Int prev_cell = new Vector3Int(0, 0, 0);

    private readonly Dictionary<int, Vector3Int> directions = new Dictionary<int, Vector3Int>
    {
        { 0, new Vector3Int(-1, 0, 0) }, // Left
        { 1, new Vector3Int(0, 1, 0)}, // Up
        { 2, new Vector3Int(1, 0, 0) }, // Right
        { 3, new Vector3Int(0, -1, 0) } // Down
    };
    // Start is called before the first frame update
    void Start()
    {
        base.Init();
    }

    // Update is called once per frame
    void Update()
    {
        float hInput = 0.0f;
        float vInput = 0.0f;

        var tcell = moveLogic.GetTargetCell();
        var ccell = moveLogic.GetCurrentCell();

        if (ccell == prev_cell) {
            moveLogic.ProcessMovement(hInput, vInput);
            return;
        }

        // TODO: Make an extension
        var direction = directions.FirstOrDefault(x => x.Value == moveLogic.direction).Key;
        var oppositeDir = GetOppositeDirection(direction);

        if (!moveLogic.CheckCell(tcell, tilemap))
        {
            int i = Random.Range(0, 4);
            var newTarget = ccell + directions[i];

            while ((!moveLogic.CheckCell(newTarget, tilemap)) || i == oppositeDir)
            {
                i = Random.Range(0, 4);
                newTarget = ccell + directions[i];
            }

            GenerateInputFromDirection(ref hInput, ref vInput, i);
        }
        else
        {
            // Here we check if we can turn here
            int[] possibleTurns = new int[2];
            int turnNo = 0;

            for (int i = 0; i < 4; i++)
            {
                if (i != direction && i != oppositeDir)
                {
                    if (moveLogic.CheckCell(ccell + directions[i], tilemap))
                    {
                        possibleTurns[turnNo++] = i;
                    }
                }
            }

            var rng = Random.Range(0, inverseTurnChance); // TODO: Expose this to public.

            if (rng < possibleTurns.Length)
            {
                // i really don't know what happens to make this check needed, but in some map configurations it is.
                if (!(ccell + directions[possibleTurns[rng]] == prev_cell)) 
                {
                    GenerateInputFromDirection(ref hInput, ref vInput, possibleTurns[rng]);
                }
            }
        }

        prev_cell = ccell;

        moveLogic.ProcessMovement(hInput, vInput);
    }

    private static int GetOppositeDirection(int direction)
    {
        int oppositeDir = 0;
        // Couldn't find anything more elegant.
        switch (direction)
        {
            case 0:
                oppositeDir = 2;
                break;
            case 1:
                oppositeDir = 3;
                break;
            case 2:
                oppositeDir = 0;
                break;
            case 3:
                oppositeDir = 1;
                break;
        }

        return oppositeDir;
    }

    private static void GenerateInputFromDirection(ref float hInput, ref float vInput, int i)
    {
        switch (i)
        {
            case 0:
                hInput = -1f;
                break;
            case 1:
                vInput = 1f;
                break;
            case 2:
                hInput = 1f;
                break;
            case 3:
            default:
                vInput = -1f;
                break;
        }
    }
}
