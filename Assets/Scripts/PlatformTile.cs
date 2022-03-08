using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-----------References
//Beginners guide to infinite runner in unity: https://www.youtube.com/watch?v=xcmYsc2BY-U

public class PlatformTile : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public GameObject[] obstacles; //to store all variants of obstacles
    [HideInInspector]
    System.Random random = RandomProvider.GetRandomVal(); //get a random number from RandomProvider class


    public void ActivateRandomObstacle() // select which obstacle to use
    {
        DeactivateAllObstacles();

        
        int randomNumber = random.Next(120, 999); // select outcome based on rand int

        randomNumber = (randomNumber * 2) / 4;
        
        int outcome;

        if(randomNumber >= 60 && randomNumber <= 200)
        {
            outcome = 0;//jump
        } else if(randomNumber > 200 && randomNumber <= 350)
        {
            outcome = 1;//duck
        }
        else
        {
            outcome = 2;//both
        }

        obstacles[outcome].SetActive(true); //set selected obstacle to active
    }

    public void DeactivateAllObstacles() // remove any active obstacle
    {
        for (int i = 0; i < obstacles.Length; i++)
        {
            obstacles[i].SetActive(false);
        }
    }
}
