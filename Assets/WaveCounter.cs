using TMPro;
using UnityEngine;

public class WaveCounter : MonoBehaviour
{
    // [SerializeField] Animator animator;
    // [SerializeField] TMP_Text waveNumberTxt;
    [SerializeField] GameObject[] numbers;

    public void OnStartWave(int waveNumber)
    {
        // waveNumberTxt.text = waveNumber.ToString();
        for (int i = 0; i < numbers.Length; i++)
        {
            numbers[i].SetActive(i == waveNumber);
        };
        Invoke(nameof(Disable), 4);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
