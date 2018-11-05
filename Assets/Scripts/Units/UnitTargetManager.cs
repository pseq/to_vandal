using System.Collections;
using UnityEngine;


public class UnitTargetManager : MonoBehaviour {

    private GameObject gameControl;
    private GameObject fountain;
    private GameObject sofa;
    private GameObject target;
    private bool isMinion;
    private ArrayList opponentArray;
    private float targetUpdateDelay = .5f;
    private GameObject firstTarget;

    // Use this for initialization
    void Start()
    {
        // Получаем объект с общими параметрами игры, фонтан и диван.
        gameControl = GameObject.FindGameObjectWithTag("GameController");
        fountain = GameObject.FindGameObjectWithTag("Fountain");
        sofa = GameObject.FindGameObjectWithTag("DeveloperSofa");

        // На какой стороне юнит?
        isMinion = gameControl.GetComponent<GameControl>().IsMinion(gameObject);

        // Получаем ссылку на массив с противниками.
        if (isMinion) opponentArray = gameControl.GetComponent<GameControl>().GetEnemies();
        else opponentArray = gameControl.GetComponent<GameControl>().GetMinions();

        // Запускаем автообновление цели
        StartCoroutine(TargetUpdater());
    }

    // Указатели на текущие цели для дебага
    private void Update()
    {
            if (target && isMinion) Debug.DrawLine(gameObject.transform.position, target.transform.position, Color.green, .1f);
            if (target && !isMinion) Debug.DrawLine(gameObject.transform.position, target.transform.position, Color.red, .1f);
    }

    // Логика автовыбора цели
    public void AutosetTarget()
    {
        // Миньоны движутся к фонтану, а при появлении врагов - к ним
        if (isMinion)
        {
            if (firstTarget) SetTarget(firstTarget);
                else
                {
                    if (opponentArray.Count > 0) ClosestEnemySearch();
                        else SetTarget(fountain);
                }
        }
        else
        {
            // Враги при отсутствии миньонов атакуют диван.
            // Если есть миньон ближе дивана и здоровье > 50% - враг атакует этого миньона
            if (opponentArray.Count > 0)
            {
                ClosestEnemySearch();
                bool sofaClosestThanMinion = (GObjDistance(gameObject, sofa) < GObjDistance(gameObject, target));
                bool badHealth = (gameObject.GetComponent<MortalScript>().GetHP() < gameObject.GetComponent<MortalScript>().maxhp / 2);
                if (sofaClosestThanMinion || badHealth) SetTarget(sofa);
            }
            else SetTarget(sofa);
        }
    }

    // Ищем ближайшего противника и устанавливаем целью
    public void ClosestEnemySearch()
    {
        SetTarget((GameObject)opponentArray[0]);
        foreach (GameObject opp in opponentArray) if (GObjDistance(gameObject, opp) < GObjDistance(gameObject, target)) target = opp;
    }

    // Расстояние между двумя объектами
    private float GObjDistance(GameObject a, GameObject b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    // Установка приоритетной цели при выделении миньона
    public void SetFirstTarget(GameObject target)
    {
        SetTarget(target);
        firstTarget = target;
    }

    // Установка цели
    public void SetTarget(GameObject target)
    {
        // Назначить новую цель
        this.target = target;
        // Миньонам - переставить маркеры цели
        if (isMinion) gameControl.GetComponent<GameControl>().TargetMarkerUpdate();

        // При выборе цели - начинаем к ней двигаться
        if (target) gameObject.GetComponent<UnitMoving>().SetNavTarget(target.transform);
    }

    // Периодическое обновление цели
    IEnumerator TargetUpdater()
    {
        while (true)
        {
            AutosetTarget();
            yield return new WaitForSeconds(targetUpdateDelay);
        }
    }

    public GameObject GetTarget()
    {
        return target;
    }

    public bool IsTargetEnemy()
    {
        if (target)
        {
            // Для минионов противники - враги.
            if (isMinion) return (opponentArray.Contains(target));
            // Для врагов - миньоны и диван
            else return (opponentArray.Contains(target) || (target.GetInstanceID() == sofa.GetInstanceID()));
        }
        else return false;
        
    }

    // Если нажали правой кнопкой на врага - назначить его целью для всех выделенных миньонов.
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) && !gameObject.GetComponent<UnitTargetManager>().isMinion)
        {
            ArrayList selectedArr = gameControl.GetComponent<GameControl>().GetSelected();
            for (int i = 0; i < selectedArr.Count; ++i)
            {
                GameObject selected = (GameObject) selectedArr[i];
                if (selected)
                {
                    selected.GetComponent<UnitTargetManager>().SetFirstTarget(gameObject);
                }
            }
        }
    }
}
