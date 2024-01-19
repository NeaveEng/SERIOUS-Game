using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using CompressedImageMsg = RosMessageTypes.Sensor.CompressedImageMsg;
using System;
using UnityEngine.UI;

public class ImageViewer : MonoBehaviour
{
    [SerializeField]
    private RawImage rawImage; // Add this field to reference your RawImage component

    [SerializeField]
    private String topicName = "/front_camera/image/compressed";
    
    Texture2D img;

    // CompressedImageDefaultVisualizer ciVisualiser = new CompressedImageDefaultVisualizer();

    // Start is called before the first frame update
    void Start()
    {
        img = new Texture2D(1000, 1000);
        ROSConnection.GetOrCreateInstance().Subscribe<CompressedImageMsg>(topicName, CompressedImageCallback); 
    }

    private void CompressedImageCallback(CompressedImageMsg msg)
    {
        
        img.LoadImage(msg.data);
        rawImage.texture = img;
    }
}
