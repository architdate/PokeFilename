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
            PG_Settings ??= new PropertyGrid();
            B_BulkRename ??= new Button();
            InitializeComponent();
            PG_Settings.SelectedObject = obj;
            CenterToParent();
            KeyDown += SettingsEditor_KeyDown;
        }

        private void SettingsEditor_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W && ModifierKeys == Keys.Control)
                Close();
        }

        private void B_BulkRename_Click(object? sender, System.EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            if (result != DialogResult.OK || string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                WinformsUtil.Alert("Rename Cancelled!");
                return;
            }
            result = WinformsUtil.Prompt(MessageBoxButtons.YesNo, "Recursively Rename Files?");
            bool deep = result == DialogResult.Yes;
            var settings = PokeFileNamePlugin.Settings;
            EntityFileNamer.Namer = settings.Create();
            BulkRename.RenameFolder(fbd.SelectedPath, deep);
            WinformsUtil.Alert($"Rename Complete. All files have been renamed using {settings.Namer}");
        }

        private void InitializeComponent()
        {
            PG_Settings = new PropertyGrid();
            B_BulkRename = new Button();
            SuspendLayout();
            // 
            // PG_Settings
            // 
            PG_Settings.Dock = DockStyle.Top;
            PG_Settings.Location = new System.Drawing.Point(0, 0);
            PG_Settings.Name = "PG_Settings";
            PG_Settings.Size = new System.Drawing.Size(285, 260);
            PG_Settings.TabIndex = 1;
            // 
            // B_BulkRename
            // 
            B_BulkRename.Location = new System.Drawing.Point(0, 266);
            B_BulkRename.Name = "B_BulkRename";
            B_BulkRename.Size = new System.Drawing.Size(285, 23);
            B_BulkRename.TabIndex = 2;
            B_BulkRename.Text = "Bulk Rename";
            B_BulkRename.UseVisualStyleBackColor = true;
            B_BulkRename.Click += B_BulkRename_Click;
            // 
            // SettingsForm
            // 
            ClientSize = new System.Drawing.Size(285, 293);
            Controls.Add(B_BulkRename);
            Controls.Add(PG_Settings);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "SettingsForm";
            Text = "PokeFilename Settings";
            ResumeLayout(false);
        }
    }
}
