using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using UnityEngine.UI;

public class BatteryMonitor : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private string topicName = "/power";

    public float BatteryVoltage;
    public float BatterySOC;

    private Color handleColour = Color.green;

    // Start is called before the first frame update
    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<BatteryStateMsg>(topicName, batteryMsgCallback);         
    }

    // Define the missing method batteryMsgCallback
    private void batteryMsgCallback(BatteryStateMsg msg)
    {
        // Debug.Log(msg.ToString());
        BatteryVoltage = msg.voltage;
        BatterySOC = msg.percentage;
        slider.value = msg.percentage * 100;
    }
}
