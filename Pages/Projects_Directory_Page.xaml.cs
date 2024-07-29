using projectPad.Public_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using projectPad.Controls;

namespace projectPad.Pages;

/// <summary>
/// Interaction logic for Projects_Directory_Page.xaml
/// </summary>
public partial class Projects_Directory_Page : Page
{
    public Projects_Directory_Page()
    {
        InitializeComponent();
    }


    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        HandleResources.NewNoteEventHandler += NewNoteEventHandler;

        // Get all files and folders in the HandleResources.FolderPath
        string[] files = System.IO.Directory.GetFiles(HandleResources.DirectoryName);
        // Process individual note files
        foreach (string file in files)
        {
            if(!file.Contains("ReadMe.bin"))
                ProcessFile(file, true, "");
        }

        //// Process folders containing readme.txt
        //foreach (string directory in directories)
        //{
        //    string readmeFilePath = System.IO.Path.Combine(directory, "ReadMe.bin");
        //    if (System.IO.File.Exists(readmeFilePath))
        //    {
        //        ProcessFile(readmeFilePath, false, directory);
        //    }
        //}
    }
    private void NewNoteEventHandler(object? sender, EventArgs e)
    {
        if (System.IO.File.Exists(HandleResources.CurrentFileName))
        {
            ProcessFile(HandleResources.CurrentFileName, true, "");
        }
    }


    private void ProcessFile(string file, bool isNote, string directory)
    {


        Note UpdateNote = Note.DeserializeFromJson(System.IO.File.ReadAllText(file));

        ButtonTile BT = new ButtonTile();
        BT.Width = 200;
        BT.TypeNote = isNote;
        if (!BT.TypeNote)
        {
            BT.Directory = directory;
        }
        BT.Height = 200;
        BT.Margin = new Thickness(10);
        BT.FileName = file;
        BT.DeleteNote += DeleteNote;
        BT.ThisNoteIsMoved += ThisNoteisMoved;
       
        main_panel.Children.Add(BT);

    }

    private void ThisNoteisMoved(object? sender, EventArgs e)
    {
        if (sender is ButtonTile BT)
        {
            main_panel.Children.Remove(BT);
        }
    }

    private void DeleteNote(object? sender, EventArgs e)
    {
        if (sender is ButtonTile BT)
        {
            // Show a confirmation message box
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (!BT.TypeNote)
                {
                    if (System.IO.Directory.Exists(BT.Directory))
                    {
                        System.IO.Directory.Delete(BT.Directory, true);
                        main_panel.Children.Remove(BT);
                    }
                    else
                    {
                        MessageBox.Show($"Directory does not exist: {BT.Directory}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    if (System.IO.File.Exists(BT.FileName))
                    {
                        System.IO.File.Delete(BT.FileName);
                        main_panel.Children.Remove(BT);
                    }
                    else
                    {
                        MessageBox.Show($"File does not exist: {BT.FileName}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
