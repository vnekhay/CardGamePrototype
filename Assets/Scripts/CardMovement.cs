// Основная библиотека
using UnityEngine;

using UnityEngine.UI;

// Подключаем для доступа к интерфейсам, которые я наследую в этом классе
using UnityEngine.EventSystems;

// Основной скрипт, с логикой перемещения игровых объектов
public class CardMovement : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    // Для нужных ссылок хранения ссылок
    private Camera _camera;
    public GameObject deck;
    public GameObject valet;
    public Sprite jack;

    // Промежуточная карта, будет временно заменять поднятую карту и показывать возможные позиции для перемещения
    public GameObject tempCard;
    
    // Карта возникающая при попытке положить карту в сброс
    public GameObject dischargeCard;
    
    // Вектор значения отступа от центра карты, понадобится для плавного полета карты
    private Vector3 _offset;
    
    // Переменные для хранения разных игровых позиций, поле public, чтоб достучаться до значения извне
    public Transform Parent, tempCardParent, dischargeCardParent;

    // Собираем нужные ссылки по сцене
    private void Awake()
    {
        _camera = Camera.allCameras[0];
        tempCard = GameObject.Find("TempCard");
        dischargeCard = GameObject.Find("DischargeCard");
        deck = GameObject.Find("Deck");
        valet = GameObject.Find("Valet");
        jack = Resources.Load<Sprite>("" + Random.Range(1, 5));
    }
    
    // Реализуем интерфейсы, заполнив три следующих метода
    
    // Метод вызывается единожды, как только мы берем карту
    public void OnBeginDrag(PointerEventData eventData)
    {
        BoxCollider2D isJack = GetComponent<BoxCollider2D>();
        if (isJack)
        {
            GameObject.Find("Deck").SetActive(false);
            GetComponent<Image>().sprite = jack;
            transform.SetParent(GameObject.Find("PlayingField").transform);
            tempCardParent = transform;
            Destroy(GetComponent<Collider2D>());
        }
        if (!isJack)
            GameObject.Find("Valet").SetActive(false);
        
        // Узнаем расстояние между нажатием и условным центром
        _offset = transform.position - _camera.ScreenToWorldPoint(eventData.position);

        // Запоминаем текущее игровое поле и положение карты в иерархии.
        // Присваиваем такие же значения промежуточной карте
        Parent = tempCardParent = transform.parent;
        tempCard.transform.SetParent(Parent);
        transform.SetParent(Parent.parent);
        
        tempCard.transform.SetSiblingIndex(transform.GetSiblingIndex());
        
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    // Функция вызывается единожды, как только мы бросаем карту
    public void OnEndDrag(PointerEventData eventData)
    {
        // Присваиваем карте ее текущее игровое поле и последний индекс промежуточной
        transform.SetParent(Parent);
        if (Parent.childCount > 6)
            Destroy(this.gameObject);
        transform.SetSiblingIndex(tempCard.transform.GetSiblingIndex());

        // Показываем промежуточной карте и карте сброса их место за пределами игрового экрана
        tempCard.transform.SetParent(GameObject.Find("Canvas").transform);
        tempCard.transform.localPosition = new Vector3(-2000,-2000);
        dischargeCard.transform.SetParent(GameObject.Find("Canvas").transform);
        dischargeCard.transform.localPosition = new Vector3(-2000,-2000);
        deck.SetActive(true);
        valet.SetActive(true);
        tempCard.SetActive(true);
        
        GameObject.Find("Manager").GetComponent<AudioSource>().Play();

        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
    
    // Функция выполняется каждый кадр что карта перемещается
    public void OnDrag(PointerEventData eventData)
    {
        // Узнаем текущие координаты карты
        Vector3 newPos = _camera.ScreenToWorldPoint(eventData.position);
        
        // Присваиваем эти координаты карте вместе с расстоянием от условного центра, который посчитали ранее
        transform.position = newPos + _offset;

        // Присваиваем актуальное игровое поле
        if (tempCard.transform.parent != tempCardParent)
            tempCard.transform.SetParent(tempCardParent);
        if (dischargeCard.transform.parent != dischargeCardParent)
            dischargeCard.transform.SetParent(dischargeCardParent);

        // Узнаем и ставим карту в правильное положение
        findRightIndex();
    }

    // Метод нахождения правильноого положение карты на игровом поле
    private void findRightIndex()
    {
        //Прохожу циклом по каждому дочернему объекту в иерархии, сравнивая его
        //позицию по иксу на игровом поле с соседним объектом, кол-во пройденных итераций соответсвует
        //индексу карты в иерархии дочерних объектов, в случае когда значение по иксу нашей карты на поле меньше
        //соседней, значит она должна быть левее на поле, и в иерархии соответсвенно выше, то есть ее правильный
        //индекс перетаскивающий ее левее будет равен  - количеству пройденных итераций в цикле - 1
        
        int maxIndex = tempCardParent.childCount;

        for (int j = 0; j < tempCardParent.childCount; j++)
        {
            if (transform.position.x < tempCardParent.GetChild(j).position.x)
            {
                maxIndex = j;

                if (tempCard.transform.GetSiblingIndex() < maxIndex)
                    maxIndex--;
                // Выходим с цикла после того как узнали куда перемещать карту
                //Debug.Log("Индекс подобранной карты в иерархии " +
                //          "дочерних объектов текущего игрового поля равен " + maxIndex);
                break ;
            }
        }
        // Присваиваем полученный индекс промежуточной карте, которая в свою очередь присвоит его
        // карте которую мы перемещаем, в методе, который выполняется последним, когда мы отпускаем карту
        
        tempCard.transform.SetSiblingIndex(maxIndex);
    }

    // самоубийство при попадании в сброс
    public void Die()
    {
        Destroy(this.gameObject);
    }
}