using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBattleController : MonoBehaviour {

    private GameObject target;
    public float attackRange;
    public float attackDamage;
    public float attackSpeed;
    private float checkDelay = .5f;
    private SpriteRenderer attackMarker;
    private Color markerColor;

    // Use this for initialization
    void Start () {
        // Получаем ссылку на маркер атаки, и выключаем его
        attackMarker = transform.Find("AttackMarker").gameObject.GetComponent<SpriteRenderer>();
        markerColor = attackMarker.material.color;
        markerColor.a = 0;
        attackMarker.material.color = markerColor;

        // Устанавливаем размер маркера атаки по радиусу атаки юнита.
        float markerSize = attackMarker.bounds.size.x / 2;
        attackMarker.transform.localScale = Vector3.one * attackRange / markerSize;

        // Запускаем проверку цели.
        StartCoroutine(TargetCheck());
    }
 
    public float GetAttackRange()
    {
        return attackRange;
    }

    IEnumerator TargetCheck()
    {
        // Циклично с задержкой проверяем цель.
        while (true)
        {
            // Проверка наличия цели.
            target = gameObject.GetComponent<UnitTargetManager>().GetTarget();
            if (target)
            {
                // Является ли цель врагом.
                if (gameObject.GetComponent<UnitTargetManager>().IsTargetEnemy())
                {
                    // Проверка расстояния до цели.
                    float distance = Vector3.Distance(transform.position, target.transform.position);
                    if (distance <= attackRange)
                    {
                        // Если всё совпадает - атакуем.
                        yield return StartCoroutine(Attack());
                    }
                }
            }
            yield return new WaitForSeconds(checkDelay);
        }
    }

    IEnumerator Attack()
    {
        // Если цель никуда не делась - атакуем её.
        if (target)
        {
            // Наносим удар.
            target.GetComponent<MortalScript>().Hit(attackDamage);
            // И рисуем маркер атаки.
            StartCoroutine(AttackAnimation());
        }
        yield return new WaitForSeconds(1 / attackSpeed);
    }
 
    IEnumerator AttackAnimation()
    {
        // Рисуем плавно гаснущий маркер атаки.
        for (float i = 1f; i > -0.1f; i -= 0.1f)
        {
            markerColor.a = i;
            attackMarker.material.color = markerColor;
            yield return new WaitForSeconds(0.03f);
        }
    }

}
