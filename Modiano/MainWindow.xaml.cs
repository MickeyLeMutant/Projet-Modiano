using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Modiano.Class;

namespace Modiano
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<ModianoInteraction> listInteraction = new List<ModianoInteraction>();
        string path = "";

        bool playing = false;

        public MainWindow()
        {
            InitializeComponent();
            mainRichText.AddHandler(RichTextBox.DragOverEvent, new DragEventHandler(RichTextBox_DragOver), true);
            mainRichText.AddHandler(RichTextBox.DropEvent, new DragEventHandler(RichTextBox_Drop), true);
            titleRichText.AddHandler(RichTextBox.DragOverEvent, new DragEventHandler(RichTextBox_DragOver), true);
            titleRichText.AddHandler(RichTextBox.DropEvent, new DragEventHandler(RichTextBox_Drop), true);
        }

        private void RichTextBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = false;
        }

        #region save/load

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Save();
        }

        private void RichTextBox_Drop(object sender, DragEventArgs e)
        {
#if DEBUG
            Console.WriteLine("Drop");
#endif

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                Load(file);
            }
        }

        void Load(string _p)
        {
            if (File.Exists(_p))
            {
                try
                {
                    if (new TextRange(titleRichText.Document.ContentStart, titleRichText.Document.ContentEnd).Text.Replace(Environment.NewLine, "") != "")
                    {
                        Save();
                    }

                    string filename = Path.GetFileNameWithoutExtension(_p);

                    mainRichText.CaretPosition = mainRichText.Document.ContentEnd;
                    mainRichText.CaretPosition.InsertTextInRun(Environment.NewLine);
                    mainRichText.Document.Blocks.Clear();
                    mainRichText.AppendText(File.ReadAllText(_p));

                    titleRichText.CaretPosition = titleRichText.Document.ContentEnd;
                    titleRichText.CaretPosition.InsertTextInRun(Environment.NewLine);
                    titleRichText.Document.Blocks.Clear();
                    titleRichText.AppendText(filename);

                    this.Title = filename + " - Projet Modiano";
                }
                catch
                {

                }
            }
        }

        void Save()
        {
#if DEBUG
            Console.WriteLine("Save");
#endif
            string filename = new TextRange(titleRichText.Document.ContentStart, titleRichText.Document.ContentEnd).Text.Replace(Environment.NewLine, "");

            //enregistrement complet
            string range = new TextRange(mainRichText.Document.ContentStart, mainRichText.Document.ContentEnd).Text;
            range += Environment.NewLine + "** L'historique commence ici **" + Environment.NewLine;
            for (int i = 0; i < listInteraction.Count; i++)
            {
                range += listInteraction[i].ToString();
            }
            File.WriteAllText(filename + ".txt", range);
            //erase old file
            if (filename != Path.GetFileNameWithoutExtension(path))
            {

            }
        }
        #endregion

        private void mainRichText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!playing)
            {
                foreach (var t in e.Changes)
                {
                    /*if (t.RemovedLength > 0 && t.AddedLength > 0)
                    {
                        Console.WriteLine("Replace at " + t.Offset + " -" + t.RemovedLength + " +" + t.AddedLength +"|"+ Select(mainRichText, t.Offset - 2, t.AddedLength)+"|");

                        ModianoDel tmp = new ModianoDel(t.Offset, t.RemovedLength);
                        listInteraction.Add(tmp);
                            ModianoAdd tmp2 = new ModianoAdd(t.Offset , Select(mainRichText, t.Offset - 2, t.AddedLength));
                            listInteraction.Add(tmp2);
                    }
                    else*/
                    {
                        if (t.RemovedLength > 0)
                        {
                            Console.WriteLine("Remove at " + t.Offset + " " + t.RemovedLength);
                            ModianoDel tmp = new ModianoDel(t.Offset, t.RemovedLength);
                            listInteraction.Add(tmp);
                        }
                        if (t.AddedLength > 0)
                        {
                            Console.WriteLine("Add at " + t.Offset + " " + t.AddedLength + "|" + Select(mainRichText, t.Offset - 2, t.AddedLength) + "|");
                            ModianoAdd tmp = new ModianoAdd(t.Offset, Select(mainRichText, t.Offset - 2, t.AddedLength));
                            listInteraction.Add(tmp);
                        }
                    }
                }
            }
        }

        internal string Select(RichTextBox rtb, int index, int length)
        {
            TextPointer pos = rtb.CaretPosition;
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);

            //if (textRange.Text.Length >= (index + length))
            {
                TextPointer start = textRange.Start.GetPositionAtOffset(index, LogicalDirection.Forward);
                TextPointer end = textRange.Start.GetPositionAtOffset(index + length, LogicalDirection.Backward);
                rtb.Selection.Select(start, end);
            }
            string res = rtb.Selection.Text;
            res = res.Replace(Environment.NewLine, "<CRLF>");
            rtb.CaretPosition = pos;
            return res;
        }

        #region poubelle
        private void mainRichText_SelectionChanged(object sender, RoutedEventArgs e)
        {
            MouseUpdate();
        }

        /// <summary>
        /// Enregistre le changement de la position du curseur ou d'une nouvelle selection
        /// TODO : retirer le "bruit" (nouvelle position dû à l'ajout d'une lettre par ex.)
        /// </summary>
        private void MouseUpdate()
        {
            int position = mainRichText.CaretPosition.GetOffsetToPosition(mainRichText.CaretPosition.DocumentStart);
            int start = mainRichText.Selection.Start.GetOffsetToPosition(mainRichText.CaretPosition.DocumentStart);
            int end = mainRichText.Selection.End.GetOffsetToPosition(mainRichText.CaretPosition.DocumentStart);
            if (start == end)
            {
#if DEBUG
                Console.WriteLine("curseur en position : " + position);
#endif
                ModianoMouseMove tmp = new ModianoMouseMove();
                tmp.Position = position;
                listInteraction.Add(tmp);
            }
            else
            {
#if DEBUG
                Console.WriteLine("Selection de " + start + " à " + end);
#endif
                ModianoMouseSelection tmp = new ModianoMouseSelection();
                tmp.Start = start;
                tmp.End = end;
                listInteraction.Add(tmp);
            }
        }
        #endregion

        #region tools
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!mainRichText.Selection.IsEmpty)
            {
                mainRichText.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
            /*if (mainRichText.CaretPosition.Parent is TextElement)
            {
                if ((mainRichText.CaretPosition.Parent as TextElement).FontWeight == FontWeights.Bold)
                {
                    (mainRichText.CaretPosition.Parent as TextElement).FontWeight = FontWeights.Normal;
                }
                else
                {
                    (mainRichText.CaretPosition.Parent as TextElement).FontWeight = FontWeights.Bold;
                }
            }*/
        }
        #endregion

        #region replay
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            playing = !playing;
            if (playing)
            {
                boutonPlay.Content = "P";
                Play();
            }
            else
            {
                boutonPlay.Content = "L";
            }

        }

        private void Play()
        {
            mainRichText.IsEnabled = false;
            mainRichText.CaretPosition = mainRichText.Document.ContentEnd;
            mainRichText.CaretPosition.InsertTextInRun(Environment.NewLine);
            mainRichText.Document.Blocks.Clear();
            for (int i = 0; i < listInteraction.Count; i++)
            {
                listInteraction[i].Run(mainRichText);
            }
        }
        #endregion
    }
}
