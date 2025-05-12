using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// SCRIPT V1 – successfully reads pose from CSV file

public class ReadPoseFromCSVData : MonoBehaviour
{
    // instance variables:
    public TextAsset textAssetData; // CSV file data (stored as a TextAsset object)

    // for now, putting in a class -- therefore commenting out:
    //public Vector3[] translationData; // .x .y .z at each index will have float loaded in
    //public Quaternion[] rotationData; // .w .x .y .z at each index will have float loaded in

    // custom class to be the 'data structure' for both the translation and rotation data from the CSV:
    [System.Serializable]
    public class TransformLists
    {
        public Vector3[] translationData;
        public Quaternion[] rotationData;

    }

    // declare an INSTANCE of this custom class (TransformLists) so that we can use it is ReadCSV():
    public TransformLists myAriaData = new TransformLists(); // empty constructor bc we just used the default one

    // Start is called before the first frame update
    void Start()
    {
        // call ReadCSV()
        ReadCSV();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: will implement in the newer script (i.e. 'ReadPoseCSVDataUpdateCamera')
    }

    // custom method to read head pose (6DOF transform with translation and rotation) data from CSV file
    void ReadCSV()
    {
        // Step 1 – read in CSV data as string, and split based on commas (,) and new lines (\n):
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, System.StringSplitOptions.None);

        // Step 2 – save size of the CSV file 'table' as an int:
        int tableSize = data.Length / 28 - 1; // data.Length / 28 (number of columns) - 1 (bc first ROW is the labels and therefore not used
        // 28 columns (A to AB)
        // 63,063 rows (each row - aside from the first - representing a timestamp of data)

        // set up the translation (Vector3) list AND rotation (Quaternion) list:
        myAriaData.translationData = new Vector3[tableSize]; // array of 3D vectors
        myAriaData.rotationData = new Quaternion[tableSize]; // array of 4D quaternions


        // Step 3 – create data structures to hold data (that we will then use to update camera transform):

        for (int i = 0; i < tableSize; i++)
        {
            // make a list of Vector3 (for position aka translation 3DOF):
            // use column 4, 5, 6 for x, y, z floats
            myAriaData.translationData[i] = new Vector3(); // memory allocation
            myAriaData.translationData[i].x = float.Parse(data[(28 * (i + 1)) + 3]); // assign x translation values to each Vector3's .x field
            myAriaData.translationData[i].y = float.Parse(data[(28 * (i + 1)) + 4]); // assign y translation values to each Vector3's .y field
            myAriaData.translationData[i].z = float.Parse(data[(28 * (i + 1)) + 5]); // assign z translation values to each Vector3's .z field

            // make a list of Quaternion (for rotation 3DOF):
            // use column 7, 8, 9, 10 for x, y, z, w floats (but w should come first in the Quaternion)
            myAriaData.rotationData[i] = new Quaternion(); // memory allocation
            myAriaData.rotationData[i].x = float.Parse(data[(28 * (i + 1)) + 6]); // assign x rotation values to each Quaternion's .x field
            myAriaData.rotationData[i].y = float.Parse(data[(28 * (i + 1)) + 7]); // assign y rotation values to each Quaternion's .y field
            myAriaData.rotationData[i].z = float.Parse(data[(28 * (i + 1)) + 8]); // assign z rotation values to each Quaternion's .z field
            myAriaData.rotationData[i].w = float.Parse(data[(28 * (i + 1)) + 9]); // assign w rotation values to each Quaternion's .w field


            // debug logs (since only Euler Angles - instead of Quaternion values - are shown for rotation in the inspector):
            //Quaternion q = myAriaData.rotationData[i];
            //Debug.Log($"Quaternion[{i}]: x={q.x}, y={q.y}, z={q.z}, w={q.w}");
        }

    }
}