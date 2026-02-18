using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MDIAplication
{
    public partial class Form1 : Form
    {
        private int documentCounter = 0;
        public TextBox TextEditor;
        private ToolStripComboBox cmbFont;
        private ToolStripComboBox cmbSize;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Editor MDI";
            this.WindowState = FormWindowState.Maximized;
            this.IsMdiContainer = true;

            CreateMenu();
            CreateToolbar();
        }

        public Form1(string title)
        {
            InitializeComponent();
            this.Text = title;
            this.IsMdiContainer = false;

            TextEditor = new TextBox();
            TextEditor.Multiline = true;
            TextEditor.Dock = DockStyle.Fill;
            TextEditor.ScrollBars = ScrollBars.Both;
            TextEditor.Font = new Font("Arial", 12);
            this.Controls.Add(TextEditor);
        }

        private void CreateMenu()
        {
            MenuStrip menu = new MenuStrip();

            ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
            fileMenu.DropDownItems.Add("New", null, (s, e) => CreateNewDocument());
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Save", null, (s, e) => SaveActiveDocument(), Keys.Control | Keys.S));
            fileMenu.DropDownItems.Add("Exit", null, (s, e) => this.Close());

            ToolStripMenuItem windowMenu = new ToolStripMenuItem("Window");
            windowMenu.DropDownItems.Add("Cascade", null, (s, e) => this.LayoutMdi(MdiLayout.Cascade));
            windowMenu.DropDownItems.Add("Tile Horizontal", null, (s, e) => this.LayoutMdi(MdiLayout.TileHorizontal));

            menu.MdiWindowListItem = windowMenu;
            menu.Items.Add(fileMenu);
            menu.Items.Add(windowMenu);

            this.MainMenuStrip = menu;
            this.Controls.Add(menu);
        }

        private void CreateToolbar()
        {
            ToolStrip toolbar = new ToolStrip();

            cmbFont = new ToolStripComboBox();
            cmbFont.Items.AddRange(new object[] { "Arial", "Times New Roman", "Consolas", "Verdana", "Calibri" });
            cmbFont.Text = "Arial";
            cmbFont.SelectedIndexChanged += (s, e) => ApplySettings();

            cmbSize = new ToolStripComboBox();
            cmbSize.Items.AddRange(new object[] { "8", "10", "12", "14", "16", "18", "24", "36", "48" });
            cmbSize.Text = "12";
            cmbSize.SelectedIndexChanged += (s, e) => ApplySettings();

            ToolStripButton btnBold = new ToolStripButton("B");
            btnBold.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnBold.Click += (s, e) => ToggleBold();

            toolbar.Items.Add(new ToolStripLabel("Font:"));
            toolbar.Items.Add(cmbFont);
            toolbar.Items.Add(new ToolStripSeparator());
            toolbar.Items.Add(new ToolStripLabel("Size:"));
            toolbar.Items.Add(cmbSize);
            toolbar.Items.Add(new ToolStripSeparator());
            toolbar.Items.Add(btnBold);

            this.Controls.Add(toolbar);
        }

        private void CreateNewDocument()
        {
            documentCounter++;
            Form1 child = new Form1($"Document {documentCounter}");
            child.MdiParent = this;

            try
            {
                child.TextEditor.Font = new Font(cmbFont.Text, float.Parse(cmbSize.Text));
            }
            catch { }

            child.Show();
        }

        private void SaveActiveDocument()
        {
            if (this.ActiveMdiChild is Form1 child)
            {
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "Text Files|*.txt|All Files|*.*";

                if (save.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(save.FileName, child.TextEditor.Text);
                        child.Text = Path.GetFileName(save.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void ApplySettings()
        {
            if (this.ActiveMdiChild is Form1 child)
            {
                try
                {
                    string fontName = cmbFont.Text;
                    float fontSize = float.Parse(cmbSize.Text);
                    FontStyle style = child.TextEditor.Font.Style;
                    child.TextEditor.Font = new Font(fontName, fontSize, style);
                }
                catch { }
            }
        }

        private void ToggleBold()
        {
            if (this.ActiveMdiChild is Form1 child)
            {
                Font oldFont = child.TextEditor.Font;
                FontStyle newStyle = oldFont.Style ^ FontStyle.Bold;
                child.TextEditor.Font = new Font(oldFont.FontFamily, oldFont.Size, newStyle);
            }
        }
    }
}