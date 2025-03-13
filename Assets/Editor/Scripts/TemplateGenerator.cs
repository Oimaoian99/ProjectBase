using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TemplateGenerator : EditorWindow
{
    private Texture2D logo;
    private float rainbowOffset = 0f;
    private string nameInput = "UI_Template";
    private string valueInput =
@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class {CLASS_NAME} : {BASE_CLASS}
{
    public override void Initialize(UiController _uiController)
    {
        base.Initialize(_uiController);
        {STATE_TYPE} = {STATE_ENUM}.{CLASS_NAME};
    }

    public override void {SHOW_METHOD}()
    {
        base.{SHOW_METHOD}();
    }

    public override void Hide()
    {
        base.Hide();
    }
}";

    private Vector2 scrollPosition = Vector2.zero;
    public static void ShowPopup()
    {
        TemplateGenerator window = GetWindow<TemplateGenerator>(true, "HA_Sdk Welcome", true);
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
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(400), GUILayout.Height(800));
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
        GUILayout.Label("Name Templates:");
        nameInput = GUILayout.TextField(nameInput, GUILayout.Width(395));


        GUILayout.Space(10);
        GUILayout.Label("Value Templates:");
        valueInput = GUILayout.TextField(valueInput, GUILayout.Width(395));


        GUILayout.Space(10);
        GUIStyle pinkStyle = new GUIStyle(GUI.skin.button);
        pinkStyle.normal.textColor = Color.magenta;
        pinkStyle.fontSize = 2 * EditorStyles.boldLabel.fontSize;
        pinkStyle.alignment = TextAnchor.MiddleCenter;

        if (GUILayout.Button("Gen Templates", pinkStyle))
        {
            CreateTemplate(nameInput, valueInput);
        }
        if (GUILayout.Button("Back"))
        {
            HA_Sdk.ShowPopup();
            this.Close();
        }
        EditorGUILayout.EndScrollView(); // Đảm bảo luôn đóng ScrollView
        Repaint();
    }
    public static void CreateTemplate(string name, string value)
    {
        string directory = "Assets/Editor/Templates";
        string templatePath = Path.Combine(directory, name + ".txt");

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(templatePath))
        {
            string templateContent = value;

            File.WriteAllText(templatePath, templateContent);
            AssetDatabase.Refresh();
            Debug.Log("UI Template created at: " + templatePath);
        }
        else
        {
            Debug.LogWarning("Template already exists!");
        }
    }
}
