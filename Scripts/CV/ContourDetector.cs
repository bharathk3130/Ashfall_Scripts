using OpenCvSharp;
using OpenCvSharp.Demo;
using UnityEngine;
using UnityEngine.UI;

public class ContourDetector : WebCamera
{
    [Header("REFERENCES")]
    [SerializeField] PolygonCollider2D _polygonCollider;
    [SerializeField] RawImage _processedRawImage;

    [Header("SETTINGS")]
    [Tooltip("Everything below this value is black and above is white")]
    [SerializeField] float _threshold = 96.4f;
    [SerializeField] float _curveAccuracy = 10;
    [SerializeField] float _minArea = 5000;

    [SerializeField] FlipMode _flipMode;
    
    [Header("COLOUR DETECTION")]
    [SerializeField] ColourDetectionType _colourDetectionType;
    [SerializeField, Range(0, 150)] int _brightnessLowerBounds = 80;
    [SerializeField] int _outlineThickness = 1;

    [Header("DEBUGGING")]
    [SerializeField, Range(0, 1)] float _processingImageAlpha;
    
    Mat _image;
    Mat _processImage;
    Texture2D _processedTexture;

    Point[][] _contours;

    // Colours
    Scalar _lightGreenLowerBounds;
    Scalar _lightGreenUpperBounds;
    Scalar _blueLowerBound;
    Scalar _blueUpperBound;
    
    // CONSTANTS
    Scalar _redColourScalar = new(0, 0, 255);

    enum ColourDetectionType
    {
        LightGreen,
        Blue,
        LightGreenAndBlue,
        Black
    }

    void Start()
    {
        SetUpColours();
        SetUpProcessingImageUI();
    }

    void SetUpColours()
    {
        _lightGreenLowerBounds = new Scalar(35, 50, _brightnessLowerBounds);
        _lightGreenUpperBounds = new Scalar(85, 255, 255);
        
        _blueLowerBound = new Scalar(90, 50, _brightnessLowerBounds);
        _blueUpperBound = new Scalar(120, 255, 255);
    }

    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        // Dispose of the old Mat if it exists
        _image?.Dispose();
        
        // Convert WebCamTexture to Mat
        _image = OpenCvSharp.Unity.TextureToMat(input);

        ProcessImage();
        
        // Initialize the output texture if it's null
        if (output == null || output.width != _image.Width || output.height != _image.Height)
        {
            // Dispose of the old texture if it exists before creating a new one
            if (output != null)
            {
                Destroy(output);
            }
            output = new Texture2D(_image.Width, _image.Height, TextureFormat.RGBA32, false);
        }

        // Convert the processed Mat back to Texture2D
        OpenCvSharp.Unity.MatToTexture(_image, output);
        
        return true; // Indicating that the output has been updated
    }

    void ProcessImage()
    {
        FlipImage();
        InitializeProcessImageIfNull();

        ColourDetection();
        FindContours();
    }

    void ColourDetection()
    { 
        switch (_colourDetectionType)
        {
            case ColourDetectionType.LightGreen:
                ConvertToColourDetection(_lightGreenLowerBounds, _lightGreenUpperBounds);
                break;
            case ColourDetectionType.Blue:
                ConvertToColourDetection(_blueLowerBound, _blueUpperBound);
                break;
            case ColourDetectionType.Black:
                ConvertToBlackAndWhite();
                break;
            case ColourDetectionType.LightGreenAndBlue:
                ConvertToColourDetection(_lightGreenLowerBounds, _lightGreenUpperBounds, _blueLowerBound, _blueUpperBound);
                break;
            default:
                Debug.LogError($"{_colourDetectionType} not implemented");
                break;
        }
    }
    
    void ConvertToColourDetection(Scalar lowerBound, Scalar upperBound)
    {
        // Convert BGR image to HSV
        Mat hsvImage = new Mat();
        Cv2.CvtColor(_image, hsvImage, ColorConversionCodes.BGR2HSV);

        // Create mask for the colour
        Mat mask = new Mat();
        Cv2.InRange(hsvImage, lowerBound, upperBound, mask);

        // Optionally apply morphology operations to clean up the mask
        Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(5, 5));
        Cv2.MorphologyEx(mask, mask, MorphTypes.Close, kernel);
        Cv2.MorphologyEx(mask, mask, MorphTypes.Open, kernel);

        // Set the processed image to be the mask
        _processImage = mask;
    }
    
    void ConvertToColourDetection(Scalar lowerBound1, Scalar upperBound1, Scalar lowerBound2, Scalar upperBound2)
    {
        // Convert BGR image to HSV
        Mat hsvImage = new Mat();
        Cv2.CvtColor(_image, hsvImage, ColorConversionCodes.BGR2HSV);

        // Create masks for both colours
        Mat mask1 = new Mat();
        Mat mask2 = new Mat();

        Cv2.InRange(hsvImage, lowerBound1, upperBound1, mask1);
        Cv2.InRange(hsvImage, lowerBound2, upperBound2, mask2);

        // Combine both masks
        Mat combinedMask = new Mat();
        Cv2.BitwiseOr(mask1, mask2, combinedMask);

        // Optionally apply morphology operations to clean up the mask
        Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(5, 5));
        Cv2.MorphologyEx(combinedMask, combinedMask, MorphTypes.Close, kernel);
        Cv2.MorphologyEx(combinedMask, combinedMask, MorphTypes.Open, kernel);

        // Set the processed image to be the combined mask
        _processImage = combinedMask;
    }

    
    protected override void RenderFrame()
    {
        Texture2D processedTexture = OpenCvSharp.Unity.MatToTexture(_processImage);
        MakeBlackInvisible(processedTexture, renderedTexture);
        
        base.RenderFrame();
        
        RenderTexture(_processedRawImage, processedTexture);
    }

    void MakeBlackInvisible(Texture2D processedTexture, Texture2D renderedTexture2D)
    {
        Color32[] processedPixels = processedTexture.GetPixels32();
        Color32[] originalPixels = renderedTexture2D.GetPixels32();

        for (int i = 0; i < processedPixels.Length; i++)
        {
            // If the pixel is black, make it transparent
            if (processedPixels[i].r == 0 && processedPixels[i].g == 0 && processedPixels[i].b == 0)
            {
                processedPixels[i].a = 0; // Set alpha to 0 (transparent)
                originalPixels[i].a = 0;
            }
            else
            {
                processedPixels[i].a = 255; // Set alpha to 255 (opaque)
                originalPixels[i].a = 255;
            }
        }

        // Set the updated pixels back to the textures
        processedTexture.SetPixels32(processedPixels);
        renderedTexture2D.SetPixels32(originalPixels);

        // Apply the changes to the textures
        processedTexture.Apply();
        renderedTexture2D.Apply();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy(); // Call base OnDestroy to ensure everything is cleaned up

        _image?.Dispose();
        _processImage?.Dispose();

        if (renderedTexture != null)
        {
            Destroy(renderedTexture); // Unity's Destroy method for textures
            renderedTexture = null;
        }
    }

    void FlipImage() => Cv2.Flip(_image, _image, _flipMode);

    void InitializeProcessImageIfNull()
    {
        if (_processImage == null || _processImage.Size() != _image.Size())
        {
            _processImage?.Dispose(); // Dispose of the old Mat if it exists
            _processImage = new Mat(_image.Size(), MatType.CV_8UC1);
        }
    }

    void FindContours()
    {
        Cv2.FindContours(_processImage,
            out _contours,
            out _,
            RetrievalModes.Tree,
            ContourApproximationModes.ApproxSimple
        );

        _polygonCollider.pathCount = 0;
        foreach (Point[] contour in _contours)
        {
            Point[] points = Cv2.ApproxPolyDP(contour, _curveAccuracy, true);
            var area = Cv2.ContourArea(contour);

            // Leave out tiny clumps of pixels
            if (area > _minArea)
            {
                if (_outlineThickness > 0)
                {
                    DrawContour(_image, _redColourScalar, _outlineThickness, points);
                }
                
                _polygonCollider.pathCount++;
                _polygonCollider.SetPath(_polygonCollider.pathCount - 1, ToVector2(points));
            }
        }
        
        // Dispose off the array to clear memory
        _contours = null;
    }

    void DrawContour(Mat image, Scalar colour, int thickness, Point[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            Cv2.Line(image, points[i], points[(i + 1) % points.Length], colour, thickness);
        }
    }

    Vector2[] ToVector2(Point[] points)
    {
        Vector2[] vectorList = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            vectorList[i] = new Vector2(points[i].X, points[i].Y);
        }

        return vectorList;
    }

    void OnValidate() => SetUpProcessingImageUI();

    void SetUpProcessingImageUI()
    {
        _processedRawImage.color = new Color(_processedRawImage.color.r,
            _processedRawImage.color.g,
            _processedRawImage.color.b,
            _processingImageAlpha);
    }
    
    void ConvertToBlackAndWhite()
    {
        Cv2.CvtColor(_image, _processImage, ColorConversionCodes.BGR2GRAY); // Convert to greyscale
        Cv2.Threshold(_processImage, _processImage, _threshold, 255, ThresholdTypes.BinaryInv); // Apply threshold
    }
}