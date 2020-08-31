using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    public int LifesLeft;
    public GameObject Life;

    private List<GameObject> lifeObjects;

    // Start is called before the first frame update
    void Start()
    {
        lifeObjects = new List<GameObject>();

        for (int i = 0; i < LifesLeft; i++)
        {
            lifeObjects.Add(Instantiate(Life, new Vector3(-3f - i, -8.7f), new Quaternion()));
        }
    }

    public void AddLife()
    {
        LifesLeft += 1;
        lifeObjects.Add(Instantiate(Life, new Vector3(-3f - lifeObjects.Count, -8.7f), new Quaternion()));
    }

    public void RemoveLife()
    {
        LifesLeft -= 1;
        Destroy(lifeObjects[lifeObjects.Count - 1]);
        lifeObjects.RemoveAt(lifeObjects.Count - 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
