using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottleController : MonoBehaviour {

    public Image[] bottleImages;

    public Sprite fullBottle;
    public Sprite emptyBottle;

    public void setBottles(int bottles) {
        for(int i = 0; i < 4; i++) {
            if(i < bottles) {
                bottleImages[i].sprite = fullBottle;
            }
            else {
                bottleImages[i].sprite = emptyBottle;
            }

        }
    }
}
