using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
//using UnityEngine.FrameTiming;

using System.IO;

// V2 of Save1080RenderTextureFrameCount – trying to get metrics for timing in milliseconds

public class SaveFramesOutputMetrics : MonoBehaviour
{
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

    void LateUpdate()
    {
        if (Time.frameCount % 100 == 0)
        {
            // TEST: int for current frame:
            int currentFrame = Time.frameCount;

            // Capture GPU timing data
            FrameTimingManager.CaptureFrameTimings();

            // Debug console logs
            Debug.Log("Capturing frame at: " + currentFrame);
            Debug.Log("FPS: " + (1.0f / Time.deltaTime));

            Capture(currentFrame);
            LogGpuTiming(currentFrame);
        }
    }

    void Capture(int currentFrame)
    {
        RenderTexture activeRenderTexture = RenderTexture.active;
        RenderTexture.active = Camera.targetTexture;

        Camera.Render();

        int width = 1920;
        int height = 1080;

        Texture2D image = new Texture2D(width, height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        image.Apply();

        RenderTexture.active = activeRenderTexture;

        byte[] bytes = image.EncodeToPNG();
        Destroy(image);

        // write output images to Outputs folder, using currentFrame as name:
        File.WriteAllBytes(Application.dataPath + "/Outputs/" + currentFrame + ".png", bytes);
    }

    void LogGpuTiming(int currentFrame)
    {
        FrameTiming[] timings = new FrameTiming[1];
        uint count = FrameTimingManager.GetLatestTimings(1, timings);

        if (count > 0)
        {
            float gpuTimeMs = (float)timings[0].gpuFrameTime;
            Debug.Log($"[Frame {currentFrame}] GPU frame render time (ms): {gpuTimeMs:F3}");
        }
        else
        {
            Debug.LogWarning($"[Frame {currentFrame}] No frame timing data available.");
        }
    }

}