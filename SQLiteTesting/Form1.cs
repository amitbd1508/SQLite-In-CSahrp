using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteTesting
{
    public partial class Form1 : Form
    {
        Bitmap img;
        string conn;
        SQLiteConnection sqlconn;
        private SQLiteDataAdapter DB;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();
        private DataGrid Grid;
        public Form1()
        {

            InitializeComponent();
            
            cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            MessageBox.Show("You are using 30days! trial version !!!!");
            conn = @"Data Source =resturent.db; Version=3;New=False;Compress=True;";
            img = new Bitmap(SQLiteTesting.Properties.Resources.icon_choose);
            pictureBox1.Image = img;
            sqlconn = new SQLiteConnection(conn);
            try
            {
                sqlconn.Open();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            selectAndFill();
            
        }
       
        
       
		
        private void selectAndFill()
        {
            string ct = "select * from products";
            DB = new SQLiteDataAdapter(ct, sqlconn);
            DS.Reset();
            DB.Fill(DS);
            DT = DS.Tables[0];
            grid.DataSource = DT;
        }
        void insert()
        {

            
            string sql;
            if (pictureBox1.Image != img)
            {
                
                //sql = @"insert into products (name, description,price,type,image) values ('" + txtName.Text + "', '" + rtxt.Text + TxtPrice.Text + "', '" + cmbType.SelectedItem + "');";
               // sql = @"insert into products (name, description,price,type,image) values ('" + txtName.Text + "', '" + rtxt.Text + "', '" + TxtPrice.Text + "', '" + cmbType.SelectedItem + "', '" + bitmapToString(new Bitmap(pictureBox1.Image)) + "');";
                //MessageBox.Show(sql);
                var data = ImageToByteArray(pictureBox1.Image);
                using (var cmd = new SQLiteCommand("INSERT INTO products (name, description,price,type,image) values(@name,@description,@price,@type, @image)", sqlconn))
                {
                    cmd.Parameters.Add("@image", DbType.Binary).Value = data;
                    cmd.Parameters.Add("@name", DbType.String).Value = txtName.Text;
                    cmd.Parameters.Add("@description", DbType.String).Value = rtxt.Text;
                    cmd.Parameters.Add("@price", DbType.String).Value = TxtPrice.Text;
                    cmd.Parameters.Add("@type", DbType.String).Value = cmbType.Text;
              
                    cmd.ExecuteNonQuery();
                    selectAndFill();
                    pictureBox1.Image = img;
                    return;
                }
                
            }
            else
                sql = @"insert into products (name, description,price,type) values ('" + txtName.Text + "', '" + rtxt.Text + "', '" + TxtPrice.Text + "', '" + cmbType.SelectedItem + "');";
            //MessageBox.Show(sql);
            SQLiteCommand ss = new SQLiteCommand(sql, sqlconn);
            ss.ExecuteNonQuery();
            selectAndFill();
            pictureBox1.Image = img;
            
        }
        public static byte[] ImageToByteArray(Image imageIn)
        {
            var ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            var ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "All Files|*.*";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    
                    pictureBox1.Image = new Bitmap(dlg.FileName);

                    
                }
            }

        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (checkPrice()&&txtName.Text != "" && TxtPrice.Text != "" )
            {
                insert();
                pictureBox1.Image = img;
                rtxt.Text = "";
                txtName.Text = "";
                TxtPrice.Text = "";
            }
            else MessageBox.Show("Check Name ,Type and Price ");
          
        }

        private bool checkPrice()
        {

            try
            {
                decimal i = Convert.ToDecimal(TxtPrice.Text.ToString());
                return true;

            }
            catch
            {
               
                return false;
                
            }
            
        }
    }
}
