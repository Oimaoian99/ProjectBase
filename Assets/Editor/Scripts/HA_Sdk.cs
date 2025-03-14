using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
public class HA_Sdk : EditorWindow
{
    private Texture2D logo;
    private float rainbowOffset = 0f;
    private Vector2 scrollPosition = Vector2.zero;
    [MenuItem("HA_Sdk/Open Tool")]
    public static void ShowPopup()
    {
        HA_Sdk window = GetWindow<HA_Sdk>(true, "HA_Sdk Welcome", true);
        window.minSize = new Vector2(400, 800);
        window.maxSize = new Vector2(400, 800);
        window.position = new Rect(
            (Screen.currentResolution.width - 400) / 2,
            (Screen.currentResolution.height - 800) / 2,
            400,
            300
        );
        window.LoadLogo();
    }

    private void LoadLogo()
    {
        logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Sprites/logo.png");
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(400), GUILayout.Height(600));
        GUIStyle rainbowStyle = new GUIStyle(EditorStyles.boldLabel);
        rainbowStyle.fontSize = 2 * EditorStyles.boldLabel.fontSize;
        rainbowStyle.alignment = TextAnchor.MiddleCenter;

        string text = "Welcome to HA_Sdk!";
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        for (int i = 0; i < text.Length; i++)
        {
            float hue = (rainbowOffset + i * 0.1f) % 1f;
            Color letterColor = Color.HSVToRGB(hue, 1f, 1f);
            GUIStyle letterStyle = new GUIStyle(rainbowStyle);
            letterStyle.normal.textColor = letterColor;
            GUILayout.Label(text[i].ToString(), letterStyle);
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        rainbowOffset += 0.01f;
        if (rainbowOffset > 1f) rainbowOffset = 0f;

        if (logo != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(logo, GUILayout.Width(380), GUILayout.Height(380));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.Label("Logo not found!");
        }

        GUILayout.Space(10);
        GUIStyle redStyle = new GUIStyle(GUI.skin.button);
        redStyle.normal.textColor = Color.red;
        redStyle.fontSize = 2 * EditorStyles.boldLabel.fontSize;
        redStyle.alignment = TextAnchor.MiddleCenter;

        if (GUILayout.Button("Template Generator", redStyle))
        {
            TemplateGenerator.ShowPopup();
            this.Close();
        }

        GUILayout.Space(10);
        GUIStyle pinkStyle = new GUIStyle(GUI.skin.button);
        pinkStyle.normal.textColor = Color.magenta;
        pinkStyle.fontSize = 2 * EditorStyles.boldLabel.fontSize;
        pinkStyle.alignment = TextAnchor.MiddleCenter;

        if (GUILayout.Button("GEN UI", pinkStyle))
        {
            Gen_UI.ShowWindow();
            this.Close();
        }

        GUILayout.Space(10);
        GUIStyle blueStyle = new GUIStyle(GUI.skin.button);
        Color color;
        if (ColorUtility.TryParseHtmlString("#00FFF1", out color))
        {
            blueStyle.normal.textColor = color;
        }
        else
        {
            Debug.LogError("Invalid color string!");
        }
        blueStyle.fontSize = 2 * EditorStyles.boldLabel.fontSize;
        blueStyle.alignment = TextAnchor.MiddleCenter;

        if (GUILayout.Button("Gen Script", blueStyle))
        {
            Gen_Script.ShowWindow();
            this.Close();
        }
        EditorGUILayout.EndScrollView(); // Đảm bảo luôn đóng ScrollView
        Repaint();
    }
}

