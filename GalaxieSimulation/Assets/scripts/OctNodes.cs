using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class Node
{
    private Vector3 position;
    private float size;
    private Node[] childsNode;
    private int nbrStar;
    private Star existingStar;
    private float mass = 0;
    private Vector3 center_mass;
    public float theta = Camera.main.GetComponent<CreationGalaxie>().theta;
    public float epsilon = Camera.main.GetComponent<CreationGalaxie>().epsilon * 9.461f * Mathf.Pow(10, 15);


    public Node(Vector3 position, float size)
    {
        this.position = position;
        this.size = size;
        childsNode = new Node[8];
    }





    // Processus de création de le HBTree (étoile par étoile...)
    public void AddStarInHBTree(Star star)
    {    
        if (nbrStar > 1) //Si ce noeud était "interne" (au moins 2 étoiles dans ce noeud)
        {
            int m = GetChildNode(star); //On cherche quelle childNode detecte l'étoile (retourne -1 si l'étoile est en dehors de tout le HBTree) 
            childsNode[m].AddStarInHBTree(star); //L'étoile est ajoutée au childNode qui l'a détectée (Tout le processus est ainsi reéxécuté dans ce childNode...) 
        }

        if(nbrStar == 1) //Si ce noeud était "externe" (1 seule étoile dans ce noeud)
        {
            CreateOctNode(); //On créé ses 8 noeuds "enfants"
            int k = GetChildNode(existingStar); //On cherche quelle childNode detecte l'ancienne étoile (retourne -1 si l'ancienne étoile est en dehors de tout le HBTree)
            childsNode[k].AddStarInHBTree(existingStar); //L'ancienne étoile est ajoutée au childNode qui l'a détectée (Tout le processus est ainsi reéxécuté dans ce childNode...)        
            int l = GetChildNode(star); //On cherche quelle childNode detecte l'étoile (retourne -1 si l'étoile est en dehors de tout le HBTree)
            childsNode[l].AddStarInHBTree(star); //L'étoile est ajoutée au childNode qui l'a détectée (Tout le processus est ainsi reéxécuté dans ce childNode...)
        }
        
        if(nbrStar == 0) //Si ce noeud était vide
        {
            existingStar = star; //On stocke l'étoile comme l'étoile du noeud           
        }

        nbrStar++; //On incrémente le nombre d'étoiles de ce noeud
    }


    public void SupprEmptyNodes()
    {
        for (int i = 0; i < 8; i++) // Pour les 8 childsNodes de ce noeud
        {
            if (childsNode[i].nbrStar > 1)
            {
                childsNode[i].SupprEmptyNodes();
            }
            if (childsNode[i].nbrStar == 0)
            {
                childsNode[i] = null;
            }
        }
    }





    // Processus du calcul de la masse / centre de masse de chaques noeuds
    public void ComputeMassAndCenter_Mass()
    {
        
        if (nbrStar == 1) //Si ce noeud est "externe" (contient 1 seule étoile)
        {
            mass = existingStar.mass; // La masse du noeud devient la masse de l'étoile
            center_mass = existingStar.position; // Le centre de masse du noeud devient le centre de masse de l'étoile
        }
        if(nbrStar > 1) //Si ce noeud est "interne" (contient au moins 2 étoiles)
        {
            for (int i = 0; i < 8; i++) // Pour les 8 childsNodes de ce noeud
            {
                if(childsNode[i] != null)
                {
                    childsNode[i].ComputeMassAndCenter_Mass(); // On leur exécute tout ce processus du calcul de masse / centre de masse ...
                    mass += childsNode[i].mass; // On ajoute la masse de chaques childNodes à la masse du noeud
                    center_mass += childsNode[i].center_mass * childsNode[i].mass; // On ajoute le centre de masse de chaques childNodes au centre de masse du noeud (ETAPE 1)
                }
            }
            center_mass = center_mass / mass; // On ajoute le centre de masse de chaques childNodes au centre de masse du noeud (ETAPE 2)

        }
        if (nbrStar == 0) // Si ce noeud est vide
        {
            mass = 0; // La masse de ce noeud est vide
            center_mass = new Vector3(0, 0, 0); // Le centre de masse de ce noeud est nul
        }
    }




    public Vector3 ForceCalculated(Star starTarget)
    {
        Vector3 force = new Vector3(0,0,0);

        if(nbrStar == 1)
        {
            force = Force(starTarget);
        }
        if(nbrStar > 1)
        {
            float d = (center_mass - starTarget.position).magnitude;
            float r = size;
            if (r/d < theta)
            {
                force = Force(starTarget);
            }
            else
            {
                for(int i = 0; i < 8; i++)
                {
                    if (childsNode[i] != null)
                        force += childsNode[i].ForceCalculated(starTarget);
                }
            }
        }
    
        return force;
    }



    public Vector3 Force(Star starTarget)
    {
        Vector3 direction = center_mass - starTarget.position;        
        float distance = direction.magnitude;
        if (distance == 0f)
            return new Vector3(0,0,0);
        distance = distance * 9.461f * Mathf.Pow(10, 15);
        double forceAlgebrique = mass * 1.989f * Mathf.Pow(10, 30) * distance * (Math.Pow(Math.Pow(distance,2) + Math.Pow(epsilon, 2),-1.5));
        return direction.normalized * (float)forceAlgebrique;       
    }


    public Vector3 ForceRb(Star starTarget)
    {
        Vector3 direction = center_mass - starTarget.position;
        float distance = direction.magnitude;
        if (distance == 0f)
            return new Vector3(0, 0, 0);
        double forceAlgebrique = mass * Math.Pow(distance,-2);
        return direction.normalized * (float)forceAlgebrique;
    }










    public void CreateOctNode()
    {
        childsNode[0] = new Node(new Vector3(position.x - (size / 4), position.y - (size / 4), position.z - (size / 4)), size / 2);
        childsNode[1] = new Node(new Vector3(position.x - (size / 4), position.y + (size / 4), position.z - (size / 4)), size / 2);
        childsNode[2] = new Node(new Vector3(position.x + (size / 4), position.y - (size / 4), position.z - (size / 4)), size / 2);
        childsNode[3] = new Node(new Vector3(position.x + (size / 4), position.y + (size / 4), position.z - (size / 4)), size / 2);
        childsNode[4] = new Node(new Vector3(position.x - (size / 4), position.y - (size / 4), position.z + (size / 4)), size / 2);
        childsNode[5] = new Node(new Vector3(position.x - (size / 4), position.y + (size / 4), position.z + (size / 4)), size / 2);
        childsNode[6] = new Node(new Vector3(position.x + (size / 4), position.y - (size / 4), position.z + (size / 4)), size / 2);
        childsNode[7] = new Node(new Vector3(position.x + (size / 4), position.y + (size / 4), position.z + (size / 4)), size / 2);
    }


    public int GetChildNode(Star star)
    {
        for(int i = 0; i < 8; i++)
        {
            if (childsNode[i].Detection(star) == true)
            {
                return i;
            }
        }
        Debug.Log("Position du probleme : " + star.position);
        return -1;
        
    }


    public bool Detection(Star star) // (+0.01 pour la marge d'erreur : si un etoile se trouve pile poil entre 2 childsnodes...)
    {      
        if (star.position.x >= position.x + ((size + 0.01f) / 2f)) 
            return false;
        if (star.position.x <= position.x - ((size + 0.01f) / 2f))
            return false;
        if (star.position.y >= position.y + ((size + 0.01f) / 2f))
            return false;
        if (star.position.y <= position.y - ((size + 0.01f) / 2f))
            return false;
        if (star.position.z >= position.z + ((size + 0.01f) / 2f))
            return false;
        if (star.position.z <= position.z - ((size + 0.01f) / 2f))
            return false;
        return true;
    }
}

