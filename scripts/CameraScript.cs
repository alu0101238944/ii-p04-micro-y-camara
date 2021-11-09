using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

  private Renderer rd;
  private WebCamTexture webcamTexture;
  void Awake() {
    rd = GetComponent<Renderer>();
  }

  void Start() { 
    GameManager.gameManager.collisionPlayerCamera += playPause;
    webcamTexture = new WebCamTexture();
    rd.material.mainTexture = webcamTexture;
  }

  void playPause() {
    if (!webcamTexture.isPlaying) {
      webcamTexture.Play();
    } else {
      webcamTexture.Pause();
    }
  }
}
