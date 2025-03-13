using UnityEngine;
using UnityEditor;
using System.IO;
using System.Net;
using System.IO.Compression;

public class GitHubDownloader : EditorWindow
{
    private string githubUrl = "https://github.com/user/repo/archive/refs/heads/main.zip"; // Thay bằng link của bạn
    private string savePath = "Assets"; // Thư mục lưu file tải về

    [MenuItem("Tools/Download Folder from GitHub")]
    public static void ShowWindow()
    {
        GetWindow<GitHubDownloader>("Download GitHub Folder");
    }

    private void OnGUI()
    {
        GUILayout.Label("Download Folder from GitHub", EditorStyles.boldLabel);
        githubUrl = EditorGUILayout.TextField("GitHub URL:", githubUrl);
        savePath = EditorGUILayout.TextField("Save Path:", savePath);

        if (GUILayout.Button("Download and Extract"))
        {
            DownloadAndExtract();
        }
    }

    private void DownloadAndExtract()
    {
        string zipPath = Path.Combine(Application.temporaryCachePath, "downloaded.zip");

        using (WebClient client = new WebClient())
        {
            client.DownloadFile(githubUrl, zipPath);
        }

        // Xóa folder cũ nếu có
        if (Directory.Exists(savePath))
        {
            Directory.Delete(savePath, true);
        }

        ZipFile.ExtractToDirectory(zipPath, savePath);
        File.Delete(zipPath);

        AssetDatabase.Refresh();
        Debug.Log("Folder downloaded and extracted to: " + savePath);
    }
}
