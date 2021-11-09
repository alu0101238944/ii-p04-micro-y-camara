using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneScript : MonoBehaviour {

  private AudioSource audioSource;
  private int state = 0;
  void Awake() {
    audioSource = GetComponent<AudioSource>();
  }

  void Start() {
    GameManager.gameManager.collisionPlayerMic += recordEndPlay;
  }

  void recordEndPlay() {
    if (Microphone.GetPosition(null) > -1) {
      if (state == 0 || state == 1) { // Record
        if (!Microphone.IsRecording(null)) {
          Debug.Log("Recording");
          audioSource.clip = Microphone.Start(null, false, 10, 44100);
          state = 1;
        } else {
          Debug.Log("End");
          Microphone.End(null);
          state = 2;
        }
      } else if (state == 2) { // Play
        Debug.Log("Play");
        audioSource.Play();
        state = 0;
      }
    }
  }
}
