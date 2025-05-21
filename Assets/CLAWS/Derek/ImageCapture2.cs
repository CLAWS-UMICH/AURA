using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Windows.WebCam;

public class ImageCapture2 : MonoBehaviour
{
    [SerializeField] private PhotoCapture photoCaptureObject = null;
    public Texture2D targetTexture = null;
    public GameObject photoSprite;

    public void takePhoto()
    {
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);

        //Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        //targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        //// Create a PhotoCapture object
        //PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject)
        //{
        //    photoCaptureObject = captureObject;
        //    CameraParameters cameraParameters = new CameraParameters();
        //    cameraParameters.hologramOpacity = 0.0f;
        //    cameraParameters.cameraResolutionWidth = cameraResolution.width;
        //    cameraParameters.cameraResolutionHeight = cameraResolution.height;
        //    cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

        //    //// Activate the camera
        //    //photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result) {
        //    //    // Take a picture
        //    //    photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        //    //});
        //    if (photoCaptureObject != null)
        //    {
        //        // Activate the camera
        //        captureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result)
        //        {
        //            if (result.success)
        //            {
        //                // Take a picture
        //                //captureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        //                captureObject.TakePhotoAsync((result, frame) => OnCapturedPhotoToMemory(result, frame));
        //            }
        //        });
        //    }
        //});
    }

    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photoCaptureObject = captureObject;

        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            // Create our Texture2D for use and set the correct resolution
            Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
            // Copy the raw image data into our target texture
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);
            // Do as we wish with the texture such as apply it to a material, etc.
        }
        // Clean up
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    //void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame, ref PhotoCapture captureObject)
    //{
    //    if (result.success)
    //    {
    //        // Copy the raw image data into the target texture
    //        photoCaptureFrame.UploadImageDataToTexture(targetTexture);

    //        // Create a GameObject to which the texture can be applied
    //        //GameObject __quad__ = GameObject.CreatePrimitive(PrimitiveType.Quad);
    //        //Renderer quadRenderer = quad.GetComponent<Renderer>() as Renderer;
    //        //quadRenderer.material = new Material(Shader.Find("Custom/Unlit/UnlitTexture"));

    //        //quad.transform.parent = this.transform;
    //        //quad.transform.localPosition = new Vector3(0.0f, 0.0f, 3.0f);

    //        //quadRenderer.material.SetTexture("_MainTex", targetTexture);
    //    }

    //    // Deactivate the camera
    //    captureObject.StopPhotoModeAsync((res) => OnStoppedPhotoMode(res, ref captureObject));
    //}

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Shutdown the photo capture resource
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }
}