using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Rebind))]
public class RebindInspector : Editor
{
    private GUIStyle bold = new GUIStyle();
    private bool create = false;
    private KeyEditInfo keyEditInfo;
    private string actionName = "";

    void OnEnable()
    {
        Rebind.SetupSerializers();
        EditorApplication.modifierKeysChanged += this.Repaint; //Only way of detecting if the "Shift" key has been pressed
        Rebind.defaultKeys.Dictionary = Rebind.defaultBindsSerializer.Read();
        Rebind.keys.Dictionary = Rebind.keyBindsSerializer.Read();
    }

    void OnDisable()
    {
        EditorApplication.modifierKeysChanged -= this.Repaint;
    }

    public override void OnInspectorGUI()
    {
        bold = new GUIStyle(GUI.skin.label);
        bold.fontStyle = FontStyle.Bold;

        if (keyEditInfo.editing == false)
        {
            if (Rebind.defaultKeys.Dictionary.Count > 0)
            {
                //Show collumn labels            
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                GUILayout.Label("Action Name", bold);
                GUILayout.FlexibleSpace();
                GUILayout.Label("Primary", bold, GUILayout.Width(105));
                GUILayout.Label("Secondary", bold, GUILayout.Width(110));
                GUILayout.Space(40);
                GUILayout.EndHorizontal();
            }

            foreach (KeyValuePair<string, List<InputCode>> pair in Rebind.defaultKeys.Dictionary)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(pair.Key);
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(pair.Value[0].ToString(), GUILayout.Width(115)))
                {
                    keyEditInfo = new KeyEditInfo(pair.Key, 0);
                    keyEditInfo.editing = true;
                }
                if (pair.Value.Count > 1)
                {
                    if (GUILayout.Button(pair.Value[1].ToString(), GUILayout.Width(115)))
                    {
                        keyEditInfo = new KeyEditInfo(pair.Key, 1);
                        keyEditInfo.editing = true;
                    }
                }
                else
                {
                    if (GUILayout.Button("Add", GUILayout.Width(115)))
                    {
                        keyEditInfo = new KeyEditInfo(pair.Key, 1);
                        keyEditInfo.editing = true;
                    }
                }

                if (GUILayout.Button("Delete"))
                {
                    deleteDictionaryEntry(pair);
                    break;
                }

                GUILayout.EndHorizontal();

            }
            if (!create)
            {
                if (GUILayout.Button("New"))
                {
                    create = true;
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                actionName = GUILayout.TextField(actionName, GUILayout.Width(150));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Create", GUILayout.Width(137)))
                {
                    createDictionaryEntry();
                }
                if (GUILayout.Button("Cancel", GUILayout.Width(76)))
                {
                    create = false;
                }
                GUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Press any key", bold);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("Reset"))
            {
                resetInputCode();
            }
        }

        if (keyEditInfo.editing)
        {
            keyEditInfo.editing = pollForInput();
        }
    }

    private void resetInputCode()
    {
        List<InputCode> keys = Rebind.defaultKeys.Dictionary[keyEditInfo.actionName];
        if (keys.Count > keyEditInfo.listIndex)
        {
            if (keys[keyEditInfo.listIndex] == 0)
            {
                keys[0] = InputCode.None;
            }
            else
            {
                keys.RemoveAt(keyEditInfo.listIndex);
            }
        }
        Rebind.defaultBindsSerializer.Save(Rebind.defaultKeys.Dictionary);
        Rebind.keyBindsSerializer.Save(Rebind.defaultKeys.Dictionary);
        EditorUtility.SetDirty(target);
        keyEditInfo.editing = false;
    }

    private bool pollForInput()
    {
        InputCode poll = checkInput();
        if (poll != InputCode.None)
        {
            Rebind.BindKeyToAction(keyEditInfo.actionName, poll, keyEditInfo.listIndex);
            Rebind.defaultBindsSerializer.Save(Rebind.defaultKeys.Dictionary);
            Rebind.keyBindsSerializer.Save(Rebind.defaultKeys.Dictionary);
            EditorUtility.SetDirty(target);
            return false;
        }
        return true;
    }

    private void createDictionaryEntry()
    {
        actionName = actionName.Replace(" ", string.Empty);
        Rebind.CreateEntry(actionName);
        actionName = "";
        create = false;
        Rebind.defaultBindsSerializer.Save(Rebind.defaultKeys.Dictionary);
        Rebind.keyBindsSerializer.Save(Rebind.defaultKeys.Dictionary);
    }

    private void deleteDictionaryEntry(KeyValuePair<string, List<InputCode>> pair)
    {
        Rebind.RemoveEntry(pair.Key);
        Rebind.defaultBindsSerializer.Save(Rebind.defaultKeys.Dictionary);
        Rebind.keyBindsSerializer.Save(Rebind.defaultKeys.Dictionary);
        EditorUtility.SetDirty(target);
    }

    private InputCode checkInput()
    {
        if (Event.current.shift)
        {
            return InputCode.LeftShift;
        }

        else if (Event.current.isKey)
        {
            return (InputCode)((int)Event.current.keyCode);
        }
        else if (Event.current.type == EventType.ScrollWheel)
        {
            if (Event.current.delta.y > 0)
            {
                return InputCode.MouseScrollDown;
            }
            else
            {
                return InputCode.MouseScrollUp;
            }
        }

        else if (Event.current.isMouse)
        {
            return (InputCode)323 + Event.current.button;
        }

        return InputCode.None;
    }
}

public struct KeyEditInfo
{
    public bool editing;
    public string actionName;
    public int listIndex;

    public KeyEditInfo(string ActionName, int ListIndex)
    {
        editing = false;
        actionName = ActionName;
        listIndex = ListIndex;
    }
}
