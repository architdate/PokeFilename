using System.Windows.Forms;

namespace PokeFilename
{
    public class SettingsForm : Form
    {
        private readonly PropertyGrid PG_Settings;

        public SettingsForm(object obj)
        {
            PG_Settings = new PropertyGrid();
            InitializeComponent();
            PG_Settings.SelectedObject = obj;
            CenterToParent();
        }

        private void SettingsEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W && ModifierKeys == Keys.Control)
                Close();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // PG_Settings
            // 
            PG_Settings.Dock = DockStyle.Fill;
            PG_Settings.Location = new System.Drawing.Point(0, 0);
            PG_Settings.Name = "PG_Settings";
            PG_Settings.Size = new System.Drawing.Size(284, 261);
            PG_Settings.TabIndex = 1;
            // 
            // SettingsForm
            // 
            ClientSize = new System.Drawing.Size(284, 261);
            Controls.Add(PG_Settings);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "SettingsForm";
            Text = "PokeFilename Settings";
            KeyDown += SettingsEditor_KeyDown;
            ResumeLayout(false);
        }
    }
}
