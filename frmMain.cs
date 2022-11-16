using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pryPonssaSP4
{
    public partial class frmMain : Form
    {
        public struct DATOS
        {
            public int participante;
            public int puntos;
        }
        public frmMain()
        {
            InitializeComponent();
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            // inicializar formulario
            int i;
            // agregar 6 filas a la grilla de participantes
            for (i = 1; i <= 6; i++)
            {
                // asignar en cada fila sólo el número de participante (i)
                dgvParticipantes.Rows.Add(i.ToString());
            }
            // configurar la primera columna para que sea de sólo lectura
            dgvParticipantes.Columns[0].ReadOnly = true;
            // deshabilitar la grilla de posiciones
            dgvPosiciones.Enabled = false;
        }
        private void btnAsignarParticipantes_Click(object sender, EventArgs e)
        {
            int i;
            dgvPosiciones.Rows.Clear();
            // verificar que estén cargados todos los participantes
            for (i = 0; i < 6; i++)
            {
                if (dgvParticipantes.Rows[i].Cells[1].Value == null ||
                dgvParticipantes.Rows[i].Cells[2].Value == null)
                {
                    MessageBox.Show("Debe completar los datos de todos los participantes",
                    "Error", MessageBoxButtons.OK);
                    return;
                }
            }
            // configurar las columnas de la grilla
            for (i = 0; i < 6; i++)
            {
                dgvPosiciones.Columns[i + 1].HeaderText =
                dgvParticipantes.Rows[i].Cells[1].Value.ToString();
            }
            // configurar las filas con las 3 pruebas del Triatlón
            for (i = 0; i < 3; i++)
            {
                dgvPosiciones.Rows.Add();
            }
            dgvPosiciones.Rows[0].Cells[0].Value = "Natación";
            dgvPosiciones.Rows[1].Cells[0].Value = "Ciclismo";
            dgvPosiciones.Rows[2].Cells[0].Value = "Carrera";
            dgvPosiciones.Columns[0].ReadOnly = true;
            dgvParticipantes.ClearSelection();
            dgvPosiciones.Enabled = true;
        }
        private void btnDeterminarGanadores_Click(object sender, EventArgs e)
        {
            int fila;
            int col;
            int i;
            string valor;
            string nombre;
            string nacionalidad;
            // controlar los datos ingresados en la grilla de posiciones
            if (!ValidarPosiciones())
            {
                MessageBox.Show("Los datos son incorrectos, revise las posiciones asignadas",
                "Error", MessageBoxButtons.OK);
                return;
            }
            dgvPosiciones.ClearSelection();
            int[,] puestos = new int[3, 6]; // arreglo bidimensional
                                            // copiar el contenido de la grilla al arreglo bidimensional
            for (fila = 0; fila < 3; fila++) // se recorren las filas de la grilla
            {
                for (col = 0; col < 6; col++) // se recorren las columnas de la grilla
                {
                    // obtener el valor de cada celda
                    valor = dgvPosiciones.Rows[fila].Cells[col + 1].Value.ToString();
                    puestos[fila, col] = int.Parse(valor); // convertirlo a entero
                }
            }

            // inicializar el arreglo para computar los puntos de cada participante
            DATOS[] datos = new DATOS[6];
            for (i = 0; i < 6; i++)
            {
                datos[i].participante = i + 1; // número de participante
                datos[i].puntos = 0; // cantidad de puntos del participante
            }
            // recorrer el arreglo bidimensional de los puestos y sumar los puntos
            for (col = 0; col < 6; col++) // las columnas son los participantes
            {
                // las filas son las posiciones obtenidas por cada participante
                for (fila = 0; fila < 3; fila++)
                {
                    datos[col].puntos += ObtenerPuntos(puestos[fila, col]);
                }
            }
            // arreglo de tipo DATOS para los ganadores
            DATOS[] ganadores = new DATOS[3];
            // obtener el primer puesto
            ganadores[0] = BuscarMayorPuntaje(datos, ganadores);
            // obtener el segundo puesto
            ganadores[1] = BuscarMayorPuntaje(datos, ganadores);
            // obtener el tercer puesto
            ganadores[2] = BuscarMayorPuntaje(datos, ganadores);

            // cargar la grilla de ganadores
            dgvGanadores.Rows.Clear();
            for (fila = 0; fila < 3; fila++)
            {
                dgvGanadores.Rows.Add(); // se agrega una nueva fila
                dgvGanadores.Rows[fila].Cells[0].Value = fila + 1; // posición final
                                                                   // obtener la fila que ocupa el participante en la grilla 1
                int f = ganadores[fila].participante - 1;
                // obtener el nombre del participante
                nombre = dgvParticipantes.Rows[f].Cells[1].Value.ToString();
                dgvGanadores.Rows[fila].Cells[1].Value = nombre;
                // obtener la nacionalidad del participante
                nacionalidad = dgvParticipantes.Rows[f].Cells[2].Value.ToString();
                dgvGanadores.Rows[fila].Cells[2].Value = nacionalidad;
                // puntos obtenidos por el participante
                dgvGanadores.Rows[fila].Cells[3].Value = ganadores[fila].puntos.ToString();
            }
            dgvGanadores.ClearSelection();
        }
        private bool ValidarPosiciones()
        {
            int f;
            int c;
            int i;
            int valor;
            bool resultado = true;
            for (f = 0; f < dgvPosiciones.RowCount; f++) // recorre las filas de la grilla
            {
                for (c = 1; c < dgvPosiciones.ColumnCount; c++) // recorre las columnas de la grilla
                {
                    valor = int.Parse(dgvPosiciones.Rows[f].Cells[c].Value.ToString());
                    if (valor < 1 || valor > 6) // deben ser valores entre 1 y 6
                    {
                        resultado = false;
                        break;
                    }
                    // verificar que el valor de esa celda no esté repetido
                    // en las siguientes posiciones de la fila
                    for (i = c + 1; i < dgvPosiciones.ColumnCount; i++)
                    {
                        if (valor == int.Parse(dgvPosiciones.Rows[f].Cells[i].Value.ToString()))
                        {
                            resultado = false;
                            break;
                        }
                    }
                }
            }
            return resultado;
        }
        private int ObtenerPuntos(int puesto)
        {
            // arreglo con los puntos por el puesto en cada prueba:
            // puesto 1: 1000 puntos
            // puesto 2: 920 puntos
            // puesto 3: 830 puntos
            // puesto 4: 740 puntos
            // puesto 5: 650 puntos
            // puesto 6: 560 puntos
            int[] puntos = new int[6] { 1000, 920, 830, 740, 650, 560 };
            // se devuele el valor del elemento puesto-1
            return puntos[puesto - 1];
        }
        private DATOS BuscarMayorPuntaje(DATOS[] d, DATOS[] g)
        {
            DATOS resultado;
            int i;
            int f;
            int ganadorPrevio;
            resultado.participante = 0;
            resultado.puntos = 0;
            for (i = 0; i < 6; i++)
            {
                if (d[i].puntos > resultado.puntos)
                {
                    ganadorPrevio = 0;
                    for (f = 0; f < g.Length; f++) // recorre 'g' (ganadores)
                    {
                        if (d[i].participante == g[f].participante)
                        {
                            ganadorPrevio = 1;
                        }
                    }
                    if (ganadorPrevio == 0)
                    {
                        resultado.puntos = d[i].puntos;
                        resultado.participante = d[i].participante;
                    }
                }
            }
            return resultado;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
