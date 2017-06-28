using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsertionSort : MonoBehaviour {
    public int size;
    Vector3[] values;
    // Use this for initialization
    void Start () {
        values = CreateRandomVectorArray(size);
        for (int i = 0; i < values.Length; i++)
        {
            Debug.Log(values[i].x+",");
          
        }
        SortX(values);
        
        for (int i = 0; i < values.Length; i++)
        {
            Debug.Log(values[i].x + ",");
           // System.Console.Write(values[i].x + ",");
        }
        
	}

    // Update is called once per frame
    int a = 0;
	void Update () {
        
        SortX(values);
        SortY(values);
        SortZ(values);
        a++;
        Debug.Log("sort:" + a + " Completed");
	}

    public Vector3[] CreateRandomVectorArray(int size)
    {
        Vector3[] values = new Vector3[size];
        for (int i = 0; i < size; i++)
        {
            values[i] = new Vector3((Random.Range(-10,10)), Random.Range(-10, 10), Random.Range(-10, 10));
        }
        return values;
    }


    int j;
    public void SortX(Vector3[] values)
    {
        for (int i=1; i < values.Length; i++)
        {
            Vector3 x = values[i];
            j = i - 1;
            while (j>=0  && values[j].x > x.x)
            {
                values[j + 1] = values[j];
                j = j - 1;
            }
            values[j + 1] = x;
        }
    }

    public void SortY(Vector3[] values)
    {
        for (int i = 1; i < values.Length; i++)
        {
            Vector3 x = values[i];
            j = i - 1;
            while (j >= 0 && values[j].y > x.y)
            {
                values[j + 1] = values[j];
                j = j - 1;
            }
            values[j + 1] = x;
        }
    }

    public void SortZ(Vector3[] values)
    {
        for (int i = 1; i < values.Length; i++)
        {
            Vector3 x = values[i];
            j = i - 1;
            while (j >= 0 && values[j].z > x.z)
            {
                values[j + 1] = values[j];
                j = j - 1;
            }
            values[j + 1] = x;
        }
    }
}
