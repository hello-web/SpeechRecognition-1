﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Collections;

namespace SpeechRecognition
{
    public partial class MainForm : Form
    {

        public MainSpeech Speech;
        private bool inProcess = false;
        public MainForm()
        {
            InitializeComponent();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (inProcess)
            {
                MessageBox.Show("Распознавание прервано");
                BtnStop_Click(sender, e);
            }
            AddForm form1 = new AddForm();
            form1.ShowDialog(this);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Logger.Add("Program started");
            WaveInput.PrepareMic();
            Speech = new MainSpeech(); //создание объекта главного класса
            for (int i = 0; i < Speech.Commands.Count; i++) //вывод сузествующих комманд в листбокс
            {
                ListCommands.Items.Add(((Command)Speech.Commands[i]).Name);
            }

        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            Logger.Add("Recognition attempt");

            if (Speech.Commands.Count == 0)
            {
                Logger.Add("Recognition is impossible. Training wasn't made.");
                tbxLogs.Text += "У-ля-ля, всё печально. Сначала требуется обучить систему!\r\n";
            }
            else
            {
                BtnStart.Visible = false;
                BtnStop.Visible = true;
                Logger.Add("Recognition start");
                inProcess = true;
                SoundProcessing.Start();
            }
            tbxLogs.SelectionStart = tbxLogs.Text.Length;
            tbxLogs.ScrollToCaret();
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {

            const double MIN_DISTANCE = 0d;
                      
            string name;
            ArrayList result = SoundProcessing.Stop();
            result = Learning.Normalize(result);
            
            double distance = Speech.Compare(result, out name);

            Logger.Add("Recognited as \"" + name + "\"");
            if (distance >= MIN_DISTANCE) tbxLogs.Text += "Похоже команда не расспознана, попробуйте ещё раз,\n но если уверены в своей правоте, то вероятней всего это была команда " + name + "\r\n";
            else
            {
                tbxLogs.Text += "Команда распозанана. Это " + name;
                System.Windows.Forms.MessageBox.Show(name);
            }

            BtnStop.Visible = false;
            BtnStart.Visible = true;
            inProcess = false;
            for (int j = 0; j < Speech.Commands.Count; j++)
            {
                this.tbxLogs.Text += "Дистанция между вектором " + ((Command)Speech.Commands[j]).Name + " и записанным вектором - " + MainSpeech.getDistance(((Command)Speech.Commands[j]).Coefs, result) + "\r\n";
            }
            tbxLogs.SelectionStart = tbxLogs.Text.Length;
            tbxLogs.ScrollToCaret();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (inProcess)
            {
                MessageBox.Show("Распознавание прервано");
                BtnStop_Click(sender, e);
            }
            if (ListCommands.SelectedIndex > -1)
            {
                int i = ListCommands.SelectedIndex;
                Speech.Commands.RemoveAt(i);
                ListCommands.Items.RemoveAt(i);               
                if (i == 0 && ListCommands.Items.Count > 0)
                    ListCommands.SelectedIndex = 0;
                else
                    ListCommands.SelectedIndex = i - 1;
            }
           
        }

        private void ListCommands_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawCoeffs();
        }
        private void DrawCoeffs()
        {
            if (ListCommands.SelectedIndex < 0) return;
            Bitmap Bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.BackgroundImage = Bmp;
            LBPlot LB = new LBPlot(Graphics.FromImage(Bmp), pictureBox1.DisplayRectangle);
            LB.ShowText = true;
            LB.AddData((double[])((Command)Speech.Commands[ListCommands.SelectedIndex]).Coefs.ToArray(typeof(System.Double)));
            /*LB.AddData(ArrayList.Repeat(-1d, RawTransform.NEEDED_COEFS));
            LB.AddData(ArrayList.Repeat(1d, RawTransform.NEEDED_COEFS));*/
            LB.SelectView(0, RawTransform.NEEDED_COEFS, 1, -1);
            //LBPlot.DrawPlot((double[])((Command)Speech.Commands[ListCommands.SelectedIndex]).Coefs.ToArray(typeof(System.Double)), Bmp);
            LB.DrawData();
        }

        private void mmClear_Click(object sender, EventArgs e)
        {
            if (inProcess)
            {
                MessageBox.Show("Распознавание прервано");
                BtnStop_Click(sender, e);
            }
            Speech.Commands.Clear();
            ListCommands.Items.Clear();
        }

        private void mmExit_Click(object sender, EventArgs e)
        {
            if (inProcess)
            {
                MessageBox.Show("Распознавание прервано");
                BtnStop_Click(sender, e);
            }
            Close();
        }

        private void mmResearch_Click(object sender, EventArgs e)
        {
            if (inProcess)
            {
                MessageBox.Show("Распознавание прервано");
                BtnStop_Click(sender, e);
            }
            DWTTest TestForm = new DWTTest();
            TestForm.ShowDialog();
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            DrawCoeffs();
        }

        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {

        }
    }
}
