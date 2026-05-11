using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public class IssueForm : Form
    {
        private TextBox txtLocation;
        private ComboBox cmbCategory;
        private RichTextBox txtDescription;
        private Label lblAttachment;
        private Label lblStatus;
        private ProgressBar progress;
        private string attachmentPath = "";

        // ── Win32 cue banner (PlaceholderText for .NET Framework 4.8) ──
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wp, string lp);
        private const uint EM_SETCUEBANNER = 0x1501;
        private static void SetPlaceholder(TextBox tb, string text)
        {
            // Must be called after the handle is created
            tb.HandleCreated += (s, e) => SendMessage(tb.Handle, EM_SETCUEBANNER, (IntPtr)1, text);
        }

        public IssueForm()
        {
            Text = "Report an Issue";
            Size = new Size(860, 640);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ── Header ──────────────────────────────────────────────
            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(0, 84, 166)
            };
            Label lblHead = new Label
            {
                Text = "📋  Report a Municipal Issue",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Top = 18, Left = 20
            };
            header.Controls.Add(lblHead);

            int lx = 40, rx = 460, fw = 360;

            // ── Location ────────────────────────────────────────────
            Label l1 = MakeLabel("Location *", lx, 95);
            txtLocation = new TextBox
            {
                Top = 118, Left = lx, Width = fw,
                Font = new Font("Segoe UI", 10)
            };
            SetPlaceholder(txtLocation, "e.g. 12 Main Street, Soweto");

            // ── Category ────────────────────────────────────────────
            Label l2 = MakeLabel("Category *", rx, 95);
            cmbCategory = new ComboBox
            {
                Top = 118, Left = rx, Width = fw,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategory.Items.AddRange(new string[]
            {
                "Sanitation", "Roads", "Utilities", "Electricity", "Water",
                "Public Lighting", "Parks & Recreation", "Waste Removal"
            });

            // ── Description ─────────────────────────────────────────
            Label l3 = MakeLabel("Description *", lx, 160);
            txtDescription = new RichTextBox
            {
                Top = 183, Left = lx, Width = 780, Height = 130,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            // ── Attachment ──────────────────────────────────────────
            Label l4 = MakeLabel("Attachment (optional — max 5 MB)", lx, 330);
            Button btnAttach = new Button
            {
                Text = "📎  Attach File",
                Top = 353, Left = lx, Width = 140, Height = 32,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(230, 240, 255),
                ForeColor = Color.FromArgb(0, 84, 166),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            lblAttachment = new Label
            {
                Text = "No file selected",
                Top = 360, Left = 195, Width = 450,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9)
            };

            // ── Progress bar ─────────────────────────────────────────
            Label l5 = MakeLabel("Completion Progress", lx, 405);
            progress = new ProgressBar
            {
                Top = 428, Left = lx, Width = 780,
                Height = 22, Maximum = 100,
                Style = ProgressBarStyle.Continuous
            };
            lblStatus = new Label
            {
                Top = 455, Left = lx, Width = 780,
                Text = "Fill in all required fields to enable submission.",
                ForeColor = Color.FromArgb(0, 84, 166),
                Font = new Font("Segoe UI", 9, FontStyle.Italic)
            };

            // ── Buttons ─────────────────────────────────────────────
            Button btnSubmit = new Button
            {
                Text = "✔  Submit Report",
                Top = 505, Left = lx, Width = 180, Height = 45,
                BackColor = Color.FromArgb(16, 137, 62),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSubmit.FlatAppearance.BorderSize = 0;

            Button btnBack = new Button
            {
                Text = "← Back",
                Top = 505, Left = 240, Width = 120, Height = 45,
                BackColor = Color.FromArgb(240, 240, 245),
                ForeColor = Color.FromArgb(60, 60, 80),
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            // ── Wire events ─────────────────────────────────────────
            txtLocation.TextChanged += UpdateProgress;
            cmbCategory.SelectedIndexChanged += UpdateProgress;
            txtDescription.TextChanged += UpdateProgress;
            btnAttach.Click += AttachFile;
            btnSubmit.Click += SubmitIssue;
            btnBack.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                header,
                l1, txtLocation,
                l2, cmbCategory,
                l3, txtDescription,
                l4, btnAttach, lblAttachment,
                l5, progress, lblStatus,
                btnSubmit, btnBack
            });
        }

        private Label MakeLabel(string text, int left, int top)
        {
            return new Label
            {
                Text = text,
                Top = top, Left = left,
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 60, 80)
            };
        }

        private void UpdateProgress(object sender, EventArgs e)
        {
            int value = 0;
            if (!string.IsNullOrWhiteSpace(txtLocation.Text))    value += 25;
            if (cmbCategory.SelectedIndex >= 0)                   value += 25;
            if (!string.IsNullOrWhiteSpace(txtDescription.Text)) value += 25;
            if (!string.IsNullOrEmpty(attachmentPath))            value += 25;

            progress.Value = value;

            if (value == 0)
                lblStatus.Text = "Fill in all required fields to enable submission.";
            else if (value < 75)
                lblStatus.Text = $"Good progress — {value}% complete. Keep going!";
            else if (value < 100)
                lblStatus.Text = $"Almost there — {value}% complete!";
            else
                lblStatus.Text = "✔  All fields complete — ready to submit!";
        }

        private void AttachFile(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Title = "Select an image or document",
                Filter = "Images & Documents|*.jpg;*.jpeg;*.png;*.pdf;*.docx;*.txt|All Files|*.*"
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (new FileInfo(dlg.FileName).Length > 5_000_000)
                {
                    MessageBox.Show("File exceeds 5 MB limit. Please select a smaller file.",
                        "File Too Large", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                attachmentPath = dlg.FileName;
                lblAttachment.Text = Path.GetFileName(attachmentPath);
                lblAttachment.ForeColor = Color.FromArgb(16, 137, 62);
                UpdateProgress(null, null);
            }
        }

        private void SubmitIssue(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            { MessageBox.Show("Please enter a location.", "Missing Field", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (cmbCategory.SelectedIndex < 0)
            { MessageBox.Show("Please select a category.", "Missing Field", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            { MessageBox.Show("Please enter a description.", "Missing Field", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var issue = new IssueReport
            {
                ReferenceId    = "MSA-" + DateTime.Now.Ticks.ToString().Substring(10),
                Location       = txtLocation.Text.Trim(),
                Category       = cmbCategory.Text,
                Description    = txtDescription.Text.Trim(),
                AttachmentPath = attachmentPath,
                SubmittedAt    = DateTime.Now,
                Status         = "Pending",
                Priority       = new Random().Next(1, 5)
            };

            AppData.Issues.Add(issue);
            AppData.Tree.Insert(issue);
            AppData.PriorityQueue.Enqueue(issue);

            AppData.Graph.AddEdge(issue.ReferenceId, "Inspection");
            AppData.Graph.AddEdge("Inspection", "Repair");
            AppData.Graph.AddEdge("Repair", "Completed");

            MessageBox.Show(
                $"Issue submitted successfully!\n\nReference ID: {issue.ReferenceId}\nCategory: {issue.Category}\nStatus: Pending",
                "Report Submitted", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Reset form
            txtLocation.Clear();
            cmbCategory.SelectedIndex = -1;
            txtDescription.Clear();
            attachmentPath = "";
            lblAttachment.Text = "No file selected";
            lblAttachment.ForeColor = Color.Gray;
            progress.Value = 0;
            UpdateProgress(null, null);
        }
    }
}
