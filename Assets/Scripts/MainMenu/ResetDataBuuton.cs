using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetDataButton : InteractiveUI
{
    protected override void HandleClick()
    {
        SaveAPI.ClearAll();
        Debug.Log("level0");
    }
}
