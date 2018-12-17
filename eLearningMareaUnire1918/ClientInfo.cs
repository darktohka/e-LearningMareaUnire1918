using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eLearningMareaUnire1918 {
    public static class ClientInfo {
        private static int clientId = -1;
        private static string clientName = null;
        private static string clientClass = null;

        public static int GetClientId() {
            return clientId;
        }

        public static string GetClientName() {
            return clientName;
        }

        public static string GetClientClass() {
            return clientClass;
        }

        public static bool IsAuthenticated() {
            return clientId != -1;
        }

        public static void SetClientId(int clientId) {
            ClientInfo.clientId = clientId;
        }

        public static void SetClientName(string clientName) {
            ClientInfo.clientName = clientName;
        }

        public static void SetClientClass(string clientClass) {
            ClientInfo.clientClass = clientClass;
        }
    }
}
