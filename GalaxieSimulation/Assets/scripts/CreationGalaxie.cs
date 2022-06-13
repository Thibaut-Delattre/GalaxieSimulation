using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class CreationGalaxie : MonoBehaviour
{
    public int nbrnodes;

    public GameObject prefabEtoile;
    public Star[] starArray;
    private Node root;
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    float sizeHBTree;
    private Vector3 rootCenterMass;
    public Text timetxt;
    public Text paramtxt;
    public Text param2txt;
    private float stepAffichage;

    public int nbrEtoile;
    public float radiusSpwan;
    public float coeffSpeed;
    public float step;
    public float epsilon;
    public float theta;
    public float mass_Max;
    public float mass_Min;
    public float black_Hole_Mass;


    void Start()
    {
        rootCenterMass = new Vector3(0, 0, 0);
        sizeHBTree = radiusSpwan * 5;
        starArray = new Star[nbrEtoile+1];

        for (int i = 0 ; i < nbrEtoile ; i++)
        {
            Vector3 initializePosition = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)).normalized * radiusSpwan * UnityEngine.Random.Range(0.1f, 1.0f);
            float mass = UnityEngine.Random.Range(mass_Min, mass_Max);
            GameObject star = Instantiate(prefabEtoile, initializePosition, Quaternion.identity);
            //star.GetComponent<Rigidbody>().AddForce(Vector3.Cross(initializePosition, Vector3.up).normalized * initialSpeed);
            starArray[i] = new Star(initializePosition, Vector3.Cross(initializePosition, Vector3.up).normalized * (float)InitialzeSpeed(initializePosition, mass), mass, star, step);            
        }
             
        GameObject black_Hole = Instantiate(prefabEtoile, new Vector3(0,0,0), Quaternion.identity);
        black_Hole.name = "black_Hole";
        starArray[nbrEtoile] = new Star(new Vector3(0,0,0), new Vector3(UnityEngine.Random.Range(-0, 0), UnityEngine.Random.Range(-0, 0), UnityEngine.Random.Range(-0, 0)), black_Hole_Mass, black_Hole, step);
    }

    public double InitialzeSpeed(Vector3 position, float mass)
    {
        double speed;
        speed = coeffSpeed * 1.087f * Math.Pow(10, -24) * Math.Pow((2 * 0.00000000006674f * mass * 1.989f * Mathf.Pow(10, 30)) / position.magnitude, 0.5f);
        return speed;
    }

    
    public void Update()
    {

        stopwatch.Start();


     
        nbrnodes = 0;
        root = new Node(rootCenterMass, sizeHBTree);  //Creation de l'arbre(la racine -> 8 childs ->...) et RESET de l'arbre  /////////////
        for (int i = 0; i <= nbrEtoile; i++) //Pour toutes les etoiles                                                               //         //
        {                                                                                                                           //         //
            if (IsInHBTree(starArray[i]) == true) // Si l'étoile est dans le volume de l'arbre                           <---    // ETAPE 1 //
            {                                                                                                                       //         //
                root.AddStarInHBTree(starArray[i]); //On les ajoute dans l'arbre                                                 //         // 
            }
            else
            {
                Debug.Log("etoile en dehors du HBTREE !!");
            }
        }                                                                                                                           ///////////// 
        root.SupprEmptyNodes();



        root.ComputeMassAndCenter_Mass(); //Update la masse / centre de masse de chaques noeuds (en partant de la racine de l'arbre, puis...)   <---   // ETAPE 2 //
        //rootCenterMass = root.center_mass;

        

        for (int i = 0; i <= nbrEtoile; i++)
        {
            Vector3 force = root.ForceCalculated(starArray[i]);
            starArray[i].ApplyForce(force);
        }

        stopwatch.Stop();
        Debug.Log("Time elapsed : " + stopwatch.ElapsedMilliseconds);



        stopwatch.Reset();

        Affichage();

    }


  


    public bool IsInHBTree(Star star)
    {
        if (star.position.x >= sizeHBTree / 2)
            return false;
        if (star.position.x <= -sizeHBTree / 2)
            return false;
        if (star.position.y >= sizeHBTree / 2)
            return false;
        if (star.position.y <= -sizeHBTree / 2)
            return false;
        if (star.position.z >= sizeHBTree / 2)
            return false;
        if (star.position.z <= -sizeHBTree / 2)
            return false;
        return true;
    }





    public void Affichage()
    {
        paramtxt.text = "Nombre d'étoiles : " + nbrEtoile + "\nRayon initial : " + radiusSpwan + " a.l." + "\nCoeff vitesse initial : " + coeffSpeed + " km/s" + "\nMasse du trou noir : " + black_Hole_Mass + " Ms" + "\nMasses des étoiles : [" + mass_Min +"," + mass_Max + "] Ms";
        param2txt.text = "θ : " + theta + "\nε : " + epsilon;
        timetxt.text = "Durée : " + stepAffichage/Mathf.Pow(10,6) + " M d'années";
        stepAffichage = stepAffichage + step;
    }










}
