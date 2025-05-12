using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ReadPoseFromCSVDataUpdateCameraFaster : MonoBehaviour
{
    // SCRIPT V3 – successfully reads pose from CSV file, then updates camera (translation) at correct rate, but NOT rotation (V4 fixes this)
        // note: V1, V2, V3 scripts actually do NOT consider the *relative* pose of the Aria glasses; this is also fixed in V4

    // instance variables:
    public TextAsset textAssetData; // CSV file data (stored as a TextAsset object)

    // custom class to be the 'data structure' for both the translation and rotation data from the CSV:
    [System.Serializable]
    public class TransformLists
    {
        public Vector3[] translationData;
        public Quaternion[] rotationData;

    }

    // declare an INSTANCE of this custom class (TransformLists) so that we can use it is ReadCSV():
    public TransformLists myAriaData = new TransformLists(); // empty constructor bc we just used the default one

    private int frameIndex = 0; // current frame

    private int totalFrames = 0;

    // NEW: Store timestamps in seconds
    private List<float> poseTimestampsSec = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        // call ReadCSV()
        ReadCSV();

        totalFrames = myAriaData.translationData.Length;
    }

    // Update is called once per frame
    void Update()
    {
        // NEW: currentTime
        float currentTime = Time.time;

        // NEW: Fast-forward frameIndex while time hasn't caught up yet
        while (frameIndex < totalFrames - 1 && poseTimestampsSec[frameIndex] < currentTime)
        {
            frameIndex++;
        }

        if (frameIndex >= totalFrames)
            return; // stop if you've reached the end

        transform.position = myAriaData.translationData[frameIndex];
        //transform.rotation = myAriaData.rotationData[frameIndex];
        //transform.rotation = myAriaData.rotationData[frameIndex] * Quaternion.Euler(0, 180f, 0);

        // COMMENTED-OUT CODE BELOW RESULTS IN INCORRECT TIMING:
        // count up time
        //timer += Time.deltaTime;

        // only update once per frame interval
        //if (timer >= (1f / playbackFrameRate))
        //{
        // Apply this frame's transform
        //transform.position = myAriaData.translationData[frameIndex];
        //transform.rotation = myAriaData.rotationData[frameIndex];

        //frameIndex++; // move to next frame
        //timer = 0f;
        //}

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

        // NEW: poseTimestampSec
        poseTimestampsSec = new List<float>(tableSize);

        // NEW:
        float firstTimestamp_us = float.Parse(data[(28 * 1) + 1]);
        float firstTimestampSec = firstTimestamp_us / 1_000_000f;

        // Step 3 – create data structures to hold data (that we will then use to update camera transform):

        for (int i = 0; i < tableSize; i++)
        {
            // NEW: store timestamp in seconds
            float timestamp_us = float.Parse(data[(28 * (i + 1)) + 1]);
            poseTimestampsSec.Add((timestamp_us / 1_000_000f) - firstTimestampSec); // subtract to start at t=0

            // make a list of Vector3 (for position aka translation 3DOF):
            // use column 4, 5, 6 for x, y, z floats
            myAriaData.translationData[i] = new Vector3(); // memory allocation
            myAriaData.translationData[i].x = float.Parse(data[(28 * (i + 1)) + 3]); // assign x translation values to each Vector3's .x field
            myAriaData.translationData[i].y = float.Parse(data[(28 * (i + 1)) + 4]); // assign y translation values to each Vector3's .y field
            myAriaData.translationData[i].z = float.Parse(data[(28 * (i + 1)) + 5]); // assign z translation values to each Vector3's .z field

            // make a list of Quaternion (for rotation 3DOF):
            // use column 7, 8, 9, 10 for x, y, z, w floats (but w should come first in the Quaternion)
            myAriaData.rotationData[i] = new Quaternion(); // memory allocation
            myAriaData.rotationData[i].x = -float.Parse(data[(28 * (i + 1)) + 6]); // NEW (neg Y-UP CORRECTION) assign x rotation values to each Quaternion's .x field
            myAriaData.rotationData[i].y = float.Parse(data[(28 * (i + 1)) + 7]); // assign y rotation values to each Quaternion's .y field
            myAriaData.rotationData[i].z = -float.Parse(data[(28 * (i + 1)) + 8]); // NEW (neg Y-UP CORRECTION) assign z rotation values to each Quaternion's .z field
            myAriaData.rotationData[i].w = float.Parse(data[(28 * (i + 1)) + 9]); // assign w rotation values to each Quaternion's .w field

        }

    }
}