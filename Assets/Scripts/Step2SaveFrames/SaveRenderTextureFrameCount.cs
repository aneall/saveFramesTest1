using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public class SaveRenderTextureFrameCount : MonoBehaviour
{
    // instance variables:
    public int fileCounter;
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

    // custom method for framecount to trigger capture of render texture:
    private void LateUpdate()
    {
        // if current frame IS at a multiple of 100 (e.g. 200, 300, 400), capture it:
        if (Time.frameCount % 100 == 0)
        {
            // debug console logs to indiciate given frame rendered and frames-per-second (FPS):
            Debug.Log("Capturing frame at: " + Time.frameCount);
            Debug.Log("FPS: " + (1.0f / Time.deltaTime));

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