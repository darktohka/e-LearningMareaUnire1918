using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eLearningMareaUnire1918 {
    public partial class eLearning1918_start : Form {

        private Timer timer = null;
        private List<Bitmap> images = new List<Bitmap>();
        private int currentImage = 0;
        private int maxImage = 0;

        public eLearning1918_start() {
            InitializeComponent();
        }

        private void LoadImages() {
            images.Add(Properties.Resources._1);
            images.Add(Properties.Resources._2);
            images.Add(Properties.Resources._3);
            images.Add(Properties.Resources._4);
            images.Add(Properties.Resources._5);
            maxImage = images.Count - 1;
        }

        private void StopTimer() {
            if (timer != null) {
                timer.Stop();
                timer = null;
            }
        }

        private void StartAutomatic() {
            backButton.Enabled = false;
            nextButton.Enabled = false;
            manualButton.Text = "Manual";

            StopTimer();
            timer = new Timer();
            timer.Tick += timer_Tick;
            timer.Interval = 2000;
            timer.Start();
        }

        private void StartManual() {
            backButton.Enabled = true;
            nextButton.Enabled = true;
            manualButton.Text = "Auto";

            StopTimer();
        }

        private void timer_Tick(object sender, EventArgs e) {
            currentImage++;

            if (currentImage > maxImage) {
                currentImage = 0;
            }

            SetCurrentImage(currentImage);
        }

        private void SetCurrentImage(int currentImage) {
            slideshowBar.Maximum = maxImage;
            slideshowBar.Value = currentImage;
            slideshow.Image = images[currentImage];
        }

        private void eLearning1918_start_Load(object sender, EventArgs e) {
            LoadImages();
            StartAutomatic();
        }

        private void manualButton_Click(object sender, EventArgs e) {
            if (timer == null) {
                StartAutomatic();
            } else {
                StartManual();
            }
        }

        private void backButton_Click(object sender, EventArgs e) {
            currentImage--;

            if (currentImage < 0) {
                currentImage = maxImage;
            }

            SetCurrentImage(currentImage);
        }

        private void nextButton_Click(object sender, EventArgs e) {
            currentImage++;

            if (currentImage > maxImage) {
                currentImage = 0;
            }

            SetCurrentImage(currentImage);
        }

        private void loginButton_Click(object sender, EventArgs e) {
            string email = emailBox.Text;
            string password = passwordBox.Text;

            if (String.IsNullOrWhiteSpace(email) || String.IsNullOrWhiteSpace(password) || !Database.AuthenticateUser(email, password)) {
                MessageBox.Show("Eroare de autentificare!");
                emailBox.Clear();
                passwordBox.Clear();
                return;
            }

            Hide();
            eLearning1918_Elev elev = new eLearning1918_Elev();
            elev.ShowDialog();
            Close();
        }
    }
}
