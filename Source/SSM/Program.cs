﻿using System;
using System.IO;
using System.Windows.Forms;
using CommonOpenFileDialog = Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog;

namespace SSM
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static int Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      try
      {
        Configuration config = LoadSettings();

        Manager m = new Manager(config.BaseDir);
        m.Move();
      }
      catch (Exception ex)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(ex);
        Console.ResetColor();
        Console.ReadKey(true);
        return 1;
      }

      Application.Run();
      return 0;
    }

    /// <summary>
    /// Loads an existing or creates a new configuration.
    /// </summary>
    /// <returns>A <see cref="SSM.Configuration"/> object.</returns>
    private static Configuration LoadSettings()
    {
      Configuration config = null;
      if (File.Exists(Configuration.DefaultFileName))
        config = Configuration.Load();
      else
        config = new Configuration();

      CheckSettings(config);
      return config;
    }

    /// <summary>
    /// Checks whether the specified configuration is valid and can be used. If the configuration
    /// is invalid, an appropiate exception is thrown. Otherwise, the function simply returns.
    /// </summary>
    /// <param name="config">The <see cref="SSM.Configuration"/> object to check.</param>
    private static void CheckSettings(Configuration config)
    {
      if (!Directory.Exists(config.BaseDir))
      {
        string foundDir = FindBaseDir();
        if (foundDir != null && Directory.Exists(foundDir))
          config.BaseDir = foundDir;
        else
          throw new Exception("No screenshot folder has been set.");
      }

      config.Save();
    }

    /// <summary>
    /// Finds and returns Steam's external screenshots folder.
    /// </summary>
    /// <returns>The full path to the folder, or null.</returns>
    private static string FindBaseDir()
    {
      // TODO: Parse Steam userdata config to find uncompressed screenshots folder
      using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
      {
        dialog.IsFolderPicker = true;
        dialog.Title = "Please select your external screenshots folder";
        if (dialog.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
        {
          return dialog.FileName;
        }
      }      

      return null;
    }
  }
}
