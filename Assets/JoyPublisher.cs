using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using System;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using System.Collections;

public class JoyPublisher : MonoBehaviour
{
    PlayerControls controls;
    Vector2 move = Vector2.zero;
    Vector2 look = Vector2.zero;
    Vector2 dpad = Vector2.zero;
    Vector2 leftPad = Vector2.zero;
    Vector2 rightPad = Vector2.zero;


    JoyMsg joyMsg;

    [SerializeField]
    bool LightOn = false;

    ROSConnection ros;

    [SerializeField]
    private String topicName = "joy";

      
    [SerializeField]
    [Tooltip("Frequency to publish messages at (in Hz)")]
    private int publishMessageFrequency = 25;

    private float publishMessageInterval;

    // Used to determine how much time has elapsed since the last message was published
    private float timeElapsed;

    void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        look = Vector2.zero;
        move = Vector2.zero;
        dpad = Vector2.zero;
    }
    private void OnDisable()
    {
        controls.Player.Disable();
        look = Vector2.zero;
        move = Vector2.zero;
        dpad = Vector2.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        publishMessageInterval = publishMessageFrequency / 1000.0f;

        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<JoyMsg>("joy");  

        joyMsg = new JoyMsg();  
        joyMsg.axes = new float[10];
        joyMsg.buttons = new int[10];

        controls.Player.ButtonA.performed += ctx => ToggleLights(ctx);
    }

    void ToggleLights(InputAction.CallbackContext context)
    {
        if(LightOn)
        {
            StartCoroutine(GetRequest("http://tb4:8000/lights_off"));
        }
        else
        {
            StartCoroutine(GetRequest("http://tb4:8000/lights_on"));
        }
        
        LightOn = !LightOn;
    }

    Vector2 CheckBounds(Vector2 v)
    {
        if (v.x > 1)
            v.x = 1;
        else if (v.x < -1)
            v.x = -1;

        if (v.y > 1)
            v.y = 1;
        else if (v.y < -1)
            v.y = -1;

        return v;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > publishMessageInterval)
        {        
            // Debug.LogFormat("Move: {0}, {1}, Look: {2}, {3}", move.x, move.y, look.x, look.y);

            move = CheckBounds(controls.Player.Move.ReadValue<Vector2>());
            look = CheckBounds(controls.Player.Look.ReadValue<Vector2>());
            dpad = CheckBounds(controls.Player.DPad.ReadValue<Vector2>());
            leftPad = CheckBounds(controls.Player.LeftPad.ReadValue<Vector2>());
            rightPad = CheckBounds(controls.Player.RightPad.ReadValue<Vector2>());

            joyMsg.axes[0] = move.x;
            joyMsg.axes[1] = move.y;    
            joyMsg.axes[2] = look.x;
            joyMsg.axes[3] = look.y;
            joyMsg.axes[4] = dpad.x;
            joyMsg.axes[5] = dpad.y;
            joyMsg.axes[6] = leftPad.x;
            joyMsg.axes[7] = leftPad.y;
            joyMsg.axes[8] = rightPad.x;
            joyMsg.axes[9] = rightPad.y;

            joyMsg.buttons[0] = (int)controls.Player.ButtonA.ReadValue<float>();
            joyMsg.buttons[1] = (int)controls.Player.ButtonB.ReadValue<float>();
            joyMsg.buttons[2] = (int)controls.Player.ButtonX.ReadValue<float>();
            joyMsg.buttons[3] = (int)controls.Player.ButtonY.ReadValue<float>();
            joyMsg.buttons[4] = (int)controls.Player.LeftBumper.ReadValue<float>();
            joyMsg.buttons[5] = (int)controls.Player.RightBumper.ReadValue<float>();
            joyMsg.buttons[6] = (int)controls.Player.RearLeftDown.ReadValue<float>();
            joyMsg.buttons[7] = (int)controls.Player.RearLeftUp.ReadValue<float>();
            joyMsg.buttons[8] = (int)controls.Player.RearRightDown.ReadValue<float>();
            joyMsg.buttons[9] = (int)controls.Player.RearRightUp.ReadValue<float>();

            ros.Publish(topicName, joyMsg);
            timeElapsed = 0;
        }
    }

    void OnValidate()
    {
        publishMessageInterval = publishMessageFrequency / 1000.0f;
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }
}
