using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SearchForFilesOnDisk
{
    class FileSearch
    {
        string path;
        string searchingFileName;
        string searchingFileSymbol;

        string fileName;

        Queue<string> foundingPaths = new Queue<string>();
        Queue<File> foundingFiles = new Queue<File>();
        Queue<File> passFilterName = new Queue<File>();
        Queue<File> passFilterSymbolInFile = new Queue<File>();

        bool mathodFoundPaths = false;
        bool mathodFoundFiles = false;
        bool mathodFilterName = false;
        bool Done = false;

        bool Pause = false;

        bool Cancel = false;

        public string Path { get => path; set => path = value; }
        public string SearchingFileName { get => searchingFileName; set => searchingFileName = value; }
        public string SearchingFileSymbol { get => searchingFileSymbol; set => searchingFileSymbol = value; }
        public string FileName { get => fileName; set => fileName = value; }

        public void FoundPaths()
        {
            mathodFoundPaths = true;
            foundingPaths.Clear();
            foundingPaths.Enqueue(Path);
            var folders = Directory.EnumerateDirectories(Path);
            var queue = new Queue<string>(folders);
            var path = "";

            while(queue.Count != 0)
            {
                if (Pause)                
                    Thread.Sleep(300);                
                else
                {
                    if (Cancel)
                        break;
                    {
                        path = queue.Dequeue();
                        try
                        {
                            foundingPaths.Enqueue(path);
                            folders = Directory.EnumerateDirectories(path);
                            foreach (var e in folders)
                                queue.Enqueue(e);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            }

            mathodFoundPaths = false;
        }

        public void FoundFiles()
        {
            mathodFoundFiles = true;
            foundingFiles.Clear();
            IEnumerable<string> files = default(IEnumerable<string>);
            var folder = "";
            try
            {
                while (foundingPaths.Count != 0 || mathodFoundFiles)
                {
                    if (foundingPaths.Count != 0)
                    {
                        folder = foundingPaths.Dequeue();
                        if (folder != null)
                        {
                            files = Directory.EnumerateFiles(folder);
                            foreach (var e in files)
                                foundingFiles.Enqueue(new File(e, folder));
                        }
                    }
                }
            }
            catch (Exception e)
            { 
            
            }
            finally { mathodFoundFiles = false; }
        }

        public void FilterName()
        {

        }
    }

    class File
    {
        string name;
        string path;

        public File(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public string Name { get => name; set => name = value; }
        public string Path { get => path; set => path = value; }
    }
}
