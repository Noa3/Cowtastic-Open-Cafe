using UnityEngine;

[CreateAssetMenu(fileName = "PossibileOrder", menuName = "PossibileOrder")]
public class PossibileOrder : ScriptableObject
{
    public Fillings StartFilling;

    public WeightedFillings[] PossibileExtraFillings;
    public WeightedToppings[] WeightedToppings;
}

[System.Serializable]
public class WeightedFillings
{
    public Fillings Filling;
    public int Weighting;
}

[System.Serializable]
public class WeightedToppings
{
    public Toppings Topping;
    public int Weighting;
}
