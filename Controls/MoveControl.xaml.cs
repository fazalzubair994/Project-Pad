using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Linq;
using projectPad.Public_Classes;

namespace Project_Pad.Controls
{
    /// <summary>
    /// Interaction logic for MoveControl.xaml
    /// </summary>
    public partial class MoveControl : UserControl
    {
        public event EventHandler NoteIsMoved;
        public MoveControl()
        {
            InitializeComponent();
            LoadCollections();
        }

        public static string FolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Remem");

        

        private void LoadCollections()
        {
            Note _TempNote = new();
            if (Directory.Exists(FolderPath))
            {
                var directories = Directory.GetDirectories(FolderPath);
                
                foreach (string directory in directories)
                {
                    string readmeFilePath = System.IO.Path.Combine(directory, "ReadMe.bin");
                    if (System.IO.File.Exists(readmeFilePath))
                    {
                        _TempNote = Note.DeserializeFromJson(System.IO.File.ReadAllText(readmeFilePath));
                        Button FolderButton = new();
                        if (_TempNote.Note_Color != null)
                            FolderButton.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(_TempNote.Note_Color)!;
                        else
                            Background = Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#414141")!;
                        FolderButton.Content = _TempNote.Note_Title;
                        FolderButton.Click += MoveButton_Click; // Attach event handler
                        CollectionsListBox.Children.Add(FolderButton);
                    }


                }
            }
            else
            {
                MessageBox.Show("The specified folder path does not exist.");
            }
        }

        public string NoteFilePath { get; set; }

        private void MoveButton_Click(object sender, RoutedEventArgs e)
        {

                Button btn = (Button)sender;
                var selectedFolder = btn.Content.ToString()!;
                var destinationPath = System.IO.Path.Combine(FolderPath, selectedFolder, System.IO.Path.GetFileName(NoteFilePath));

                try
                {
                    File.Move(NoteFilePath, destinationPath);
                    NoteIsMoved?.Invoke(this, e);
                MessageBox.Show($"This Note is Moved to: {btn.Content}");
            }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to move note: {ex.Message}");
                }
           
        }
    }
}
