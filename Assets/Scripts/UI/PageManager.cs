using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PageManager : MonoBehaviour
{
    [SerializeField] List<GameObject> pages = new();
    [SerializeField] TMP_Text pageDisplay;

    Dictionary<string, GameObject> pageDict = new();

    GameObject currentPage;

    private void Start()
    {
        foreach (var page in pages)
        {
            pageDict[page.name] = page;
            page.SetActive(false);
        }
        if (pages.Count > 0) TransitionToPage(pages[0]);
    }
    public void TransitionToNextPage()
    {
        int currentIndex = pages.IndexOf(currentPage);
        int nextPage = currentIndex + 1;
        if (pages.Count  - 1 > nextPage)
        {
            nextPage = 0;
        }
        TransitionToPage(pages[nextPage]);
    }

    public void TransitionToPage(string pageName)
    {
        if (currentPage.name == pageName) { return; }
        if (!pageDict.ContainsKey(pageName)) { return; }
        
       if (currentPage != null) currentPage.SetActive(false);
        currentPage = pageDict[pageName];
        currentPage.SetActive(true);
        if (pageDisplay != null) pageDisplay.text = currentPage.name;
    }

    public void TransitionToPage(GameObject page)
    {
        if (currentPage == page) { return; }
        if (!pageDict.ContainsKey(page.name))
        {
            Debug.LogWarning("Page did not exist in dict, adding it");
            pageDict[page.name] = page;
            pages.Add(page);
        }

        if (currentPage != null) currentPage.SetActive(false);
        currentPage = page; 
        currentPage.SetActive(true);
        if (pageDisplay != null) pageDisplay.text = currentPage.name;
    }
}
