using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnUnitsWithNoBehaviours : MonoBehaviour {
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
                    objects[i, j, h] = (GameObject)Instantiate(Resources.Load("Seagull"), new Vector3(random.Next(-spawnArea, spawnArea), random.Next(-spawnArea, spawnArea), random.Next(-spawnArea, spawnArea)), Quaternion.identity);
                    objects[i, j, h].GetComponent<Boid>().target = target;
                    objects[i, j, h].GetComponent<Boid>().flee = flee;


                }
            }

        }
        return objects;
    }

   
}
