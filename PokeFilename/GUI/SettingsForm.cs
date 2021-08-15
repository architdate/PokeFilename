using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokeFilename
{
    public partial class SettingsForm : Form
    {
        private PropertyGrid PG_Settings;

        public SettingsForm(object obj)
        {
            InitializeComponent();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            PG_Settings.SelectedObject = obj;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            CenterToParent();
        }

        private void SettingsEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W && ModifierKeys == Keys.Control)
                Close();
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ALM Settings
            PokeFilename.SetNamerSettings();

            Properties.PokeFilename.Default.Save();
        }

        private void InitializeComponent()
        {
            this.PG_Settings = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // PG_Settings
            // 
            this.PG_Settings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PG_Settings.Location = new System.Drawing.Point(0, 0);
            this.PG_Settings.Name = "PG_Settings";
            this.PG_Settings.Size = new System.Drawing.Size(284, 261);
            this.PG_Settings.TabIndex = 1;
            // 
            // SettingsForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.PG_Settings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "SettingsForm";
            this.Text = "PokeFilename Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SettingsEditor_KeyDown);
            this.ResumeLayout(false);

        }
    }
}
