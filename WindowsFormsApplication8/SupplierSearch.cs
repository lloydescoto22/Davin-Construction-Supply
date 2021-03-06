﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApplication8
{
    public partial class SupplierSearch : Form
    {
        MySqlConnection conn;
        string connectionString = "server=localhost;userid=root;password=;database=hardwaredatabase";
        string caption = "Invalid Input Detected";
        public SupplierSearch()
        {
            InitializeComponent();
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Font = new System.Drawing.Font("Arial", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            button1.MouseEnter += OnMouseEnterButton1;
            button1.MouseLeave += OnMouseLeaveButton1;
            button2.MouseEnter += OnMouseEnterButton2;
            button2.MouseLeave += OnMouseLeaveButton2;
            panel1.MouseDown += panel1_MouseDown;
            panel1.MouseUp += panel1_MouseUp;
            panel1.MouseMove += panel1_MouseMove;
            dataGridView1.Columns["SupplierEmail"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["SupplierContact"].SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private bool _dragging = false;
        private Point _start_point = new Point(0, 0);

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _start_point = new Point(e.X, e.Y);
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - this._start_point.X, p.Y - this._start_point.Y);
            }
        }

        private void SupplierSearch_Load(object sender, EventArgs e)
        {
            conn = new MySqlConnection(connectionString);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            MySqlDataReader reader;
            cmd.CommandText = "SELECT * FROM Suppliers";
            using (reader = cmd.ExecuteReader())
            {
                while(reader.Read())
                {
                    dataGridView1.Rows.Add(reader["SupplierName"], reader["SupplierEmail"], reader["SupplierContactNumber"], reader["SupplierAddress"]);
                }
                reader.Close();
            }
        }

        //Hover - change color
        private void OnMouseEnterButton1(object sender, EventArgs e)
        {
            button1.BackColor = Color.SeaGreen;
            button1.FlatAppearance.BorderColor = Color.White;
            button1.ForeColor = Color.White;
        }
        private void OnMouseLeaveButton1(object sender, EventArgs e)
        {
            button1.BackColor = Color.SeaGreen;
            button1.FlatAppearance.BorderColor = Color.White;
            button1.ForeColor = Color.White;
        }
        private void OnMouseEnterButton2(object sender, EventArgs e)
        {
            button2.BackColor = Color.SeaGreen;
            button2.FlatAppearance.BorderColor = Color.MediumAquamarine;
            button2.ForeColor = Color.White;
        }
        private void OnMouseLeaveButton2(object sender, EventArgs e)
        {
            button2.BackColor = Color.MintCream;
            button2.FlatAppearance.BorderColor = Color.SeaGreen;
            button2.ForeColor = Color.Black;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int indexpass = dataGridView1.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridView1.Rows[indexpass];
                for (int i = 0; i < Application.OpenForms.Count; i++)
                {
                    if (Application.OpenForms[i].Name == "AddItem")
                    {
                        AddItem addItemForm = (AddItem)Application.OpenForms[i];
                        if (string.IsNullOrEmpty(addItemForm.textBox5.Text) || addItemForm.textBox5.ReadOnly == false)
                        {
                            addItemForm.textBox5.Text = selectedRow.Cells["SupplierName"].Value.ToString();
                            addItemForm.textBox7.Text = selectedRow.Cells["SupplierEmail"].Value.ToString();
                            addItemForm.textBox6.Text = selectedRow.Cells["SupplierContact"].Value.ToString();
                            addItemForm.textBox8.Text = selectedRow.Cells["SupplierAddress"].Value.ToString();
                            this.Close();
                        }
                        else
                        {
                            string message = "Supplier is already ";
                            MessageBoxButtons buttons = MessageBoxButtons.OK;
                            DialogResult result;

                            result = MessageBox.Show(message, caption, buttons, MessageBoxIcon.Error);
                            if (result == DialogResult.OK)
                            {
                                this.Close();
                            }
                        }
                    }
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtSearch.Clear();
            txtSearch.ForeColor = Color.Black;
        }

        private void Search()
        {
            conn = new MySqlConnection(connectionString);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            MySqlDataReader reader;
            dataGridView1.Rows.Clear();
            cmd.CommandText = "SELECT * FROM suppliers WHERE SupplierName like '%" + txtSearch.Text + "%'";
            cmd.Parameters.AddWithValue("@supname", txtSearch.Text);
            using (reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        cmd.Parameters.Clear();
                        dataGridView1.Rows.Add(reader["SupplierName"], reader["SupplierEmail"], reader["SupplierContactNumber"], reader["SupplierAddress"]);
                    }
                }
                else
                    //MessageBox.Show("Value did not match any record.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                reader.Close();
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            Search();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            button2.PerformClick();
        }
    }
}
