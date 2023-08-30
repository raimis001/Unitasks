using System.Collections.Generic;
using UnityEngine;

public enum SimpleTaskStatus
{
    Created, Started, Done
}
public enum SimpleTaskType
{
    Bug, Task, Wish
}

[System.Serializable]
public class SimpleTask
{
    public string name;
    public string description;
    public int priority;
    public SimpleTaskStatus status;
    public SimpleTaskType type;
}

public class TaskListManager : ScriptableObject
{
    public List<SimpleTask> tasksList = new List<SimpleTask>();
}
