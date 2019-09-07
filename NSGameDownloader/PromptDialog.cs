using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NSGameDownloader
{
    class PromptDialog
    {
        public static string ShowDialog(string caption, string initValue = "")
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            TextBox textBox = new TextBox() { Left = 10, Top = 12, Width = 450 };
            textBox.Text = initValue;
            Button confirmation = new Button() { Text = "Ok", Left = 360, Width = 100, Height = 45, Top = 60, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
