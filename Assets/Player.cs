using TMPro;
using UnityEngine;

public class Player : Maneuverable
{
    public TMP_Text score_text;
    public int score = 0;
    public bool paused = true;
    public WaveTimer timer;


    // Start is called before the first frame update
    void Start()
    {
        score_text.text = score.ToString();
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer.player_dead)
        {
            return;
        }

        var cell = tilemap.GetTile(moveLogic.GetCurrentCell());

        if (!cell.IsNull())
        {
            if (cell.name == "Tile_coltiles_2") // If a pellet
            {
                score += 10;

                tilemap.SetTile(moveLogic.GetCurrentCell(), tilemap.GetTile(new Vector3Int(1, 1, 1)));
            }

            if (cell.name == "Tile_coltiles_3") // If a booster
            {
                score += 50;

                timer.StartFrightenedBehaviour();

                tilemap.SetTile(moveLogic.GetCurrentCell(), tilemap.GetTile(new Vector3Int(1, 1, 1)));
            }
        }

        score_text.text = score.ToString();

        if (!paused)
        {
            moveLogic.ProcessMovement(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        UpdateAnimation();

    }

    public new void Reset()
    {
        base.Reset();
        Init();
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (moveLogic.direction == new Vector3Int(1, 0, 0))
        {
            animPlayer.Play("Base Layer.Right");
        }
        if (moveLogic.direction == new Vector3Int(-1, 0, 0))
        {
            animPlayer.Play("Base Layer.Left");
        }
        if (moveLogic.direction == new Vector3Int(0, 1, 0))
        {
            animPlayer.Play("Base Layer.Up");
        }
        if (moveLogic.direction == new Vector3Int(0, -1, 0))
        {
            animPlayer.Play("Base Layer.Down");
        }
    }
}