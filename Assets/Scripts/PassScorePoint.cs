using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassScorePoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Agregar puntos
        GameManager.singleton.AddScore(1);

        // Aumentar 1 al perfectPass
        FindObjectOfType<BallControler>().perfectPass++;
    }
}
