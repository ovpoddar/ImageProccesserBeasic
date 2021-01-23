using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AccessPixelsInCSharp
{
    public partial class Form1 : Form
    {
        private string _path;
        private string _data;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() == DialogResult.OK)
            {
                _path = open.FileName;
                textBox1.Text = _path;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _path = textBox1.Text;
        }

        private void GetData_Click(object sender, EventArgs e)
        {
            //Task<string> task = new Task<string>(CalculateAllPixels);
            //task.Start();
            //this.Text = "Processing...";
            //textBox2.Text = await task;
            //this.Text = "Form1";
            if (!File.Exists(_path))
            {
                MessageBox.Show("file not found");
                return;
            }
            try
            {
                Thread thread = new Thread(() =>
                {
                    _data = CalculateAllPixels();
                    Action action = () => textBox2.Text = _data;
                    BeginInvoke(action);
                });
                thread.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string CalculateAllPixels()
        {
            Bitmap bitmap = new Bitmap(Image.FromFile(_path));
            var result = "";

            for (var y = 0; y < bitmap.Height; y++)
            {
                var row = "";
                for (var x = 0; x < bitmap.Width; x++)
                {
                    var onePixel = colorToString(bitmap.GetPixel(x, y));
                    row += $"{onePixel} \t";
                }
                result += $"{row} {Environment.NewLine}";
            }
            return result;
        }

        private string colorToString(Color color)
        {
            return $"{color.R} {color.G} {color.B} {color.A}";
        }


    }
}
