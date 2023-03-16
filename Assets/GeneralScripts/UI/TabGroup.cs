// Based On tutorial by Game Dev Guide https://www.youtube.com/watch?v=211t6r12XPQ&t=64s
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private TabSelectorButton selectedTab;

    [Header("Tab States")]
    [SerializeField] private Color tabIdleColor;
    [SerializeField] private Color tabHoverColor;
    [SerializeField] private Color tabActiveColor;

    private List<TabSelectorButton> tabButtons;

    private void Start()
    {
        if (selectedTab != null)
        {
            OnTabSelected(selectedTab);
        }
    }

    public void Subscribe(TabSelectorButton tabButton)
    {
        tabButtons ??= new List<TabSelectorButton>();

        tabButtons.Add(tabButton);
    }

    public void OnTabEnter(TabSelectorButton tabButton)
    {
        ResetTabs();

        if (tabButton == selectedTab) { return; }

        tabButton.background.color = tabHoverColor;
    }

    public void OnTabExit()
    {
        ResetTabs();
    }

    public void OnTabSelected(TabSelectorButton tabButton)
    {
        if (selectedTab != null)
        {
            selectedTab.Deselect();
        }

        selectedTab = tabButton;

        selectedTab.Select();

        ResetTabs();
        tabButton.background.color = tabActiveColor;
    }

    public void ResetTabs()
    {
        if (tabButtons == null) { return; }

        foreach (TabSelectorButton tabButton in tabButtons)
        {
            if (tabButton == selectedTab) { continue; }
            tabButton.background.color = tabHoverColor;
        }
    }
}

