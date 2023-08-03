using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimatorControllerArray", menuName = "Custom/AnimatorControllerArray")]
public class AnimatorControllerArrayAsset : ScriptableObject
{
    public AnimatorController[] controllers;
}
