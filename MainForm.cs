using System.Drawing;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public class MainForm : Form
    {
        public MainForm()
        {
            Text = "Municipal Services";
            Size = new Size(900, 600);
            BackColor = Color.FromArgb(30, 30, 40);

            Label title = new Label
            {
                Text = "Municipal Services Dashboard",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                AutoSize = true,
                Top = 40,
                Left = 180
            };

            Button btnReport = CreateButton("Report Issues", 150);
            Button btnEvents = CreateButton("Events & Announcements", 250);
            Button btnStatus = CreateButton("Track Requests", 350);

            btnReport.Click += (s, e) => new IssueForm().ShowDialog();
            btnEvents.Click += (s, e) => new EventsForm().ShowDialog();
            btnStatus.Click += (s, e) => new StatusForm().ShowDialog();

            Controls.AddRange(new Control[] { title, btnReport, btnEvents, btnStatus });
        }

        private Button CreateButton(string text, int top)
        {
            return new Button
            {
                Text = text,
                Top = top,
                Left = 250,
                Width = 350,
                Height = 60,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
        }
    }
}