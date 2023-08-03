using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.Collections.Generic;
using System;

public class AnimatorControllerMerger : EditorWindow
{
    private AnimatorController mainController;
    private AnimatorControllerArrayAsset childControllersAsset;

    private bool[] isEnabledArray = new bool[0];

    [MenuItem("Window/Animator Controller Merger")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AnimatorControllerMerger));
    }

    private void OnGUI()
    {
        GUILayout.Label("Animator Controller Merger", EditorStyles.boldLabel);

        mainController = EditorGUILayout.ObjectField("Main Controller", mainController, typeof(AnimatorController), false) as AnimatorController;
        EditorGUILayout.Space();

        childControllersAsset = EditorGUILayout.ObjectField("Child Controllers Asset", childControllersAsset, typeof(AnimatorControllerArrayAsset), false) as AnimatorControllerArrayAsset;

        EditorGUILayout.Space();

        if (childControllersAsset != null)
        {
            EditorGUILayout.LabelField("Child Controllers to Merge", EditorStyles.boldLabel);

            int newCount = childControllersAsset.controllers.Length;
            if (newCount != isEnabledArray.Length)
            {
                Array.Resize(ref isEnabledArray, newCount);
            }

            GUILayout.BeginVertical();

            for (int i = 0; i < childControllersAsset.controllers.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                isEnabledArray[i] = EditorGUILayout.Toggle(isEnabledArray[i], GUILayout.Width(20));
                EditorGUILayout.ObjectField(childControllersAsset.controllers[i], typeof(AnimatorController), false);
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Merge Controllers"))
        {
            MergeControllers();
        }
    }

    private void MergeControllers()
    {
        if (mainController == null || childControllersAsset == null)
        {
            Debug.LogWarning("Please set the main controller and select the child controllers asset to merge.");
            return;
        }

        // deez
        List<AnimatorController> enabledControllers = new List<AnimatorController>();
        for (int i = 0; i < isEnabledArray.Length; i++)
        {
            if (isEnabledArray[i])
            {
                enabledControllers.Add(childControllersAsset.controllers[i]);
            }
        }
        MergeParameters(mainController, enabledControllers.ToArray());
        foreach (AnimatorController controller in enabledControllers)
        {
            if (controller != null)
            {
                foreach (AnimatorControllerLayer layer in controller.layers)
                {
                    if (!HasLayer(mainController, layer.name))
                    {
                        mainController.AddLayer(layer);
                    }
                }
            }
        }

        Debug.Log("Animator controllers merged successfully!");
    }

    private void MergeParameters(AnimatorController mainController, AnimatorController[] childControllers)
    {
        foreach (AnimatorController controller in childControllers)
        {
            if (controller != null)
            {
                foreach (AnimatorControllerParameter parameter in controller.parameters)
                {
                    if (!HasParameter(mainController, parameter.name))
                    {
                        mainController.AddParameter(parameter);
                    }
                    else
                    {
                        // If parameter exists in both controllers, use the value from the child controller
                        AnimatorControllerParameter mainParam = GetParameter(mainController, parameter.name);
                        AnimatorControllerParameter childParam = GetParameter(controller, parameter.name);

                        mainParam.defaultBool = childParam.defaultBool;
                        mainParam.defaultFloat = childParam.defaultFloat;
                        mainParam.defaultInt = childParam.defaultInt;
                    }
                }
            }
        }
    }

    private bool HasLayer(AnimatorController controller, string layerName)
    {
        foreach (AnimatorControllerLayer layer in controller.layers)
        {
            if (layer.name == layerName)
            {
                return true;
            }
        }
        return false;
    }

    private bool HasParameter(AnimatorController controller, string parameterName)
    {
        foreach (AnimatorControllerParameter parameter in controller.parameters)
        {
            if (parameter.name == parameterName)
            {
                return true;
            }
        }
        return false;
    }

    private AnimatorControllerParameter GetParameter(AnimatorController controller, string parameterName)
    {
        foreach (AnimatorControllerParameter parameter in controller.parameters)
        {
            if (parameter.name == parameterName)
            {
                return parameter;
            }
        }
        return null;
    }
}
