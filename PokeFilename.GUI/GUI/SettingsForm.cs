using System.Windows.Forms;
using PokeFilename.API;
using PKHeX.Core;

namespace PokeFilename
{
    public class SettingsForm : Form
    {
        private Button B_BulkRename;
        private PropertyGrid PG_Settings;

        public SettingsForm(object obj)
        {
            if (PG_Settings == null)
                PG_Settings = new PropertyGrid();
            if (B_BulkRename == null)
                B_BulkRename = new Button();
            InitializeComponent();
            PG_Settings.SelectedObject = obj;
            CenterToParent();
        }

        private void SettingsEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W && ModifierKeys == Keys.Control)
                Close();
        }

        private void B_BulkRename_Click(object sender, System.EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result != DialogResult.OK || string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    WinformsUtil.Alert("Rename Cancelled!");
                    return;
                }
                result = WinformsUtil.Prompt(MessageBoxButtons.YesNo, "Recursively Rename Files?");
                var deep = false;
                if (result == DialogResult.Yes)
                    deep = true;
                var settings = PokeFileNamePlugin.Settings;
                EntityFileNamer.Namer = settings.Create();
                BulkRename.RenameFolder(fbd.SelectedPath, deep);
                WinformsUtil.Alert($"Rename Complete. All files have been renamed using {settings.Namer}");
            }
        }

        private void InitializeComponent()
        {
            this.PG_Settings = new System.Windows.Forms.PropertyGrid();
            this.B_BulkRename = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // PG_Settings
            // 
            this.PG_Settings.Dock = System.Windows.Forms.DockStyle.Top;
            this.PG_Settings.Location = new System.Drawing.Point(0, 0);
            this.PG_Settings.Name = "PG_Settings";
            this.PG_Settings.Size = new System.Drawing.Size(285, 260);
            this.PG_Settings.TabIndex = 1;
            // 
            // B_BulkRename
            // 
            this.B_BulkRename.Location = new System.Drawing.Point(0, 266);
            this.B_BulkRename.Name = "B_BulkRename";
            this.B_BulkRename.Size = new System.Drawing.Size(285, 23);
            this.B_BulkRename.TabIndex = 2;
            this.B_BulkRename.Text = "Bulk Rename";
            this.B_BulkRename.UseVisualStyleBackColor = true;
            this.B_BulkRename.Click += new System.EventHandler(this.B_BulkRename_Click);
            // 
            // SettingsForm
            // 
            this.ClientSize = new System.Drawing.Size(285, 293);
            this.Controls.Add(this.B_BulkRename);
            this.Controls.Add(this.PG_Settings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "SettingsForm";
            this.Text = "PokeFilename Settings";
            this.ResumeLayout(false);

        }
    }
}
