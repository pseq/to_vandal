using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectorScript : EventTrigger {

    private Vector2 startDragPoint;
    private Vector2 endDragPoint;
    private ArrayList selectableUnits;
    private GameObject selectionRect;
    private RectTransform selectionTransform;
    public GameObject gameControl;

    // Use this for initialization
    void Start () {
        // Получаем объект и трансформ рамки выделения.
        selectionRect = GameObject.FindWithTag("SelectionRect");
        selectionTransform = selectionRect.gameObject.GetComponent<RectTransform>();
        // Тушим рамку
        selectionRect.GetComponent<Image>().enabled = false;
        // Ставим анкер рамки вниз-влево
        // и выставляем размеры панели по размерам холста, для адекватного расчета координат
        selectionTransform.anchorMin = Vector2.zero;
        selectionTransform.anchorMax = Vector2.zero;
        gameObject.GetComponent<RectTransform>().sizeDelta = gameObject.transform.parent.GetComponent<RectTransform>().sizeDelta;
        // Получаем массив выделяемых объектов
        selectableUnits = gameControl.GetComponent<GameControl>().GetMinions();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        // При начале перетаскивания - включаем рамку.
        selectionRect.GetComponent<Image>().enabled = true;
        // Точка начала драга.
        startDragPoint = Input.mousePosition;
        // Ставим в нее угол рамки.
        selectionTransform.anchoredPosition = startDragPoint;
    }

    // Вспомогательный метод пересчета координат углов прямоугольника в rect.
    private Rect CoordsToRect(Vector2 startPoint, Vector2 endPoint, Vector2 pivotValues)
    {
        Rect rectangle = new Rect(Vector2.zero, Vector2.zero);
        //вычисляем размеры прямоугольника, независимо от знака и значений координат.
        rectangle.width = Mathf.Abs(startPoint.x - endPoint.x);
        rectangle.height = Mathf.Abs(startPoint.y - endPoint.y);

        // Вычисляем координаты пивота.
        if (startPoint.x > endPoint.x) rectangle.x = pivotValues.x;
        else rectangle.x = pivotValues.y;
        if (startPoint.y > endPoint.y) rectangle.y = pivotValues.x;
        else rectangle.y = pivotValues.y;

        return rectangle;
    }

    // Тот же метод, но без перестановки пивота.
    private Rect CoordsToRect(Vector2 startPoint, Vector2 endPoint)
    {
        Rect rectangle = new Rect(Vector2.zero, Vector2.zero);
        // Вычисляем размеры прямоугольника, независимо от знака и значений координат.
        rectangle.width = Mathf.Abs(startPoint.x - endPoint.x);
        rectangle.height = Mathf.Abs(startPoint.y - endPoint.y);

        // Вычисляем координаты прямоугольника.
        if (startPoint.x > endPoint.x) rectangle.x = endPoint.x; 
        else rectangle.x = startPoint.x;
        if (startPoint.y > endPoint.y) rectangle.y = endPoint.y;
        else rectangle.y = startPoint.y;

        return rectangle;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        // Переставляем пивот рамки в зависимости от направления выделения.
        selectionTransform.pivot = CoordsToRect(startDragPoint, Input.mousePosition, new Vector2(1f, 0f)).position;
        // Рисуем рамку вслед за курсором.
        selectionTransform.sizeDelta = CoordsToRect(startDragPoint, Input.mousePosition, new Vector2(1f, 0f)).size;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        // При окончании перетаскивания - выключаем рамку и получаем точку окончания.
        endDragPoint = Input.mousePosition;
        selectionRect.GetComponent<Image>().enabled = false;

        // Получаем прямоугольник, попавший в выделение.
        Rect SelectRect = CoordsToRect(startDragPoint, endDragPoint);

        // Сначала снимаем выделение со всех объектов.
        foreach (GameObject SelectableUnit in selectableUnits) SelectableUnit.gameObject.GetComponent<UnitScript>().UnsetSelected();
        // А потом - выделяем те, что попали в прямоугольник.
        foreach (GameObject SelectableUnit in selectableUnits)
        {
            Vector2 UnitScreenPosition = Camera.main.WorldToScreenPoint(SelectableUnit.transform.position);
            if (SelectRect.Contains(UnitScreenPosition)) SelectableUnit.gameObject.GetComponent<UnitScript>().SetSelected();
        }
    }
}
