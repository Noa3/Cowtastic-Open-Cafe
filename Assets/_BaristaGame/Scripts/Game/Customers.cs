using UnityEngine;

[CreateAssetMenu(fileName = "Customer", menuName = "Customer")]
public class Customers : ScriptableObject
{
    public CustomerAvatar Avatar;

    public Dialogue DialogeTextForOrder;


    public Fillings[] OrderFillings = new Fillings[] { Fillings.Coffee };
    public Toppings[] Toppings = new Toppings[] { };

    [Tooltip("Optional")]
    public Dialogue DialogeTextForOrderSuccess;

    [Tooltip("Optional")]
    public Dialogue DialogeTextForOrderFail;

}
