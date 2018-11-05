using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class UnitMoving : MonoBehaviour {

    private NavMeshAgent agent;
    private Transform target;
    public float stoppingDistance;
    private float recalculateMovingDelay = .5f;
    private float recalculateDestRange = .5f;
    private Vector3 oldDestination;


    // Use this for initialization
    void Start () {
        // Получаем навигационного агента юнита.
        agent = gameObject.GetComponent<NavMeshAgent>();

        oldDestination = Vector3.zero;

        // И запускаем пересчет движения.
        StartCoroutine(MovingRecalc());
    }

    public void SetNavTarget(Transform target)
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        // При получении новой цели - заставляем агента двигаться.
        this.target = target;
        //Debug.Log("nav target = " + target);

        if (agent.isActiveAndEnabled) agent.isStopped = false;
    }
	
    IEnumerator MovingRecalc()
    {

        // Циклично, с задержкой пересчитываем движение
        while (agent.isActiveAndEnabled)
        {
            // Если обозначена цель - начинаем движение к ней.
            if (target)
            {
                // Если позиция цели изменилась не сильно
                if (Vector3.Distance(target.position, oldDestination) > recalculateDestRange)
                {
                    oldDestination = target.position;
                    agent.SetDestination(target.position);
                }
            }
            // А если нет - останавливаемся.
            else agent.isStopped = true;
            // Проверяем тип цели - враг или нет
            bool isEnemy = gameObject.GetComponent<UnitTargetManager>().IsTargetEnemy();
            // Если враг - то подходим к нему не ближе, чем на половину расстояния атаки
            if (isEnemy) agent.stoppingDistance = gameObject.GetComponent<UnitBattleController>().attackRange;
            else agent.stoppingDistance = stoppingDistance;
            yield return new WaitForSeconds(recalculateMovingDelay);
        }
    }
}
