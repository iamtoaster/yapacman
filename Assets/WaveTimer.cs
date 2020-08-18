using System.Collections.Generic;
using UnityEngine;

public class WaveTimer : MonoBehaviour
{
    private Timer timer;
    private int wave;

    public int StartingWave = 0;

    public List<GhostAI> Ghosts;
    internal State waveState = State.Scatter;

    // Start is called before the first frame update
    void Start()
    {
        timer = Timer.Register(7f, WaveUpdate);
    }

    private void WaveUpdate()
    {
        wave++;

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

    }
}
