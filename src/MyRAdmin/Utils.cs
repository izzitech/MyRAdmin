using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyRAdmin {
	class Utils {
		public bool IsUp(string address)
		{
			bool isUp = false;
			Ping ping = new Ping();
			try {
				PingReply reply = ping.Send(address, 1000);
				if (reply.Status == IPStatus.Success) isUp = true;
			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex.Message);
				isUp = false;
			}
			return isUp;
		}

		public bool IsPortUp(string address, int port)
		{
			bool isUp = false;
			TcpClient tcpScanner = new TcpClient();

			try {
				tcpScanner.SendTimeout = 3000;
				tcpScanner.ReceiveTimeout = 3000;
				tcpScanner.Connect(address, port);
				isUp = true;
			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex.Message);
				isUp = false;
			}
			finally {
				tcpScanner.Close();
			}
			return isUp;
		}
	}
}
