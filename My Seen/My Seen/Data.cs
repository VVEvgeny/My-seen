﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace My_Seen
{
    public partial class Data : Form
    {
        public Data()
        {
            InitializeComponent();
        }
        private Users user;
        public Users User
        {
            get
            {
                return user;
            }
            set
            {
                user = value;
            }
        }

        private void Data_Load(object sender, EventArgs e)
        {
            toolStripComboBox1.Text = toolStripComboBox1.Items[0].ToString();
        }

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Config form = new Config();
            form.User = User;
            form.ShowDialog();
        }


        private void UpdateListViewColumns(bool films)
        {
            listView1.Columns.Clear();

            ColumnHeader cl_id = new ColumnHeader();
            cl_id.Text = "id";
            cl_id.Width = 10;
            listView1.Columns.Add(cl_id);

            ColumnHeader cl_name = new ColumnHeader();
            cl_name.Text = "Name";
            listView1.Columns.Add(cl_name);

            if (films)
            {
                ColumnHeader cl_date = new ColumnHeader();
                cl_date.Text = "Date";
                listView1.Columns.Add(cl_date);
            }
            else
            {
                ColumnHeader cl_last_ep = new ColumnHeader();
                cl_last_ep.Text = "Last episode";
                listView1.Columns.Add(cl_last_ep);

                ColumnHeader cl_date_last = new ColumnHeader();
                cl_date_last.Text = "Date Last";
                listView1.Columns.Add(cl_date_last);

                ColumnHeader cl_date_begin = new ColumnHeader();
                cl_date_begin.Text = "Date Begin";
                listView1.Columns.Add(cl_date_begin);
            }
            ColumnHeader cl_rate = new ColumnHeader();
            cl_rate.Text = "Rate";
            listView1.Columns.Add(cl_rate);
        }

        private void LoadFilms()
        {
            LoadFilms(string.Empty);
        }
        private void LoadFilms(string filter)
        {
            ModelContainer mc = new ModelContainer();
            foreach (FilmsResult film in 
                mc.FilmsSet.Select(f => new FilmsResult() { Id = -f.Id, Name = f.Name, DateSee = f.DateSee, Rate = f.Rate, UsersId = f.UsersId }).Where(
                    f => f.UsersId == User.Id && filter == string.Empty ? 1 == 1 : f.Name.Contains(filter)
                ).Union(
                mc.Films_NewSet.Select(f => new FilmsResult() { Id = f.Id, Name = f.Name, DateSee = f.DateSee, Rate = f.Rate, UsersId = f.UsersId }).Where(
                    f => f.UsersId == User.Id && filter == string.Empty ? 1 == 1 : f.Name.Contains(filter))
                ).OrderByDescending(t => t.DateSee))
            {
                LoadItemsToListView(film);
            }
        }
        private void LoadItemsToListView(bool films)
        {
            listView1.Items.Clear();
            if (films)
            {
                LoadFilms();
            }
        }
        private void LoadItemsToListView(FilmsResult film)
        {
            LoadItemsToListView(film, false);
        }
        private void LoadItemsToListView(FilmsResult film, bool onetoTop)
        {
            if (onetoTop) listView1.Items.Insert(0, new ListViewItem(new string[] { film.Id.ToString(), film.Name, film.DateSee.ToString(), film.Rate.ToString() }));
            else listView1.Items.Add(new ListViewItem(new string[] { film.Id.ToString(), film.Name, film.DateSee.ToString(), film.Rate.ToString() }));
            
            toolStripStatusLabel2.Text = listView1.Items.Count.ToString();
        }

        private void Edit()
        {
            if (listView1.SelectedItems.Count == 0) return;

            if (toolStripComboBox1.Text == "Films")
            {
                ListViewItem lvi = listView1.SelectedItems[0];

                Add_Film form = new Add_Film();
                form.EditData(lvi.SubItems[0].Text, lvi.SubItems[1].Text, lvi.SubItems[2].Text, lvi.SubItems[3].Text);
                form.ShowDialog();
                if (form.NewFilm != null)
                {
                    ModelContainer mc = new ModelContainer();
                    int f_id=Convert.ToInt32(lvi.SubItems[0].Text);
                    if(f_id>0)
                    {
                        Films_New film = mc.Films_NewSet.First(f => f.Id == f_id);
                        film.UsersId = User.Id;
                        film.Name = form.NewFilm.Name;
                        film.DateSee = form.NewFilm.DateSee;
                        film.Rate = form.NewFilm.Rate;
                    }
                    else
                    {
                        Films film = mc.FilmsSet.First(f => f.Id == -f_id);
                        film.UsersId = User.Id;
                        film.Name = form.NewFilm.Name;
                        film.DateSee = form.NewFilm.DateSee;
                        film.Rate = form.NewFilm.Rate;
                    }
                    lvi.SubItems[1].Text = form.NewFilm.Name;
                    lvi.SubItems[2].Text = form.NewFilm.DateSee.ToString();
                    lvi.SubItems[3].Text = form.NewFilm.Rate.ToString();
                    mc.SaveChanges();
                }
                form.Close();
            }
            else
            {

            }
        }
        private void Add()
        {
            if (toolStripComboBox1.Text == "Films")
            {
                Add_Film form = new Add_Film();
                form.ShowDialog();
                if (form.NewFilm != null)
                {
                    ModelContainer mc = new ModelContainer();
                    Films_New film = new Films_New() { UsersId = User.Id, Name = form.NewFilm.Name, DateSee = form.NewFilm.DateSee, Rate = form.NewFilm.Rate };
                    mc.Films_NewSet.Add(film);
                    LoadItemsToListView(new FilmsResult(film), true);
                    mc.SaveChanges();
                }
                form.Close();
            }
            else
            {

            }
        }
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("11+" + toolStripComboBox1.Text);
            UpdateListViewColumns(toolStripComboBox1.Text == "Films");
            LoadItemsToListView(toolStripComboBox1.Text == "Films");
        }

        private bool FastFind = false;
        private void fastFindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(!FastFind)
            {
                FastFind = true;
                toolStripComboBox2.Visible = true;
                toolStripComboBox2.Text = "";
                toolStripComboBox2.Items.Clear();
                foreach (ListViewItem lvi in listView1.Items) toolStripComboBox2.Items.Add(lvi.SubItems[1].Text);
            }
            else
            {
                FastFind = false;
                toolStripComboBox2.Visible = false;
                toolStripComboBox2.Text = "";
            }
        }

        private void toolStripComboBox2_TextChanged(object sender, EventArgs e)
        {
            if (toolStripComboBox1.Text == "Films")
            {
                listView1.Items.Clear();
                LoadFilms(toolStripComboBox2.Text);
            }
        }

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Add();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Edit();
        }

        private void addToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Add();
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Edit();
        }
    }
}
