using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Maneuverable
{
    // Start is called before the first frame update
    void Start()
    {
        base.Init();
    }

    // Update is called once per frame
    void Update()
    {
        moveLogic.ProcessMovement(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

}