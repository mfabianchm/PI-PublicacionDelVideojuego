using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelixController : MonoBehaviour
{
    // Movimiento
    private Vector2 lastTapPosition;
    // Posicion Helix
    private Vector3 starRotation;

    public Transform topTransform;
    public Transform goalTransform;
    public GameObject helixLevelPrefab;
    public List<Stage> allStages = new List<Stage>();
    public float helixDistance;
    private List<GameObject> spawnedLevels = new List<GameObject>();

    private void Awake()
    {
        starRotation = transform.localEulerAngles;
        helixDistance = topTransform.localPosition.y - (goalTransform.localPosition.y + .1f);
        LoadStage(0);
    }

    void Update()
    {
        // Izquierdo
        if (Input.GetMouseButton(0))
        {
            // Donde hemos tocado con el mouse
            Vector2 currentTapPosition = Input.mousePosition;

            if (lastTapPosition == Vector2.zero)
            {
                lastTapPosition = currentTapPosition;
            }

            float distance = lastTapPosition.x - currentTapPosition.x;

            // Actualizar la ultima posicion
            lastTapPosition = currentTapPosition;

            // Mover pantalla
            transform.Rotate(Vector3.up * distance);
        }

        if (Input.GetMouseButtonUp(0))
        {
            lastTapPosition = Vector2.zero;
        }
    }

    public void LoadStage(int stageNumber)
    {
        // No cargar mas de los que hay registrados
        Stage stage = allStages[Math.Clamp(stageNumber, 0, allStages.Count - 1)];
        if (stage == null)
        {
            Debug.Log("No hay niveles");
            return;
        }

        // Colores
        Camera.main.backgroundColor = allStages[stageNumber].stageBackgroundColor;
        FindObjectOfType<BallControler>().GetComponent<Renderer>().material.color = allStages[stageNumber].stageBallColor;

        // Posicion inicial
        transform.localEulerAngles = starRotation;

        // Limpiar niveles anteriores
        foreach (GameObject go in spawnedLevels)
        {
            Destroy(go);
        }

        float levelDistance = helixDistance / stage.levels.Count;
        // Position y
        float spawnPosY = topTransform.localPosition.y;

        // Crear diferentes plataformas
        for (int i = 0; i < stage.levels.Count; i++)
        {
            // Distancia entre una plataforma y la otra
            spawnPosY -= levelDistance;

            // Spawn level
            GameObject level = Instantiate(helixLevelPrefab, transform);

            // Posiciones correctas
            level.transform.localPosition = new Vector3(0, spawnPosY, 0);

            // Añadir a spawned levels
            spawnedLevels.Add(level);

            // Partes abiertas
            int partsToDisable = 12 - stage.levels[i].partCount;

            // Saber cuales descavtivamos
            List<GameObject> disabledParts = new List<GameObject>();

            while (disabledParts.Count < partsToDisable)
            {
                GameObject randomPart = level.transform.GetChild(
                    UnityEngine.Random.Range(0, level.transform.childCount)).gameObject;

                // Esta parte se desactiva
                if (!disabledParts.Contains(randomPart))
                {
                    randomPart.SetActive(false);
                    // Añadir a la lista
                    disabledParts.Add(randomPart);
                }
            }

            // Cambios de colores
            // Partes que nos sobran
            List<GameObject> leftParts = new List<GameObject>();

            foreach (Transform t in level.transform)
            {
                t.GetComponent<Renderer>().material.color = allStages[stageNumber].stageLevelPartColor;
                if (t.gameObject.activeInHierarchy)
                {
                    leftParts.Add(t.gameObject);
                }
            }

            List<GameObject> deathParts = new List<GameObject>();

            while (deathParts.Count < stage.levels[i].deathPartCount)
            {
                GameObject randomParts = leftParts[UnityEngine.Random.Range(0,leftParts.Count)];

                if (!deathParts.Contains(randomParts))
                {
                    // Agregar el script de death part
                    randomParts.gameObject.AddComponent<DeathPart>();
                    // Añadir a la lista
                    deathParts.Add(randomParts);
                }
            }
        }
    }
}
