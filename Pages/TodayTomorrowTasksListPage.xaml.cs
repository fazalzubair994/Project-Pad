using projectPad.Controls;
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
using System.Xml.Linq;

namespace Project_Pad.Pages
{
    /// <summary>
    /// Interaction logic for TodayTomorrowTasksListPage.xaml
    /// </summary>
    public partial class TodayTomorrowTasksListPage : Page
    {
        public TodayTomorrowTasksListPage()
        {
            InitializeComponent();
        }

        public bool LoadToDays { get; set; }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            HandleResources.ToDayTomorrowPageOpened = true;

            // Get all files and folders in the HandleResources.FolderPath
            string[] files = System.IO.Directory.GetFiles(HandleResources.FolderPath);
            string[] directories = System.IO.Directory.GetDirectories(HandleResources.FolderPath);

            // Process individual note files
            foreach (string file in files)
            {
                ProcessFile(file, true, "");
            }

            // Process folders containing readme.txt
            foreach (string directory in directories)
            {
                files = System.IO.Directory.GetFiles(directory);

                foreach (string file in files)
                {
                    ProcessFile(file, true, "");
                }
            }

            if(LoadToDays)
                HeaderofPage.Content = "List of Todays Tasks";
            else
                HeaderofPage.Content = "List of Tomorrow's Tasks";

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
            // Deserialize the note from the JSON file
            Note UpdateNote = Note.DeserializeFromJson(System.IO.File.ReadAllText(file));

            // Check if the note's reminder date is not null
            if (UpdateNote.Reminder_Date != null)
            {
                DateTime dt = (DateTime)UpdateNote.Reminder_Date;

                // Load today's or tomorrow's notes based on LoadToDays flag
                if (LoadToDays)
                {
                    if (dt.Date != DateTime.Now.Date)
                        return;
                }
                else
                {
                    if (dt.Date != DateTime.Now.Date.AddDays(1))
                        return;
                }

                // Create and configure a new ButtonTile
                ButtonTile BT = new ButtonTile
                {
                    Width = 200,
                    TypeNote = isNote,
                    Height = 200,
                    Margin = new Thickness(10),
                    FileName = file
                };

                // Set the directory if it's not a note
                if (!BT.TypeNote)
                {
                    BT.Directory = directory;
                }


                // Add the ButtonTile to the main panel
                main_panel.Children.Add(BT);
            }
        }
       
    }
}
