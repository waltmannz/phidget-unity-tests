using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Phidget22;using Phidget22.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{

  public GameObject obstacles;
  public float difMult;
  public float speed;
  private Rigidbody rb;
  private float vValue;
  private float psi;
  private float kPa;
  private float firstKpa;
  private int score;
  public Text scoreText;
  public Text winText;

  private VoltageRatioInput accel = new VoltageRatioInput();


  void Start(){
    rb = GetComponent<Rigidbody>();
    firstKpa = 0;
    score = 0;
    SetScoreText();
    winText.text = "";
    accel.IsHubPortDevice = true;
    accel.DeviceSerialNumber = 558696;
    //accel.DeviceSerialNumber = 560237;
    accel.Channel = 0;
    accel.HubPort = 0;
    accel.VoltageRatioChange += onVoltageRatioChange;
    //accel.SensorChange += onRatio_SensorChange;
    accel.Open(5000);



    
  }

  void onAttach(object sender, Phidget22.Events.AttachEventArgs e) {
        Debug.Log("Attach");
        //TODO: Set data interval to minimum
    }

    void onDetach(object sender, DetachEventArgs e)
    {
        Debug.Log("Detach");
        //TODO: Pause game 
    }

  void onVoltageRatioChange(object sender, VoltageRatioInputVoltageRatioChangeEventArgs e)
  {
    Debug.Log("VOLTAGE DATA");
    psi = (float)(((float)accel.VoltageRatio * 59.89) + 0.504);
    kPa = (float)((accel.VoltageRatio * 413.05 ) + 3.478);
    vValue = (float)accel.VoltageRatio;
  }

  void onRatio_SensorChange(object sender, VoltageRatioInputSensorChangeEventArgs e) {
      
      Debug.Log("SENSE");
    }

  void FixedUpdate ()
  {
    if(firstKpa == 0){
      firstKpa = kPa;
    }
    float difference = kPa - firstKpa;
    if (difference < 0.1){
      difference = 0;
    } else {

      rb.velocity = new Vector3(0, 0, rb.velocity.z);
      Vector3 diffMovement = new Vector3 (0, difference*difMult, 0);
      rb.AddForce (diffMovement, ForceMode.Impulse);
    }
    Debug.Log("difference" + difference + ", KPA: " + kPa + ", Score: " + score);

    if (Input.GetKey(KeyCode.Space))
    {
      rb.velocity = new Vector3(0, 0, 0);
      Vector3 spaceMovement = new Vector3 (0, 2*difMult, -1*speed);
      rb.AddForce(spaceMovement, ForceMode.Impulse);
    }

    Vector3 movement = new Vector3 (0, 0, -1*speed);
    rb.AddForce (movement);
    
  }

  void OnApplicationQuit()
  {
    Debug.Log("QUIT");
    //accel.Close();
    if (Application.isEditor)
      Phidget.ResetLibrary();
    else
      Phidget.FinalizeLibrary(0);
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.CompareTag("Pick Up"))
    {
      other.gameObject.SetActive(false);
      score++;
      SetScoreText();
    }
    if (other.gameObject.CompareTag("Obstacle") || other.gameObject.CompareTag("Floor")){
      SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }
  }

  void SetScoreText()
  {
    scoreText.text = "Count: " + score.ToString();
    if(score >= 11){
      winText.text = "FUCK YEAH";
    }
  }


}
