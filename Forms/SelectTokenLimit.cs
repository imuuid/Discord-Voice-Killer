using MetroSuite;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

public partial class SelectTokenLimit : MetroForm
{
    public SelectTokenLimit()
    {
        try
        {
            InitializeComponent();

            if (!System.IO.File.Exists("tokens.txt"))
            {
                System.IO.File.WriteAllText("tokens.txt", "");
            }

            metroTextbox2.Text = System.IO.File.ReadAllText("tokens.txt");

            metroLabel2.Text = Utils.GetTotalTokensCount().ToString() + " tokens loaded.";
        }
        catch
        {
            Process.GetCurrentProcess().Kill();
            return;
        }
    }

    private void pictureBox24_MouseEnter(object sender, System.EventArgs e)
    {
        pictureBox24.Size = new Size(18, 18);
    }

    private void pictureBox24_MouseLeave(object sender, System.EventArgs e)
    {
        pictureBox24.Size = new Size(20, 20);
    }

    private void pictureBox22_MouseEnter(object sender, System.EventArgs e)
    {
        pictureBox22.Size = new Size(18, 18);
    }

    private void pictureBox22_MouseLeave(object sender, System.EventArgs e)
    {
        pictureBox22.Size = new Size(20, 20);
    }

    private void pictureBox24_Click(object sender, System.EventArgs e)
    {
        Process.GetCurrentProcess().Kill();
    }

    private void pictureBox22_Click(object sender, System.EventArgs e)
    {
        WindowState = FormWindowState.Minimized;
    }

    private void metroButton2_Click(object sender, System.EventArgs e)
    {
        try
        {
            Utils.tokensList = metroTextbox2.Text;

            if (metroTextbox2.Text.Replace(" ", "").Trim().Replace('\t'.ToString(), "") == "")
            {
                MessageBox.Show("Please, insert some valid tokens!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Microsoft.VisualBasic.Information.IsNumeric(metroTextbox1.Text))
            {
                MessageBox.Show("Please, put in a valid number!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int i = int.Parse(metroTextbox1.Text), max = Utils.GetTotalTokensCount();

            if (i <= 0)
            {
                MessageBox.Show("Please, the number needs to be greater than zero!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (i > max)
            {
                MessageBox.Show("Please, the number needs to be in a range between 1 and " + max.ToString() + ".", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Utils.tokenLimit = i;
                new MainForm().Show();

                this.Hide();
                this.Size = new System.Drawing.Size(0, 0);
                this.Visible = false;
                this.Enabled = false;
                this.Opacity = 0;
            }
        }
        catch
        {
            MessageBox.Show("Please, put in a valid number!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void metroTextbox2_TextChanged(object sender, System.EventArgs e)
    {
        Utils.tokensList = metroTextbox2.Text;
        metroLabel2.Text = Utils.GetTotalTokensCount().ToString() + " tokens loaded.";
        System.IO.File.WriteAllText("tokens.txt", Utils.tokensList);
    }

    private void metroButton1_Click(object sender, System.EventArgs e)
    {
        openFileDialog1.FileName = "";

        if (openFileDialog1.ShowDialog().Equals(DialogResult.OK))
        {
            metroTextbox2.Text = System.IO.File.ReadAllText(openFileDialog1.FileName);
        }
    }
}