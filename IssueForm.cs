using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public partial class IssueForm : Form
    {
        TextBox txtLocation;
        ComboBox cmbCategory;
        RichTextBox txtDescription;
        Label lblAttachment;
        Label lblStatus;
        ProgressBar progress;

        string attachmentPath = "";

        public IssueForm()
        {
            Text = "Report Issue";
            Size = new Size(800, 600);
            BackColor = Color.White;

            txtLocation = new TextBox { Top = 80, Left = 50, Width = 300 };

            cmbCategory = new ComboBox
            {
                Top = 140,
                Left = 50,
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cmbCategory.Items.AddRange(new string[]
            {
                "Sanitation","Roads","Utilities","Electricity","Water"
            });

            txtDescription = new RichTextBox
            {
                Top = 200,
                Left = 50,
                Width = 500,
                Height = 120
            };

            Button btnAttach = new Button
            {
                Text = "Attach File",
                Top = 340,
                Left = 50
            };

            lblAttachment = new Label
            {
                Text = "No file selected",
                Top = 345,
                Left = 200,
                Width = 400
            };

            progress = new ProgressBar
            {
                Top = 400,
                Left = 50,
                Width = 500,
                Maximum = 100
            };

            lblStatus = new Label
            {
                Top = 430,
                Left = 50,
                Width = 500
            };

            Button btnSubmit = new Button
            {
                Text = "Submit",
                Top = 480,
                Left = 50,
                BackColor = Color.Green,
                ForeColor = Color.White
            };

            Button btnBack = new Button
            {
                Text = "Back",
                Top = 480,
                Left = 150
            };

            // EVENTS
            txtLocation.TextChanged += UpdateProgress;
            cmbCategory.SelectedIndexChanged += UpdateProgress;
            txtDescription.TextChanged += UpdateProgress;

            btnAttach.Click += AttachFile;
            btnSubmit.Click += SubmitIssue;
            btnBack.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                txtLocation, cmbCategory, txtDescription,
                btnAttach, lblAttachment,
                progress, lblStatus,
                btnSubmit, btnBack
            });
        }

        private void UpdateProgress(object sender, EventArgs e)
        {
            int value = 0;

            if (!string.IsNullOrWhiteSpace(txtLocation.Text)) value += 25;
            if (cmbCategory.SelectedIndex >= 0) value += 25;
            if (!string.IsNullOrWhiteSpace(txtDescription.Text)) value += 25;
            if (!string.IsNullOrEmpty(attachmentPath)) value += 25;

            progress.Value = value;

            lblStatus.Text = value == 100 ? "Ready to submit" : $"Progress: {value}%";
        }

        private void AttachFile(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();

            if (open.ShowDialog() == DialogResult.OK)
            {
                if (new FileInfo(open.FileName).Length > 5000000)
                {
                    MessageBox.Show("Max 5MB");
                    return;
                }

                attachmentPath = open.FileName;
                lblAttachment.Text = Path.GetFileName(attachmentPath);
                UpdateProgress(null, null);
            }
        }

        private void SubmitIssue(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLocation.Text) ||
                cmbCategory.SelectedIndex < 0 ||
                string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Fill all fields");
                return;
            }

            var issue = new IssueReport
            {
                ReferenceId = "MSA-" + DateTime.Now.Ticks.ToString().Substring(10),
                Location = txtLocation.Text,
                Category = cmbCategory.Text,
                Description = txtDescription.Text,
                AttachmentPath = attachmentPath,
                SubmittedAt = DateTime.Now,

                // 🔥 CRÍTICO
                Status = "Pending",
                Priority = new Random().Next(1, 5)
            };

            AppData.Issues.Add(issue);
            AppData.Tree.Insert(issue);
            AppData.PriorityQueue.Enqueue(issue);

            // GRAPH FLOW
            AppData.Graph.AddEdge(issue.ReferenceId, "Inspection");
            AppData.Graph.AddEdge("Inspection", "Repair");
            AppData.Graph.AddEdge("Repair", "Completed");

            MessageBox.Show("Submitted!");

            // reset
            txtLocation.Clear();
            cmbCategory.SelectedIndex = -1;
            txtDescription.Clear();
            attachmentPath = "";
            lblAttachment.Text = "No file selected";
            progress.Value = 0;
        }
    }
}