using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public class SaveRenderTexture : MonoBehaviour
{
    // instance variables:
    public int fileCounter;
    public KeyCode screenshotKey;
    private Camera Camera
    {
        get
        {
            if (!_camera)
            {
                _camera = Camera.main;
            }
            return _camera;
        }
    }
    private Camera _camera;

    // custom method for input screenshotKey to trigger capture of render texture:
    private void LateUpdate()
    {
        if (Input.GetKeyDown(screenshotKey))
        {
            Capture(); // call our other custom Capture() method
        }
    }


    public void Capture()
    {
        RenderTexture activeRenderTexture = RenderTexture.active;
        RenderTexture.active = Camera.targetTexture;

        Camera.Render();

        Texture2D image = new Texture2D(Camera.targetTexture.width, Camera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, Camera.targetTexture.width, Camera.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = activeRenderTexture;

        byte[] bytes = image.EncodeToPNG();
        Destroy(image);

        // original path to write to:
        // File.WriteAllBytes(Application.dataPath + "/Backgrounds/" + fileCounter + ".png", bytes);

        // test path to write to: 
        File.WriteAllBytes(Application.dataPath + "/Outputs/" + fileCounter + ".png", bytes);
        fileCounter++;
    }
}