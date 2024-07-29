using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Windows.Media; // Use System.Windows.Media.Color if you're using WPF

namespace projectPad.Public_Classes
{
    [Serializable]
    public class Note
    {
        // Title of the note
        public string Note_Title { get; set; }

        // Actual content of the note
        public string Note_Text { get; set; }

        // Status to check if the note/task is completed
        public bool Is_Completed { get; set; }

        // Type of note (e.g., whether it is a text note or other type)
        public bool Is_Type_Note { get; set; }

        // Deadline to complete the note/task
        public DateTime? Due_Date { get; set; }
        public DateTime? Reminder_Date { get; set; }

        // Date the note was created
        public string Created_Date { get; set; }

        
        public string Note_Color { get; set; }

        // Default constructor
        public Note() { }

        // Parameterized constructor
        public Note(string title, string text, bool isCompleted, DateTime? ReminderDate, DateTime? dueDate, string createdDate, bool isTypeNote, string noteColor)
        {
            Note_Title = title;
            Note_Text = text;
            Is_Completed = isCompleted;
            Due_Date = dueDate;
            Reminder_Date = ReminderDate;
            Created_Date = createdDate;
            Is_Type_Note = isTypeNote;
            Note_Color = noteColor;
        }

        // Serialize to JSON
        public static string SerializeToJson(Note note)
        {
            using (var memoryStream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(Note));
                serializer.WriteObject(memoryStream, note);
                return System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        // Deserialize from JSON
        public static Note DeserializeFromJson(string json)
        {
            using (var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof(Note));
                return (Note)serializer.ReadObject(memoryStream);
            }
        }
    }
}
