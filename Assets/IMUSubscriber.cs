using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using ImuMsg = RosMessageTypes.Sensor.ImuMsg;
using System;
using UnityEngine.UI;

public class IMUSubscriber : MonoBehaviour
{
    [SerializeField]
    private GameObject model;

    [SerializeField]
    private String topicName = "/front_camera/image/compressed";

    private Quaternion initialOrientation;
    private Quaternion latestOrientation;


    // Start is called before the first frame update
    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<ImuMsg>(topicName, imuCallback); 
        initialOrientation = model.transform.rotation;
    }

    private void imuCallback(ImuMsg msg)
    {
        Debug.Log("Received IMU message");
        Debug.Log("Orientation: " + msg.orientation);
        Debug.Log("Angular Velocity: " + msg.angular_velocity);
        Debug.Log("Linear Acceleration: " + msg.linear_acceleration);
        latestOrientation.x = (float)msg.orientation.y;
        latestOrientation.y = -(float)msg.orientation.z;
        latestOrientation.z = (float)msg.orientation.x;
        latestOrientation.w = (float)msg.orientation.w;

        model.transform.rotation = initialOrientation * latestOrientation;
    }
}
