using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string name;

    [TextArea(2, 10)]
    public string[] sentences = new string[] { };

    public Dialogue(string[] s)
    {
        sentences = s;
    }

    public bool GetIsDialougeNullOrEmpty()
    {
        if (sentences == null || sentences.Length == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
