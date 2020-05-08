using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    AndroidJavaClass plugincall;
    public LevelManager levelmanager;
    public Camera myCam;
    private Rigidbody2D player;
    private float maxWidth;
    private bool cancontrol = true;
    public Animator animator;

    float PlayerXLimit;
    float PlayerSingleDisplacement;
    float CameraXpos;
    float PlayerStartPosition;
   
    //string textstring = "Null";
    //string previousString = "Null";

    //variable to hold a reference to our SpriteRenderer component
    //private SpriteRenderer mySpriteRenderer;


    //FMDRiver Variable declaration
    static AndroidJavaClass _pluginClass;
    static AndroidJavaObject _pluginInstance;
    const string driverPathName = "com.fitmat.fitmatdriver.Connection.DeviceControlActivity";
    string FMResponseCount = "";


    private bool IsFacingRight;

       
    class UnityCallback : AndroidJavaProxy
    {
        private System.Action<string> initializeHandler;
        public UnityCallback(System.Action<string> initializeHandlerIn) : base(driverPathName + "$UnityCallback")
        {
            initializeHandler = initializeHandlerIn;
        }

        public void sendMessage(string message)
        {
            Debug.Log("sendMessage: " + message);
            if (initializeHandler != null)
            {
                initializeHandler(message);
            }
        }
    }



    public static AndroidJavaClass PluginClass
    {
        get
        {
            if (_pluginClass == null)
            {
                _pluginClass = new AndroidJavaClass(driverPathName);
            }
            return _pluginClass;
        }
    }

    public static AndroidJavaObject PluginInstance
    {
        get
        {
            if (_pluginInstance == null)
            {
                AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
                _pluginInstance = PluginClass.CallStatic<AndroidJavaObject>("getInstance", activity);
            }
            return _pluginInstance;
        }
    }

    // Now make methods that you can provide the iOS functionality
    public static void InitBLEFramework()
    {
        // We check for UNITY_IPHONE again so we don't try this if it isn't iOS platform.
        #if UNITY_IPHONE
                // Now we check that it's actually an iOS device/simulator, not the Unity Player. You only get plugins on the actual device or iOS Simulator.
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    _InitBLEFramework();
                }
        #elif UNITY_ANDROID
                        if (Application.platform == RuntimePlatform.Android)
                        {
                            System.Action<string> callback = ((string message) =>
                            {
                                BLEFramework.Unity.BLEControllerEventHandler.OnBleDidInitialize(message);
                      
                            });
                            string macaddress = "A4:34:F1:A5:99:18";
                            int gameId = 2;
                            PluginInstance.Call("_setMACAddress", macaddress);
                            PluginInstance.Call("_setGameID", gameId);
                            PluginInstance.Call("_InitBLEFramework",new object[] { new UnityCallback(callback) });
                    
                }
        #endif
    }

    void Awake()
    {
        InitBLEFramework();
    }

    // Start is called before the first frame update
    void Start()
    {
        // get a reference to the SpriteRenderer component on this gameObject
        //mySpriteRenderer = GetComponent<SpriteRenderer>();
        player = GetComponent<Rigidbody2D>();
        IsFacingRight = false;
        if (myCam == null)
        {
            myCam = Camera.main;
        }
        Vector3 UpperCorner = new Vector3(Screen.width, Screen.height, 0.0f);
        Vector3 SceneWidth = myCam.ScreenToWorldPoint(UpperCorner);
        maxWidth = SceneWidth.x;

        PlayerStartPosition = myCam.transform.position.x;
        Vector3 InitialPlayerPosition = new Vector3(PlayerStartPosition, player.position.y, 0.0f);
        player.MovePosition(InitialPlayerPosition);

        //Depending upon the scene, wheteher 3 laned or 5 laned, player movment would be restricted.
        if ((levelmanager.currentLevel == "ECLevel1") || (levelmanager.currentLevel == "ECLevel3"))
        {
            //Decide X displacement limits for the player.
            PlayerSingleDisplacement = (maxWidth / 2);
            PlayerXLimit = PlayerSingleDisplacement;
        }
        else if (levelmanager.currentLevel == "ECLevel2")
        {
            //Decide X displacement limits for the player.
            PlayerSingleDisplacement = (maxWidth / 4);
            PlayerXLimit = 2 * PlayerSingleDisplacement;
        }
    }

    void Update()
    {
        if (cancontrol)
        {
            //#if UNITY_ANDROID
            string FMResponse = PluginClass.CallStatic<string>("_getFMResponse");
            Debug.Log("UNITY FMResponse: " + FMResponse);
            //Sample FMResponse : 
           /*
                {response_count}.{response}
                1.Left Move
                2.Right Move
                3.Jumping
                4.Bending
            */

            string[] FMTokens = FMResponse.Split('.');
            Debug.Log("UNITY FMTokens: " + FMTokens[0]);
            
            if (!FMTokens[0].Equals(FMResponseCount))
            {
                FMResponseCount = FMTokens[0];
                if (FMTokens[1] == "Right Move")
                {
                    float targetwidth = Mathf.Clamp(player.position.x - PlayerSingleDisplacement, PlayerStartPosition - PlayerXLimit, PlayerStartPosition + PlayerXLimit);
                    Vector3 newPlayerPosition = new Vector3(targetwidth, player.position.y, 0.0f);
                    player.MovePosition(newPlayerPosition);
                }
                if (FMTokens[1] == "Left Move")
                {
                    float targetwidth = Mathf.Clamp(player.position.x + PlayerSingleDisplacement, PlayerStartPosition - PlayerXLimit, PlayerStartPosition + PlayerXLimit);
                    Vector3 newPlayerPosition = new Vector3(targetwidth, player.position.y, 0.0f);
                    player.MovePosition(newPlayerPosition);
                }

            }
            
            

            //textstring = plugincall.CallStatic<string>("ReturnData");
            //if (previousString != textstring)
            //{
            //    previousString = textstring;
            //    if (textstring == "Left Move")
            //    {
            //        float targetwidth = Mathf.Clamp(player.position.x - PlayerSingleDisplacement, PlayerStartPosition - PlayerXLimit, PlayerStartPosition + PlayerXLimit);
            //        Vector3 newPlayerPosition = new Vector3(targetwidth, player.position.y, 0.0f);
            //        //if (IsFacingRight)
            //          //  FlipPlayer();
            //        player.MovePosition(newPlayerPosition);
            //    }
            //    if (textstring == "Right Move")
            //    {
            //        float targetwidth = Mathf.Clamp(player.position.x + PlayerSingleDisplacement, PlayerStartPosition - PlayerXLimit, PlayerStartPosition + PlayerXLimit);
            //        Vector3 newPlayerPosition = new Vector3(targetwidth, player.position.y, 0.0f);
            //        //if (!IsFacingRight)
            //         //   FlipPlayer();
            //        player.MovePosition(newPlayerPosition);
            //    }
            //    if (textstring == "Jumping")
            //    {

            //    }
            //    if (textstring == "Bending")
            //    {

            //    }
            //}

            //Read KeyBoard Input
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                float targetwidth = Mathf.Clamp(player.position.x - PlayerSingleDisplacement, PlayerStartPosition - PlayerXLimit, PlayerStartPosition + PlayerXLimit);
                Vector3 newPlayerPosition = new Vector3(targetwidth, player.position.y, 0.0f);
                player.MovePosition(newPlayerPosition);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                float targetwidth = Mathf.Clamp(player.position.x + PlayerSingleDisplacement, PlayerStartPosition - PlayerXLimit, PlayerStartPosition + PlayerXLimit);
                Vector3 newPlayerPosition = new Vector3(targetwidth, player.position.y, 0.0f);
                player.MovePosition(newPlayerPosition);
            }

            //Read Touch screen Input
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector3 touchPosition = myCam.ScreenToWorldPoint(touch.position);
                if (touch.phase == TouchPhase.Began)
                {
                    if (touchPosition.x < 0)
                    {
                        float targetwidth = Mathf.Clamp(player.position.x - PlayerSingleDisplacement, PlayerStartPosition - PlayerXLimit, PlayerStartPosition + PlayerXLimit);
                        Vector3 newPlayerPosition = new Vector3(targetwidth, player.position.y, 0.0f);
                        player.MovePosition(newPlayerPosition);
                    }
                    if (touchPosition.x > 0)
                    {
                        float targetwidth = Mathf.Clamp(player.position.x + PlayerSingleDisplacement, PlayerStartPosition - PlayerXLimit, PlayerStartPosition + PlayerXLimit);
                        Vector3 newPlayerPosition = new Vector3(targetwidth, player.position.y, 0.0f);
                        player.MovePosition(newPlayerPosition);
                    }
                }
            }
        }
    }

    public void toggelControl(bool toggle)
    {
        cancontrol = toggle;
    }

    //private void FlipPlayer()
    //{
    //    // if the variable isn't empty (we have a reference to our SpriteRenderer
    //    if (mySpriteRenderer != null)
    //    {
    //        Debug.Log("Flipping Player !!");
    //        //mySpriteRenderer.flipX = !mySpriteRenderer.flipX;
    //        IsFacingRight = !IsFacingRight;
    //    }
    //}

}

