using UnityEditor;
using UnityEngine;
using PacknCraft.Inventory;

[CustomEditor(typeof(ItemConfig))]
public class ItemConfigEditor : Editor
{
    private const int CELL_SIZE = 25;
    private const int PADDING = 4;

    private bool isDragging;
    private bool paintValue;

    public override void OnInspectorGUI()
    {
        var config = (ItemConfig)target;

        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Id"));
        serializedObject.ApplyModifiedProperties();

        if (config.Shape == null)
        {
            if (GUILayout.Button("Create Shape"))
            {
                Undo.RecordObject(config, "Create Shape");
                config.Shape = new ShapeData();
                config.Shape.Resize(2);
                EditorUtility.SetDirty(config);
            }
            return;
        }

        int newSize = EditorGUILayout.IntField("Size", config.Shape.size);

        if (newSize != config.Shape.size && newSize > 0)
        {
            Undo.RecordObject(config, "Resize Shape");
            config.Shape.Resize(newSize);
            EditorUtility.SetDirty(config);
        }

        EditorGUILayout.Space();
        DrawTools(config);
        EditorGUILayout.Space();
        DrawGridCentered(config);
    }

    private void DrawTools(ItemConfig config)
    {
        var shape = config.Shape;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Fill"))
        {
            Undo.RecordObject(config, "Fill Shape");

            for (int i = 0; i < shape.data.Length; i++)
                shape.data[i] = true;

            EditorUtility.SetDirty(config);
        }

        if (GUILayout.Button("Clear"))
        {
            Undo.RecordObject(config, "Clear Shape");

            for (int i = 0; i < shape.data.Length; i++)
                shape.data[i] = false;

            EditorUtility.SetDirty(config);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawGridCentered(ItemConfig config)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginVertical();
        DrawGrid(config);
        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawGrid(ItemConfig config)
    {
        var shape = config.Shape;
        Event e = Event.current;

        for (int y = 0; y < shape.size; y++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < shape.size; x++)
            {
                Rect rect = GUILayoutUtility.GetRect(
                    CELL_SIZE,
                    CELL_SIZE,
                    GUILayout.Width(CELL_SIZE),
                    GUILayout.Height(CELL_SIZE)
                );

                bool value = shape.Get(x, y);

                EditorGUI.DrawRect(rect, value ? Color.green : Color.white);

                Handles.color = Color.black;
                Handles.DrawAAPolyLine(1.5f,
                    new Vector3(rect.x, rect.y),
                    new Vector3(rect.xMax, rect.y),
                    new Vector3(rect.xMax, rect.yMax),
                    new Vector3(rect.x, rect.yMax),
                    new Vector3(rect.x, rect.y)
                );

                HandleInput(rect, config, shape, x, y, e);

                GUILayout.Space(PADDING);
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(PADDING);
        }
    }

    private void HandleInput(Rect rect, ItemConfig config, ShapeData shape, int x, int y, Event e)
    {
        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            Undo.RecordObject(config, "Paint Cell");

            paintValue = !shape.Get(x, y);
            shape.Set(x, y, paintValue);

            isDragging = true;
            e.Use();
        }

        if (e.type == EventType.MouseDrag && isDragging && rect.Contains(e.mousePosition))
        {
            Undo.RecordObject(config, "Paint Cell");
            shape.Set(x, y, paintValue);
            e.Use();
        }

        if (e.type == EventType.MouseUp)
        {
            isDragging = false;
        }
    }
}