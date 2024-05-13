using UnityEngine;
using UnityEngine.UI;

public class FixOutfitHelper : MonoBehaviour
{
    public Button FixOutfitButton;
    private BaristaController controller;

    private void Awake()
    {
        if (FixOutfitButton == null)
        {
            FixOutfitButton = GetComponent<Button>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = BaristaController.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.AutoFixOutfit == true)
        {
            FixOutfitButton.gameObject.SetActive(false);
        }
        else
        {
            FixOutfitButton.interactable = controller.CanFixApron;
            FixOutfitButton.gameObject.SetActive(true);
        }
    }

    public void FixOutfit()
    {
        Debug.Log("Fix Outfit Button Pressed");
        controller.DoFixOutfit();
    }

}
