using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace Punto.Forms
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();

            btnLogin.Click += btnLogin_Click;
            txtPassword.PasswordChar = '*';
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string usuario = txtUser.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Ingresa usuario y contraseña.");
                return;
            }

            try
            {
                Conexion conexionClase = new Conexion();

                using (MySqlConnection con = conexionClase.GetConeccion())
                {
                    if (con == null) return;

                    string consulta = "SELECT nombre_completo FROM usuarios WHERE username = @usuario AND password = @password";

                    using (MySqlCommand cmd = new MySqlCommand(consulta, con))
                    {
                        cmd.Parameters.AddWithValue("@usuario", usuario);
                        cmd.Parameters.AddWithValue("@password", password);

                        object resultado = cmd.ExecuteScalar();

                        if (resultado != null)
                        {
                            MessageBox.Show("Bienvenido " + resultado.ToString());

                            frmPrincipal principal = new frmPrincipal();
                            principal.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Usuario o contraseña incorrectos.");
                            txtPassword.Clear();
                            txtUser.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message);
            }
        }
    }
}