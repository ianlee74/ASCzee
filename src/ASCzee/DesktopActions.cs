using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ASCzee;

public static class DesktopActions
{
    public static bool TryCopyToClipboard(string text, out string message)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c clip",
                        RedirectStandardInput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.StandardInput.Write(text ?? string.Empty);
                process.StandardInput.Close();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    message = "Clipboard copy failed.";
                    return false;
                }

                message = "Prompt copied to clipboard.";
                return true;
            }

            message = "Clipboard copy is currently supported on Windows in this build.";
            return false;
        }
        catch
        {
            message = "Clipboard copy failed.";
            return false;
        }
    }

    public static bool TryOpenUrl(string url, out string message)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });

            message = "Opened suno.com in your default browser.";
            return true;
        }
        catch
        {
            message = "Unable to open browser.";
            return false;
        }
    }

    public static bool TryOpenFileInNotepad(string filePath, out string message)
    {
        try
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                message = "Opening in Notepad is currently supported on Windows in this build.";
                return false;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = "notepad.exe",
                Arguments = $"\"{filePath}\"",
                UseShellExecute = true
            });

            message = "Opened prompt file in Notepad.";
            return true;
        }
        catch
        {
            message = "Unable to open Notepad.";
            return false;
        }
    }
}
