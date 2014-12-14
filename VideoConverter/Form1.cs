using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace VideoConverter
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 2013-10-23
        /// FLV  added to input files(it works only in  choosing manual files method not in automatic method).
        /// </summary>
        string _DirPath = "";
        int _Index;
        DataTable DTable = new DataTable();
        DataColumn col1 = new DataColumn("#");
        DataColumn col2 = new DataColumn("FileName");
        DataColumn col3 = new DataColumn("Status");
        int RowIndex = 0;
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //openFileDialog1.Filter = "Avi files (*.avi)|*.avi|All files (*.*)|*.*";
            //openFileDialog1.Multiselect = true;
            //openFileDialog1.ShowDialog();


            folderBrowserDialog2.ShowDialog();
            //if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            //{
            string selectedPath = folderBrowserDialog2.SelectedPath;
            textBox1.Text = selectedPath + "\\";
            // _DirPath = selectedPath + "\\";
        }
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            _Index = -1;
            //axVLCPlugin21.playlist.items.clear();
            //axVLCPlugin21.playlist.add("file:///" + openFileDialog1.FileName, "dfccdcdcd", null);
            //axVLCPlugin21.playlist.playItem(0);


            DataTable DTable = new DataTable();

            DataColumn col1 = new DataColumn("#");
            DataColumn col2 = new DataColumn("FileName");
            DataColumn col3 = new DataColumn("Status");

            DTable.Columns.Add(col1);
            DTable.Columns.Add(col2);
            DTable.Columns.Add(col3);

            int RowIndex = 0;

            foreach (string item in openFileDialog1.FileNames)
            {
               // textBox1.Text = Path.GetDirectoryName(item) + "\\";

                RowIndex++;
                DataRow row = DTable.NewRow();
                row[col1] = RowIndex.ToString();
                row[col2] =item;
                row[col3] = "Waiting";
                DTable.Rows.Add(row);
            }

            dataGridView1.DataSource = DTable;
            //if (dataGridView1.Rows.Count > 0)
            //{
            //    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Selected = true;
            //    dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
            //}
            dataGridView1.Columns[0].Width = 50;
            dataGridView1.Columns[1].Width = 700;
            dataGridView1.Columns[2].Width = 85;

        }
        private void button2_Click(object sender, EventArgs e)
        {
            button2.ForeColor = Color.White;
            button2.Text = "Started";
            button2.BackColor = Color.Red;


            progressBar1.Value = 0;
            progressBar2.Value = 0;
            label1.Text = "0%";
            label2.Text = "0%";
            richTextBox1.Text = "";
            Process proc = new Process(); if (Environment.Is64BitOperatingSystem)
            {
                proc.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg64";
            }
            else
            {
                proc.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg32";
            }
            // -i 2.avi  -r 25 -b 3000k  -ar 48000 -ab 192k -async 1   -flags +ildct+ilme -y  1.mp4
            string InterLaced = "-flags +ildct+ilme";
            if (radioButton1.Checked)
            {
                InterLaced = "";
            }

            if (_Index >= 0 && _Index < dataGridView1.RowCount)
            {
                string FilePath = dataGridView1.Rows[_Index].Cells[1].Value.ToString();

                string sss = Path.GetDirectoryName(FilePath.Replace("\\\\", "\\"));
                //    DirectoryInfo Dir = new DirectoryInfo);
                string[] Paths = FilePath.Replace(textBox1.Text.Trim(), "").Split('\\');
                string NewPath = "";
                for (int i = 0; i < Paths.Length - 1; i++)
                {
                    NewPath += "\\" + Paths[i];
                }
                DirectoryInfo Dir = new DirectoryInfo(textBox4.Text + NewPath);

                string DestFilePath = Dir.ToString() + "\\" + Path.GetFileNameWithoutExtension(dataGridView1.Rows[_Index].Cells[1].Value.ToString());

                if (radioButton4.Checked)
                {
                    DestFilePath = textBox4.Text.ToString() + "\\" + Path.GetFileNameWithoutExtension(dataGridView1.Rows[_Index].Cells[1].Value.ToString());
                }
                else
                {
                    if (!Dir.Exists)
                    {
                        Dir.Create();
                    }
                    DestFilePath = Dir.ToString() + "\\" + Path.GetFileNameWithoutExtension(dataGridView1.Rows[_Index].Cells[1].Value.ToString());
                }

                proc.StartInfo.Arguments = "-i " + "\"" + FilePath + "\"" + "  -r 25 -b " + textBox2.Text.Trim() + "k  -ar 48000 -ab 192k -async 1 " + InterLaced + "   -y  " + "\"" + DestFilePath + ".mp4" + "\"";
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.EnableRaisingEvents = true;
                if (!checkBox1.Checked)
                {
                    proc.Exited += new EventHandler(myProcess_Exited);
                }
                dataGridView1.Rows[_Index].Cells[2].Value = "In Proccess";
                dataGridView1.ClearSelection();
                dataGridView1.Rows[_Index].Selected = true;
                dataGridView1.FirstDisplayedScrollingRowIndex = _Index;
                // button2.Text = "Stop Convert";
              //  button2.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
               // button1.Enabled = false;
              //  button3.Enabled = false;
                textBox4.Enabled = false;
              //  button6.Enabled = true;


                if (!proc.Start())
                {
                    richTextBox1.Text += " \n" + "Error starting";
                    dataGridView1.Rows[_Index].Cells[2].Value = "Error";
                    _Index = QeueProcess();
                    if (_Index == -1)
                    {
                        button2.ForeColor = Color.White;
                        button2.Text = "Start";
                        button2.BackColor = Color.Navy;

                        button6.ForeColor = Color.White;
                        button6.Text = "Stop";
                        button6.BackColor = Color.Navy;


                        button1.Enabled = true;
                        //  button3.Enabled = true;
                        button2.Enabled = true;
                    }
                    else
                    {
                        button2_Click(new object(), new EventArgs());
                    }

                    return;
                }

                proc.PriorityClass = ProcessPriorityClass.RealTime;
                // button1.Enabled = false;
                //button3.Enabled = false;
                //button2.Enabled = false;
                proc.Start();
                StreamReader reader = proc.StandardError;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (richTextBox1.Lines.Length > 5)
                    {
                        richTextBox1.Text = "";
                    }

                    FindDuration(line, "1");
                    richTextBox1.Text += (line) + " \n";
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                    Application.DoEvents();
                }
              //  progressBar1.Value = progressBar1.Maximum;
               // label1.Text = "100%";
              //  proc.Close();

                #region LowRes
                if (checkBox1.Checked)
                {
                    Process proc2 = new Process();
                    if (Environment.Is64BitOperatingSystem)
                    {
                        proc2.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg64";
                    }
                    else
                    {
                        proc2.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg32";
                    }
                    //   proc2.StartInfo.FileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\ffmpeg";
                    proc2.StartInfo.Arguments = "-i " + "\"" + FilePath + "\"" + " -vf \"movie=logo.png [watermark]; [in][watermark] overlay=10:main_h-overlay_h-10 [out]\"   -b 200k -s 320x240   -ar 48000 -ab 192k -async 1 " + InterLaced + "   -y  " + "\"" + DestFilePath + "_Low.mp4" + "\"";
                    proc2.StartInfo.RedirectStandardError = true;
                    proc2.StartInfo.UseShellExecute = false;
                    proc2.StartInfo.CreateNoWindow = true;
                    proc2.EnableRaisingEvents = true;
                    proc2.Exited += new EventHandler(myProcess_Exited);
                    dataGridView1.Rows[_Index].Cells[2].Value = "In Proccess";
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[_Index].Selected = true;
                    dataGridView1.FirstDisplayedScrollingRowIndex = _Index;
                    // button2.Text = "Stop Convert";
                    button2.Enabled = false;
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    button1.Enabled = false;
                    button3.Enabled = false;
                    textBox4.Enabled = false;
                    button6.Enabled = true;


                    if (!proc2.Start())
                    {
                        richTextBox1.Text += " \n" + "Error starting";
                        dataGridView1.Rows[_Index].Cells[2].Value = "Error Low";
                        _Index = QeueProcess();
                        if (_Index == -1)
                        {
                            button1.Enabled = true;
                            //  button3.Enabled = true;
                            button2.Enabled = true;

                            button2.ForeColor = Color.White;
                            button2.Text = "Start";
                            button2.BackColor = Color.Navy;

                            button6.ForeColor = Color.White;
                            button6.Text = "Stop";
                            button6.BackColor = Color.Navy;
                        }
                        else
                        {
                            button2_Click(new object(), new EventArgs());
                        }

                        return;
                    }

                    proc2.PriorityClass = ProcessPriorityClass.RealTime;
                    // button1.Enabled = false;
                    //button3.Enabled = false;
                    //button2.Enabled = false;

                    reader = proc2.StandardError;
                    line = "";
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (richTextBox1.Lines.Length > 15)
                        {
                            richTextBox1.Text = "";
                        }

                        FindDuration(line, "2");
                        richTextBox1.Text += (line) + " \n";
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        richTextBox1.ScrollToCaret();
                        Application.DoEvents();
                    }
                    proc2.Close();
                    progressBar2.Value = progressBar2.Maximum;
                    label2.Text = "100%";
                } 
                #endregion

            }
            else
            {
                _Index = QeueProcess();
                if (_Index == -1)
                {
                    button1.Enabled = true;
                    //  button3.Enabled = true;
                    button2.Enabled = true;

                    button2.ForeColor = Color.White;
                    button2.Text = "Start";
                    button2.BackColor = Color.Navy;

                    button6.ForeColor = Color.White;
                    button6.Text = "Stop";
                    button6.BackColor = Color.Navy;
                }
                else
                {
                    button2_Click(new object(), new EventArgs());
                }
            }
        }
        protected int QeueProcess()
        {
            int Index = -1;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[2].Value.ToString() == "Waiting")
                {
                    Index = i;
                    return Index;
                }
            }
            return Index;
        }
        private void myProcess_Exited(object sender, System.EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate()
                 {
                     richTextBox1.Text += (_Index+1) + " Exited \n";
                     richTextBox1.SelectionStart = richTextBox1.Text.Length;
                     richTextBox1.ScrollToCaret();
                     Application.DoEvents();

                         if (dataGridView1.Rows[_Index].Cells[2].Value == "Skipped")
                         {

                         }
                         else
                         {
                             dataGridView1.Rows[_Index].Cells[2].Value = "Done";
                         }

                         _Index = QeueProcess();
                         if (_Index == -1)
                         {
                             // button2.Text = "Start Convert";
                             button2.Enabled = true;
                             textBox1.Enabled = true;
                             textBox2.Enabled = true;
                             button1.Enabled = true;
                             button3.Enabled = true;
                             textBox4.Enabled = true;
                         }
                         else
                         {
                             button2_Click(new object(), new EventArgs());
                         }
                     
                    
                 }));
        }
        protected void FindDuration(string Str, string ProgressControl)
        {
            string TimeCode = "";
            if (Str.Contains("Duration:"))
            {
                TimeCode = Str.Substring(Str.IndexOf("Duration: "), 21).Replace("Duration: ", "").Trim();
                string[] Times = TimeCode.Split('.')[0].Split(':');
                double Frames = double.Parse(Times[0].ToString()) * (3600) * (25) +
                    double.Parse(Times[1].ToString()) * (60) * (25) +
                    double.Parse(Times[2].ToString()) * (25);
                if (ProgressControl == "1")
                {
                    progressBar1.Maximum = int.Parse(Frames.ToString());
                }
                else
                {
                    if (ProgressControl == "2")
                    {
                        progressBar2.Maximum = int.Parse(Frames.ToString());
                    }
                }
                // label2.Text = Frames.ToString();

            }
            if (Str.Contains("time="))
            {
                try
                {
                    string CurTime = "";
                    CurTime = Str.Substring(Str.IndexOf("time="), 16).Replace("time=", "").Trim();
                    string[] CTimes = CurTime.Split('.')[0].Split(':');
                    double CurFrame = double.Parse(CTimes[0].ToString()) * (3600) * (25) +
                        double.Parse(CTimes[1].ToString()) * (60) * (25) +
                        double.Parse(CTimes[2].ToString()) * (25);

                    if (ProgressControl == "1")
                    {
                        progressBar1.Value = int.Parse(CurFrame.ToString());

                        label1.Text = ((progressBar1.Value * 100) / progressBar1.Maximum).ToString() + "%";
                    }
                    else
                    {
                        if (ProgressControl == "2")
                        {
                            progressBar2.Value = int.Parse(CurFrame.ToString());

                            label2.Text = ((progressBar2.Value * 100) / progressBar2.Maximum).ToString() + "%";
                        }
                    }

                    //label3.Text = CurFrame.ToString();
                    Application.DoEvents();
                }
                catch
                {


                }

            }
            if (Str.Contains("fps="))
            {

                string Speed = "";

                Speed = Str.Substring(Str.IndexOf("fps="), 8).Replace("fps=", "").Trim();

                label4.Text = "Speed: " + (float.Parse(Speed) / 25).ToString() + " X ";
                Application.DoEvents();


            }




        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //ProgressBar Pr = new ProgressBar();
            //Pr.Value = 10;
            //Pr.Location = new Point(10, 100);
            //Pr.Name = "Pr1";
            //this.Controls.Add(Pr);

        }
        private void button3_Click_1(object sender, EventArgs e)
        {
            ProgressBar Pr = (ProgressBar)this.Controls["Pr1"];
            Pr.Value += 10;
        }
        private void button3_Click_2(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();

            string selectedPath = folderBrowserDialog1.SelectedPath;
            textBox4.Text = selectedPath + "\\";
            _DirPath = selectedPath + "\\";


        }
        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;

        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                DTable = new DataTable();
                col1 = new DataColumn("#");
                col2 = new DataColumn("FileName");
                col3 = new DataColumn("Status");
                DTable.Columns.Add(col1);
                DTable.Columns.Add(col2);
                DTable.Columns.Add(col3);
                RowIndex = 0;
                foreach (string f in Directory.GetFiles(textBox1.Text.Trim(), "*.avi"))
                {
                    RowIndex++;
                    DataRow row = DTable.NewRow();
                    row[col1] = RowIndex.ToString();
                    row[col2] = f;
                    row[col3] = "Waiting";
                    DTable.Rows.Add(row);

                }
                dataGridView1.DataSource = DTable;
                dataGridView1.Columns[0].Width = 50;
                dataGridView1.Columns[1].Width = 700;
                dataGridView1.Columns[2].Width = 85;
            }
            else
            {
                if (radioButton3.Checked)
                {
                    RowIndex = 0;
                    DTable = new DataTable();
                    col1 = new DataColumn("#");
                    col2 = new DataColumn("FileName");
                    col3 = new DataColumn("Status");
                    DTable.Columns.Add(col1);
                    DTable.Columns.Add(col2);
                    DTable.Columns.Add(col3);
                    foreach (string f in Directory.GetFiles(textBox1.Text.Trim(), "*.avi"))
                    {
                        RowIndex++;
                        DataRow row = DTable.NewRow();
                        row[col1] = RowIndex.ToString();
                        row[col2] = f;
                        row[col3] = "Waiting";
                        DTable.Rows.Add(row);

                    }

                    DirSearch(textBox1.Text);

                    dataGridView1.DataSource = DTable;
                    dataGridView1.Columns[0].Width = 50;
                    dataGridView1.Columns[1].Width = 700;
                    dataGridView1.Columns[2].Width = 85;

                }
                else
                {
                    if (radioButton5.Checked)
                    {
                        openFileDialog1.Filter = "Avi files (*.avi)|*.avi|All files (*.*)|*.*|FLV files (*.flv)|*.flv";
                        openFileDialog1.Multiselect = true;
                        openFileDialog1.InitialDirectory = textBox1.Text;
                        openFileDialog1.ShowDialog();
                    }
                }
            }
        }
        protected void DirSearch(string sDir)
        {
            foreach (string d in Directory.GetDirectories(sDir))
            {
                foreach (string f in Directory.GetFiles(d, "*.avi"))
                {
                    RowIndex++;
                    DataRow row = DTable.NewRow();
                    row[col1] = RowIndex.ToString();
                    row[col2] = f;
                    row[col3] = "Waiting";
                    DTable.Rows.Add(row);

                }
                DirSearch(d);
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            button2.ForeColor = Color.White;
            button2.Text = "Start";
            button2.BackColor = Color.Navy;

            button6.ForeColor = Color.White;
            button6.Text = "Stop";
            button6.BackColor = Color.Navy;

            try
            {
                foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses())
                {
                    if (p.ProcessName == "ffmpeg")
                    {
                        p.Kill();
                    }
                }
               // button6.Enabled = false;
                label1.Text = "0%";
                progressBar1.Value = 0;
                button2.Enabled = true;
                dataGridView1.Rows[_Index].Cells[2].Value = "Skipped";
              
            }
            catch
            {
            }
          


        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses())
            {
                if (p.ProcessName == "ffmpeg")
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {

                    }

                }
            }
        }
      
    }
}