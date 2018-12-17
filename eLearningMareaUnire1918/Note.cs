using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearningMareaUnire1918 {
    public class Note {
        public int note { get; set; }
        public DateTime time { get; set; }

        public Note(int note, DateTime time) {
            this.note = note;
            this.time = time;
        }
    }
}
