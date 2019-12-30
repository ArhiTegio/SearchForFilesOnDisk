using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SearchForFilesOnDisk
{
    public static class ViewModel
    {
        static FileSearch fileSearch = new FileSearch();
        public static bool Pause { get => fileSearch.Pause1; set => fileSearch.Pause1 = value; }
        public static bool Cancel { get => fileSearch.Cancel1; set => fileSearch.Cancel1 = value; }
        public static string SymbolSearch { get => fileSearch.SearchingFileSymbol; set => fileSearch.SearchingFileSymbol = value; }
        public static string SearchingNameFile { get => fileSearch.SearchingFileName; set => fileSearch.SearchingFileName = value; }
        public static string FileName { get => fileSearch.FileName; }
        public static string Path { get => fileSearch.Path; set => fileSearch.Path = value; }
        public static Queue<FileSelect> PassFile { get => fileSearch.PassFilterSymbolInFile; set => fileSearch.PassFilterSymbolInFile = value; }

        public static void StartSearch() => (new Task(fileSearch.StartSeaching)).Start();

    }
}
