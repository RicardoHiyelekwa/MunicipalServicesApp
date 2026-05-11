using System.Drawing;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public class MainForm : Form
    {
        public MainForm()
        {
            Text = "Municipal Services — South Africa";
            Size = new Size(900, 620);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(30, 30, 40);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ── Header ──────────────────────────────────────────────
            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.FromArgb(0, 84, 166)
            };

            Label lblTitle = new Label
            {
                Text = "Municipal Services",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                AutoSize = true,
                Top = 18,
                Left = 30
            };

            Label lblSub = new Label
            {
                Text = "Citizen Engagement Platform — South Africa",
                ForeColor = Color.FromArgb(200, 220, 255),
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Top = 62,
                Left = 32
            };

            header.Controls.AddRange(new Control[] { lblTitle, lblSub });

            // ── Menu Cards ──────────────────────────────────────────
            Label lblMenu = new Label
            {
                Text = "SELECT A SERVICE",
                ForeColor = Color.FromArgb(160, 180, 210),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Top = 130,
                Left = 50
            };

            Button btnReport = CreateMenuButton(
                "📋  Report Issues",
                "Submit a new municipal issue report",
                Color.FromArgb(0, 120, 215), 170);

            Button btnEvents = CreateMenuButton(
                "📅  Local Events & Announcements",
                "Browse upcoming events and announcements",
                Color.FromArgb(16, 137, 62), 280);

            Button btnStatus = CreateMenuButton(
                "🔍  Track Service Requests",
                "Check the status of submitted requests",
                Color.FromArgb(136, 0, 27), 390);

            btnReport.Click += (s, e) => new IssueForm().ShowDialog();
            btnEvents.Click += (s, e) => new EventsForm().ShowDialog();
            btnStatus.Click += (s, e) => new StatusForm().ShowDialog();

            // ── Footer label ────────────────────────────────────────
            Label lblFooter = new Label
            {
                Text = "© 2026 IIE Rosebank College — AAPD7112/w",
                ForeColor = Color.FromArgb(100, 110, 130),
                Font = new Font("Segoe UI", 8),
                AutoSize = true,
                Top = 545,
                Left = 50
            };

            Controls.AddRange(new Control[]
            {
                header, lblMenu,
                btnReport, btnEvents, btnStatus,
                lblFooter
            });
        }

        private Button CreateMenuButton(string title, string subtitle, Color colour, int top)
        {
            Button btn = new Button
            {
                Top = top,
                Left = 50,
                Width = 780,
                Height = 80,
                BackColor = colour,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                Text = title + "\n" + subtitle,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }
    }
}
