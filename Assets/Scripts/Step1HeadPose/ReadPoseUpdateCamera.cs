using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ReadPoseUpdateCamera : MonoBehaviour
{
    // SCRIPT V4 – successfully reads pose from Telepresence CSV file, then updates camera (translation AND rotation)
        // note: currently does NOT work w Office CSV.. look at ReadPoseUpdateCameraFinal for better solution :)

    // variable to store CSV file (Aria glasses data):
    public TextAsset textAssetData;

    // Custom class to store transform components (translation, quaternion) in arrays
    [System.Serializable]
    public class TransformLists
    {
        public Vector3[] translationData; // array of Vector3 (3D vectors with x, y, z position data)
        public Quaternion[] rotationData; // array of Quaternion (4D values with x, y, z, w rotation data)
    }

    // CSV variable:
    public TransformLists myAriaData = new TransformLists(); // store Aria glasses data (from CSV) as a TransformLists object

    // frame variables:
    private int frameIndex = 0; // index of current frame to update Camera pose
    private int totalFrames = 0; // total number of frames (timestamps from CSV file)

    // timing variable:
    private List<float> poseTimestampsSec = new List<float>(); // list of float numbers representing timestamps in seconds


    // frame-of-reference variables:
    private Quaternion initialAriaRotInv; // for inverse of initial Aria data Quaternion values
    private Vector3 initialAriaPos; // for initial Aria data Vector3 values

    private Quaternion unityStartRot; // usually Quaternion.identity
    private Vector3 unityStartPos; // usually new Vector3(0, 1, 0)

    // Start() method runs once at beginning
    void Start()
    {
        // Parse data from Aria glasses CSV:
        ReadCSV();
        totalFrames = myAriaData.translationData.Length;

        // Store Aria glasses' initial pose:
        initialAriaRotInv = Quaternion.Inverse(myAriaData.rotationData[0]); // rotation inverse
        initialAriaPos = myAriaData.translationData[0]; // translation

        // Store Unity camera's initial pose
        unityStartPos = transform.position; // e.g., (0, 1, 0)
        unityStartRot = transform.rotation; // usually identity
    }

    // Update() method runs continuously
    void Update()
    {
        // Set current time in Unity:
        float currentTime = Time.time;

        // Increment current frame (frameIndex) if less than total (totalFrames): 
        while (frameIndex < totalFrames - 1 && poseTimestampsSec[frameIndex] < currentTime)
        {
            frameIndex++;
        }

        if (frameIndex >= totalFrames)
            return;

        // Assign Aria glasses' pose of current frame:
        Vector3 ariaPos = myAriaData.translationData[frameIndex]; // translation
        Quaternion ariaRot = myAriaData.rotationData[frameIndex]; // rotation

        // Calculate relative Aria and Unity camera pose:
        Vector3 relAriaPos = ariaPos - initialAriaPos; // relative Aria translation = current frame translation - initial (start) frame translation
        Vector3 unityRelPos = new Vector3(-relAriaPos.y, -relAriaPos.x, relAriaPos.z); // relative Unity translation = relative Aria translation, but change coordinate system
            // Aria: RH, -X Up
            // Unity: LH, +Y Up
            // So, Aria (x, y, z) -> Unity (Aria -y, Aria -x, Aria z)
            // We swap *and* negative x and y, but leave z the same

        Quaternion relAriaRot = initialAriaRotInv * ariaRot; // relative Aria rotation = initial (start) frame rotation * current frame rotation
        Quaternion unityRot = Quaternion.LookRotation(Vector3.forward, Vector3.up) * relAriaRot; // Unity rotation = 

        // Apply overall pose transform to Unity camera:
        transform.position = unityStartPos + unityRelPos;
        transform.rotation = unityRot;


        // Console output for rotation debugging rotation:
        Debug.Log("Unity rot (Euler): " + unityRot.eulerAngles.ToString("F1"));
    }



    void ReadCSV()
    {
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, System.StringSplitOptions.None);

        int tableSize = data.Length / 28 - 1;

        myAriaData.translationData = new Vector3[tableSize];
        myAriaData.rotationData = new Quaternion[tableSize];
        poseTimestampsSec = new List<float>(tableSize);

        float firstTimestamp_us = float.Parse(data[(28 * 1) + 1]);
        float firstTimestampSec = firstTimestamp_us / 1_000_000f;


        for (int i = 0; i < tableSize; i++)
        {
            float timestamp_us = float.Parse(data[(28 * (i + 1)) + 1]);
            poseTimestampsSec.Add((timestamp_us / 1_000_000f) - firstTimestampSec);


            myAriaData.translationData[i] = new Vector3
            {
                x = float.Parse(data[(28 * (i + 1)) + 3]),
                y = float.Parse(data[(28 * (i + 1)) + 4]),
                z = float.Parse(data[(28 * (i + 1)) + 5])
            };

            // Still read rotation for structure but don’t apply it
            myAriaData.rotationData[i] = new Quaternion
            {
                x = float.Parse(data[(28 * (i + 1)) + 6]),
                y = float.Parse(data[(28 * (i + 1)) + 7]),
                z = float.Parse(data[(28 * (i + 1)) + 8]),
                w = float.Parse(data[(28 * (i + 1)) + 9])
            };
        }
    }
}