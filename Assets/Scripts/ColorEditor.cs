using UnityEditor;
using UnityEngine;

public class ColorEditor : EditorWindow
{
    public AbilityColors abilityColors;   

    void Awake()
    {
        //var t = Resources.FindObjectsOfTypeAll<AbilityColors>();
        //var t = (AbilityColors)Resources.FindObjectsOfTypeAll(typeof(AbilityColors))[0];
        //abilityColors = t[0];
        //abilityColors = t;

        abilityColors = Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors;
    }

    [MenuItem("Window/Ability Colors")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ColorEditor));
    }
    

    void OnGUI()
    {
        //var t = Resources.FindObjectsOfTypeAll<AbilityColors>();
        //abilityColors = t[0];

        abilityColors.DefaultColor = EditorGUILayout.ColorField("Default Color", abilityColors.DefaultColor);
        abilityColors.PushColor = EditorGUILayout.ColorField("Push Color", abilityColors.PushColor);
        abilityColors.AttackColor = EditorGUILayout.ColorField("Attack Color", abilityColors.AttackColor);
        abilityColors.DashColor = EditorGUILayout.ColorField("Dash Color", abilityColors.DashColor);
        abilityColors.JumpColor = EditorGUILayout.ColorField("Jump Color", abilityColors.JumpColor);
    }
    
}
