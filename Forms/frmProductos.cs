using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace Punto.Forms
{
    public partial class frmProductos : Form
    {
        int IdProducSelecc = 0;

        public frmProductos()
        {
            InitializeComponent();

            this.Load += frmProductos_Load;
            btnNuevo.Click += btnNuevo_Click;
            btnEditar.Click += btnEditar_Click;
            btnEliminar.Click += btnEliminar_Click;
            dgvProductos.CellClick += dgvProductos_CellClick;
            txtBusqueda.TextChanged += txtBusqueda_TextChanged;
        }

        private void frmProductos_Load(object sender, EventArgs e)
        {
            CargarProductos();
        }

        private void CargarProductos()
        {
            try
            {
                Conexion conClase = new Conexion();

                using (MySqlConnection con = conClase.GetConeccion())
                {
                    if (con == null) return;

                    string query = "SELECT producto_id, codigo, descripcion, precio, stock FROM productos";

                    MySqlDataAdapter adaptador = new MySqlDataAdapter(query, con);
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    dgvProductos.DataSource = tabla;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message);
            }
        }

        private bool ValidarCampos()
        {
            if (txtCodigo.Text == "" || txtNombre.Text == "" || txtPrecio.Text == "" || txtStock.Text == "")
            {
                MessageBox.Show("Completa todos los campos.");
                return false;
            }

            if (!decimal.TryParse(txtPrecio.Text, out _))
            {
                MessageBox.Show("El precio debe ser numérico.");
                return false;
            }

            if (!int.TryParse(txtStock.Text, out _))
            {
                MessageBox.Show("El stock debe ser un número entero.");
                return false;
            }

            return true;
        }

        private void Limpiar()
        {
            txtCodigo.Clear();
            txtNombre.Clear();
            txtPrecio.Clear();
            txtStock.Clear();
            IdProducSelecc = 0;
            txtCodigo.Focus();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                Conexion conClase = new Conexion();

                using (MySqlConnection con = conClase.GetConeccion())
                {
                    if (con == null) return;

                    string sql = @"INSERT INTO productos 
                                   (codigo, descripcion, precio, stock) 
                                   VALUES 
                                   (@codigo, @descripcion, @precio, @stock)";

                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@codigo", txtCodigo.Text.Trim());
                        cmd.Parameters.AddWithValue("@descripcion", txtNombre.Text.Trim());
                        cmd.Parameters.AddWithValue("@precio", decimal.Parse(txtPrecio.Text));
                        cmd.Parameters.AddWithValue("@stock", int.Parse(txtStock.Text));

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Producto registrado correctamente.");
                CargarProductos();
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar producto: " + ex.Message);
            }
        }

        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvProductos.Rows[e.RowIndex];

                IdProducSelecc = Convert.ToInt32(fila.Cells["producto_id"].Value);
                txtCodigo.Text = fila.Cells["codigo"].Value.ToString();
                txtNombre.Text = fila.Cells["descripcion"].Value.ToString();
                txtPrecio.Text = fila.Cells["precio"].Value.ToString();
                txtStock.Text = fila.Cells["stock"].Value.ToString();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (IdProducSelecc == 0)
            {
                MessageBox.Show("Selecciona un producto primero.");
                return;
            }

            if (!ValidarCampos()) return;

            try
            {
                Conexion conClase = new Conexion();

                using (MySqlConnection con = conClase.GetConeccion())
                {
                    if (con == null) return;

                    string sql = @"UPDATE productos SET 
                                   codigo = @codigo,
                                   descripcion = @descripcion,
                                   precio = @precio,
                                   stock = @stock
                                   WHERE producto_id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@codigo", txtCodigo.Text.Trim());
                        cmd.Parameters.AddWithValue("@descripcion", txtNombre.Text.Trim());
                        cmd.Parameters.AddWithValue("@precio", decimal.Parse(txtPrecio.Text));
                        cmd.Parameters.AddWithValue("@stock", int.Parse(txtStock.Text));
                        cmd.Parameters.AddWithValue("@id", IdProducSelecc);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Producto modificado correctamente.");
                CargarProductos();
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar producto: " + ex.Message);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (IdProducSelecc == 0)
            {
                MessageBox.Show("Selecciona un producto primero.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Seguro que deseas eliminar este producto?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (respuesta == DialogResult.No) return;

            try
            {
                Conexion conClase = new Conexion();

                using (MySqlConnection con = conClase.GetConeccion())
                {
                    if (con == null) return;

                    string sql = "DELETE FROM productos WHERE producto_id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@id", IdProducSelecc);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Producto eliminado correctamente.");
                CargarProductos();
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar producto: " + ex.Message);
            }
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Conexion conClase = new Conexion();

                using (MySqlConnection con = conClase.GetConeccion())
                {
                    if (con == null) return;

                    string query = @"SELECT producto_id, codigo, descripcion, precio, stock 
                                     FROM productos
                                     WHERE codigo LIKE @buscar 
                                     OR descripcion LIKE @buscar";

                    MySqlDataAdapter adaptador = new MySqlDataAdapter(query, con);
                    adaptador.SelectCommand.Parameters.AddWithValue("@buscar", "%" + txtBusqueda.Text.Trim() + "%");

                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    dgvProductos.DataSource = tabla;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar producto: " + ex.Message);
            }
        }
    }
}