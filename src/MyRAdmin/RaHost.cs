namespace MyRAdmin {
	public class RaHost {
		public string ID;
		public string Name;
		public string IP;
		public ushort Port;
		public string Sector;
		public string Tipo;
		public string User;
		public string Details;

		public override string ToString()
		{
			string toReturn =
$@"
ID: { ID }
Name: { Name }
IP: { IP }
Port: { Port }
Sector: { Sector }
Tipo: { Tipo }
User: { User }
Details: { Details }
";

			return toReturn;
		}
	}
}
