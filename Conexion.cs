using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace Punto.Forms
{
    internal class Conexion
    {
        private readonly string Cadena;

        public Conexion()
        {
            Cadena = "Server=127.0.0.1;Database=PuntoDB;Uid=root;Pwd=;Port=3306;";
        }

        public MySqlConnection GetConeccion()
        {
            try
            {
                MySqlConnection con = new MySqlConnection(Cadena);
                con.Open();
                return con;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al conectar con MySQL:\n" + ex.Message);
                return null;
            }
        }
    }
}