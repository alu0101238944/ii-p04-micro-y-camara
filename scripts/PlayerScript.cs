using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {
  void OnCollisionEnter(Collision collision) {
    if (collision.gameObject.tag == "Camera") {
      GameManager.gameManager.ThrowCollisionPlayerCamera();
    }
    if (collision.gameObject.tag == "Mic") {
      GameManager.gameManager.ThrowCollisionPlayerMic();
    }
  }
}
