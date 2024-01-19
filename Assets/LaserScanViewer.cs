using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using LaserScanMsg = RosMessageTypes.Sensor.LaserScanMsg;
using System;
using System.Runtime.InteropServices;
using UnityEngine.UI; // Add this using directive to resolve the 'RawImage' error

public class LaserScanViewer : MonoBehaviour
{
    [SerializeField]
    private String topicName = "/scan";

    [SerializeField]
    int radarSizeHeight;
    [SerializeField]
    int radarSizeWidth;

    [SerializeField]
    float markerSize;

    [SerializeField]
    int markerCount;
    private ParticleSystem.Particle[] points;

    [SerializeField]
    private ParticleSystem particleSystem;

    [SerializeField]
    Color markerColor;

    // Start is called before the first frame update
    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<LaserScanMsg>(topicName, LaserScanCallback); 
        points = new ParticleSystem.Particle[markerCount];
    }

    void Update()
    {
        particleSystem.SetParticles(points, points.Length);
    }   

    Vector2 minPos, maxPos;
    float angle;

    private float range;
    
    [SerializeField]
    float min_range = 0.05f;

    [SerializeField]
    float max_range = 2f;
    
    Vector2 position;

    [SerializeField]
    private Color min_color = Color.red;

    [SerializeField]
    private Color max_color = Color.green;


    private void LaserScanCallback(LaserScanMsg message)
    {    
        minPos = new Vector2(float.MaxValue, float.MaxValue);
        maxPos = new Vector2(float.MinValue, float.MinValue);
                
        // Loop through the ranges in the LaserScan message
        for (int i = 0; i < message.ranges.Length; i++)
        {
            // Calculate the angle for this range
            angle = ((float)i / message.ranges.Length) * 2 * Mathf.PI;

            // Get the range
            range = message.ranges[i];

            // Check if the range or angle is NaN
            if (float.IsNaN(range) || float.IsNaN(angle) || range > max_range || range < min_range)
            {
                // Debug.LogWarning("Invalid range or angle value");
                points[i].startSize = 0;
                continue;
            }

            // Calculate the position of the sphere
            position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * range;

            if(position.x < minPos.x) minPos.x = position.x;
            if(position.y < minPos.y) minPos.y = position.y;
            if(position.x > maxPos.x) maxPos.x = position.x;
            if(position.y > maxPos.y) maxPos.y = position.y;

            // Debug.LogFormat("minPos: {0}, maxPos: {1}", minPos, maxPos);

            points[i].position = position;
            points[i].color = Color.Lerp(min_color, max_color, (range - min_range) / (max_range - min_range));
            points[i].startSize = markerSize;
        }

        for (int i = message.ranges.Length; i < points.Length; i++)
        {
            points[i].startSize = 0;
        }

        particleSystem.SetParticles(points, points.Length);
    }
}