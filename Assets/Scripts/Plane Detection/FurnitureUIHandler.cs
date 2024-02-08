using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureUIHandler : MonoBehaviour
{
    [SerializeField] private FurniturePlacementManager furniturePlacementManager;
    [SerializeField] private List<Button> buttonList;

    public void SetSelectedFurniture(int idx)
    {
        furniturePlacementManager.SetSelectedFurniture(idx);
        for (int i = 0; i < buttonList.Count; i++)
        {
            buttonList[i].interactable = true;
        }

        buttonList[idx].interactable = false;
    }
}
