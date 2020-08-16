using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : Maneuverable
{
    public TMP_Text score_text;
    public int score = 0;
    // Start is called before the first frame update
    void Start()
    {
        score_text.text = score.ToString();
        base.Init();
    }

    // Update is called once per frame
    void Update()
    {
        var cell = tilemap.GetTile(moveLogic.GetCurrentCell());
        if (!cell.IsNull())
        {
            if (cell.name == "Tile_coltiles_2") // If a pellet
            {
                score += 10;
                score_text.text = score.ToString();

                tilemap.SetTile(moveLogic.GetCurrentCell(), tilemap.GetTile(new Vector3Int(1, 1, 1)));
            }
            if (cell.name == "Tile_coltiles_3") // If a booster
            {
                score += 50;
                score_text.text = score.ToString();

                tilemap.SetTile(moveLogic.GetCurrentCell(), tilemap.GetTile(new Vector3Int(1, 1, 1)));
            }
        }
        moveLogic.ProcessMovement(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

}