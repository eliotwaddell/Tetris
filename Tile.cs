using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // Start is called before the first frame update
    // Update is called once per frame
    public void On()
    {
        GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 255f);;
    }

    public void Off()
    {
        GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0f);;
    }
}
