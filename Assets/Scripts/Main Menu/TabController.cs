using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Image[] tabImages;
    public GameObject[] pages;
    void Start()
    {
        ActivateTab(2);
    }

    // Update is called once per frame
    public void ActivateTab(int tabNumber)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            tabImages[i].color = Color.gray;
        }
        tabImages[tabNumber].color = Color.white;
        pages[tabNumber].SetActive(true);
        
    }
}
