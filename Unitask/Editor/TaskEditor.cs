using UnityEditor;
using UnityEngine;

public class TaskEditor : EditorWindow
{
    public static TaskEditor OpenTaskEditor(SimpleTask task)
    {
        TaskEditor windowInstance = ScriptableObject.CreateInstance<TaskEditor>();
        windowInstance.Init(task);
        windowInstance.Show();
        return windowInstance;
    }

    SimpleTask currentTask;

    private void OnGUI()
    {
        Color oldColor = GUI.backgroundColor;
        EditorStyles.label.alignment = TextAnchor.LowerLeft;

        EditorGUILayout.BeginVertical();

        currentTask.name = EditorGUILayout.TextField("Name", currentTask.name);
        EditorGUILayout.LabelField("Despcription");
        currentTask.description = EditorGUILayout.TextArea(currentTask.description, GUILayout.Height(100));

        EditorGUILayout.BeginHorizontal();

        for (int i = 0; i < 5; i++)
        {
            GUI.backgroundColor = i == currentTask.priority ? Color.yellow : oldColor;
            if (GUILayout.Button(i.ToString()))
            {
                currentTask.priority = i;
            }
        }
        GUI.backgroundColor = oldColor;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < 3; i++)
        {
            GUI.backgroundColor = i == (int)currentTask.type ? Color.yellow : oldColor;
            if (GUILayout.Button(((SimpleTaskType)i).ToString()))
            {
                currentTask.type = (SimpleTaskType)i;
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    void Init(SimpleTask task)
    {
        currentTask = task;
    }

    void OnLostFocus()
    {
        Close();
    }
}
