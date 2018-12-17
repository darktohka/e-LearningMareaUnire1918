using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearningMareaUnire1918 {
    public class Answer {
        public int id { get; set; }
        public int type { get; set; }
        public string description { get; set; }
        public string userAnswer { get; set; }
        public string correctAnswer { get; set; }

        public Answer(int id, int type, string description, string userAnswer, string correctAnswer) {
            this.id = id;
            this.type = type;
            this.description = description;
            this.userAnswer = userAnswer;
            this.correctAnswer = correctAnswer;
        }
    }
}
