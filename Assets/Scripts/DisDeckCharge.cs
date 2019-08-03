// Основная библиотека
using UnityEngine;

// Подключаем библиотеку и наследуем интерфейсы
using UnityEngine.EventSystems;

// Скрипт с логикой уничтожения карт при попадании в сброс или колоду
public class DisDeckCharge : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    // Описывая функцию OnDrop мы реализует наследуемый IDropHandler интерфейс,
    // функция срабатывает в момент после того как мы "опустили" карту на стол
    
    public void OnDrop(PointerEventData eventData)
    {
        // Узнаем игровое поле, на которое упала карта
        // и присваиваем его упавшей карте как стандартное
        
        CardMovement card = eventData.pointerDrag.GetComponent<CardMovement>();

        if (card)
        {
            card.valet.SetActive(true);
            card.tempCard.transform.SetParent(GameObject.Find("Canvas").transform);
            card.tempCard.transform.localPosition = new Vector3(-2000,-2000);
            card.dischargeCard.transform.SetParent(GameObject.Find("Canvas").transform);
            card.dischargeCard.transform.localPosition = new Vector3(-2000,-2000);
            GameObject.Find("Manager").GetComponent<AudioSource>().Play();
            card.Die();
        }
    }
    
    // Реализовываем наследуемые IPointerEnterHandler, IPointerExitHandler
    // описывая следующие две функции соответсвенно, они реагируют на начало
    // нашего контакта с картой и на момент когда этот контакт теряем
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Если мы не перемещаем карту, выходим ничего не делая
        if (!eventData.pointerDrag)
            return ;
        
        // Даем знать промежуточной карте ее текущее игровом поле
        CardMovement card = eventData.pointerDrag.GetComponent<CardMovement>();
        
        if (card)
            card.dischargeCardParent = this.transform;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!eventData.pointerDrag)
            return ;
        
        CardMovement card = eventData.pointerDrag.GetComponent<CardMovement>();
        
        // Дополнительное условие, которое возвращает карту на свое старое место,
        // в случае если мы ее опустим не в игровом поле

        if (card)
        {
            card.dischargeCardParent = GameObject.Find("Canvas").transform;
            card.dischargeCardParent.localPosition = new Vector3(-2000, -2000);
            card.dischargeCard.transform.SetParent(GameObject.Find("Canvas").transform);
            card.dischargeCard.transform.localPosition = new Vector3(-2000,-2000);
        }
    }
}