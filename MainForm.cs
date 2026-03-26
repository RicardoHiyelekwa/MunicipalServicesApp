using System.Drawing;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public class MainForm : Form
    {
        public MainForm()
        {
            Text = "Municipal Services Application";
            Width = 700;
            Height = 450;
            BackColor = Color.WhiteSmoke;
            StartPosition = FormStartPosition.CenterScreen;

            // Title
            Label title = new Label
            {
                Text = "Municipal Services Dashboard",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = true,
                Top = 30,
                Left = 160
            };

            // Buttons
            Button btnReport = CreateButton("Report Issues", 100, true);
            Button btnEvents = CreateButton("Local Events and Announcements", 180, false);
            Button btnStatus = CreateButton("Service Request Status", 260, false);

            // Open Issue Form
            btnReport.Click += (s, e) => new IssueForm().ShowDialog();

            Controls.Add(title);
            Controls.Add(btnReport);
            Controls.Add(btnEvents);
            Controls.Add(btnStatus);
        }

        // Method to create buttons consistently
        private Button CreateButton(string text, int top, bool enabled)
        {
            return new Button
            {
                Text = text,
                Top = top,
                Left = 180,
                Width = 300,
                Height = 50,
                Enabled = enabled,
                BackColor = enabled ? Color.SteelBlue : Color.LightGray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
        }
    }
}
