namespace BLEFramework.Unity
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class BLEControllerEventHandler : MonoBehaviour
    {
        //native events
        public delegate void OnBleDidConnectEventDelegate(string error);
        public static event OnBleDidConnectEventDelegate OnBleDidConnectEvent;

        public delegate void OnBleDidDisconnectEventDelegate(string error);
        public static event OnBleDidDisconnectEventDelegate OnBleDidDisconnectEvent;

        public delegate void OnBleDidReceiveDataEventDelegate(byte[] data, int numOfBytes);
        public static event OnBleDidReceiveDataEventDelegate OnBleDidReceiveDataEvent;

        public delegate void OnBleDidInitializeEventDelegate(string error);
        public static event OnBleDidInitializeEventDelegate OnBleDidInitializeEvent;

        public delegate void OnBleDidCompletePeripheralScanEventDelegate(string peripherals, string error);
        public static event OnBleDidCompletePeripheralScanEventDelegate OnBleDidCompletePeripheralScanEvent = HandleOnBleDidCompletePeripheralScanEvent;

        //Instance methods used by iOS Unity Send Message
        void OnBleDidInitializeMessage(string message)
        {
            BLEControllerEventHandler.OnBleDidInitialize(message);
        }

        public static void OnBleDidInitialize(string message)
        {
            string errorMessage = message != "Success" ? message : null;
#if UNITY_IOS
                Debug.Log("calling scanning peripherals.");
                InitBLE.ScanForPeripherals();
#else
            OnBleDidInitializeEvent?.Invoke(errorMessage);
#endif
        }


        void OnBleDidConnectMessage(string message)
        {
            BLEControllerEventHandler.OnBleDidConnect(message);
        }
        public static void OnBleDidConnect(string message)
        {
            string errorMessage = message != "Success" ? message : null;
#if UNITY_IPHONE
                //Debug.Log("calling BLEStatus");
                InitBLE.setMatConnectionStatus("CONNECTED");
#endif
            OnBleDidConnectEvent?.Invoke(errorMessage);
        }

        void OnBleDidDisconnectMessage(string message)
        {
            BLEControllerEventHandler.OnBleDidDisconnect(message);
        }
        public static void OnBleDidDisconnect(string message)
        {
            string errorMessage = message != "Success" ? message : null;
#if UNITY_IPHONE
                //Debug.Log("calling BLEStatus");
                InitBLE.setMatConnectionStatus("DISCONNECTED");
#endif
            OnBleDidDisconnectEvent?.Invoke(errorMessage);
        }

        void OnBleDidReceiveDataMessage(string message)
        {
            BLEControllerEventHandler.OnBleDidReceiveData(message);
        }
        public static void OnBleDidReceiveData(string message)
        {
            int numOfBytes = 0;
            if (int.TryParse(message, out numOfBytes))
            {
                if (numOfBytes != 0)
                {
                    Debug.Log("BLEController.GetData(); start");
                    // byte[] data = BLEController.GetData(numOfBytes);
                    Debug.Log("BLEController.GetData(); end");
                    // OnBleDidReceiveDataEvent?.Invoke(data, numOfBytes);
                }
                else
                {
                    Debug.Log("WARNING: did receive OnBleDidReceiveData even if numOfBytes is zero");
                }
            }
        }

        void OnBleDidCompletePeripheralScanMessage(string message)
        {
            BLEControllerEventHandler.OnBleDidCompletePeripheralScan(message);
        }

        public static void OnBleDidCompletePeripheralScan(string message)
        {
            string errorMessage = message != "Success" ? message : null;
            string peripheralJsonList = (errorMessage == null) ? InitBLE.GetListOfDevices() : null;

            /*
            if (peripheralJsonList != null)
            {
                Dictionary<string, object> dictObject = Json.Deserialize(peripheralJsonList) as Dictionary<string, object>;

                object receivedByteDataArray;
                if (dictObject.TryGetValue("deviceList", out receivedByteDataArray))
                {
                    peripheralsList = (List<object>)receivedByteDataArray;
                }
            }
            */


            Debug.LogError("peripheralJsonList from ble controller : " + peripheralJsonList);

            OnBleDidCompletePeripheralScanEvent?.Invoke(peripheralJsonList, errorMessage);
        }


        static async void HandleOnBleDidCompletePeripheralScanEvent(string peripherals, string errorMessage)
        {
            if (errorMessage == null)
            {
                if (InitBLE.isInitActive)
                {
                    /*********************************************************************
                         Received String 
                         A4:DA:32:4F:C2:54|YIPLI,F4:BF:80:63:E3:7A|honor Band 4-37A,F5:FB:4A:55:76:22|Mi Smart Band 4
                    **********************************************************************/
                    string[] allBleDevices = peripherals.Split(',');
                    for (int i = 0; i < allBleDevices.Length; i++)
                    {


                        string[] tempSplits = allBleDevices[i].Split('|');

                        Debug.Log("Mac : " + tempSplits[0] + " Device Name:" + tempSplits[1]);
                        if (tempSplits[1].Contains("YIPLI") && tempSplits[1].Length > 5)
                        {
                            string[] matID = tempSplits[1].Split('-');

                            /***********************************
                            // FOR Batch of 250 
                            // MAT NAME - YIPLI-001
                            /**********************************/

                            Debug.Log("Fetching data of Mat ID: " + matID[1]);

                            //MAC received from FB based on MAT ID
                            string macAddress = await FirebaseDBHandler.GetMacAddressFromMatIDAsync(matID[1]);

                            Debug.Log(macAddress + " " + InitBLE.MAC_ADDRESS);
                            if (InitBLE.MAC_ADDRESS == macAddress)
                            {
                                InitBLE.ConnectPeripheral(tempSplits[0]);
                            }
                        }
                        else if (tempSplits[1].Contains("YIPLI") && tempSplits[1].Length == 5)
                        {

                            /***********************************
                            // FOR NRF Boards and Batch 1 boards
                            // MAT NAME - YIPLI
                            /**********************************/


                            //----------
                            // Directly connect to MAT ID if valid mac address 
                            // FOR BATCH-1 BOARDS
                            //----------
                            string macAddress = tempSplits[0];

                            //----------
                            // Get MacAddress from GATT 
                            // FOR NRF BOARDS
                            //----------
                            // ~ TODO ~


                            Debug.Log(macAddress + " " + InitBLE.MAC_ADDRESS);
                            if (InitBLE.MAC_ADDRESS == macAddress)
                            {
                                InitBLE.ConnectPeripheral(tempSplits[0]);
                            }

                        }
                    }


                }
            }
        }
    }
}