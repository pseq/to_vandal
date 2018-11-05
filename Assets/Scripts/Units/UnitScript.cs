using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitScript : MonoBehaviour {

    private bool selected;
    private SpriteRenderer selectMarker;
    private MeshRenderer targetMarker;
    private ArrayList selectableUnits;
    private GameObject gameControl;

    // Use this for initialization
    void Start () {
        // Получаем ссылку на маркер выбора юнита, и выключаем его.
        selectMarker = transform.Find("SelectMarker").GetComponent<SpriteRenderer>();
        selectMarker.enabled = false;
        // Получаем ссылку на маркер цели этого юнита, и выключаем его.
        targetMarker = transform.Find("TargetMarker").GetComponent<MeshRenderer>();
        targetMarker.enabled = false;

        // Получаем объект с общими параметрами игры.
        gameControl = GameObject.FindGameObjectWithTag("GameController");

        // Получаем массив выбираемых юнитов
        selectableUnits = gameControl.GetComponent<GameControl>().GetMinions();
    }

    public void SetSelected ()
    {
        // При выборе юнита - обозначаем его как выделенный.
        selected = true;
        selectMarker.enabled = true;

        // Добавляем в массив выбранных
        gameControl.GetComponent<GameControl>().AddSelected(gameObject);

        // Показываем маркер текущей цели миньона.
        gameControl.GetComponent<GameControl>().TargetMarkerUpdate();
    }

    public void UnsetSelected ()
    {
        // При отмене выбора юнита - снимаем выделение.
        selected = false;
        selectMarker.enabled = false;

        // Удаляем из массива выбранных
        gameControl.GetComponent<GameControl>().DeleteSelected(gameObject);

        // Выключаем маркер текущей цели
        if (targetMarker) gameControl.GetComponent<GameControl>().TargetMarkerUpdate();
    }

    public void TargetMarkerOn()
    {
        if (targetMarker) targetMarker.enabled = true;
    }

    public void TargetMarkerOff()
    {
        if (targetMarker) targetMarker.enabled = false;
    }

    private void OnMouseDown()
    {
        // По нажатию кнопки - снимаем все остальные выделения
        // и выделяем юнита. Но только если он миньон.
        if (selectableUnits.Contains(gameObject))
        {
            foreach (GameObject SelectableUnit in selectableUnits) SelectableUnit.gameObject.GetComponent<UnitScript>().UnsetSelected();
            SetSelected();
        }
    }





}
