using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class Gen_UI : EditorWindow
{
    private Texture2D logo;
    private string userInput = "";
    private float rainbowOffset = 0f;
    private Vector2 scrollPosition = Vector2.zero;

    private UIType selectedUIType = UIType.Screen;

    private enum UIType
    {
        Screen,
        Popup
    }

    // [MenuItem("HA_Sdk/Open Tool")]
    public static void ShowWindow()
    {
        Gen_UI window = GetWindow<Gen_UI>(true, "Gen_UI Popup", true);
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
        GUIStyle pinkStyle = new GUIStyle(EditorStyles.boldLabel);
        pinkStyle.normal.textColor = Color.magenta;
        pinkStyle.fontSize = 2 * EditorStyles.boldLabel.fontSize;
        pinkStyle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label("GEN UI", pinkStyle);

        GUILayout.Space(10);
        GUILayout.Label("UI Type:");
        selectedUIType = (UIType)EditorGUILayout.EnumPopup(selectedUIType);

        GUILayout.Space(10);
        GUILayout.Space(10);
        GUILayout.Label("Enter UI Names (comma or new line separated):");
        userInput = EditorGUILayout.TextArea(userInput, GUILayout.Height(100), GUILayout.Width(395));


        GUILayout.Space(10);
        if (GUILayout.Button("Gen script"))
        {
            GenerateScript();
        }

        if (GUILayout.Button("Delete script"))
        {
            DeleteScript();
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
            Debug.LogError("Script name cannot be empty!");
            return;
        }

        string[] uiNames = userInput.Split(new[] { ',', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        string templatePath = "Assets/Editor/Templates/UI_Template.txt";
        if (!File.Exists(templatePath))
        {
            Debug.LogError("Template file not found: " + templatePath);
            return;
        }

        string templateContent = File.ReadAllText(templatePath);
        string directory = Path.Combine(Application.dataPath, "Projects/Scripts/UI");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        foreach (string name in uiNames)
        {
            string cleanName = name.Trim();
            if (string.IsNullOrEmpty(cleanName)) continue;

            string className = cleanName + (selectedUIType == UIType.Screen ? "Screen" : "Popup");
            string baseClass = selectedUIType == UIType.Screen ? "BaseScreen" : "BasePopup";
            string stateType = selectedUIType == UIType.Screen ? "screenState" : "popupState";
            string stateEnum = selectedUIType == UIType.Screen ? "ScreenState" : "PopupState";
            string showMethod = selectedUIType == UIType.Screen ? "Active" : "Show";

            string scriptContent = templateContent
                .Replace("{CLASS_NAME}", className)
                .Replace("{BASE_CLASS}", baseClass)
                .Replace("{STATE_TYPE}", stateType)
                .Replace("{STATE_ENUM}", stateEnum)
                .Replace("{SHOW_METHOD}", showMethod);

            string path = Path.Combine(directory, className + ".cs");
            if (File.Exists(path))
            {
                Debug.LogWarning("Script already exists: " + className);
                continue;
            }

            File.WriteAllText(path, scriptContent);
            Debug.Log("Script generated: " + path);
        }

        AssetDatabase.Refresh();
        AddStateToGameMasterState(uiNames);
    }

    private void DeleteScript()
    {
        if (string.IsNullOrEmpty(userInput))
        {
            Debug.LogError("Script name cannot be empty!");
            return;
        }

        string directory = "Assets/Projects/Scripts/UI";
        string[] uiNames = userInput.Split(new[] { ',', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        List<string> deletedScripts = new List<string>();

        foreach (string name in uiNames)
        {
            string cleanName = name.Trim();
            if (string.IsNullOrEmpty(cleanName)) continue;

            string path = Path.Combine(directory, cleanName + (selectedUIType == UIType.Screen ? "Screen.cs" : "Popup.cs"));

            if (File.Exists(path))
            {
                File.Delete(path);
                deletedScripts.Add(cleanName);
                Debug.Log("Deleted script: " + path);
            }
            else
            {
                Debug.LogWarning("Script not found: " + cleanName);
            }
        }

        AssetDatabase.Refresh();
        RemoveStateFromGameMasterState(deletedScripts.ToArray());
    }

    private void AddStateToGameMasterState(string[] uiNames)
    {
        string gameMasterPath = Path.Combine(Application.dataPath, "Projects/Scripts/Controller/GameMasterState.cs");
        if (!File.Exists(gameMasterPath))
        {
            Debug.LogError("GameMasterState.cs not found!");
            return;
        }

        string content = File.ReadAllText(gameMasterPath);
        string stateEnum = selectedUIType == UIType.Screen ? "ScreenState" : "PopupState";

        int enumIndex = content.IndexOf("enum " + stateEnum);
        if (enumIndex == -1)
        {
            Debug.LogError("Enum " + stateEnum + " not found in GameMasterState.cs!");
            return;
        }

        int startIndex = content.IndexOf("{", enumIndex);
        int endIndex = content.IndexOf("}", startIndex);
        if (startIndex == -1 || endIndex == -1)
        {
            Debug.LogError("Malformed enum structure in GameMasterState.cs!");
            return;
        }

        string enumContent = content.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();
        List<string> states = new List<string>(enumContent.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries));

        foreach (string name in uiNames)
        {
            string cleanName = name.Trim();
            if (string.IsNullOrEmpty(cleanName)) continue;

            string newState = cleanName + (selectedUIType == UIType.Screen ? "Screen" : "Popup");
            if (!states.Contains(newState))
            {
                states.Add("    " + newState);
            }
        }

        string updatedEnum = string.Join(",\n", states);
        content = content.Substring(0, startIndex + 1) + "\n" + updatedEnum + "\n" + content.Substring(endIndex);

        File.WriteAllText(gameMasterPath, content);
        Debug.Log("Added multiple states to " + stateEnum);
    }
    private void RemoveStateFromGameMasterState(string[] uiNames)
    {
        string gameMasterPath = Path.Combine(Application.dataPath, "Projects/Scripts/Controller/GameMasterState.cs");
        if (!File.Exists(gameMasterPath))
        {
            Debug.LogError("GameMasterState.cs not found!");
            return;
        }

        string content = File.ReadAllText(gameMasterPath);
        string stateEnum = selectedUIType == UIType.Screen ? "ScreenState" : "PopupState";

        int enumIndex = content.IndexOf("enum " + stateEnum);
        if (enumIndex == -1)
        {
            Debug.LogError("Enum " + stateEnum + " not found in GameMasterState.cs!");
            return;
        }

        int startIndex = content.IndexOf("{", enumIndex);
        int endIndex = content.IndexOf("}", startIndex);
        if (startIndex == -1 || endIndex == -1)
        {
            Debug.LogError("Malformed enum structure in GameMasterState.cs!");
            return;
        }

        string enumContent = content.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();
        List<string> states = new List<string>(enumContent.Split(new[] { ',', '\n' }, System.StringSplitOptions.RemoveEmptyEntries));

        foreach (string name in uiNames)
        {
            string cleanName = name.Trim();
            if (string.IsNullOrEmpty(cleanName)) continue;

            string targetState = cleanName + (selectedUIType == UIType.Screen ? "Screen" : "Popup");
            states.RemoveAll(s => s.Trim().Equals(targetState, System.StringComparison.OrdinalIgnoreCase));
        }

        if (states.Count > 0)
        {
            string updatedEnum = string.Join(",\n    ", states.Select(s => s.Trim()));
            content = content.Substring(0, startIndex + 1) + "\n    " + updatedEnum + "\n" + content.Substring(endIndex);
        }
        else
        {
            // Nếu không còn state nào, giữ enum nhưng để trống tránh lỗi cú pháp
            content = content.Substring(0, startIndex + 1) + "\n    \n" + content.Substring(endIndex);
        }

        File.WriteAllText(gameMasterPath, content);
        Debug.Log("Removed multiple states from " + stateEnum);
    }

}
