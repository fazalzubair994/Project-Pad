using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using projectPad.Controls;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using projectPad.Public_Classes;

namespace projectPad.MessegeBoxes;

/// <summary>
/// Interaction logic for NoteWindow.xaml
/// </summary>
public partial class NoteWindow : Window
{
    public NoteWindow()
    {
        InitializeComponent();
    }
    public bool IsCollection = false; // check that user want to generate a collection or not. 
    public string FileName = "";
    public bool IsSaved = false; // used to remember that we need to Update the UI for the changes.
    public bool ShouldUpdate = false;
    public Note ClassNote = new();

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if(!IsCollection)
        {
            //if this window is not for the collection
            // then we need to change some UI Elements 
            saveBtn.Visibility = Visibility.Visible;
            createCollectionBtn.Visibility = Visibility.Collapsed;
            
            // if the provided fileName is not exists then 
            if (!System.IO.File.Exists(FileName))
            {
                // load a new window for the user to add new Note
                SetDateandTime();
                titleBox.Focus();
            }
            else
            {
                // is load the data of existing file. 
                IsSaved = true;
                ClassNote = Note.DeserializeFromJson(System.IO.File.ReadAllText(FileName));
                titleBox.Text = ClassNote.Note_Title;
                noteBox.Text = ClassNote.Note_Text;
                StartTime.Content = ClassNote.Created_Date;
                noteBox.Focus();
            }

        }
        else
        {
            titleBox.Placeholder = "Collection Name....";
            noteBox.Placeholder = "Small Description.....";
            this.Width = 400;
            this.Height = 400;
            saveBtn.Visibility = Visibility.Collapsed;
            createCollectionBtn.Visibility = Visibility.Visible;
            SetDateandTime();
            titleBox.Focus();
        }

    }
    // SetDateandTime() method will add a new Date and Time for the New Note. 
    private void SetDateandTime()
    {
        string date = DateTime.Now.ToString("dd MMMM yyyy");
        string time = DateTime.Now.ToString("hh:mm tt");
        string formattedDateTime = $"{date}          {time}";
        StartTime.Content = formattedDateTime;
    }

    // user has the option to cancel the creation of note or collection
    private void cancelBtn_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    // user also can drag this widow
    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        this.DragMove();
    }
    
    // if the clicks on the saveBtn
    private void saveBtn_Click(object sender, RoutedEventArgs e)
    {

        // then we will store the new note data and send to the HandleResources file to create a new note or update the Existing file 
        HandleResources.CreateNote(FileName, CreateData());
        if(IsSaved)
        {
            ShouldUpdate = true;
        }
        this.Close();
    }

    private Note CreateData()
    {
        Note Obj = new();
        Obj.Note_Title = titleBox.Text;
        Obj.Note_Text = noteBox.Text;
        Obj.Is_Completed = false;
        Obj.Created_Date = StartTime.Content.ToString();
        Obj.Due_Date = ClassNote.Due_Date;
        Obj.Reminder_Date = ClassNote.Reminder_Date;
        Obj.Note_Color = ClassNote.Note_Color;
        return Obj;
    }


    private void createCollectionBtn_Click(object sender, RoutedEventArgs e)
    {
        string path = HandleResources.FolderPath + "/" + titleBox.Text;
        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
            Note Temp = CreateData();

            string data = Note.SerializeToJson(Temp);
            System.IO.File.WriteAllText(path+ "/" + "ReadMe.bin", data);
            
            // now at the end to trigger the Collections Event we need to pass the path to the HandleResources. 
            HandleResources.CreateCollection(path);
            this.Close();

        }
        else
        {
            MessageBox.Show("Sory the Collection name is Already added, please change the Collection Name...");
        }
    }
}
