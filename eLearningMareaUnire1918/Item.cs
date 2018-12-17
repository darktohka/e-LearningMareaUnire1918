
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearningMareaUnire1918 {
    public class Item {
        public int id { get; set; }
        public int type { get; set; }
        public string description { get; set; }
        public string first { get; set; }
        public string second { get; set; }
        public string third { get; set; }
        public string fourth { get; set; }
        public string correct { get; set; }

        public Item(int id, int type, string description, string first, string second, string third, string fourth, string correct) {
            this.id = id;
            this.type = type;
            this.description = description;
            this.first = first;
            this.second = second;
            this.third = third;
            this.fourth = fourth;
            this.correct = correct;
        }
    }

}
