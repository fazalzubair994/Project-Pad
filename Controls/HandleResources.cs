using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using projectPad.Public_Classes;
namespace projectPad.Controls;

public static class  HandleResources
{
   public static string FolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Remem");

    public static  int FileNumber = 0;
    public static bool DarkModeTheme = true;
    public static bool ToDayTomorrowPageOpened = false;

    public static string CurrentFileName = "";
    public static string DirectoryName = "";
    public static string CollectionTitle = "";
    public static bool IsDirectoryOpens = false;

    static HandleResources()
    {
        // Ensure the directory exists
        if (!System.IO.Directory.Exists(FolderPath))
        {
            System.IO.Directory.CreateDirectory(FolderPath);
        }

        // Get the count of files in the directory
        string[] files = System.IO.Directory.GetFiles(FolderPath);
        FileNumber = files.Length + 1; // Increment the file number by one
    }

    static public  event EventHandler? NewNoteEventHandler;
    static public  event EventHandler? NewCollectionEventHandler;
    static public  event EventHandler? OpenCollectionFolder;
    public static void CreateNote(string ProvidedFileName, Note obj)
    {
        string data = Note.SerializeToJson(obj);
        if (!System.IO.File.Exists(ProvidedFileName))
        {
            string guid = Guid.NewGuid().ToString();
            string filename = "";
            if (IsDirectoryOpens)
                filename = $"{DirectoryName}/file_{guid}.txt";
            else
                filename = $"{FolderPath}/file_{guid}.txt";
            CurrentFileName = filename;
            System.IO.File.WriteAllText(filename, data);
            NewNoteEventHandler?.Invoke(null, EventArgs.Empty);
        }
        else
        {
            System.IO.File.WriteAllText(ProvidedFileName, data);
        }  

    }

    public static void CreateCollection(string Directory)
    {
        // this will save the Directory path 
        CurrentFileName = Directory;
        // and rise the event to load the newly created collection folder to the main page. 
        NewCollectionEventHandler?.Invoke(null, EventArgs.Empty);
    }

    public static void OpenCollections()
    {
        IsDirectoryOpens = true;
        OpenCollectionFolder?.Invoke(null, EventArgs.Empty);
    }

}
