using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public enum State { Chase, Frightened, Scatter, InBox }
public enum GhostType { Red, Cyan, Magenta, Yellow }

public class GhostAI : Maneuverable
{
    public int wave = 0;
    public State state = State.Scatter;
    public GhostType ghostType = GhostType.Red;
    public Player pacman;
    public GhostAI redGhost; // Only needed for cyan ghost

    private BoundsInt startingBox = new BoundsInt(new Vector3Int(14, -17, 0), new Vector3Int(3, 2, 0)); // hardcoded start box because why not
    private Vector3Int prevCell = new Vector3Int(0, 0, 0);
    private Vector3Int targetCell = new Vector3Int(0, 0, 0);
    private Timer timer;

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

        var ccell = moveLogic.GetCurrentCell();

        if (ccell == prevCell)
        {
            if (state == State.InBox)
            {
                moveLogic.ProcessMovement(hInput, vInput, false);
            }
            else
            {
                moveLogic.ProcessMovement(hInput, vInput);
            }
            return;
        }


        UpdateState();
        UpdateTargetPoint();

        var oppositeDir = GetOppositeDirection(directions.GetKeyByValue(moveLogic.direction));

        List<int> possibleTurns = new List<int>();

        for (int i = 0; i < 4; i++)
        {
            if (state == State.InBox)
            {
                if (moveLogic.CheckCell(ccell + directions[i], tilemap, false))
                {
                    if (i != oppositeDir)
                    {
                        possibleTurns.Add(i);
                    }
                }
            }
            else
            {
                if (moveLogic.CheckCell(ccell + directions[i], tilemap))
                {
                    if (i != oppositeDir)
                    {
                        possibleTurns.Add(i);
                    }
                }
            }
        }

        switch (state)
        {
            case State.Frightened:
                FrightenedBehaviour(ref hInput, ref vInput, ccell, possibleTurns);
                moveLogic.ProcessMovement(hInput, vInput);
                break;
            case State.Scatter:
            case State.Chase:
                PathFindingBehaviour(ref hInput, ref vInput, ccell, possibleTurns);
                moveLogic.ProcessMovement(hInput, vInput);
                break;
            case State.InBox:
                PathFindingBehaviour(ref hInput, ref vInput, ccell, possibleTurns);
                moveLogic.ProcessMovement(hInput, vInput, false);
                break;
            default:
                throw new InvalidDataException("Unknown state.");
        }

        prevCell = ccell;
    }

    private void UpdateState()
    {
        if (!IsInBox())
        {
            state = State.Scatter;
        }
    }

    private bool IsInBox()
    {
        return (CurrentCell.x > 10 && CurrentCell.x < 17 && CurrentCell.y < -15 && CurrentCell.y > -19);
    }

    private void PathFindingBehaviour(ref float hInput, ref float vInput, Vector3Int ccell, List<int> possibleTurns)
    {
        var closestDirection = 0;
        var closestDistance = float.MaxValue;

        for (int i = 0; i < possibleTurns.Count; i++)
        {
            var newTile = directions[possibleTurns[i]] + ccell;
            var newDistance = Vector3Int.Distance(newTile, targetCell);

            if (newDistance < closestDistance)
            {
                closestDistance = newDistance;
                closestDirection = i;
            }
        }

        GenerateInputFromDirection(ref hInput, ref vInput, possibleTurns[closestDirection]);
    }

    private void FrightenedBehaviour(ref float hInput, ref float vInput, Vector3Int ccell, List<int> possibleTurns)
    {
        var rng = UnityEngine.Random.Range(0, possibleTurns.Count);

        GenerateInputFromDirection(ref hInput, ref vInput, possibleTurns[rng]);
    }

    private void UpdateTargetPoint()
    {
        switch (state)
        {
            case State.Chase:
                targetCell = GetChasePoint();
                break;
            case State.Frightened:
                // no need for target cell in this state
                break;
            case State.Scatter:
                targetCell = GetScatterPoint();
                break;
            case State.InBox:
                targetCell = new Vector3Int(14, -13, 0);
                break;
            default:
                throw new InvalidDataException("Unknown state.");
        }
    }

    private Vector3Int GetScatterPoint()
    {
        var bounds = moveLogic.tilemap.cellBounds;
        switch (ghostType)
        {
            case GhostType.Red:
                return new Vector3Int(bounds.xMax, bounds.yMax, 0); // Top-Right
            case GhostType.Yellow:
                return new Vector3Int(bounds.xMin, bounds.yMin, 0); // Down-Left
            case GhostType.Magenta:
                return new Vector3Int(bounds.xMin, bounds.yMax, 0); // Top-Left
            case GhostType.Cyan:
                return new Vector3Int(bounds.xMax, bounds.yMin, 0); // Down-Right
            default:
                throw new InvalidDataException("Unknown ghost type.");
        }
    }

    private Vector3Int GetChasePoint()
    {
        switch (ghostType)
        {
            case GhostType.Red:
                return pacman.CurrentCell;
            case GhostType.Yellow:
                if (Vector3Int.Distance(moveLogic.GetCurrentCell(), pacman.CurrentCell) < 8)
                {
                    return GetScatterPoint();
                }
                else
                {
                    return pacman.CurrentCell;
                }
            case GhostType.Magenta:
                return pacman.CurrentCell + (pacman.Direction * 4); // four cells ahead of pacman
            case GhostType.Cyan:
                var target1 = pacman.CurrentCell + (pacman.Direction * 2);
                return target1 + (target1 - redGhost.CurrentCell);
            default:
                throw new InvalidDataException("Unknown ghost type.");
        }
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
