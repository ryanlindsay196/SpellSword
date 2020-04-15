using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationScript : MonoBehaviour
{
    public int intelligence = 5;

    private void Update()
    {
        Greet();
    }

    void Greet()
    {
        switch (intelligence)
        {
            case 5:
                print("Why hellow there good sir! Let me teach you about Trigonomotry!");
                break;
            case 4:
                print("Hello and Good Day!");
                break;
            case 3:
                print("Whadaya want?");
                break;
            case 2:
                print("Grog SMASH");
                break;
            case 1:
                print("Fluifda;hdjfklashfdksal");
                break;
            default:
                print("Incorrect intelligence level");
                break;
        }
    }
}
