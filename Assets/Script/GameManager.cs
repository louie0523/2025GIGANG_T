using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public MapControl mapControl;

    private void Start()
    {
        InitGame();
    }

    public void InitGame()
    {
        if(GameObject.Find("GaneData") == null)
        {
            return;
        }

        mapControl = new GameObject("MapControl").AddComponent<MapControl>();
    }
}
