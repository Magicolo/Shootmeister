using System.Diagnostics;
using System.IO;
using Entia.Core;
using UnityEngine;

public class zTest : MonoBehaviour
{
    public bool Spawn;

    void OnValidate()
    {
        if (Spawn.Change(false))
        {
            var path = Path.Combine(Application.streamingAssetsPath, "testy.py");
            var process = Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = Application.streamingAssetsPath,
                FileName = "python.exe",
                Arguments = "testy.py",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = false,
            });
            process.StandardInput.WriteLine("Karl");
            var line = process.StandardOutput.ReadLine();
            UnityEngine.Debug.Log(line);
        }
    }
}