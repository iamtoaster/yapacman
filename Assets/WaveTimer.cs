using SuperTiled2Unity;
using SuperTiled2Unity.Editor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaveTimer : MonoBehaviour
{
    public Grid MapGrid;
    public int StartingWave = 0;
    public List<GhostAI> Ghosts;
    public SuperTileset superTileset;
    public Player Pacman;
    public LifeManager lifeManager;
    public GameObject GameOverPanel;
    public Timer frightTimer;

    public float moveSpeed;

    internal State waveState = State.Idle;
    internal State prevState = State.Idle;

    private int ghost_eaten = 0;
    private TileBase[] startingTilemapState;
    private Timer timer;
    private int wave;
    private Tilemap tilemap;
    private bool gameOverActive = false;
    internal bool player_dead = false;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = Ghosts[0].moveSpeed;
        timer = Timer.Register(3f, WaveUpdate);
        tilemap = MapGrid.GetComponentInChildren<Tilemap>();
        startingTilemapState = tilemap.GetTilesBlock(tilemap.cellBounds);
    }

    private void WaveUpdate()
    {
        Pacman.paused = player_dead;

        if (waveState == State.Idle)
        {
            Timer.Cancel(timer);
            waveState = State.Scatter;
            Pacman.paused = false;
            timer = Timer.Register(7f, WaveUpdate);
            return;
        }

        if(timer.isPaused)
        {
            timer.Resume();
            Timer.Cancel(frightTimer);

            waveState = prevState;

            for (int i = 0; i < Ghosts.Count; i++)
            {
                Ghosts[i].moveLogic.moveSpeed = moveSpeed;
            }

            return;
        }

        wave++;
        Timer.Cancel(timer);

        switch (wave)
        {
            case 1:
                waveState = State.Chase;
                timer = Timer.Register(20f, WaveUpdate);
                break;
            case 2:
                waveState = State.Scatter;
                timer = Timer.Register(7f, WaveUpdate);
                break;
            case 3:
                waveState = State.Chase;
                timer = Timer.Register(20f, WaveUpdate);
                break;
            case 4:
                waveState = State.Scatter;
                timer = Timer.Register(5f, WaveUpdate);
                break;
            case 5:
                waveState = State.Chase;
                timer = Timer.Register(20f, WaveUpdate);
                break;
            case 6:
                waveState = State.Scatter;
                timer = Timer.Register(5f, WaveUpdate);
                break;
            default:
                waveState = State.Chase;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        SuperTile tile;
        superTileset.TryGetTile(1, out tile); // pellet
        if (!tilemap.ContainsTile(tile))
        {
            for (int i = 0; i < Ghosts.Count; i++)
            {
                Ghosts[i].moveSpeed *= 1.1f;
                Ghosts[i].moveLogic.moveSpeed = Ghosts[i].moveSpeed;
            }
            Pacman.moveSpeed *= 1.1f;
            Pacman.moveLogic.moveSpeed = Pacman.moveSpeed;
            RestartLevel();
        }

        for (int i = 0; i < Ghosts.Count; i++)
        {
            if (Ghosts[i].moveLogic.GetCurrentCell() == Pacman.moveLogic.GetCurrentCell() && !player_dead)
            {
                if (!frightTimer.IsNull() && !frightTimer.isDone)
                {
                    if (!Ghosts[i].wasEatenRecently && Ghosts[i].state != State.Spirit)
                    {
                        Ghosts[i].state = State.Spirit;

                        Ghosts[i].UpdateState();

                        switch (ghost_eaten)
                        {
                            case 0:
                                Pacman.score += 200;
                                ghost_eaten++;
                                break;
                            case 1:
                                Pacman.score += 400;
                                ghost_eaten++;
                                break;
                            case 2:
                                Pacman.score += 800;
                                ghost_eaten++;
                                break;
                            case 3:
                                Pacman.score += 1600;
                                ghost_eaten++;
                                break;
                            default:
                                ghost_eaten = 0;
                                Timer.Cancel(frightTimer);
                                WaveUpdate();
                                break;
                        } 
                    }

                    if (Ghosts[i].wasEatenRecently && Ghosts[i].state != State.Spirit)
                    {
                        player_dead = true;
                        waveState = State.Idle;
                        Pacman.paused = true;
                        StartCoroutine(Pacman.animPlayer.PlayAndWaitForAnim("Base Layer.Death", RestartLevel));
                    }
                    return;
                }

                
                player_dead = true;
                waveState = State.Idle;
                Pacman.paused = true;
                StartCoroutine(Pacman.animPlayer.PlayAndWaitForAnim("Base Layer.Death", RestartLevel));
                
                
            }
        }
    }

    public void RestartLevel()
    {
        Timer.Cancel(timer);
        Timer.Cancel(frightTimer);

        if (player_dead && !gameOverActive)
        {
            if (lifeManager.LifesLeft > 0) {
                lifeManager.RemoveLife();
            } 
            else
            {
                GameOverPanel.SetActive(true);
                gameOverActive = true;

                return;
            }
        }

        if (gameOverActive)
        {
            GameOverPanel.SetActive(false);
            gameOverActive = false;

            Pacman.score = 0;

            lifeManager.AddLife();
            lifeManager.AddLife();
            lifeManager.AddLife();
        }

        Pacman.Reset();
        Pacman.paused = true;
        player_dead = false;
        waveState = State.Idle;
        wave = 0;

        for (int i = 0; i < Ghosts.Count; i++)
        {
            Ghosts[i].Reset();
        }

        timer = Timer.Register(3f, WaveUpdate);
        tilemap.SetTilesBlock(tilemap.cellBounds, startingTilemapState);
    }

    public void StartFrightenedBehaviour()
    {
        if (!(waveState == State.Frightened))
        {
            prevState = waveState;
        }
        waveState = State.Frightened;
        ghost_eaten = 0;

        for (int i = 0; i < Ghosts.Count; i++)
        {
            Ghosts[i].moveLogic.moveSpeed = Ghosts[i].moveSpeed * 0.75f;
            Ghosts[i].wasEatenRecently = false;
        }

        Timer.Cancel(frightTimer);

        timer.Pause();
        frightTimer = Timer.Register(20f, WaveUpdate);
    }
}
