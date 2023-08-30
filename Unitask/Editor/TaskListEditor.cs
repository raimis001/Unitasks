using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TaskListEditor : EditorWindow
{

    [MenuItem("Assets/Create/Task manager")]
    public static void CreateTasksAsset()
    {
        TaskListManager asset = ScriptableObject.CreateInstance<TaskListManager>();
        AssetDatabase.CreateAsset(asset, "Assets/NewTasks.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    [MenuItem("Tasks/Task manager %#t")]
    static void Init()
    {
        GetWindow(typeof(TaskListEditor));
    }

    TaskListManager manager;

    void OnEnable()
    {
        manager = null;

        if (Selection.activeObject != null && Selection.activeObject is TaskListManager)
        {
            manager = Selection.activeObject as TaskListManager;
        }

        if (!manager)
        {
            TaskListManager[] gui = Resources.FindObjectsOfTypeAll<TaskListManager>();
            if (gui.Length > 0)
                manager = gui[0];
        }

    }
    const int columnWidth = 300;

    Color oldColor;
    Color selectColor = Color.grey;
    Color[] priorityColors = new Color[] { Color.red, Color.blue, Color.green, Color.yellow, Color.gray };
    Color[] typeColors = new Color[] { Color.red, Color.blue, Color.yellow };
    int currentFilter = -1;
    string searchString = "";
    void OnGUI()
    {

        if (!manager)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Label("No manager selected");
            if (GUILayout.Button("Create task asset", GUILayout.Width(200)))
            {
                CreateTasksAsset();
            }
            EditorGUILayout.EndVertical();
            if (Selection.activeObject != null && Selection.activeObject is TaskListManager)
            {
                manager = Selection.activeObject as TaskListManager;
            }
            return;
        }

        EditorStyles.textField.wordWrap = true;
        EditorStyles.textArea.wordWrap = true;
        EditorStyles.label.wordWrap = true;

        oldColor = GUI.backgroundColor;

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        //filters
        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = currentFilter == -1 ? Color.yellow : oldColor;
        if (GUILayout.Button("All", GUILayout.Width(70))) { currentFilter = -1; }
        GUI.backgroundColor = currentFilter == 0 ? Color.yellow : oldColor;
        if (GUILayout.Button("Bugs", GUILayout.Width(70))) { currentFilter = 0; }
        GUI.backgroundColor = currentFilter == 1 ? Color.yellow : oldColor;
        if (GUILayout.Button("Tasks", GUILayout.Width(70))) { currentFilter = 1; }
        GUI.backgroundColor = currentFilter == 2 ? Color.yellow : oldColor;
        if (GUILayout.Button("Wishes", GUILayout.Width(70))) { currentFilter = 2; }
        GUI.backgroundColor = oldColor;

        EditorGUILayout.LabelField("", GUI.skin.verticalSlider, GUILayout.Width(10));
        searchString = EditorGUILayout.TextField("Search", searchString, GUILayout.Width(300));
        EditorGUILayout.LabelField("", GUI.skin.verticalSlider, GUILayout.Width(10));

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorStyles.label.alignment = TextAnchor.LowerCenter;
        EditorGUILayout.BeginHorizontal();

        DrawSection(SimpleTaskStatus.Created);
        DrawSection(SimpleTaskStatus.Started);
        DrawSection(SimpleTaskStatus.Done);

        EditorGUILayout.EndHorizontal();


        EditorGUILayout.EndVertical();
        this.Repaint();
        EditorUtility.SetDirty(manager);
    }
    Vector2[] scrolls = new Vector2[3];


    void DrawSection(SimpleTaskStatus status)
    {
        string caption = "";
        switch (status)
        {
            case SimpleTaskStatus.Created:
                caption = "created tasks";
                break;
            case SimpleTaskStatus.Started:
                caption = "started tasks";
                break;
            case SimpleTaskStatus.Done:
                caption = "completed tasks";
                break;
        }

        EditorStyles.label.alignment = TextAnchor.LowerCenter;
        EditorGUILayout.BeginVertical(GUILayout.Width(columnWidth + 20));

        EditorGUILayout.LabelField(caption, GUILayout.Width(columnWidth));
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.Width(columnWidth));

        if (status == SimpleTaskStatus.Created)
        {
            if (GUILayout.Button("Create"))
            {
                SimpleTask t = new SimpleTask()
                {
                    name = "new task",
                    status = SimpleTaskStatus.Created,
                    priority = 3
                };
                manager.tasksList.Add(t);
                TaskEditor.OpenTaskEditor(t);
            }
            EditorGUILayout.Space();
        }
        scrolls[(int)status] = GUILayout.BeginScrollView(scrolls[(int)status], GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        List<SimpleTask> tasks = new List<SimpleTask>(manager.tasksList);
        tasks.Sort((p1, p2) => p1.priority.CompareTo(p2.priority));

        foreach (SimpleTask task in tasks)
        {
            if (task.status != status) continue;
            if (currentFilter > -1 && currentFilter != (int)task.type) continue;
            if (searchString != "" && !task.name.ToLower().Contains(searchString.ToLower())) continue;

            DrawTask(task);
        }

        GUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        EditorGUILayout.LabelField("", GUI.skin.verticalSlider, GUILayout.Width(10));
    }

    void DrawTask(SimpleTask task)
    {
        GUIStyle style = new GUIStyle();
        switch (task.status)
        {
            case SimpleTaskStatus.Created:
                style.normal.background = GetBackground(1);
                break;
            case SimpleTaskStatus.Started:
                style.normal.background = GetBackground(2);
                break;
            case SimpleTaskStatus.Done:
                style.normal.background = GetBackground(3);
                break;
        }
        //style.normal.background = GetBackground(0);

        EditorStyles.label.alignment = TextAnchor.LowerLeft;
        EditorGUILayout.BeginVertical(style, GUILayout.Width(columnWidth - 10), GUILayout.Height(100));


        EditorGUILayout.LabelField("Name", task.name);
        EditorGUILayout.LabelField("Task", task.description, EditorStyles.textArea);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.Width(columnWidth - 100));

        GUI.backgroundColor = typeColors[(int)task.type];
        GUILayout.Button(task.type.ToString(), GUILayout.Width(60));

        GUI.backgroundColor = priorityColors[task.priority];
        GUILayout.Button(task.priority.ToString());
        GUI.backgroundColor = oldColor;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("e", GUILayout.Width(30)))
        {
            TaskEditor.OpenTaskEditor(task);
        }

        if (GUILayout.Button("-", GUILayout.Width(30)))
        {
            if (EditorUtility.DisplayDialog("Delete task?", "Are you sure delete task " + task.name + "?", "Delete", "Cancel"))
            {
                manager.tasksList.Remove(task);
            }
        }

        if (task.status != SimpleTaskStatus.Done)
        {
            EditorGUILayout.LabelField("");
            if (task.status == SimpleTaskStatus.Created)
            {
                if (GUILayout.Button("start", GUILayout.Width(60)))
                {
                    task.status = SimpleTaskStatus.Started;
                }
            }
            if (task.status == SimpleTaskStatus.Started)
            {
                if (GUILayout.Button("cancel", GUILayout.Width(60)))
                {
                    task.status = SimpleTaskStatus.Created;
                }
                if (GUILayout.Button("done", GUILayout.Width(60)))
                {
                    task.status = SimpleTaskStatus.Done;
                }

            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        //EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.Width(columnWidth));
    }

    Texture2D GetBackground(int id)
    {
        Texture2D[] texture_array = new Texture2D[4] {
            new Texture2D(1, 1),
            new Texture2D(1, 1),
            new Texture2D(1, 1),
            new Texture2D(1, 1)
        };

        texture_array[0].SetPixel(0, 0, Color.clear);
        texture_array[0].Apply();

        texture_array[1].SetPixel(0, 0, Color.grey * 0.1f);
        texture_array[1].Apply();

        texture_array[2].SetPixel(0, 0, Color.yellow * 0.2f);
        texture_array[2].Apply();

        texture_array[3].SetPixel(0, 0, Color.green * 0.2f);
        texture_array[3].Apply();

        return texture_array[id];
    }
}
