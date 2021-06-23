using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject TilePrefeb; // obiekt umieszczony na scenie

    public Sprite[] sprites; // tablica na tekstury dla run;

    Tile[] Tiles; //tabica do przechowywania run

    public Vector2 TilesOffset = Vector2.one; // wektor odpowiedzialny za rozmieszczenie kafelków względem siebie

    public bool CanMove = false; // zmienna określająca czy w danym momencien gracz może wykonywać ruch czy nie

    public int Width = 6;
    public int Height = 4;

    public TextMesh WinText; // tekst wygranej, na początku dobrze jest go ukryć

    IEnumerator Start() 
    {
        WinText.GetComponent<Renderer>().enabled = false; // odnosimy się do komponentu renderującego i ustawiamy enable na false
                                                          // czyli go ukrywamy
        CreateTiles(Width, Height);
        ShuffleTiles();
        PlaceTiles();
        yield return new WaitForSeconds(2f); // opóżnienie czasu na 2 sekundy
        HideTiles();
        CanMove = true; // odblokowanie możliwości ruchu na kafelku
    }
        
    void CreateTiles(int x, int y) // generowanie planszy
    {
        var lenght = x * y; // przygotowanie długości tablicy
        Tiles = new Tile[lenght]; // prrzypisanie długości do tablicy

        for (int i = 0; i < lenght; i++)
        {
            var sprite = sprites[i / 2]; // zmienna tymczasowa, która będzie przechowywała teksturę dla naszego obiektu z wnętrza tablicy Sprite
            Tiles[i] = CreateTile(sprite); // dodawanie nowo utwrzonego obiektu do tablicy
        }    
    }

    Tile CreateTile(Sprite faceSprite) // funkcja odpowiedzialna do tworzenia pojedynczego kafelka, przyjmuje sprite,kóry będzie przypisywany do runy
    {
        var gameobject = Instantiate(TilePrefeb); // funkcja ta pobiera obiekt, który ma zostać umieszczony na scenie
        gameobject.transform.parent = transform;// przypisujemy obiektowi game object nowego rodzica zmienna parent potrzebuje
                                                // komponentu transform, nas interesuje transform planczy czyli po prostu transform
        var tile = gameobject.GetComponent<Tile>();  // pobieranie komponentu, w nawiasach podajemy jego typ,
        tile.Uncovered = false;// odwracanie kafelka wzorem do góry
        tile.frontFace = faceSprite; // na tym komponencie zmieniamy obraz używając metody generującej randomowy sprite
        return tile; // funkcja ta teraz zwraca runę
    }

    void PlaceTiles()// funkcja odpowiedzialna za rozmieszczenie poszczególnych elementów na planszy
    {
        for (int i = 0; i < Width * Height; i++)
        {
            int x = i % Width; //obliczanie położenia kafelka
            int y = i / Width; //obliczanie położenia kafelka
            Tiles[i].transform.localPosition = new Vector3(x * TilesOffset.x, y * TilesOffset.y, 0); // przesuwanie każdego stworzonego kafelka
        }
    }
    void ShuffleTiles()//funkcja mieszająca kafelki na planszy
    {
        for (int i = 0; i < 1000; i++) // mieszanie dwóch kafelków tyle razy dla lepszego efektu
        {
            int indexA = Random.Range(0, Tiles.Length); //indeks pierwszego zamienianego kafelka
            int indexB = Random.Range(0, Tiles.Length); //indeks drugiego zamienianego kafelka

            var tileA = Tiles[indexA]; // pobieranie kafelka o wylosowanym indeksie
            var tileB = Tiles[indexB]; // pobieranie kafelka o wylosowanym indeksie

            Tiles[indexA] = tileB; // podmiana wartości kafelka 
            Tiles[indexB] = tileA; // podmiana wartości kafelka 
        }
    }

    void HideTiles() // metoda zasłaniająca wszystkie kafelki
    {
        for (int i = 0; i < Tiles.Length; i++) // iteracja po tablicy z kafelkami
        {
            Tiles[i].Uncovered = true; // odwrócenie kafelka wzorem do dołu 
        }
    }

    bool CheckIfEnd() // funkcja speawdzająca czy gra dobiegła końca
    {
        return Tiles.All(tile => !tile.Activate); // sprawdzanie czy wszystkie kafelki na tablicy są nieaktywne
    }

    public void CheckPair() // funkcja podpięta do kliknięcia myszką uruchamiająca nowego wątku
    {
        StartCoroutine(CheckPairCoroutine());// funkcja StartCoroutine uruchamia nowy wątek
    }

    IEnumerator CheckPairCoroutine() // funkcja sprawdzająca czy para odkrytych kafelków ma takie same runy
    {
        var tilesUncovered = Tiles.Where(tile => tile.Activate).Where(tile => !tile.Uncovered).ToArray();
        // szukanie wszystkich odkrytych i aktywnych kafelków oraz przekształcenie ich na Tblicę
        if (tilesUncovered.Length != 2) // sprawdzenie ile kafelków jest aktualnie odkrytych
        {
            yield break;
        }
        var tileA = tilesUncovered[0]; // przypisanie kafelka
        var tileB = tilesUncovered[1]; // przypisanie kafelka

        CanMove = false; // zatrzymanie możliwości ruchu
        yield return new WaitForSeconds(2f); // opóżnienie czasu na 2 sekundy
        CanMove = true; // uwolnienie możliwości ruchu

        if (tilesUncovered.Length == 2)
        {
            if (tileA.frontFace == tileB.frontFace) //sprawdzenie czy para kafelków jest identyczna
            {
                tileA.Activate = false; // usuwanie identycznych kafelków
                tileB.Activate = false; // usuwanie identycznych kafelków
            }
            else // odwracanie różnych kafelków
            {
                tileA.Uncovered = true;
                tileB.Uncovered = true;
            }
        }
        if (CheckIfEnd()) // sprawdzeniie czy gra się skończyła czyli czy wszystkie kafelki są nieaktywne
        {
            CanMove = true; // wyłączenie możliwości ruchu
            Debug.Log("Koniec gry!"); // tekst do konsoli
            WinText.GetComponent<Renderer>().enabled = true; // wyświetlanie tekstu o wygranej
            yield return new WaitForSeconds(2f); // opóżnienie czasu na 2 sekundy
            Application.Quit(); // zamknięcie gry
        }
    }

}
