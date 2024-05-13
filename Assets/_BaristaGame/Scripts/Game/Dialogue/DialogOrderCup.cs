using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogOrderCup : MonoBehaviour
{
    //Gameobjects
    public GameObject ParentGameObject;
    public Image ImageChocolate;
    public Image ImageMilk;
    public Image ImageTea;
    public Image ImageCream;
    public Image ImageEspresso;
    public Image ImageSugar;
    public Image ImageCoffee;
    public Image ImageBreastMilk;

    public Image ImageBoba;
    public Image ImageIce;
    public GameObject GOWhipedCream;
    public GameObject GOWhipedChocolateSauce;
    public GameObject GOWhipedCaramelSauce;
    public GameObject GOWhipedSprinkles;
    public GameObject GOUnwhipedChocolateSauce;
    public GameObject GOUnwhipedCaramelSauce;
    public GameObject GOUnwhipedSprinkles;

    [Header("Values")]
    //Fillings
    public float Chocolate;
    public float Milk;
    public float Tea;
    public float Cream;
    public float Espresso;
    public float Sugar;
    public float Coffee;
    public float BreastMilk;
    //Fillings without value
    public bool Boba;
    public bool Ice;

    //Toppings
    public bool WhipedCream = false;
    public bool ChocolateSauce = false;
    public bool CaramelSauce = false;
    public bool Sprinkles = false;

    public void ResetValues()
    {
        Chocolate = 0;
        Milk = 0;
        Tea = 0;
        Cream = 0;
        Espresso = 0;
        Sugar = 0;
        Coffee = 0;
        BreastMilk = 0;
        Boba = false;
        Ice = false;
        WhipedCream = false;
        ChocolateSauce = false;
        CaramelSauce = false;
        Sprinkles = false;
    }

    public void SetValues(List<float> values) //Need to be 13 values
    {
        //Chocolate = values[0] / 100;
        //Milk = values[1] / 100;
        //Tea = values[2] / 100;
        //Cream = values[3] / 100;
        //Espresso = values[4] / 100;
        //Sugar = values[5] / 100;
        //Coffee = values[6] / 100;
        //Boba = ((values[7] / 100) > 0.01f);
        //Ice = ((values[8] / 100) > 0.01f);
        //WhipedCream = ((values[9] / 100) > 0.01f);
        //ChocolateSauce = ((values[10] / 100) > 0.01f);
        //CaramelSauce = ((values[11] / 100) > 0.01f);
        //Sprinkles = ((values[12] / 100) > 0.01f);

        Chocolate = values[0] / 100;
        Milk = values[1] / 100;
        Tea = values[2] / 100;
        Cream = values[3] / 100;
        Espresso = values[4] / 100;
        Sugar = values[5] / 100;
        Coffee = values[6] / 100;
        Boba = ((values[7]) > 0.01f);

        Ice = ((values[8]) >= 0.01f);

        WhipedCream = ((values[9]) >= 0.01f);
        ChocolateSauce = ((values[10]) >= 0.01f);
        CaramelSauce = ((values[11]) >= 0.01f);
        Sprinkles = ((values[12]) >= 0.01f);
        BreastMilk = values[13] / 100;

        UpdateCupGrapics();
    }

    public void UpdateCupGrapics()
    {
        ImageChocolate.gameObject.SetActive(Chocolate >= 0.01f);
        ImageMilk.gameObject.SetActive(Milk >= 0.01f);
        ImageTea.gameObject.SetActive(Tea >= 0.01f);
        ImageCream.gameObject.SetActive(Cream >= 0.01f);
        ImageEspresso.gameObject.SetActive(Espresso >= 0.01f);
        ImageSugar.gameObject.SetActive(Sugar >= 0.01f);
        ImageCoffee.gameObject.SetActive(Coffee >= 0.01f);
        ImageBreastMilk.gameObject.SetActive(BreastMilk >= 0.01f);

        ImageBoba.gameObject.SetActive(Boba);
        ImageIce.gameObject.SetActive(Ice);

        GOWhipedCream.SetActive(WhipedCream);

        if (WhipedCream == true)
        {
            GOWhipedChocolateSauce.SetActive(ChocolateSauce);
            GOWhipedCaramelSauce.SetActive(CaramelSauce);
            GOWhipedSprinkles.SetActive(Sprinkles);
            GOUnwhipedChocolateSauce.SetActive(false);
            GOUnwhipedCaramelSauce.SetActive(false);
            GOUnwhipedSprinkles.SetActive(false);
        }
        else
        {
            GOWhipedChocolateSauce.SetActive(false);
            GOWhipedCaramelSauce.SetActive(false);
            GOWhipedSprinkles.SetActive(false);
            GOUnwhipedChocolateSauce.SetActive(ChocolateSauce);
            GOUnwhipedCaramelSauce.SetActive(CaramelSauce);
            GOUnwhipedSprinkles.SetActive(Sprinkles);
        }


        float additionalFill = 0;

        //Here is to watch on the reverse order to the gameobject, to show correct the cup "Cup Customer Request/ Ingredients Mask / *** "

        //Old
        //ImageBreastMilk.fillAmount = BreastMilk + additionalFill;
        //additionalFill = additionalFill + BreastMilk;
        //ImageChocolate.fillAmount = Chocolate + additionalFill;
        //additionalFill = additionalFill + Chocolate;
        //ImageTea.fillAmount = Tea + additionalFill;
        //additionalFill = additionalFill + Tea;
        //ImageEspresso.fillAmount = Espresso + additionalFill;
        //additionalFill = additionalFill + Espresso;
        //ImageCoffee.fillAmount = Coffee + additionalFill;
        //additionalFill = additionalFill + Coffee;
        //ImageMilk.fillAmount = Milk + additionalFill;
        //additionalFill = additionalFill + Milk;
        //ImageCream.fillAmount = Cream + additionalFill;
        //additionalFill = additionalFill + Cream;
        //ImageSugar.fillAmount = Sugar + additionalFill;
        //additionalFill = additionalFill + Sugar;

        ImageBreastMilk.fillAmount = BreastMilk + additionalFill;
        additionalFill = additionalFill + BreastMilk;
        ImageChocolate.fillAmount = Chocolate + additionalFill;
        additionalFill = additionalFill + Chocolate;
        ImageSugar.fillAmount = Sugar + additionalFill;
        additionalFill = additionalFill + Sugar;
        ImageEspresso.fillAmount = Espresso + additionalFill;
        additionalFill = additionalFill + Espresso;
        ImageMilk.fillAmount = Milk + additionalFill;
        additionalFill = additionalFill + Milk;
        ImageCoffee.fillAmount = Coffee + additionalFill;
        additionalFill = additionalFill + Coffee;
        ImageCream.fillAmount = Cream + additionalFill;
        additionalFill = additionalFill + Cream;
        ImageTea.fillAmount = Tea + additionalFill;
        additionalFill = additionalFill + Tea;

    }
}
