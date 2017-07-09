using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CielaSpike;


public class spawnUnitsSeekAndAvoid : MonoBehaviour
{

    public int size;
    Vector3[,,] values;
    int numChanges = 0;
    GameObject[,,] objects;
    public Transform target;
    public Transform flee;
    public int radius;
    public float desiredSeperation;
    public int spawnArea;

    public float neighbourDistance;

    void Start()
    {
        objects = new GameObject[size, size, size];
        values = new Vector3[size, size, size];

        CreateGameObjects(size);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                for (int h = 0; h < size; h++)
                {
                    objects[i, j, h].GetComponent<Boid>().SetBoidArraySize(size);
                    //consider usng a static.for much faster startup
                }
            }
        }
        GetPositions();
        // this.StartCoroutineAsync(SortAll());


    }


    public GameObject[,,] CreateGameObjects(int size)
    {
        System.Random random = new System.Random();

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                for (int h = 0; h < size; h++)
                {
                    objects[i, j, h] = (GameObject)Instantiate(Resources.Load("CubeSA"), new Vector3(random.Next(-spawnArea, spawnArea), random.Next(-spawnArea, spawnArea), random.Next(-spawnArea, spawnArea)), Quaternion.identity);
                    objects[i, j, h].GetComponent<BoidSeekAndAvoid>().target = target;
                    objects[i, j, h].GetComponent<BoidSeekAndAvoid>().flee = flee;


                }
            }

        }
        return objects;
    }

    public void GetPositions()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                for (int h = 0; h < size; h++)
                {
                    values[i, j, h] = objects[i, j, h].transform.position;

                }
            }

        }

    }
}



   

