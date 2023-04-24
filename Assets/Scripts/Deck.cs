using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Random = UnityEngine.Random;
using Microsoft.Win32.SafeHandles;

public class Deck : MonoBehaviour
{
    
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text playerBlackjack;
    public Text dealerBlackjack;
    public Text probMessage;
    public Text playerPuntuacion;
    public Text dealerPuntuacion;
    public Button apostarButton;
    public Slider apostarSlider;
    public Text banca;

    public Text probPasarse;
    public Text probBuena;
    public Text probDealerMasQuePlayer;

    public Sprite[] faces;
    public int[] values = new int[52];

    public Sprite[] shuffledFaces;
    public int[] shuffledValues = new int[52];

    int dineroBanca = 1000;
    int dineroApostado;

    int cardIndex = 0;    
       
    private void Awake()
    {    
        InitCardValues();        

    }

    private void Start()
    {
        PlayAgain();
    }

    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */
        int indice = 0;
        for (int i = 0; i < 4; i++) {
            for (int i2 = 0; i2 < 13; i2++)
            {
                if (i2 == 0)
                {
                    values[indice] = 11;
                }
                else if (i2 == 10 || i2 == 11 || i2 == 12)
                {
                    values[indice] = 10;
                }
                else
                {
                    values[indice] = i2+1;
                }
                indice++;
            }
        }
    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */

         
        int initialSeed = Random.Range(0, 100000);

        Random.InitState(initialSeed);

        values = values.OrderBy(x => Random.Range(0,52)).ToArray();

        Random.InitState(initialSeed);

        faces = faces.OrderBy(x => Random.Range(0,52)).ToArray();




    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
        }
        if (player.GetComponent<CardHand>().points == 21)
        {
            finalMessage.text = "Has ganado!";
            playerBlackjack.text = "BLACKJACK";
            hitButton.interactable = false;
            stickButton.interactable = false;
            dineroBanca += dineroApostado;
        }
        else if (dealer.GetComponent<CardHand>().points == 21)
        {
            finalMessage.text = "Has perdido";
            dealerBlackjack.text = "BLACKJACK";
            dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
            hitButton.interactable = false;
            stickButton.interactable = false;
            dineroBanca -= dineroApostado;
           
        }
        else
        {
            playerPuntuacion.text = player.GetComponent<CardHand>().points.ToString();
            dealerPuntuacion.text = dealer.GetComponent<CardHand>().cards[1].GetComponent<CardModel>().value.ToString();

        }


        CalculateProbabilities();

    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */


        




        //Probabilidad de que el jugador obtenga más de 21 si pide una carta
        int nuestraPuntuacion = player.GetComponent<CardHand>().points;
        int dealerPuntuacion = dealer.GetComponent<CardHand>().cards[1].GetComponent<CardModel>().value;
        float cantidadDeCartasConLaQueNosPasamos = 0;
        float cantidadDeCartasBuenas = 0;
        float cantidadDeCartasQueHacenGanarAlDealer = 0;
        int cartasEnMesa = 2 + player.GetComponent<CardHand>().cards.Count;
        int cartasRestantes = 52 - cartasEnMesa;
        for (int i = cartasEnMesa; i<52;i++)
        {
            if (values[i] + nuestraPuntuacion > 21)
            {
                if (values[i] != 11)
                {
                    cantidadDeCartasConLaQueNosPasamos++;
                }
                else if (values[i] + nuestraPuntuacion == 21)
                {
                    cantidadDeCartasConLaQueNosPasamos++;
                }
            }
            if (values[i] + nuestraPuntuacion >= 17 && values[i] + nuestraPuntuacion <= 21)
            {
                cantidadDeCartasBuenas++;
            }
            else if (values[i] == 11)
            {
                cantidadDeCartasBuenas++;
            }

            if (values[i] + dealerPuntuacion > nuestraPuntuacion && values[i] + dealerPuntuacion<22)
            {
                cantidadDeCartasQueHacenGanarAlDealer++;
            }
        }

        if (values[1] + nuestraPuntuacion > 21)
        {
            if (values[1] != 11)
            {
                cantidadDeCartasConLaQueNosPasamos++;
            }
            else if (values[1] + nuestraPuntuacion == 21)
            {
                cantidadDeCartasConLaQueNosPasamos++;
            }
        }
        if (values[1] + nuestraPuntuacion >= 17 && values[1] + nuestraPuntuacion <= 21)
        {
            cantidadDeCartasBuenas++;
        }
        if (values[1] + dealerPuntuacion > nuestraPuntuacion && values[1] + dealerPuntuacion < 22)
        {
            cantidadDeCartasQueHacenGanarAlDealer++;
        }

        float probabilidadPasarse = cantidadDeCartasConLaQueNosPasamos / (cartasRestantes+1);
        float probabilidadCartaBuena = cantidadDeCartasBuenas / (cartasRestantes+1);
        float probabilidadDealerMasQuePlayer = cantidadDeCartasQueHacenGanarAlDealer/ (cartasRestantes + 1);
        probPasarse.text = probabilidadPasarse.ToString();
        probBuena.text = probabilidadCartaBuena.ToString();
        probDealerMasQuePlayer.text = probabilidadDealerMasQuePlayer.ToString();
        Debug.Log(dealerPuntuacion);
    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardIndex],values[cardIndex]);
        cardIndex++;        
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        
    }       

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        //NO dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);

        //Repartimos carta al jugador
        PushPlayer();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */
        if (player.GetComponent<CardHand>().points > 21)
        {
            finalMessage.text = "Has perdido";
            hitButton.interactable = false;
            stickButton.interactable = false;
            dineroBanca -= dineroApostado;
        }
        if (player.GetComponent<CardHand>().points == 21)
        {
            finalMessage.text = "Has ganado!";
            playerBlackjack.text = "BLACKJACK";
            hitButton.interactable = false;
            stickButton.interactable = false;
            dineroBanca += dineroApostado;
        }

        playerPuntuacion.text = player.GetComponent<CardHand>().points.ToString();
        CalculateProbabilities();
    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */

        while (dealer.GetComponent<CardHand>().points <= 16)
        {
            PushDealer();
        }
        if (dealer.GetComponent<CardHand>().points > 21)
        {
            finalMessage.text = "Has ganado";
            hitButton.interactable = false;
            stickButton.interactable = false;
            dineroBanca += dineroApostado;
        }
        else if (dealer.GetComponent<CardHand>().points == 21)
        {
            dealerBlackjack.text = "BLACKJACK";
            finalMessage.text = "Has perdido";
            hitButton.interactable = false;
            stickButton.interactable = false;
            dineroBanca -= dineroApostado;
        }
        else if (dealer.GetComponent<CardHand>().points > player.GetComponent<CardHand>().points)
        {
            finalMessage.text = "Has perdido";
            hitButton.interactable = false;
            stickButton.interactable = false;
            dineroBanca -= dineroApostado;
        }
        else if (dealer.GetComponent<CardHand>().points == player.GetComponent<CardHand>().points)
        {
            finalMessage.text = "Empate";
            hitButton.interactable = false;
            stickButton.interactable = false;
            
        }
        else
        {
            finalMessage.text = "Has ganado";
            hitButton.interactable = false;
            stickButton.interactable = false;
            dineroBanca += dineroApostado;
        }
        dealerPuntuacion.text = dealer.GetComponent<CardHand>().points.ToString();


    }

    public void Apostar()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        playAgainButton.interactable = true;
        apostarButton.interactable = false;
        finalMessage.text = "";
        dineroApostado = Int16.Parse(apostarSlider.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().text);
        StartGame();
    }

    public void PlayAgain()
    {
        hitButton.interactable = false;
        stickButton.interactable = false;
        playAgainButton.interactable = false;
        apostarButton.interactable = true;
        finalMessage.text = "HAZ TU APUESTA";
        playerBlackjack.text = "";
        dealerBlackjack.text = "";
        playerPuntuacion.text = "";
        dealerPuntuacion.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        apostarSlider.maxValue = dineroBanca/10;
        apostarSlider.value = 1;
        ShuffleCards();

        if (dineroBanca == 0)
        {
            finalMessage.text = "TE HAS QUEDADO SIN FICHAS";
            apostarButton.interactable = false;
        }



    }

    private void Update()
    {
        if (apostarButton.interactable)
        {
            apostarSlider.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>().text = (apostarSlider.value * 10).ToString();
            banca.text = (dineroBanca - (apostarSlider.value * 10)).ToString();
        }
    }

}
