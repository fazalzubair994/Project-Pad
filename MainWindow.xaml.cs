using System.Diagnostics;
using System.IO;
using System.Text;
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
using projectPad.MessegeBoxes;
using System.Windows.Media.Animation;
using projectPad.Pages;
using System.Reflection.PortableExecutable;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;
using System.Reflection;
using projectPad.Public_Classes;
using Project_Pad.Pages;
using Project_Pad.Properties;
using Notifications.Wpf.Core;
using System.Xml;
using Notifications.Wpf.Core.Controls;
using System.ComponentModel;



namespace projectPad;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // when the window is loaded 
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {

        // Apply theme based on saved setting
        if ((bool)Project_Pad.Properties.Settings.Default["DarkTheme"])
            LoadTheme("NightMode");
        else
            LoadTheme("LightMode");

        if ((bool)Project_Pad.Properties.Settings.Default["IsSideBarOpen"])
            {
            PaneClosed = false;
            OpenPane_Click(OpenPane, e);

        }
        else
          {
            PaneClosed = true;
            OpenPane_Click(OpenPane, e);
        }

        if ((bool)Project_Pad.Properties.Settings.Default["ShowNotifications"])
            ShowUncompletedTasks();

        AppSettingsPage AppSettings = new();
        AppSettings.ThemeChanged += LoadTheme;
        CollectionsFrame.Content = AppSettings;

        // then i have to subscribe some events of HandleResources
        HandleResources.NewNoteEventHandler += NewNoteEventHandler;
        HandleResources.NewCollectionEventHandler += NewCollectionEventHandler;
        HandleResources.OpenCollectionFolder += OpenCollectionsFolder;

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
            string readmeFilePath = System.IO.Path.Combine(directory, "ReadMe.bin");
            if (System.IO.File.Exists(readmeFilePath))
            {
                ProcessFile(readmeFilePath, false, directory);
            }
        }

    }

    private void ShowUncompletedTasks()
{
    // Get all files and folders in the HandleResources.FolderPath
    string[] files = System.IO.Directory.GetFiles(HandleResources.FolderPath);
    string[] directories = System.IO.Directory.GetDirectories(HandleResources.FolderPath);
    int TasksCounter = 0;

    // Process individual note files
    foreach (string file in files)
    {
        if (isToDaysTask(file))
        {
            TasksCounter++;
        }
    }

    // Process folders containing readme.txt
    foreach (string directory in directories)
    {
        files = System.IO.Directory.GetFiles(directory);

        foreach (string file in files)
        {
            if (isToDaysTask(file))
            {
                TasksCounter++;
            }
        }
    }

    var notificationManager = new NotificationManager();

    var notificationContent = new NotificationContent
    {
        Title = "Today's Tasks",
        Message = $"You have {TasksCounter} tasks to complete today.",
        Type = NotificationType.Information
    };

    notificationManager.ShowAsync(notificationContent);
        // Keep the app running long enough to show the notification
        Task.Delay(TimeSpan.FromSeconds(10));
    }

    private bool isToDaysTask(string file)
    {
        // Deserialize the note from the JSON file
        Note UpdateNote = Note.DeserializeFromJson(System.IO.File.ReadAllText(file));

        // Check if the note's reminder date is not null
        if (UpdateNote.Reminder_Date != null)
        {
            DateTime dt = (DateTime)UpdateNote.Reminder_Date;

            if (dt.Date == DateTime.Now.Date)
                return true;
        }
        return false;
    }

    #region Events of HandleResources class, these event rised when any new note or collection is created and the data is saved by the HandleResources class and from there these events also rised, so that the changes are load dynamically in the main page of application. 
    private void NewNoteEventHandler(object? sender, EventArgs e)
    {
        if (!HandleResources.IsDirectoryOpens)
        {
            if (System.IO.File.Exists(HandleResources.CurrentFileName))
            {
                ProcessFile(HandleResources.CurrentFileName, true, "");
            }
        }
    }

    private void NewCollectionEventHandler(object? sender, EventArgs e)
    {
        string readmeFilePath = System.IO.Path.Combine(HandleResources.CurrentFileName, "ReadMe.bin");
        if (System.IO.File.Exists(readmeFilePath))
        {
            ProcessFile(readmeFilePath, false, HandleResources.CurrentFileName);
        }
    }


    private void OpenCollectionsFolder(object? sender, EventArgs e)
    {
        CollectionsFrame.Visibility = Visibility.Visible;
        BacktoHomeBtn.Visibility = Visibility.Visible;
        CollectionTitle.Content = HandleResources.CollectionTitle;
        Projects_Directory_Page PDP = new();
        CollectionsFrame.Content = PDP;
        HomePage.Visibility = Visibility.Collapsed;
        AddCollections.Visibility = Visibility.Collapsed;
    }

    private void ToDaysCollections_Click(object sender, RoutedEventArgs e)
    {
        CollectionsFrame.Visibility = Visibility.Visible;
        BacktoHomeBtn.Visibility = Visibility.Visible;
        CollectionTitle.Content = HandleResources.CollectionTitle;
        TodayTomorrowTasksListPage toDaysList = new();
        toDaysList.LoadToDays = true;
        CollectionsFrame.Content = toDaysList;
        HomePage.Visibility = Visibility.Collapsed;
        AddCollections.IsEnabled = false;
        addBtn.IsEnabled = false;
    }

    private void TomorrowCollections_Click(object sender, RoutedEventArgs e)
    {
        CollectionsFrame.Visibility = Visibility.Visible;
        BacktoHomeBtn.Visibility = Visibility.Visible;
        CollectionTitle.Content = HandleResources.CollectionTitle;
        TodayTomorrowTasksListPage toDaysList = new();
        toDaysList.LoadToDays = false;
        CollectionsFrame.Content = toDaysList;
        HomePage.Visibility = Visibility.Collapsed;
        AddCollections.IsEnabled = false;
        addBtn.IsEnabled = false;
    }

    #endregion

    // process file method will create a new note or collection object for the file or directory 
    private void ProcessFile(string file, bool isNote, string directory)
    {
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


    #region Code for AddNote, AddCollection and BacktoHomeBtn buttons in the Navigation Pane 
    private void addBtn_Click(object sender, RoutedEventArgs e)
    {
        NoteWindow NW = new();
        NW.IsCollection = false;

        NW.ShowDialog();
    }
    private void AddCollections_Click(object sender, RoutedEventArgs e)
    {
        NoteWindow NW = new();
        NW.IsCollection = true;
        NW.ShowDialog();
    }
    



    private void BacktoHomeBtn_Click(object sender, RoutedEventArgs e)
    {
        CollectionTitle.Content = "./Home-Page";
        HandleResources.IsDirectoryOpens = false;
        HandleResources.ToDayTomorrowPageOpened = false;
        CollectionsFrame.Visibility = Visibility.Collapsed;
        BacktoHomeBtn.Visibility = Visibility.Collapsed;
        HomePage.Visibility = Visibility.Visible;
        AddCollections.Visibility = Visibility.Visible;
        AddCollections.IsEnabled = true;
        addBtn.IsEnabled = true;
    }
    #endregion

    #region Max, Min and Close Button Code and Titlebar Draging Code
    private void MaxBtn_Click(object sender, RoutedEventArgs e)
    {
        if (this.WindowState == WindowState.Normal)
        {
            this.WindowState = WindowState.Maximized;
            MaxBtn.Content = "\ue923";
            WindowGrid.Margin = new Thickness(5);
        }
        else
        {
            this.WindowState = WindowState.Normal;
            MaxBtn.Content = "\uf16b";
            WindowGrid.Margin = new Thickness(0);
        }
    }
    private void MinBtn_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void CloseBtn_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void TitlebarMouseLeft_Down(object sender, MouseButtonEventArgs e)
    {
        this.DragMove();
    }
    #endregion


    private bool PaneClosed = false; // for the navigation pane in the left side of window

    private void OpenPane_Click(object sender, RoutedEventArgs e)
    {
        


        if (PaneClosed)
        {
            OpenPane.Content = "\uea49";
            AnimateWidth(SidePane.ActualWidth, 200, 0.3); // Adjust duration as needed
            PaneClosed = false;
            Project_Pad.Properties.Settings.Default["IsSideBarOpen"] = false;
            Project_Pad.Properties.Settings.Default.Save();
        }
        else
        {
            OpenPane.Content = "\uea5b";
            AnimateWidth(SidePane.ActualWidth, 50, 0.2); // Adjust duration as needed
            PaneClosed = true;
            Project_Pad.Properties.Settings.Default["IsSideBarOpen"] = true;
            Project_Pad.Properties.Settings.Default.Save();
        }
    }

    private void AnimateWidth(double from, double to, double durationInSeconds)
    {
        DoubleAnimation animation = new DoubleAnimation
        {
            From = from,
            To = to,
            Duration = new Duration(TimeSpan.FromSeconds(durationInSeconds)),
        };

        SidePane.BeginAnimation(WidthProperty, animation);
    }

    #region we have added a custom popup for the buttons in the Navigation Manu in the left side
    private void addBtn_MouseEnter(object sender, MouseEventArgs e)
    {
        if (PaneClosed)
        {
            Popup.PlacementTarget = addBtn;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Add Task";
        }
    }

    private void addBtn_MouseLeave(object sender, MouseEventArgs e)
    {
        Popup.Visibility = Visibility.Collapsed;
        Popup.IsOpen = false;
    }

    private void AddCollections_MouseEnter(object sender, MouseEventArgs e)
    {
        if (PaneClosed)
        {
            Popup.PlacementTarget = AddCollections;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Add Collection for multiple Tasks...";
        }
    }

    private void BacktoHomeBtn_MouseEnter(object sender, MouseEventArgs e)
    {
        if (PaneClosed)
        {
            Popup.PlacementTarget = BacktoHomeBtn;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = " < Back to Home Page...";
        }
    }

    private void ToDaysCollections_MouseEnter(object sender, MouseEventArgs e)
    {
        if (PaneClosed)
        {
            Popup.PlacementTarget = ToDaysCollections;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Look Todays Tasks...";
        }
    }

    private void TomorrowCollections_MouseEnter(object sender, MouseEventArgs e)
    {
        if (PaneClosed)
        {
            Popup.PlacementTarget = TomorrowCollections;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Look Tomorrow Tasks...";
        }
    }

    

    private void Settings_MouseEnter(object sender, MouseEventArgs e)
    {
        if (PaneClosed)
        {
            Popup.PlacementTarget = Settings;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "App Settings..";
        }
    }

    #endregion
    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        CollectionsFrame.Visibility = Visibility.Visible;
        BacktoHomeBtn.Visibility = Visibility.Visible;
        AppSettingsPage AppSettings = new();
        AppSettings.ThemeChanged += LoadTheme;
        CollectionsFrame.Content = AppSettings;
        HomePage.Visibility = Visibility.Collapsed;
        AddCollections.IsEnabled = false;
        addBtn.IsEnabled = false;
    }

    private void LoadTheme(object sender, string e)
    {
       
        LoadTheme(e);
    }

   


    public void LoadTheme(string theme)
    {
        var appResources = Application.Current.Resources;

        // Clear existing resource dictionaries
        appResources.MergedDictionaries.Clear();
        // Load and add the new theme resource dictionary
        var newTheme = new ResourceDictionary
        {
            Source = new Uri($"/Resource_Dictionaries/{theme}.xaml", UriKind.Relative)
        };
        appResources.MergedDictionaries.Add(newTheme);
        // Load and add control resources
        var controlResources = new ResourceDictionary
        {
            Source = new Uri("/Resource_Dictionaries/ControlsStyles.xaml", UriKind.Relative)
        };
        appResources.MergedDictionaries.Add(controlResources);

        
    }


}
