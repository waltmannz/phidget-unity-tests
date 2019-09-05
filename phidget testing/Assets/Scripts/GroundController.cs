using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phidget22;using Phidget22.Events;

public class GroundController : MonoBehaviour
{

  public GameObject player;
  private Rigidbody rb;
  private float rollAngle;
  private float pitchAngle;
  private float rollA;
  private float pitchA;
  private float timeCount = 0.0f;
  private Spatial accel = new Spatial();


  void Start(){
    rb = GetComponent<Rigidbody>();
    
    accel.DeviceSerialNumber = 558696;
    accel.HubPort = 1;
    accel.Channel = 0;
    accel.Attach += onAttach;
    accel.Detach += onDetach;
    accel.SpatialData += onSpatialData;
    accel.Open(5000);
    accel.DataInterval = 100;

  }

  void onAttach(object sender, AttachEventArgs e)
    {
        Debug.Log("Attach");
        accel.DataInterval = 100;
        Debug.Log(accel.MinDataInterval);
        Debug.Log(accel.DataInterval);

        //TODO: Set data interval to minimum
    }

    void onDetach(object sender, DetachEventArgs e)
    {
        Debug.Log("Detach");
        //TODO: Pause game 
    }

  void onSpatialData(object sender, SpatialSpatialDataEventArgs e)
  {
    Debug.Log("SPATIAL DATA");
    //example of accessing acceleration data!
    float xAxis = (float)e.Acceleration[0];
    float yAxis = (float)e.Acceleration[1];
    float zAxis = (float)e.Acceleration[2];
    //Debug.Log(xAxis + " " + yAxis + " " + zAxis);
    //TODO: Calculate tilt angles
    rollAngle = Mathf.Atan2(yAxis, zAxis);
    pitchAngle = Mathf.Atan(-xAxis / (yAxis * Mathf.Sin(rollAngle) + zAxis * Mathf.Cos(rollAngle)));

    rollA = rollAngle * ((float)180.0 / Mathf.PI);
    pitchA = (float)pitchAngle * ((float)180.0 / Mathf.PI);
    //pitchAngle = Mathf.Atan(xAxis/zAxis);
    Debug.Log(rollAngle * (180.0 / Mathf.PI) + " " + pitchAngle * (180.0 / Mathf.PI));
    //store angles in variables for use in Fixed Update
  }

  void FixedUpdate ()
  {
    //Convert accelerometer angles to quaternion format
    Quaternion target = Quaternion.Euler(pitchA, 0, rollA);

    player.transform.rotation = Quaternion.Slerp(player.transform.rotation, target, timeCount);
    //player.transform.rotation = target;
    //player.transform.Rotate (pitchAngle * Time.deltaTime, 0 * Time.deltaTime, rollAngle * Time.deltaTime);
    timeCount = timeCount + Time.deltaTime;
    
  }

  void OnApplicationQuit()
  {
    Debug.Log("QUIT");
    if (Application.isEditor)
      Phidget.ResetLibrary();
    else
      Phidget.FinalizeLibrary(0);
  }


}
