using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SearchForFilesOnDisk
{
    public class FileSearch
    {
        string path = "C:\\";
        string searchingFileName = "";
        string searchingFileSymbol = "";

        string fileName = "";

        Queue<string> foundingPaths = new Queue<string>();
        Queue<FileSelect> foundingFiles = new Queue<FileSelect>();
        Queue<FileSelect> passFilterName = new Queue<FileSelect>();
        Queue<FileSelect> passFilterSymbolInFile = new Queue<FileSelect>();

        bool mathodFoundPaths = false;
        bool mathodFoundFiles = false;
        bool mathodFilterName = false;
        bool Done = false;

        bool Pause = false;

        bool Cancel = false;

        public FileSearch()
        {
            this.path = Properties.Settings.Default.Path;
            this.searchingFileName = Properties.Settings.Default.FileName;
            this.searchingFileSymbol = Properties.Settings.Default.SymbolInFile;
        }

        public string Path { get => path; set => Properties.Settings.Default.Path = path = value; }
        public string SearchingFileName { get => searchingFileName; set => Properties.Settings.Default.FileName = searchingFileName = value; }
        public string SearchingFileSymbol { get => searchingFileSymbol; set => Properties.Settings.Default.SymbolInFile = searchingFileSymbol = value; }
        public string FileName { get => fileName; set => fileName = value; }
        internal Queue<FileSelect> PassFilterSymbolInFile { get => passFilterSymbolInFile; set => passFilterSymbolInFile = value; }
        public bool Cancel1 { get => Cancel; set => Cancel = value; }
        public bool Pause1 { get => Pause; set => Pause = value; }

        public void StartSeaching()
        {
            mathodFoundPaths = true;
            mathodFoundFiles = true;
            mathodFilterName = true;
            var mass = (new Task[] { new Task(FoundPaths), new Task(FoundFiles), new Task(FilterName), new Task(FilterSymbolInFile) });
            foreach (var e in mass)
                e.Start();

            Task.WaitAll(mass);
        }
        
        private void FoundPaths()
        {
            foundingPaths.Clear();
            foundingPaths.Enqueue(Path);
            var folders = Directory.EnumerateDirectories(Path);
            var queue = new Queue<string>(folders);
            var path = "";

            while(queue.Count != 0)
            {
                if (Pause1)
                {
                    Thread.SpinWait(300);
                    if (Cancel1)
                        break;
                }
                else
                {
                    if (Cancel1)
                        break;

                    path = queue.Dequeue();
                    try
                    {
                        foundingPaths.Enqueue(path);
                        folders = Directory.EnumerateDirectories(path);
                        foreach (var e in folders)
                        {
                            while (Pause1)
                            {
                                Thread.SpinWait(300);
                            }
                            if (Cancel1)
                                break;
                            queue.Enqueue(e);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                Thread.Sleep(TimeSpan.FromMilliseconds(0.1));
            }

            mathodFoundPaths = false;
        }

        private void FoundFiles()
        {
            foundingFiles.Clear();
            IEnumerable<string> files = default(IEnumerable<string>);
            var folder = "";
            try
            {
                while (foundingPaths.Count != 0 || mathodFoundPaths)
                {
                    if (Pause1)
                    {
                        Thread.SpinWait(300);
                        if (Cancel1)
                            break;
                    }
                    else
                    {
                        if (Cancel1)
                            break;
                        if (foundingPaths.Count != 0)
                        {
                            folder = foundingPaths.Dequeue();
                            if (folder != null)
                            {
                                try
                                {
                                    files = Directory.EnumerateFiles(folder);
                                    foreach (var e in files)
                                    {
                                        FileName = e;
                                        while (Pause1)
                                        {
                                            Thread.SpinWait(300);
                                        }
                                        if (Cancel1)
                                            break;

                                        foundingFiles.Enqueue(new FileSelect(e, folder));
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                        }
                    }
                    Thread.Sleep(TimeSpan.FromMilliseconds(0.1));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally { mathodFoundFiles = false; }
            fileName = "";
        }

        private void FilterName()
        {
            passFilterName.Clear();
            var fileProcessed = new FileSelect("", "");
            var name = new string[0];
            try
            {
                while(mathodFoundFiles || foundingFiles.Count != 0 || mathodFoundPaths)
                {
                    if (Pause1)
                    {
                        Thread.SpinWait(300);
                        if (Cancel1) break;
                    }
                    else
                    {
                        if (Cancel1) break;
                        if (foundingFiles.Count != 0)
                        {

                            fileProcessed = foundingFiles.Dequeue();
                            if (fileProcessed != null)
                                if (searchingFileName != "")
                                {
                                    name = fileProcessed.Name.Split('\\');
                                    if (name.Length > 0 && Regex.IsMatch(name[name.Length - 1], searchingFileName))
                                        passFilterName.Enqueue(fileProcessed);
                                }
                                else                                
                                    passFilterName.Enqueue(fileProcessed);

                        }
                    }
                    Thread.Sleep(TimeSpan.FromMilliseconds(0.1));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                mathodFilterName = false;
            }
            fileName = "";
        }

        private void FilterSymbolInFile()
        {
            PassFilterSymbolInFile.Clear();
            var fileProcessed = new FileSelect("", "");

            try
            {
                while (mathodFilterName || passFilterName.Count != 0 || mathodFoundFiles || mathodFoundPaths)
                {
                    if (Pause1)
                    {
                        Thread.Sleep(300);
                        if (Cancel1)
                            break;
                    }
                    else
                    {
                        if (Cancel1)
                            break;
                        if (passFilterName.Count != 0)
                        {
                            fileProcessed = passFilterName.Dequeue();
                            var text = "";
                            if (fileProcessed != null)
                            {
                                text = "";
                                try
                                {
                                    text = File.ReadAllText(fileProcessed.Name);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                if (text != "")
                                {
                                    if (searchingFileSymbol != "")
                                    {
                                        if (text.Length >= searchingFileSymbol.Length && Regex.IsMatch(text, searchingFileSymbol))
                                            PassFilterSymbolInFile.Enqueue(fileProcessed);
                                    }
                                    else
                                    {
                                        PassFilterSymbolInFile.Enqueue(fileProcessed);
                                    }
                                }
                            }
                        }
                    }
                    Thread.Sleep(TimeSpan.FromMilliseconds(0.1));
                }
            }
            catch (Exception e)
            {
               Console.WriteLine(e.Message);
            }
            finally
            {

            }

            Cancel1 = false;
            fileName = "Поиск завершен.";
        }
    }

    public class FileSelect
    {
        string name;
        string path;

        public FileSelect(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public string Name { get => name; set => name = value; }
        public string Path { get => path; set => path = value; }
    }
}
