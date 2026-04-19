using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SpawnEntry))]
public class SpawnEntryDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        // 提取当前的类型，让折叠栏标题更清晰
        SerializedProperty typeProp = property.FindPropertyRelative("type");
        string displayName = typeProp != null ? $"[{typeProp.enumNames[typeProp.enumValueIndex]}] Spawn Entry" : "Spawn Entry";

        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, displayName, true);
        rect.y += EditorGUIUtility.singleLineHeight + 2;

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            // 公共属性
            EditorGUI.PropertyField(rect, typeProp);
            rect.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.PropertyField(rect, property.FindPropertyRelative("hasFog"));
            rect.y += EditorGUIUtility.singleLineHeight + 2;

            // 根据所选择的 SpawnType 动态显示关联属性
            SpawnType typeEnum = (SpawnType)typeProp.enumValueIndex;

            if (typeEnum == SpawnType.Obstacle)
            {
                // 仅 Obstacle 显示是否可摧毁
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("isIndestructible"));
                rect.y += EditorGUIUtility.singleLineHeight + 2;

                // 仅 Obstacle 显示是否围绕金币
                SerializedProperty hasCoinProp = property.FindPropertyRelative("hasCoinAround");
                EditorGUI.PropertyField(rect, hasCoinProp);
                rect.y += EditorGUIUtility.singleLineHeight + 2;

                if (hasCoinProp.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.PropertyField(rect, property.FindPropertyRelative("coinAroundCount"), new GUIContent("金币数量 (单侧)"));
                    rect.y += EditorGUIUtility.singleLineHeight + 2;
                    EditorGUI.PropertyField(rect, property.FindPropertyRelative("coinAroundArcHeight"), new GUIContent("金币弧度"));
                    rect.y += EditorGUIUtility.singleLineHeight + 2;
                    EditorGUI.PropertyField(rect, property.FindPropertyRelative("coinAroundSpacing"), new GUIContent("金币间距"));
                    rect.y += EditorGUIUtility.singleLineHeight + 2;
                    EditorGUI.indentLevel--;
                }
            }
            else if (typeEnum == SpawnType.CoinCluster)
            {
                // 仅 CoinCluster 显示相关的独立属性
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("coinCount"), new GUIContent("簇内金币总数"));
                rect.y += EditorGUIUtility.singleLineHeight + 2;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("arcHeight"), new GUIContent("簇金币弧度"));
                rect.y += EditorGUIUtility.singleLineHeight + 2;
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

        // 默认显示：折叠头 + Type + hasFog
        int lines = 3;

        SerializedProperty typeProp = property.FindPropertyRelative("type");
        if (typeProp != null)
        {
            SpawnType typeEnum = (SpawnType)typeProp.enumValueIndex;

            if (typeEnum == SpawnType.Obstacle)
            {
                lines += 2; // isIndestructible, hasCoinAround
                if (property.FindPropertyRelative("hasCoinAround").boolValue)
                {
                    lines += 3; // count, arcHeight, spacing
                }
            }
            else if (typeEnum == SpawnType.CoinCluster)
            {
                lines += 2; // coinCount, arcHeight
            }
        }

        return lines * (EditorGUIUtility.singleLineHeight + 2);
    }
}
