using MetroSuite;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System;
using Leaf.xNet;

public partial class MainForm : MetroForm
{
    public DiscordSessionManager sessionManager;

    public MainForm()
    {
        InitializeComponent();

        this.Text = "Discord Voice Killer V5.0 - " + Utils.tokenLimit.ToString() + " tokens loaded (" + Utils.GetTotalTokensCount().ToString() + " in stock)";
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
        CheckForIllegalCrossThreadCalls = false;
        sessionManager = new DiscordSessionManager();

        foreach (DiscordSession session in sessionManager.sessions)
        {
            new Thread(() => session.getUserId()).Start();
        }
    }

    private void pictureBox24_Click(object sender, System.EventArgs e)
    {
        Process.GetCurrentProcess().Kill();
    }

    private void pictureBox22_Click(object sender, System.EventArgs e)
    {
        WindowState = FormWindowState.Minimized;
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

    private void metroButton1_Click(object sender, System.EventArgs e)
    {
        new Thread(startJoinGuild).Start();
    }

    public void startJoinGuild()
    {
        try
        {
            Tuple<bool, string, string, string> info = Utils.GetInviteInformations(metroTextbox1.Text, Utils.GetRandomToken());

            if (info.Item1)
            {
                foreach (DiscordSession session in sessionManager.sessions)
                {
                    new Thread(() => session.joinGuild(Utils.GetInviteCodeByInviteLink(metroTextbox1.Text), Utils.GetXCP(info.Item2, info.Item3, info.Item4), info.Item2, info.Item3)).Start();
                }
            }
            else
            {
                MessageBox.Show("The invite link / code is not valid!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch
        {
            MessageBox.Show("The invite link / code is not valid!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void metroButton2_Click(object sender, System.EventArgs e)
    {
        new Thread(startLeaveGuild).Start(); 
    }

    public void startLeaveGuild()
    {
        try
        {
            Tuple<bool, string, string, string> info = Utils.GetInviteInformations(metroTextbox1.Text, Utils.GetRandomToken());
            string guildId = "";

            if (!info.Item1)
            {
                if (info.Item2.Length == 18)
                {
                    guildId = info.Item2;
                }
                else
                {
                    MessageBox.Show("The invite link / code is not valid!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                guildId = info.Item2;
            }

            foreach (DiscordSession session in sessionManager.sessions)
            {
                new Thread(() => session.leaveGuild(guildId)).Start();
            }
        }
        catch
        {
            MessageBox.Show("The invite link / code is not valid!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void metroButton4_Click(object sender, System.EventArgs e)
    {
        new Thread(startConnectWebSocket).Start(); 
    }

    public void startConnectWebSocket()
    {
        foreach (DiscordSession session in sessionManager.sessions)
        {
            new Thread(() => session.connectWebSocket(metroChecker7.Checked)).Start();
        }
    }

    private void metroButton3_Click(object sender, System.EventArgs e)
    {
        new Thread(startDisconnectWebSocket).Start(); 
    }

    public void startDisconnectWebSocket()
    {
        foreach (DiscordSession session in sessionManager.sessions)
        {
            new Thread(() => session.disconnectWebSocket()).Start();
        }
    }

    private void metroButton6_Click(object sender, System.EventArgs e)
    {
        new Thread(startJoinVoice).Start();
    }

    public void startJoinVoice()
    {
        foreach (DiscordSession session in sessionManager.sessions)
        {
            new Thread(() => session.joinVoice(metroTextbox2.Text, metroTextbox3.Text, metroTextbox4.Text, !metroChecker1.Checked, !metroChecker2.Checked, metroChecker3.Checked, metroChecker4.Checked, metroChecker5.Checked, metroChecker6.Checked, metroChecker9.Checked, metroChecker8.Checked)).Start();
        }
    }

    private void metroButton5_Click(object sender, System.EventArgs e)
    {
        new Thread(startLeaveVoice).Start();
    }

    public void startLeaveVoice()
    {
        foreach (DiscordSession session in sessionManager.sessions)
        {
            new Thread(() => session.leaveVoice()).Start();
        }
    }

    private void metroButton7_Click(object sender, System.EventArgs e)
    {
        new Thread(startEnableMic).Start(); 
    }

    public void startEnableMic()
    {
        foreach (DiscordSession session in sessionManager.sessions)
        {
            new Thread(() => session.setMicrophone(true)).Start();
        }
    }

    private void metroButton8_Click(object sender, System.EventArgs e)
    {
        new Thread(startDisableMic).Start();
    }

    public void startDisableMic()
    {
        foreach (DiscordSession session in sessionManager.sessions)
        {
            new Thread(() => session.setMicrophone(false)).Start();
        }
    }

    private void metroButton9_Click(object sender, System.EventArgs e)
    {
        new Thread(startEnableHeadphones).Start();
    }

    public void startEnableHeadphones()
    {
        foreach (DiscordSession session in sessionManager.sessions)
        {
            new Thread(() => session.setHeadphones(true)).Start();
        }
    }

    private void metroButton10_Click(object sender, System.EventArgs e)
    {
        new Thread(startDisableHeadphones).Start();
    }

    public void startDisableHeadphones()
    {
        foreach (DiscordSession session in sessionManager.sessions)
        {
            new Thread(() => session.setHeadphones(false)).Start();
        }
    }

    private void metroButton11_Click(object sender, System.EventArgs e)
    {
        new Thread(startWebcam).Start();
    }

    public void startWebcam()
    {
        foreach (DiscordSession session in sessionManager.sessions)
        {
            new Thread(() => session.setWebcam(true)).Start();
        }
    }

    private void metroButton12_Click(object sender, System.EventArgs e)
    {
        new Thread(disableWebcam).Start();
    }

    public void disableWebcam()
    {
        foreach (DiscordSession session in sessionManager.sessions)
        {
            new Thread(() => session.setWebcam(false)).Start();
        }
    }

    private void metroButton13_Click(object sender, System.EventArgs e)
    {
        new Thread(startScreenshare).Start();
    }

    public void startScreenshare()
    {
        foreach (DiscordSession session in sessionManager.sessions)
        {
            new Thread(() => session.setScreenshare(true)).Start();
        }
    }

    private void metroButton14_Click(object sender, System.EventArgs e)
    {
        new Thread(disableScreenshare).Start();
    }

    public void disableScreenshare()
    {
        foreach (DiscordSession session in sessionManager.sessions)
        {
            new Thread(() => session.setScreenshare(false)).Start();
        }
    }

    private void metroButton15_Click(object sender, System.EventArgs e)
    {
        new Thread(startJoinScreenshare).Start(); 
    }

    public void startJoinScreenshare()
    {
        foreach (DiscordSession session in sessionManager.sessions)
        {
            new Thread(() => session.setJoinScreenshare(true, metroTextbox5.Text)).Start();
        }
    }

    private void metroButton16_Click(object sender, System.EventArgs e)
    {
        new Thread(startLeaveScreenshare).Start();
    }

    public void startLeaveScreenshare()
    {
        foreach (DiscordSession session in sessionManager.sessions)
        {
            new Thread(() => session.setJoinScreenshare(false)).Start();
        }
    }
}