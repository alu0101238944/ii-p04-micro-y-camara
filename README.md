# Práctica 4: Micrófono y cámara <!-- omit in toc -->

## Tabla de contenidos <!-- omit in toc -->
- [Objetivo](#objetivo)
  - [Definición](#definición)
  - [Evidencia gráfica](#evidencia-gráfica)
- [Apartados](#apartados)
  - [Apartado 1: Colisión Player con la cámara y sus efectos](#apartado-1-colisión-player-con-la-cámara-y-sus-efectos)
  - [Apartado 2: Colisión Player con el micrófono y sus efectos](#apartado-2-colisión-player-con-el-micrófono-y-sus-efectos)

## Realizado por: <!-- omit in toc -->

- José Daniel Escánez Expósito (alu0101238944)

# Objetivo

## Definición

El objetivo de esta práctica familiarizarse con los componentes de cámara y micrófono de Unity3D. Se debe crear una escena en la que se pueda visualizar por pantalla el vídeo capturado por una cámara, y opcionalmente permitir su pausa y reanudación mediante elementos de la interfaz. En lo que al micrófono se refiere, se deberá de permitir también la grabación de pequeños segmentos de audio y la reproducción de los mismos. En la entrega se solicita este mismo informe que exponga el funcionamiento de la aplicación generada, acompañado de todos los [scripts](./scripts) desarrollados.

## Evidencia gráfica

[![Miniatura del vídeo demostrativo de la escena realizada](./img/miniatura)](https://youtu.be/_-xzEbgMZWM)

# Apartados

Antes de describir los objetivos logrados, se presentará la disposición inicial de la escena.

Se utiliza un terreno con una textura aplicada a modo de suelo. Se incluye una Cámara principal fija y una luz direccional. Se descargaron tres modelos 3D de la Asset Store de Unity: Un [Third Person Controller](https://assetstore.unity.com/packages/tools/game-toolkits/third-person-controller-basic-locomotion-free-82048), una [cámara](https://assetstore.unity.com/packages/3d/props/electronics/camera-hq-128822) y un [micrófono](https://assetstore.unity.com/packages/3d/props/tools/microphone-mic-dj-pbr-123201). El primero, para mover al personaje que tendrá un cubo en la cabeza con la intención de imprimir en el la textura del vídeo recogido por la cámara en tiempo real; el segundo, para accionar los eventos relativos a la repreducción, pausa y reanudación del vídeo; y el tercero, para realizar un proceso similar con la grabación y reproducción de audio.

También se diseñó una clase [GameManager](./scripts/GameManager.cs) que se encarga de gestionar los correspondientes eventos de manera global en la escena. Esta cuenta con un atributo estático público `gameManager`, para acceder a los eventos desde otros scripts; y 2 eventos públicos, uno para cada evento (`collisionPlayerCamera` y `collisionPlayerMic`). Se inicializa en el `Awake` de manera que solo pueda existir una instancia del mismo. Por último cuanta con las funciones públicas que lanzan los eventos anteriormente mencionados (`ThrowCollisionPlayerCamera` y `ThrowCollisionPlayerMic`). Este Script se le añadió a un `Empty GameObject` con el nombre `GameManager`.

```cs
// GameManager.cs

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

```

## Apartado 1: Colisión Player con la cámara y sus efectos

- Cuando el jugador `Player` colisiona con un objeto con el tag `Camera`, se envía un evento desde `Player` hasta el cubo que tiene el TPC como cabeza para que ejecute el método `playPause`. Este detendrá el vídeo, si este se encontraba activo; o lo reanudará, en caso contrario.

```cs
// En PlayerScript.cs
...

public class PlayerScript : MonoBehaviour {
  void OnCollisionEnter(Collision collision) {
    if (collision.gameObject.tag == "Camera") {
      GameManager.gameManager.ThrowCollisionPlayerCamera();
    }
    ...
  }
}
```

- El Script [CameraScript.cs](./scripts/CameraScript.cs) cuenta también con dos atributos privados: uno que corresponde al `Renderer` y otro a la `WebCamTexture`. El primero se inicializa en el `Awake` y el segundo en el `Start`. En este último se aplica la textura al material del Renderer y se suscribe al evento `collisionPlayerCamera`, emitido por el `gameManager`.

```cs
// CameraScript.cs, aplicado al cubo que hace de cabeza del TPC

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
```

## Apartado 2: Colisión Player con el micrófono y sus efectos

- Cuando el jugador `Player` colisiona con un objeto con el tag `Mic`, se envía un evento desde `Player` hasta el micrófono para que ejecute el método `recordEndPlay`. Este iniciará la grabación de audio en la primera colisión, la detendrá en la segunda y la reproducirá en la tercera.

```cs
// En PlayerScript.cs
...

public class PlayerScript : MonoBehaviour {
  void OnCollisionEnter(Collision collision) {
    ...
    if (collision.gameObject.tag == "Mic") {
      GameManager.gameManager.ThrowCollisionPlayerMic();
    }
  }
}
```

- El Script [MicrophoneScript.cs](./scripts/MicrophoneScript.cs) cuenta también con dos atributos privados: uno que corresponde al `AudioSource` y otro a un identificador entero `state`. El primero se inicializa en el `Awake`, tomando el componente del gameObject; y el segundo en la declaración del mismo.

- En el método `Start`, se suscribe al evento `collisionPlayerMic` el método `recordEndPlay`. Este se encargará de tener el control del estado en el que está para ir realizando las diferentes tareas: grabar (gracias al método `Microphone.Start`), detener la grabación (con `Microphone.End`) y reproducirla (por medio de `audioSource.Play`).

```cs
// MicrophoneScript.cs

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

```
