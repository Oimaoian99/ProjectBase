using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
public class HA_Sdk : EditorWindow
{
    private Texture2D logo;
    private float rainbowOffset = 0f;

    [MenuItem("HA_Sdk/Open Tool")]
    public static void ShowPopup()
    {
        HA_Sdk window = GetWindow<HA_Sdk>(true, "HA_Sdk Welcome", true);
        window.minSize = new Vector2(400, 800);
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
        GUIStyle pinkStyle = new GUIStyle(GUI.skin.button);
        pinkStyle.normal.textColor = Color.magenta;
        pinkStyle.fontSize = 2 * EditorStyles.boldLabel.fontSize;
        pinkStyle.alignment = TextAnchor.MiddleCenter;

        if (GUILayout.Button("GEN UI", pinkStyle))
        {
            Gen_UI.ShowWindow();
            this.Close();
        }
    }
}

public class Gen_UI : EditorWindow
{
    private Texture2D logo;
    private string userInput = "";
    private float rainbowOffset = 0f;
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
        GUILayout.Label("Name Script:");
        userInput = GUILayout.TextField(userInput, GUILayout.Width(395));

        GUILayout.Space(10);
        if (GUILayout.Button("Gen script"))
        {
            GenerateScript();
            AddStateToGameMasterState();
        }

        if (GUILayout.Button("Delete script"))
        {
            DeleteScript();
            RemoveStateFromGameMasterState();
        }

        if (GUILayout.Button("Back"))
        {
            HA_Sdk.ShowPopup();
            this.Close();
        }

        Repaint();
    }

    private void GenerateScript()
    {
        if (string.IsNullOrEmpty(userInput))
        {
            Debug.LogError("Script name cannot be empty!");
            return;
        }

        string directory = "D:/Projects/GamePlugin/Assets/Projects/Scripts/UI";
        string path;
        if (selectedUIType == UIType.Screen) path = Path.Combine(directory, userInput + "Screen.cs");
        else path = Path.Combine(directory, userInput + "Popup.cs");
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (File.Exists(path))
        {
            Debug.LogError("Script already exists!");
            return;
        }

        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("using System.Collections;");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using UnityEngine;");
            writer.WriteLine("");
            if (selectedUIType == UIType.Screen)
            {
                writer.WriteLine("public class " + userInput + "Screen : BaseScreen");
                writer.WriteLine("{");
                writer.WriteLine("    public override void Initialize(UiController _uiController)");
                writer.WriteLine("    {");
                writer.WriteLine("        base.Initialize(_uiController);");
                writer.WriteLine("        screenState = ScreenState." + userInput + "Screen;");
                writer.WriteLine("    }");
                writer.WriteLine("");
                writer.WriteLine("    public override void Active()");
                writer.WriteLine("    {");
                writer.WriteLine("        base.Active();");
                writer.WriteLine("    }");
            }
            else
            {
                writer.WriteLine("public class " + userInput + "Popup : BasePopup");
                writer.WriteLine("{");
                writer.WriteLine("    public override void Initialize(UiController _uiController)");
                writer.WriteLine("    {");
                writer.WriteLine("        base.Initialize(_uiController);");
                writer.WriteLine("        popupState = PopupState." + userInput + "Popup;");
                writer.WriteLine("    }");
                writer.WriteLine("");
                writer.WriteLine("    public override void Show()");
                writer.WriteLine("    {");
                writer.WriteLine("        base.Show();");
                writer.WriteLine("    }");
            }
            writer.WriteLine("");
            writer.WriteLine("    public override void Hide()");
            writer.WriteLine("    {");
            writer.WriteLine("        base.Hide();");
            writer.WriteLine("    }");
            writer.WriteLine("}");
        }

        Debug.Log("Script generated: " + path);
    }
    private void DeleteScript()
    {
        if (string.IsNullOrEmpty(userInput))
        {
            Debug.LogError("Script name cannot be empty!");
            return;
        }

        string directory = "Assets/Projects/Scripts/UI";
        string path;
        if (selectedUIType == UIType.Screen) path = Path.Combine(directory, userInput + "Screen.cs");
        else path = Path.Combine(directory, userInput + "Popup.cs");
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Deleted script: " + path);
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError("Script not found!");
        }
    }
    private void AddStateToGameMasterState()
    {
        string gameMasterPath = "D:/Projects/GamePlugin/Assets/Projects/Scripts/Controller/GameMasterState.cs";
        if (File.Exists(gameMasterPath))
        {
            string content = File.ReadAllText(gameMasterPath);
            if (!content.Contains(userInput + (selectedUIType == UIType.Screen ? "Screen" : "Popup")))
            {
                int index;
                if (selectedUIType == UIType.Screen) index = content.IndexOf("enum ScreenState");
                else index = content.IndexOf("enum PopupState");
                if (index != -1)
                {
                    int braceIndex = content.IndexOf("}", index);
                    if (braceIndex != -1)
                    {
                        content = content.Insert(braceIndex, ",    " + userInput + (selectedUIType == UIType.Screen ? "Screen" : "Popup"));
                        File.WriteAllText(gameMasterPath, content);
                    }
                }
            }
        }
    }
    private void RemoveStateFromGameMasterState()
    {
        // Lấy đường dẫn tương đối đến thư mục Assets
        string gameMasterPath = Path.Combine(Application.dataPath, "Projects/Scripts/Controller/GameMasterState.cs");

        if (File.Exists(gameMasterPath))
        {
            string content = File.ReadAllText(gameMasterPath);

            // Tìm enum ScreenState
            int enumIndex;
            if (selectedUIType == UIType.Screen) enumIndex = content.IndexOf("enum ScreenState");
            else enumIndex = content.IndexOf("enum PopupState");
            if (enumIndex != -1)
            {
                int startIndex = content.IndexOf("{", enumIndex);
                int endIndex = content.IndexOf("}", startIndex);
                if (startIndex != -1 && endIndex != -1)
                {
                    string enumContent = content.Substring(startIndex + 1, endIndex - startIndex - 1);
                    string[] states = enumContent.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

                    List<string> newStates = new List<string>();
                    foreach (string state in states)
                    {
                        if (!state.Trim().Equals(userInput + (selectedUIType == UIType.Screen ? "Screen" : "Popup"), System.StringComparison.OrdinalIgnoreCase))
                        {
                            newStates.Add(state.Trim());
                        }
                    }

                    // Nếu còn state, giữ lại dấu phẩy đúng cú pháp
                    string updatedEnum = string.Join(",\n    ", newStates);
                    content = content.Substring(0, startIndex + 1) + "\n    " + updatedEnum + "\n" + content.Substring(endIndex);

                    File.WriteAllText(gameMasterPath, content);
                    Debug.Log("State removed from ScreenState: " + userInput + (selectedUIType == UIType.Screen ? "Screen" : "Popup"));
                }
            }
        }
        else
        {
            Debug.LogError("GameMasterState.cs not found at: " + gameMasterPath);
        }
    }

}
