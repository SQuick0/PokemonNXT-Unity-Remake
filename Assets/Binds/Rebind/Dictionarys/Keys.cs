using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Keys
{
    public Dictionary<string, List<InputCode>> Dictionary = new Dictionary<string, List<InputCode>>();  

    public void ResetAllKeysToDefaults()
    {
        Dictionary = Rebind.defaultKeys.Dictionary;
    }

    public void ResetKeyToDefault(string actionName, InputCode keyToReset)
    {
        for (int i = 0; i < Dictionary[actionName].Count; i++)
        {
            if (Dictionary[actionName][i] == keyToReset)
            {
                Dictionary[actionName][i] = Rebind.defaultKeys.Dictionary[actionName][i];
            }
        }
    }


    public void ResetKeysToDefault(string actionName)
    {
        Dictionary[actionName] = Rebind.defaultKeys.Dictionary[actionName];
    }
}
