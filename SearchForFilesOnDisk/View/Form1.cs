using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchForFilesOnDisk
{
    public partial class Form1 : Form
    {
        object locked_ = new object();
        static Task task;

        public Form1()
        {
            InitializeComponent();
            textBox1.Text = Properties.Settings.Default.Path;
            textBox2.Text = Properties.Settings.Default.FileName;
            textBox3.Text = Properties.Settings.Default.SymbolInFile;
            ChagingName(toolStripStatusLabel1.Text);
        }


        public void ChagingName(string text)
        {
            treeView1.ImageList = new ImageList();
            treeView1.ImageList.ImageSize = new Size(24, 24);
            treeView1.ImageList.Images.Add("0", Image.FromFile("Properties\\0.png"));
            treeView1.ImageList.Images.Add("1", Image.FromFile("Properties\\1.png"));
            (new Task(AsyncChangePos)).Start();
            (new Task(AsyncChangeTreeView)).Start();
        }

        public void AsyncChangeTreeView()
        {
            var file = new FileSelect("", "");
            bool root = false;
            while (true)
            {
                if (ViewModel.PassFile.Count != 0)
                {
                    file = ViewModel.PassFile.Dequeue();
                    var path = file.Path.Split('\\');
                    var node = treeView1.Nodes;

                    foreach (var e in path)
                    {
                        if (treeView1.Nodes.Count == 0 && !root)
                        {
                            root = !root;
                            BeginInvoke(new Action(() => treeView1.Nodes.Add(e, e, 0))).AsyncWaitHandle.WaitOne();
                            node = node.Find(e, true)[0].Nodes;
                        }
                        else
                        {
                            if (!node.ContainsKey(e))
                            {
                                BeginInvoke(new Action(() => node.Add(e, e, 0))).AsyncWaitHandle.WaitOne();
                                node = node.Find(e, true)[0].Nodes;
                            }
                            else
                            {
                                node = node.Find(e, true)[0].Nodes;
                            }
                        }


                    }
                    var list = file.Name.Split('\\');
                    BeginInvoke(new Action(() => node.Add(file.Name, list[list.Length - 1], 1)));
                }
                else
                {
                    Thread.Sleep(200);
                }
            }

        }

        public void AsyncChangePos()
        {
            while (true)
            {
                if (ViewModel.FileName != toolStripStatusLabel1.Text)
                {
                    Thread.Sleep(1);
                    BeginInvoke(new Action(() => toolStripStatusLabel1.Text = ViewModel.FileName));
                }
                else
                    Thread.Sleep(50);
            }
        }

        private void splitter1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if (FBD.ShowDialog() == DialogResult.OK) textBox1.Text = ViewModel.Path = FBD.SelectedPath;            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                treeView1.Nodes.Clear();
                ViewModel.StartSearch();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ViewModel.Cancel = !ViewModel.Cancel;
            
            button3.BackColor = Color.Empty;
            ViewModel.Pause = false;
            ViewModel.PassFile.Clear();
            treeView1.Nodes.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ViewModel.Pause = !ViewModel.Pause;

            if (ViewModel.Pause) button3.BackColor = Color.Green;
            else button3.BackColor = Color.Empty;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Path = ViewModel.Path = textBox1.Text;
            Properties.Settings.Default.Save();
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.FileName = ViewModel.SearchingNameFile = textBox2.Text;
            Properties.Settings.Default.Save();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SymbolInFile = ViewModel.SymbolSearch = textBox3.Text;
            Properties.Settings.Default.Save();
        }
        
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }    
}
