using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextChangePage : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI textMeshPro;

    [Header("Etc.")]
    public int totalPages = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void PageNext()
    {

        if (textMeshPro.pageToDisplay < totalPages)
        {
            textMeshPro.pageToDisplay++;
        }
    }

    public void PageBack()
    {

        if (textMeshPro.pageToDisplay > 1)
        {
            textMeshPro.pageToDisplay--;
        }
    }
}
