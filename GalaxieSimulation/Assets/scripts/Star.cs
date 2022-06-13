using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star
{
    private float G = 0.00000000006674f;
    private GameObject myStarGO;
    public float mass;
    public Vector3 position;
    private Vector3 previousPosition;
    private Vector3 acceleration;
    private float deltaT;
    //private Rigidbody rb;

    public Star(Vector3 position, Vector3 speed, float mass, GameObject myStarGO, float annee)
    {
        this.position = position;
        this.mass = mass;
        this.myStarGO = myStarGO;
        deltaT = 3.154f * Mathf.Pow(10, 7) * annee;
        this.previousPosition = position - (speed * deltaT);
        //this.rb = myStarGO.GetComponent<Rigidbody>();
        //rb.mass = mass;
    }
        
        

    public void ApplyForce(Vector3 force)
    {
        Vector3 tempPosition = position;
        acceleration = G * force * 1.057f * Mathf.Pow(10, -16);
        position = (2.0f * position) - previousPosition + (acceleration * Mathf.Pow(deltaT, 2.0f));
        previousPosition = tempPosition;
        myStarGO.transform.position = position;

        
        //    rb.AddForce(G * mass * force * 100000000f);
        //   this.position = rb.position;

        if(myStarGO.name == "black_Hole")
        {
            myStarGO.transform.position = new Vector3(0, 0, 0);
            this.position = myStarGO.transform.position;
        }
        
    }




    public void UpdateColorStar(int rang)
    {

    }


}
