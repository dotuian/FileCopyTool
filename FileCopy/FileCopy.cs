using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileCopy
{
    public partial class FileCopy : Form
    {
        public FileCopy()
        {
            InitializeComponent();
            init();
        }

        private void init() {

            //可输入多行
            txtFileList.Multiline = true;
            //垂直、水平滚动条
            txtFileList.ScrollBars = ScrollBars.Both;
            //禁止自动换行
            txtFileList.WordWrap = false;

            //结果表示
            txtResult.Multiline = true;
            txtResult.ScrollBars = ScrollBars.Both;
            txtResult.WordWrap = false;
            txtResult.ReadOnly = true;
            txtResult.BackColor = System.Drawing.SystemColors.ControlLight;

            // 进度条不显示
            progressBar.Hide();

        }

        /// <summary>
        /// 文件复制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void execute_Click(object sender, EventArgs e)
        {
            txtResult.Text = null;
            lblResult.Text = null;

            if (!CheckInput()) {
                return;
            }

            string sourcePath = txtSource.Text;
            string destinationPath = txtDestination.Text;
            string filelist = txtFileList.Text;

            string[] files = filelist.Split(new string[] {"\r\n", "\n"}, StringSplitOptions.None);

            progressBar.Show();
            progressBar.Maximum = files.Count();

            //
            int totalCount = 0;
            int successedCount = 0;
            int failedCount = 0;
            foreach (string file in files) {
                progressBar.Value = ++totalCount;

                if (String.IsNullOrEmpty(file)) {
                    txtResult.Text += "\r\n";
                    continue;
                }

                string sourceFileName = sourcePath + file;
                string destFileName = destinationPath + file;

                // 目录不存在的情况下，创建目录
                string targetPath = Path.GetDirectoryName(destFileName);
                if (!System.IO.Directory.Exists(targetPath))
                {
                    System.IO.Directory.CreateDirectory(targetPath);
                }

                // 目标文件存在，先删除
                if (File.Exists(destFileName)) {
                    File.Delete(destFileName);
                }

                if (File.Exists(sourceFileName))
                {
                    // 复制文件
                    File.Copy(sourceFileName, destFileName);
                    successedCount++;
                    txtResult.Text += String.Format("{0}  |   copied！\r\n", file);
                }
                else {
                    failedCount++;
                    txtResult.Text += String.Format("{0}  |   file not exsit！\r\n", file);
                }
                
            }

            lblResult.Text = string.Format("finished! successed {0} , failed : {1}", successedCount, failedCount);
            if (failedCount > 0) {
                lblResult.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// 执行前输入内容检查
        /// </summary>
        /// <returns></returns>
        private bool CheckInput() {
            string sourcePath = txtSource.Text;
            string destinationPath = txtDestination.Text;
            string filelist = txtFileList.Text;

            if (string.IsNullOrEmpty(sourcePath)) {
                txtSource.Focus();
                MessageBox.Show(string.Format("Source is empty !"));
                return false;
            }

            if (!Directory.Exists(sourcePath))
            {
                MessageBox.Show(string.Format("Source is not directory!"));
                txtSource.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(destinationPath))
            {
                MessageBox.Show(string.Format("Destination is empty !"));
                txtDestination.Focus();
                return false;
            }

            if (!Directory.Exists(destinationPath))
            {
                MessageBox.Show(string.Format("Destination is not directory!"));
                txtDestination.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(filelist)) {
                MessageBox.Show(string.Format("FileList is not directory!"));
                txtFileList.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// 文件夹选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSource_DoubleClick(object sender, EventArgs e)
        {
            string path = SelectFolder("");
            if (!string.IsNullOrEmpty(path)) {
                txtSource.Text = path;
                reset();
            }
        }

        /// <summary>
        /// 文件夹选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDestination_DoubleClick(object sender, EventArgs e)
        {

            string path = SelectFolder("");
            if (!string.IsNullOrEmpty(path))
            {
                txtDestination.Text = path;
                reset();
            }
        }

        private void reset() {
            progressBar.Hide();
            lblResult.Text = null;
            txtResult.Text = null;
        }

        /// <summary>
        /// 文件夹选择框
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private string SelectFolder(string title) {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.Description = title;
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            fbd.SelectedPath = @"C:\";
            fbd.ShowNewFolderButton = true;

            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                return fbd.SelectedPath;
            }

            return null;
        }

    }
}
