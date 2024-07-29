using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using projectPad.Public_Classes;
using projectPad.MessegeBoxes;
using System.Windows.Controls.Primitives;
using Microsoft.VisualBasic;
using Project_Pad.Controls;

namespace projectPad.Controls
{
    /// <summary>
    /// Interaction logic for ButtonTile.xaml
    /// </summary>
    public partial class ButtonTile : Border
    {
        public ButtonTile()
        {
            InitializeComponent();
        }
        public event EventHandler ThisNoteIsMoved;
        private Note _note; // will remember the saved data for the note
        public string FileName { get; set; } // will hold the actual file path of note
        public string Directory = ""; // if the type is not Note then this varaible will hold the Directory path for the collection 
        private bool _typeNote = true; // will remember that the type is note or collection
        public bool TypeNote
        {
            get { return _typeNote; }
            set
            {
                _typeNote = value;
                settype();
            }
        }

        public Note Note
        {
            get => _note;
            set
            {
                _note = value;
                UpdateUI();
            }
        }

        // when the this UI is loaded then it will read the file's data and 
        // add initialize the _note variables. so that the UpdateUI() will be call.
        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            LoadNoteFromFile();
            isCompleted.IsChecked = _note?.Is_Completed ?? false;
        }

        private void LoadNoteFromFile()
        {
            var json = File.ReadAllText(FileName); // Assuming FileName is the Note_Title
            _note = Note.DeserializeFromJson(json);
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_note == null) return;
            // if the saved Color is not defined then load the default color for the note
            if(_note.Note_Color != null)
                DateCreated.Foreground = Background = (SolidColorBrush)new BrushConverter().ConvertFromString(_note.Note_Color)!;
           

            // now load the other data of the note
            isCompleted.IsChecked = _note.Is_Completed;
            NoteText.Text = _note.Note_Text;
            NoteTitle.Text = _note.Note_Title;
            DateCreated.Content = _note.Created_Date;

            if (_note.Due_Date != null)
            {
                DateTime dt = (DateTime)_note.Due_Date;
                DueDate.Content = dt.ToString("dd MMM yyyy");
                OpenDatePicker.IsEnabled = false;
            }
            else
            {
                DueDateControl.Visibility = Visibility.Collapsed;
                OpenDatePicker.IsEnabled = true;
            }
            if (_note.Reminder_Date != null)
            {
                DateTime dt = (DateTime)_note.Reminder_Date;
                ReminderDate.Content = dt.ToString("dd MMM yyyy");
                AddReminder.IsEnabled = false;
            }
            else
            {
                ReminderControl.Visibility = Visibility.Collapsed;
                AddReminder.IsEnabled = true;
            }


            settype();
        }

        // this method will change the Note's UI according the TypeNote property, 
        private void settype()
        {
            if(HandleResources.ToDayTomorrowPageOpened)
            {
                OpenPanel.Visibility = DelteBtn.Visibility = OpenDatePicker.Visibility  = Visibility.Collapsed;
                return;
            }
            if (_note == null) return;

            // if the typeNote is not true, means this UI is for Collection
            if (!_typeNote)
            {
                // then change the TypeName Icon to Folder
                typeName.Content = "\uEc50";

                if (_note.Note_Color != null)
                    DateCreated.Foreground = Background = (SolidColorBrush)new BrushConverter().ConvertFromString(_note.Note_Color)!;
               

                // we need to collapsed the some UI elements
                MoveToBtn.Visibility = AddReminder.Visibility = OpenDatePicker.Visibility = DueandReminderPanel.Visibility = isCompleted.Visibility = StatColor.Visibility = Visibility.Collapsed;
                // show the OpenFolder button 
                OpenBtn.Visibility = Visibility.Visible;

                // set the Tooltips for the buttons
                DelteBtn.ToolTip = "Delete this Collection";
                EditBtn.ToolTip = "Edit Collection's Title & Description";
                OpenBtn.ToolTip = "Open this Collection";
            }

            else
            {
                DelteBtn.ToolTip = "Delete this Task...";
                EditBtn.ToolTip = "Edit this Task...";
            }
        }

        #region DeleteButton Code
        public event EventHandler? DeleteNote;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DeleteNote?.Invoke(this, EventArgs.Empty);
        }
        #endregion
       

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            NoteWindow NW = new();
            NW.FileName = FileName; 
            NW.ShowDialog();
            if (NW.ShouldUpdate)
            {
                LoadNoteFromFile();
            }
        }

       

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(_note.Note_Color != null)
                ApplyShadow((Color)ColorConverter.ConvertFromString(_note.Note_Color));
            StatColor.Visibility = Visibility.Collapsed;
            DueandReminderPanel.Visibility = Visibility.Collapsed;
        }

        private void ApplyShadow(Color c)
        {
            DropShadowEffect shadowEffect = new DropShadowEffect
            {
                Color = c,
                BlurRadius = 10,
                ShadowDepth = 2,
                Opacity = 0.8
            };
            this.Effect = shadowEffect;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplyShadow(Colors.Black);
            StatColor.Visibility = Visibility.Visible;
            DueandReminderPanel.Visibility = Visibility.Visible;

        }

        private void isCompleted_Click(object sender, RoutedEventArgs e)
        {
            _note.Is_Completed = isCompleted.IsChecked == true ? true : false;
            updateFile();
        }


        private void updateFile()
        {
            HandleResources.CreateNote(FileName, _note);
        }

        private void RemoveShadow()
        {
            this.Effect = null;
        }

        

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            HandleResources.DirectoryName = Directory;
            HandleResources.CollectionTitle = "./Home-Page/" + _note.Note_Title;
            HandleResources.OpenCollections();
        }

        private bool isPanelOpened = false;
        private void OpenPanel_Click(object sender, RoutedEventArgs e)
        {
            if (isPanelOpened)
            {
                OpenPanel.Content = "\ue70d";
                AnimateHeight(130, 0, .3);
                isPanelOpened = false;
            }
            else
            {
                OpenPanel.Content = "\ue70e";
                AnimateHeight(0, 130, .3);
                isPanelOpened = true;
            }
        }

        private void AnimateHeight(double from, double to, double durationInSeconds)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(durationInSeconds)),
            };
            OtherUIPanel.BeginAnimation(HeightProperty, animation);
        }

        private void ColorPickerbtn_Click(object sender, RoutedEventArgs e)
        {
            colorPickerPopup.IsOpen = true;
        }

       

        private void OpenDatePicker_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ColorPicker_ColorSelected(object sender, Brush e)
        {
            SolidColorBrush brush = (SolidColorBrush)e;
            DateCreated.Foreground = Background = brush;
            _note.Note_Color = brush.ToString();
            colorPickerPopup.IsOpen = false;
            updateFile();
        }

        bool DueDateSelection = false;
        private void AddReminder_Click(object sender, RoutedEventArgs e)
        {
            DatePickerPopup.IsOpen = true;
            DueDateSelection = false;
        }
        private void OpenDatePicker_Click_1(object sender, RoutedEventArgs e)
        {
            DatePickerPopup.IsOpen = true;
            DueDateSelection = true;
           
        }

        private void CustomDatePicker_DateSelected(object sender, DateTime e)
        {
            DatePickerPopup.IsOpen = false;
            if(DueDateSelection)
            {
                DueDate.Content = e.ToString("dd MMM yyyy");
                DueDateSelection = false;

                DueDateControl.Visibility = Visibility.Visible;
                OpenDatePicker.IsEnabled = false;
                updateFile();

                _note.Due_Date = e;
                updateFile();

            }
            else
            {
                ReminderDate.Content = e.ToString("dd MMM yyyy");
                DueDateSelection = false;
                ReminderControl.Visibility = Visibility.Visible;
                AddReminder.IsEnabled = false;
                _note.Reminder_Date = e;
                updateFile();
            }

        }

        private void OpenDatePicker_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Popup.PlacementTarget = OpenDatePicker;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Add Due Date...";
        }

        private void OpenDatePicker_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Popup.IsOpen = false;
        }

        private void ColorPickerbtn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Popup.PlacementTarget = ColorPickerbtn;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Add Color....";
        }

        private void AddReminder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Popup.PlacementTarget = AddReminder;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Add Reminder....";
        }

        private void DeleteDueDate_Click(object sender, RoutedEventArgs e)
        {
            DueDateControl.Visibility = Visibility.Collapsed;
            OpenDatePicker.IsEnabled = true;
            _note.Due_Date = null;
            updateFile();
        }

        private void DeleteReminder_Click(object sender, RoutedEventArgs e)
        {
            ReminderControl.Visibility = Visibility.Collapsed;
            AddReminder.IsEnabled = true;
            _note.Reminder_Date = null;
            updateFile();
        }

        private void MoveToBtn_Click(object sender, RoutedEventArgs e)
        {
            MoveControl MC = new();
            MC.NoteIsMoved += MoveControl_NoteIsMoved!;
            MovePopup.Child = MC;
            MC.NoteFilePath = FileName;
            MovePopup.IsOpen = true;
        }

        private void MoveControl_NoteIsMoved(object sender, EventArgs e)
        {
            ThisNoteIsMoved?.Invoke(this, e);
        }
    }
}
