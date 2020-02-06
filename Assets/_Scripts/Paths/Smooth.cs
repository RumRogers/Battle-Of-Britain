using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Smooth : MonoBehaviour
{
    [SerializeField]
    private List<float> coordinates;
    [SerializeField]
    private List<float> distances;
    private List<List<float>> array;
    [SerializeField]
    private float t = 0;
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 20), "Interpolate!"))
        {
            array = new List<List<float>>();
            
            for(int i = 0; i < coordinates.Count; i++)
            {
                List<float> row = new List<float>();
                row.Add(distances[i]);
                row.Add(coordinates[i]);
                
                array.Add(row);
            }

            int dimension = array[0].Count;
            List<float> res = new List<float>();

            for(int i = 0; i < dimension; i++)
            {
                CubicInterpolator interpolator = new CubicInterpolator(GetColumn(array, i).ToArray());
                res.Add(interpolator.Interpolate(t));
                print(res[i]);
            }

        }
    }

    private List<float> GetColumn(List<List<float>> list, int col)
    {
        List<float> res = new List<float>();

        for(int i = 0; i < list.Count; i++)
        {
            res.Add(list[i][col]);
        }

        return res;
    }


}