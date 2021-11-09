using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CollisionPlayerCamera();
public class GameManager : MonoBehaviour {
  public static GameManager gameManager;

  public event CollisionPlayerCamera collisionPlayerCamera;
  public event CollisionPlayerCamera collisionPlayerMic;

  void Awake() {
    if (!gameManager) {
      gameManager = this;
      DontDestroyOnLoad(this);
    } else if (gameManager != this) {
      Destroy(gameObject);
    }
  }

  public void ThrowCollisionPlayerCamera() {
    collisionPlayerCamera();
  }
  public void ThrowCollisionPlayerMic() {
    collisionPlayerMic();
  }
}
