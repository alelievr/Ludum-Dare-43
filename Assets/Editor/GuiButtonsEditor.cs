// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;
// using UnityEditorInternal;

// [CustomEditor(typeof(GuiButtons))]
// public class GuiButtonsEditor : Editor
// {
//     GuiButtons      buttons;
//     ReorderableList list;

//     private void OnEnable()
//     {
//         buttons = target as GuiButtons;
//         list = new ReorderableList(buttons.buttonList, typeof(GuiButtons.ButtonList));

//         list.onAddCallback = (list) => {
//             buttons.buttonList.Add(new GuiButtons.ButtonList{
//                 index = 0,
//                 actif = true,
//             });
//         };

//         list.drawElementCallback = (rect, index, active, selected) => {
//             rect.height = EditorGUIUtility.singleLineHeight;
//             Rect labelRect = rect;
//             Rect gameObjectRect = rect;
//             gameObjectRect.y += EditorGUIUtility.singleLineHeight + 6;
//             Rect actifRect = rect;
//             actifRect.y += EditorGUIUtility.singleLineHeight * 2 + 8;
//             // buttons.buttonList[index].name = EditorGUI.TextField(labelRect, "Name", buttons.buttonList[index].name);
//             buttons.buttonList[index].index = (PlayerController.Key)EditorGUI.Popup(labelRect, "Key", (int)buttons.buttonList[index].index, System.Enum.GetNames(typeof(PlayerController.Key)));
//             buttons.buttonList[index].guiButton = EditorGUI.ObjectField(gameObjectRect, "GUI button", buttons.buttonList[index].guiButton as Object, typeof(GameObject), true) as GameObject;
//             buttons.buttonList[index].guiLayer = EditorGUI.ObjectField(gameObjectRect, "GUI Layer", buttons.buttonList[index].guiLayer as Object, typeof(GameObject), true) as GameObject;
//             buttons.buttonList[index].actif = EditorGUI.Toggle(actifRect, "Active", buttons.buttonList[index].actif);
//         };

//         list.elementHeight = EditorGUIUtility.singleLineHeight * 4 + 16;
//     }

//     public override void OnInspectorGUI()
//     {
//         list.DoLayoutList();
//     }
// }
