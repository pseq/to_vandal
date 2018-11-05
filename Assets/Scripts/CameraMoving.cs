using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : MonoBehaviour {

    public float camSpeed = 5f;
    private int border = 10;
    private int maxX;
    private int maxY;
    public GameObject leftStopper;
    public GameObject rightStopper;
    public GameObject topStopper;
    public GameObject downtStopper;
    private Renderer leftStopperRenderer;
    private Renderer rightStopperRenderer;
    private Renderer topStopperRenderer;
    private Renderer downtStopperRenderer;

    // Use this for initialization
    void Start () {
        // Определяем зону экрана, которая будет двигать камеру.
        maxX = Screen.width - border;
        maxY = Screen.height - border;

        // Получаем рендереры ограничителей перемещения камеры.
        leftStopperRenderer = leftStopper.GetComponent<Renderer>();
        rightStopperRenderer = rightStopper.GetComponent<Renderer>();
        topStopperRenderer = topStopper.GetComponent<Renderer>();
        downtStopperRenderer = downtStopper.GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {
        // Двигаем камеру.
        if (Input.mousePosition.x < border && !leftStopperRenderer.isVisible) gameObject.transform.position = gameObject.transform.position + Vector3.left*camSpeed / 10;
        if (Input.mousePosition.x > maxX && !rightStopperRenderer.isVisible) gameObject.transform.position = gameObject.transform.position + Vector3.right*camSpeed / 10;
        if (Input.mousePosition.y > maxY && !topStopperRenderer.isVisible) gameObject.transform.position = gameObject.transform.position + Vector3.forward*camSpeed / 10;
        if (Input.mousePosition.y < border && !downtStopperRenderer.isVisible) gameObject.transform.position = gameObject.transform.position + Vector3.back*camSpeed / 10;
    }

}
