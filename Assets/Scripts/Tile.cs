using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool  Uncovered = true; //zmienna odpowiedzialna za zakrywanie kafelka
    public bool  Activate = true; //zmienna odpowiedzialna za usuwanie kafelka
    public Sprite frontFace; //zmienna przechowująca teksturę kafelka

    
    void Start()
    {
        transform.rotation = GetTargetRotation(); // przypisujmey na poczatku gry nasza funkcje do aktualnej rotacji
        var frontObject = transform.FindChild("front"); // funkcja ta pobiera nazwę dziecka jakie chcemy odszukać
        var spriteRenderer = frontObject.transform.GetComponent<SpriteRenderer>(); // szukamy typ komponentu jaki nas interesuje, podajemy go w nawisach 
        spriteRenderer.sprite = frontFace; // nadanie obiektowi odpowiedniej tekstury
    }

    void Update()
    {
        var targetRotation = GetTargetRotation(); //pobieramy aktualna rotacje jaka powinien przyjac nasz kafelek
        transform.rotation = Quaternion.Lerp( //nastepnie przypisujemy nasza rotacje tak jak w funkcji start,
                                              //aby animacja byla plynna uzywamy funkcji lerp, która posiada 3 argumenty
            transform.rotation, // 1kwaterion poczatkowy czyli rotacja obiektu
            targetRotation, // 2 kwaterion docelowy
            Time.deltaTime * 5f); // czas zmiany obiektu
        if (Activate == false) // usuwanie nieaktywnego kafelka
        {
            Destroy(gameObject); // usuwanie obiektu z przestrzeni trójwymiarowej nie z tablicy
        }

    }
    Quaternion GetTargetRotation() //metoda zamieniajaca wektor na kwaterion
    {
        var rotation = Uncovered ? Vector3.zero : (Vector3.up * 180f);
        return Quaternion.Euler(rotation);
    }

    public void OnMouseDown() //funkcja odpowiedzialna za obrót kafelka po jego kliknięciu myszą
    {
        var board = FindObjectOfType<Board>(); // wyciągnięcie obiektu tablicy
        if (board.CanMove == false)// szukanie obiektu planszy i sprawdzenie możliwości ruchu
        {
            return; // jeśli CanMove == false to kafelka nie można odwracać
        }
        Uncovered = !Uncovered;
        board.CheckPair();
    }
}