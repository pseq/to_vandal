using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortalScript : MonoBehaviour {

    private float hp;
    public float maxhp;
    public float healHPinSec;
    public int priseForHead;
    public float armor;
    public GameObject grave;
    private GameObject gameControl;
    private bool isMinion;


    // Use this for initialization
    void Start () {
        // Получаем объект с общими параметрами игры.
        gameControl = GameObject.FindGameObjectWithTag("GameController");
        // На какой стороне юнит?
        isMinion = gameControl.GetComponent<GameControl>().GetMinions().Contains(gameObject);

        hp = maxhp;
    }

    public void Hit(float damage)
    {
        // При получении удара уменьшаем здоровье, с учетом брони.
        damage *= 1 - armor;

        // Уменьшаем индикатор и здоровье.
        gameObject.GetComponent<HPbarScript>().HPchange((hp - damage) / hp);
        hp -= damage;

        // Если здоровье на нуле - умираем.
        if (hp <= 0) Death();
    }

    public void Death()
    {
        // Сообщаем общему скрипту, что нужно удалить юнита из всех списков.
        gameControl.GetComponent<GameControl>().DeleteUnit(gameObject);
        
        //Если уничтожен диван - выводим геймовер
        if (gameObject.name == "DeveloperSofa") gameControl.GetComponent<GameControl>().GameOver();

        // Делаем проверку на победу
        // Проверяем количество живых противников
        if (gameControl.GetComponent<GameControl>().GetEnemies().Count < 1)
        {
            bool respawnInProgress = false;
            // Получаем список точек респауна врагов и проверяем, что с ними происходит
            EnemyRespawner[] respawners = (EnemyRespawner[]) FindObjectsOfType(typeof(EnemyRespawner));
            foreach(EnemyRespawner resp in respawners)
            {
                respawnInProgress = respawnInProgress || resp.AnybodyElse(); //resp.GetComponent<EnemyRespawner>().AnybodyElse();
            }
            // Если больше не планируется волн врагов и не запущена генерация - победа
            if (!respawnInProgress) gameControl.GetComponent<GameControl>().Win();
        }

        //Если убит враг + золото
        if (!isMinion) gameControl.GetComponent<GameControl>().GoldIncrease(priseForHead);

        // Рисуем могилку или руины.
        Instantiate(grave, transform.position, Quaternion.identity);
        // Удаляем юнита.
        Destroy(gameObject);
    }

    // Если юнит зашел в фонтан - начинаем его лечить
    private void OnTriggerEnter(Collider other)
    {
        if ((other.name == "Fountain") && isMinion) StartCoroutine(Healer());
    }

    // Останавливаем лечение при выходе из фонтана
    private void OnTriggerExit(Collider other)
    {
        StopCoroutine("Healer");
    }

    public float GetHP ()
    {
        return hp;
    }

    // Выполняем лечение раз в секунду
    IEnumerator Healer()
    {
        while (hp < maxhp)
        {
            // Увеличиваем индикатор хп
            gameObject.GetComponent<HPbarScript>().HPchange((hp + healHPinSec) / hp);
            // Увеличиваем хп
            hp += healHPinSec;
            // Выжидаем и повторяем.
            yield return new WaitForSeconds(1);
        }
       
    }
}
