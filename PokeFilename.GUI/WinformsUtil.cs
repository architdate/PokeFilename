using System;
using System.Media;
using System.Windows.Forms;

namespace PokeFilename
{
    public static class WinformsUtil
    {
        public static DialogResult Alert(params string[] lines)
        {
            SystemSounds.Asterisk.Play();
            string msg = string.Join(Environment.NewLine + Environment.NewLine, lines);
            return MessageBox.Show(msg, nameof(Alert), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult Prompt(MessageBoxButtons btn, params string[] lines)
        {
            SystemSounds.Question.Play();
            string msg = string.Join(Environment.NewLine + Environment.NewLine, lines);
            return MessageBox.Show(msg, nameof(Prompt), btn, MessageBoxIcon.Asterisk);
        }

        public static DialogResult Error(params string[] lines)
        {
            SystemSounds.Hand.Play();
            string msg = string.Join(Environment.NewLine + Environment.NewLine, lines);
            return MessageBox.Show(msg, nameof(Error), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
