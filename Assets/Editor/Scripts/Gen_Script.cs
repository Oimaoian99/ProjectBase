using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
public class Gen_Script : EditorWindow
{
    private Texture2D logo;
    private string userInput = "";
    private Object selectedFolder;
    private float rainbowOffset = 0f;
    private Vector2 scrollPosition = Vector2.zero;
    private MonoScript baseClassScript;
    public static void ShowWindow()
    {
        Gen_Script window = GetWindow<Gen_Script>(true, "Gen_Script Popup", true);
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
        GUIStyle blueStyle = new GUIStyle(EditorStyles.boldLabel);
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

        GUILayout.Label("Gen Script", blueStyle);

        GUILayout.Space(10);
        GUILayout.Label("Base Script (Drag & Drop Template):");
        baseClassScript = (MonoScript)EditorGUILayout.ObjectField(baseClassScript, typeof(MonoScript), false);

        GUILayout.Space(10);
        GUILayout.Label("Enter multiple script names (comma or newline):");
        userInput = GUILayout.TextArea(userInput, GUILayout.Height(100));

        GUILayout.Space(10);
        GUILayout.Label("Select Folder to Save Scripts:");
        selectedFolder = EditorGUILayout.ObjectField(selectedFolder, typeof(DefaultAsset), false);

        GUILayout.Space(10);
        GUIStyle redStyle = new GUIStyle(GUI.skin.button);
        redStyle.normal.textColor = Color.red;
        redStyle.fontSize = 2 * EditorStyles.boldLabel.fontSize;
        redStyle.alignment = TextAnchor.MiddleCenter;

        if (GUILayout.Button("Gen Script", redStyle))
        {
            GenerateScript();
        }

        if (GUILayout.Button("Back"))
        {
            HA_Sdk.ShowPopup();
            this.Close();
        }
        EditorGUILayout.EndScrollView(); // Đảm bảo luôn đóng ScrollView
        Repaint();
    }
    private void GenerateScript()
    {
        if (string.IsNullOrEmpty(userInput))
        {
            Debug.LogError("Script names cannot be empty!");
            return;
        }

        // Tách danh sách tên UI từ chuỗi nhập vào
        string[] scriptNames = userInput.Split(new[] { ',', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

          string directory = selectedFolder != null ? AssetDatabase.GetAssetPath(selectedFolder) : "Assets/Projects/Scripts/UI";
        if (!Directory.Exists(directory))
        {
            Debug.LogError("Selected folder is invalid!");
            return;
        }

        string templateContent = "";

        if (baseClassScript != null)
        {
            // Nếu đã có baseClassScript, đọc nội dung từ đó
            string baseScriptPath = AssetDatabase.GetAssetPath(baseClassScript);
            templateContent = File.ReadAllText(baseScriptPath);
        }
        else
        {
            // Nếu baseClassScript == null, tự động dùng Base.cs làm template
            string defaultBasePath = "Assets/Editor/Templates/Base.cs";
            if (File.Exists(defaultBasePath))
            {
                templateContent = File.ReadAllText(defaultBasePath);
                Debug.Log("Using default Base.cs as template.");
            }
            else
            {
                Debug.LogError("Base.cs not found at: " + defaultBasePath);
                return;
            }
        }

        foreach (string name in scriptNames)
        {
            string cleanName = name.Trim();
            if (string.IsNullOrEmpty(cleanName)) continue;

            string className = cleanName;
            string newScriptContent = templateContent.Replace(baseClassScript != null ? baseClassScript.name : "Base", className);

            string path = Path.Combine(directory, className + ".cs");
            if (File.Exists(path))
            {
                Debug.LogWarning("Script already exists: " + path);
                continue;
            }

            File.WriteAllText(path, newScriptContent);
            Debug.Log("Script created: " + path);
        }

        AssetDatabase.Refresh();
    }

}