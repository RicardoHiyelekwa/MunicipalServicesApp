using System.Drawing;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public class MainForm : Form
    {
        public MainForm()
        {
            Text = "Municipal Services";
            Size = new Size(700, 460);
            MinimumSize = new Size(700, 460);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(30, 30, 40);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(136, 0, 27)
            };
            Label title = new Label
            {
                Text = "Municipal Services Dashboard",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            header.Controls.Add(title);

            Label subtitle = new Label
            {
                Text = "South Africa — Serving Our Communities",
                ForeColor = Color.FromArgb(180, 180, 200),
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Left = 0, Top = 90, Width = 684, Height = 30
            };

            Button btnReport = CreateButton("📋  Report Issues", 140);
            Button btnEvents = CreateButton("📅  Events & Announcements", 210);
            Button btnStatus = CreateButton("🔍  Track Service Requests", 280);

            btnReport.Click += (s, e) => new IssueForm().ShowDialog();
            btnEvents.Click += (s, e) => new EventsForm().ShowDialog();
            btnStatus.Click += (s, e) => new StatusForm().ShowDialog();

            Label footer = new Label
            {
                Text = "AAPD7112/w  |  PROG7312  |  IIE 2026",
                ForeColor = Color.FromArgb(100, 100, 120),
                Font = new Font("Segoe UI", 8),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Left = 0, Top = 390, Width = 684, Height = 24
            };

            Controls.AddRange(new Control[]
            {
                header, subtitle,
                btnReport, btnEvents, btnStatus,
                footer
            });
        }

        private Button CreateButton(string text, int top)
        {
            var btn = new Button
            {
                Text = text, Top = top, Left = 175,
                Width = 350, Height = 52,
                BackColor = Color.FromArgb(0, 84, 166),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 104, 196);
            return btn;
        }
    }
}
