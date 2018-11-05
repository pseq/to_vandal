using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BarrackScript : MonoBehaviour {

    public float respawnTimeDelta;
    public int maxLevel;
    public int levelImprover;
    public int countToUpgrade;
    public int unitCost;
    public GameObject unit;
    public Material unitMaterial;
    public GameObject gameControl;
    private int counter;
    private int level;


    // Use this for initialization
    void Start()
    {
        // Устанавливаем начальные значения уровня и счетчика произведенных юнитов
        level = 1;
        counter = 0;
        // Начинаем генерацию юнитов.
        StartCoroutine(UnitGenerationCycle());
    }

    // Цикл генерации юнитов
    IEnumerator UnitGenerationCycle()
    {
        while (true)
        {

            // Если хватает золота
            if (gameControl.GetComponent<GameControl>().GetGoldReserve() >= unitCost)
            {

                // Берем за юнита золото.
                gameControl.GetComponent<GameControl>().GoldDecrease(unitCost);
                yield return StartCoroutine(MinionGenerate());
            }
            // Выжидаем и повторяем.
            else yield return new WaitForSeconds(.1f);
        }
    }

    // Генерация юнита
    IEnumerator MinionGenerate()
    {
        float delta = respawnTimeDelta / 100;
        float status = respawnTimeDelta;
        // Управление прогресс-баром казармы
        while (status > delta)
        {
            gameObject.GetComponent<HPbarScript>().HPchange((status - delta) / status);
            status -= delta;
            yield return new WaitForSeconds(delta);
        }
        // Создаем экземпляр нового юнита и помещаем его за казарму.
        GameObject newBornMinion = Instantiate(unit);
        newBornMinion.transform.position = new Vector3(transform.position.x, 0, transform.position.z - 10);
        // Добавляем юнита в массив миньонов и увеличиваем счетчик
        gameControl.GetComponent<GameControl>().AddMinion(newBornMinion);
        counter++;
        // Применяем к юниту материал.
        newBornMinion.GetComponent<MeshRenderer>().material = unitMaterial;
        // Возвращаем прогресс-бар в исходное.
        gameObject.GetComponent<HPbarScript>().HPchange(true);
        // Если количество произведенных юнитов больше заданного - апгрейдим казарму.
        if (counter >= countToUpgrade) BarrackUpgrade();
    }

    // Повышение уровня казармы
    private void BarrackUpgrade()
    {
        if (level < maxLevel)
        {
            level++;
            // В зависимости от уровня казармы меняется время и цена генерации юнита.
            respawnTimeDelta = respawnTimeDelta / (level * levelImprover);
            unitCost = unitCost / (level * levelImprover);

            StartCoroutine(UpgradeAnimation());
        }
    }

    IEnumerator UpgradeAnimation()
    {
        // Рисуем плавный подъем казармы.
        for (float i = 0; i < 1; i += .05f)
        {
            transform.Translate(Vector3.up * .15f);
            yield return new WaitForSeconds(.05f);
        }
    }
}
