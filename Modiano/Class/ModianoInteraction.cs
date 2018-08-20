using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;

namespace Modiano.Class
{
    class ModianoInteraction
    {
        private string stringSaved = "";

        public string StringSaved { get => stringSaved; set => stringSaved = value; }

        override public string ToString()
        {
            return StringSaved + ";" + Environment.NewLine;
        }

        virtual public void Run(RichTextBox _richTextBox)
        {

        }
    }

    class ModianoAdd : ModianoInteraction
    {
        private string str = "";
        private int caretPosition = 0;

        public string Str { get => str; set => str = value; }
        public int CaretPosition { get => caretPosition; set => caretPosition = value; }

        public ModianoAdd(int _p, string _s)
        {
            CaretPosition = _p;
            Str = _s;
            StringSaved = "A_" + CaretPosition + "_" + Str;
        }

        override public void Run(RichTextBox _richTextBox)
        {
            _richTextBox.CaretPosition.GetPositionAtOffset(caretPosition);
            _richTextBox.CaretPosition.InsertTextInRun(str.Replace("<CRLF>", Environment.NewLine));
        }
    }


    class ModianoDel : ModianoInteraction
    {
        private int caretPosition = 0;
        private int lenght = 0;

        public int CaretPosition { get => caretPosition; set => caretPosition = value; }
        public int Lenght { get => lenght; set => lenght = value; }

        public ModianoDel(int _s, int _l)
        {
            CaretPosition = _s;
            Lenght = _l;
            StringSaved = "D_" + CaretPosition + "_" + Lenght;
        }

        override public void Run(RichTextBox _richTextBox)
        {
            _richTextBox.CaretPosition.GetPositionAtOffset(CaretPosition);
            _richTextBox.CaretPosition.DeleteTextInRun(Lenght);
        }
    }

    class ModianoKey : ModianoInteraction
    {
        private string keyPressed;


        public string KeyPressed
        {
            get => keyPressed;
            set
            {
                keyPressed = value;
                StringSaved = "K_" + KeyPressed;
            }
        }


        override public void Run(RichTextBox _richTextBox)
        {
            _richTextBox.CaretPosition.InsertTextInRun(keyPressed);
        }
    }

    class ModianoMouseSelection : ModianoInteraction
    {
        private int start;
        private int end;

        public int Start { get => start; set => start = value; }
        public int End
        {
            get => end;
            set
            {
                end = value;
                StringSaved = "S_" + Start + "_" + End;
            }
        }

        override public void Run(RichTextBox _richTextBox)
        {
        }
    }

    class ModianoMouseMove : ModianoInteraction
    {
        private int position;

        public int Position
        {
            get => position;
            set
            {
                position = value;
                StringSaved = "M_" + Position;
            }
        }

        override public void Run(RichTextBox _richTextBox)
        {
            _richTextBox.CaretPosition.GetPositionAtOffset(position);
        }
    }

    class ModianoTools : ModianoInteraction
    {
        private string tool;

        public string Tool
        {
            get => tool;
            set
            {
                tool = value;
                StringSaved = "T_" + Tool;
            }
        }
    }
}
