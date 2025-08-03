using System.Collections;
using TMPro;
using UnityEngine;

public class LetterTyper : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro; // Assign in Inspector
    public float delayBetweenLetters = 0.05f;

    [TextArea]
    public string fullText = " He is determind"; // Your desired text

    private void Start()
    {
        StartCoroutine(TypeLetterByLetter());
    }

    IEnumerator TypeLetterByLetter()
    {
        textMeshPro.text = "";

        foreach (char c in fullText)
        {
            textMeshPro.text += c;
            yield return new WaitForSeconds(delayBetweenLetters);
        }
    }
}
