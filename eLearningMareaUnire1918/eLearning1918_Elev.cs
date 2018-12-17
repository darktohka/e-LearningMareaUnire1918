using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace eLearningMareaUnire1918 {
    public partial class eLearning1918_Elev : Form {
        private List<Item> testItems = new List<Item>();
        private static List<Answer> answers = new List<Answer>();
        private int currentItem = 0;
        private int maxItem = 0;
        private double points = 1;

        public eLearning1918_Elev() {
            InitializeComponent();
        }

        private void iesireToolStripMenuItem_Click(object sender, EventArgs e) {
            Close();
        }

        public int GetPoints() {
            return (int) Math.Round(points);
        }

        private void UpdatePunctaj() {
            pointLabel.Text = "Punctaj: " + GetPoints();
        }

        private void startButton_Click(object sender, EventArgs e) {
            tryLabel.Hide();
            itemNrLabel.Show();
            descriptionBox.Show();
            points = 1;
            UpdatePunctaj();

            List<Item> items = Database.GetItems();
            testItems.Clear();
            answers.Clear();

            List<int> requiredItems = new List<int>();
            List<int> indices = new List<int>();

            for (int i = 1; i <= 4; i++) {
                requiredItems.Add(i);
            }

            for (int i = 0; i < items.Count; i++) {
                indices.Add(i);
            }
            
            Random random = new Random();

            while (testItems.Count != 9) {
                int randomIndex = random.Next(0, indices.Count - 1);
                int index = indices[randomIndex];
                Item item = items[index];

                if (requiredItems.Count != 0 && !requiredItems.Contains(item.type)) {
                    continue;
                }

                if (requiredItems.Count != 0) {
                    requiredItems.Remove(item.type);
                }

                indices.RemoveAt(randomIndex);
                testItems.Add(item);
            }

            currentItem = 0;
            maxItem = testItems.Count - 1;
            ShowItem(currentItem);
        }

        private void ShowItem(int index) {
            Item item = testItems[index];
            itemNrLabel.Text = "Item tip " + item.type;
            descriptionBox.Text = item.description;
            HideAllTests();
            respondButton.Show();
            respondButton.Enabled = true;
            nextButton.Hide();

            if (item.type == 1) {
                responseLabel.Show();
                responseBox.Clear();
                responseBox.Show();
            } else if (item.type == 2) {
                if (!item.first.Equals("NULL")) {
                    radioButton1.Show();
                    radioButton1.Text = item.first;
                }
                if (!item.second.Equals("NULL")) {
                    radioButton2.Show();
                    radioButton2.Text = item.second;
                }
                if (!item.third.Equals("NULL")) {
                    radioButton3.Show();
                    radioButton3.Text = item.third;
                }
                if (!item.fourth.Equals("NULL")) {
                    radioButton4.Show();
                    radioButton4.Text = item.fourth;
                }
            } else if (item.type == 3) {
                if (!item.first.Equals("NULL")) {
                    checkBox1.Show();
                    checkBox1.Text = item.first;
                }
                if (!item.second.Equals("NULL")) {
                    checkBox2.Show();
                    checkBox2.Text = item.second;
                }
                if (!item.third.Equals("NULL")) {
                    checkBox3.Show();
                    checkBox3.Text = item.third;
                }
                if (!item.fourth.Equals("NULL")) {
                    checkBox4.Show();
                    checkBox4.Text = item.fourth;
                }
            } else if (item.type == 4) {
                radioButton5.Show();
                radioButton6.Show();
            }
        }

        private void HideAllTests() {
            responseLabel.Hide();
            responseBox.Hide();
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = false;
            radioButton6.Checked = false;
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;
            radioButton1.ForeColor = Color.Black;
            radioButton2.ForeColor = Color.Black;
            radioButton3.ForeColor = Color.Black;
            radioButton4.ForeColor = Color.Black;
            radioButton5.ForeColor = Color.Black;
            radioButton6.ForeColor = Color.Black;
            checkBox1.ForeColor = Color.Black;
            checkBox2.ForeColor = Color.Black;
            checkBox3.ForeColor = Color.Black;
            checkBox4.ForeColor = Color.Black;
            responseBox.ForeColor = Color.Black;
            radioButton1.Hide();
            radioButton2.Hide();
            radioButton3.Hide();
            radioButton4.Hide();
            radioButton5.Hide();
            radioButton6.Hide();
            checkBox1.Hide();
            checkBox2.Hide();
            checkBox3.Hide();
            checkBox4.Hide();
        }

        private void eLearning1918_Elev_Load(object sender, EventArgs e) {
            HideAllTests();
            nextButton.Hide();
            itemNrLabel.Hide();
            descriptionBox.Hide();
            respondButton.Hide();
            ReloadNotes();
            carnetTitle.Text = "Carnetul de note al elevului " + ClientInfo.GetClientName();
        }

        private void respondButton_Click(object sender, EventArgs e) {
            Item item = testItems[currentItem];
            bool correct = true;
            StringBuilder userAnswer = new StringBuilder();
            StringBuilder correctAnswer = new StringBuilder();

            if (item.type == 1) {
                string[] answer = responseBox.Text.Trim().ToLower().Split(new char[] { ' ' });
                StringBuilder builder = new StringBuilder();

                foreach (string ans in answer) {
                    if (!String.IsNullOrWhiteSpace(ans)) {
                        builder.Append(ans);
                        builder.Append(" ");
                    }
                }

                if (builder.Length > 1) {
                    builder.Remove(builder.Length - 1, 1);
                }

                string ourAnswer = builder.ToString();

                if (ourAnswer.Equals(item.correct.ToLower())) {
                    responseBox.ForeColor = Color.Green;
                } else {
                    responseBox.ForeColor = Color.Red;
                    responseBox.Text = item.correct;
                    correct = false;
                }

                userAnswer.Append(responseBox.Text);
                correctAnswer.Append(item.correct);
            } else if (item.type == 3) {
                bool atleastOne = false;
                bool first = item.correct.Contains('1');
                bool second = item.correct.Contains('2');
                bool third = item.correct.Contains('3');
                bool fourth = item.correct.Contains('4');

                if (checkBox1.Checked) {
                    if (first) {
                        checkBox1.ForeColor = Color.Green;
                        atleastOne = true;
                    } else {
                        checkBox1.ForeColor = Color.Red;
                        correct = false;
                    }
                }
                if (checkBox2.Checked) {
                    if (second) {
                        checkBox2.ForeColor = Color.Green;
                        atleastOne = true;
                    } else {
                        checkBox2.ForeColor = Color.Red;
                        correct = false;
                    }
                }
                if (checkBox3.Checked) {
                    if (third) {
                        checkBox3.ForeColor = Color.Green;
                        atleastOne = true;
                    } else {
                        checkBox3.ForeColor = Color.Red;
                        correct = false;
                    }
                }
                if (checkBox4.Checked) {
                    if (fourth) {
                        checkBox4.ForeColor = Color.Green;
                        atleastOne = true;
                    } else {
                        checkBox4.ForeColor = Color.Red;
                        correct = false;
                    }
                }
                if (first && !checkBox1.Checked) {
                    checkBox1.ForeColor = Color.Green;
                    correct = false;
                }
                if (second && !checkBox2.Checked) {
                    checkBox2.ForeColor = Color.Green;
                    correct = false;
                }
                if (third && !checkBox3.Checked) {
                    checkBox3.ForeColor = Color.Green;
                    correct = false;
                }
                if (fourth && !checkBox4.Checked) {
                    checkBox4.ForeColor = Color.Green;
                    correct = false;
                }

                if (first) {
                    correctAnswer.Append(checkBox1.Text);
                    correctAnswer.Append(",");
                }
                if (second) {
                    correctAnswer.Append(checkBox2.Text);
                    correctAnswer.Append(",");
                }
                if (third) {
                    correctAnswer.Append(checkBox3.Text);
                    correctAnswer.Append(",");
                }
                if (fourth) {
                    correctAnswer.Append(checkBox4.Text);
                    correctAnswer.Append(",");
                }

                if (checkBox1.Checked) {
                    userAnswer.Append(checkBox1.Text);
                    userAnswer.Append(",");
                }
                if (checkBox2.Checked) {
                    userAnswer.Append(checkBox2.Text);
                    userAnswer.Append(",");
                }
                if (checkBox3.Checked) {
                    userAnswer.Append(checkBox3.Text);
                    userAnswer.Append(",");
                }
                if (checkBox4.Checked) {
                    userAnswer.Append(checkBox4.Text);
                    userAnswer.Append(",");
                }

                if (atleastOne) {
                    points += 0.5;
                }
            } else if (item.type == 2) {
                if (radioButton1.Checked && !item.correct.Equals("1")) {
                    radioButton1.ForeColor = Color.Red;
                    correct = false;
                }
                if (radioButton2.Checked && !item.correct.Equals("2")) {
                    radioButton2.ForeColor = Color.Red;
                    correct = false;
                }
                if (radioButton3.Checked && !item.correct.Equals("3")) {
                    radioButton3.ForeColor = Color.Red;
                    correct = false;
                }
                if (radioButton4.Checked && !item.correct.Equals("4")) {
                    radioButton4.ForeColor = Color.Red;
                    correct = false;
                }
                if (item.correct.Equals("1")) {
                    radioButton1.ForeColor = Color.Green;
                }
                if (item.correct.Equals("2")) {
                    radioButton2.ForeColor = Color.Green;
                }
                if (item.correct.Equals("3")) {
                    radioButton3.ForeColor = Color.Green;
                }
                if (item.correct.Equals("4")) {
                    radioButton4.ForeColor = Color.Green;
                }

                if (item.correct.Equals("1")) {
                    correctAnswer.Append(radioButton1.Text);
                } else if (item.correct.Equals("2")) {
                    correctAnswer.Append(radioButton2.Text);
                } else if (item.correct.Equals("3")) {
                    correctAnswer.Append(radioButton3.Text);
                } else if (item.correct.Equals("4")) {
                    correctAnswer.Append(radioButton4.Text);
                }

                if (radioButton1.Checked) {
                    userAnswer.Append(radioButton1.Text);
                    userAnswer.Append(",");
                } else if (radioButton2.Checked) {
                    userAnswer.Append(radioButton2.Text);
                    userAnswer.Append(",");
                } else if (radioButton3.Checked) {
                    userAnswer.Append(radioButton3.Text);
                    userAnswer.Append(",");
                } else if (radioButton4.Checked) {
                    userAnswer.Append(radioButton4.Text);
                    userAnswer.Append(",");
                }
            } else if (item.type == 4) {
                if (radioButton5.Checked) {
                    if (item.correct.Equals("1")) {
                        radioButton5.ForeColor = Color.Green;
                    } else {
                        radioButton5.ForeColor = Color.Red;
                        radioButton6.ForeColor = Color.Green;
                        correct = false;
                    }
                }
                if (radioButton6.Checked) {
                    if (item.correct.Equals("0")) {
                        radioButton6.ForeColor = Color.Green;
                    } else {
                        radioButton6.ForeColor = Color.Red;
                        radioButton5.ForeColor = Color.Green;
                        correct = false;
                    }
                }

                if (item.correct.Equals("1")) {
                    correctAnswer.Append("Adevarat");
                } else {
                    correctAnswer.Append("Fals");
                }

                if (radioButton6.Checked) {
                    userAnswer.Append("Fals");
                } else {
                    userAnswer.Append("Adevarat");
                }
            }

            if (correct) {
                points += 1;
            }

            Answer answ = new Answer(item.id, item.type, item.description, userAnswer.ToString(), correctAnswer.ToString());
            answers.Add(answ);
            UpdatePunctaj();
            respondButton.Enabled = false;
            nextButton.Show();
        }

        private void nextButton_Click(object sender, EventArgs e) {
            if (currentItem == maxItem) {
                StringBuilder builder = new StringBuilder();
                builder.Append("Wow! Ai primit ");
                builder.Append(GetPoints().ToString());
                builder.Append(" de puncte! Raportajul:\n\n");

                foreach (Answer answer in answers) {
                    builder.Append("Intrebarea ");
                    builder.Append(answer.id);
                    builder.Append(". de tip ");
                    builder.Append(answer.type);
                    builder.Append("\nAi raspuns: ");
                    builder.Append(answer.userAnswer);
                    builder.Append("\nRaspuns corect: ");
                    builder.Append(answer.correctAnswer);
                    builder.Append("\n\n");
                }

                MessageBox.Show(builder.ToString());
                nextButton.Hide();
                Database.CreateEvaluare(ClientInfo.GetClientId(), DateTime.Now, GetPoints());
                ReloadNotes();
            } else {
                currentItem++;
                ShowItem(currentItem);
            }
        }
        private void pd_PrintPage(object sender, PrintPageEventArgs ev) {
            float yPos = 0;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            string line = null;
            var printFont = new Font("Arial", 10);

            foreach (Note note in Database.LoadNotes(ClientInfo.GetClientId())) {
                line = "Nota " + note.note + " pe " + note.time.ToString();
                yPos = topMargin + (count * printFont.GetHeight(ev.Graphics));
                ev.Graphics.DrawString(line, printFont, Brushes.Black, leftMargin, yPos, new StringFormat());
                count++;
            }

            ev.HasMorePages = false;
        }
        private void button1_Click(object sender, EventArgs e) {
            PrintPreviewDialog dialog = new PrintPreviewDialog();
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(this.pd_PrintPage);
            dialog.Document = pd;
            dialog.ShowDialog();
        }

        private void ReloadNotes() {
            notaView.Rows.Clear();
            var myNotes = chart1.Series[0];
            var classNotes = chart1.Series[1];

            myNotes.Points.Clear();
            classNotes.Points.Clear();

            List<Note> notes = Database.LoadNotes(ClientInfo.GetClientId());

            foreach (Note note in notes) {
                notaView.Rows.Add(note.note, note.time.ToString());
                myNotes.Points.Add(note.note);
            }

            double media = Database.MediaNotelor(ClientInfo.GetClientClass());
            classNotes.Points.Add(media);
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {

        }

        private void testeToolStripMenuItem_Click(object sender, EventArgs e) {
            tabControl1.SelectedIndex = 0;
        }

        private void carnetDeNteToolStripMenuItem_Click(object sender, EventArgs e) {
            tabControl1.SelectedIndex = 1;
        }

        private void graficNoteToolStripMenuItem_Click(object sender, EventArgs e) {
            tabControl1.SelectedIndex = 2;
        }
    }
}
