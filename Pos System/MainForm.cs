using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bunifu.UI.WinForm;
using Bunifu.UI.WinForms;
using System.Data.OleDb;
using Bunifu.UI.WinForms.BunifuButton;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing.Printing;
using System.Diagnostics;
using Microsoft.Reporting.WinForms;
using System.Threading;
using System.Data.SQLite;

namespace Pos_System
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

        }
        public static string shop_name = "";
        public static string shop_phone = "";
        public static string shop_phone2 = "";
        public static string shop_address = "";
        void LoadRestaurantInfo()
        {
            using (cmd = new SQLiteCommand("select  * from shop_info limit 1", connect))
            {
                read = cmd.ExecuteReader();
                read.Read();
                if (read.HasRows)
                {
                    shop_name = read[0].ToString();
                    shop_address = read[1].ToString();
                    shop_phone = read[2].ToString();
                    shop_phone2 = read[3].ToString();

                    text_ResName.Text = read[0].ToString();
                    text_ResAddress.Text = read[1].ToString();
                    text_ResPhone.Text = read[2].ToString();
                    text_ResPhone2.Text = read[3].ToString();

                    if (!read.IsDBNull(4))
                    {
                        byte[] byt = (byte[])read[4];
                        MemoryStream stream = new MemoryStream(byt);
                        Logo.Image = Image.FromStream(stream);
                        PictureBoxLogo.Image = Image.FromStream(stream);
                    }
                    LabelTitle.Text = read[0].ToString();
                    this.Text = read[0].ToString();
                   
                }

              
                read.Close();
            }
        }
        void LoadAndSetUpTables()
        {


            adapt = new  SQLiteDataAdapter("select item_name from items", connect);
            adapt.Fill(itemstable);
            adapt.Dispose();

            adapt = new  SQLiteDataAdapter("select * from products " + ComboItemName2.Text + "", connect);
            adapt.Fill(productstable);
            adapt.Dispose();

            saleadapt = new  SQLiteDataAdapter("select * from sales limit " + displaysalescount + "", connect);
            saleadapt.Fill(salestable);
            saleadapt.FillSchema(salestable, SchemaType.Mapped);
            adapt.Dispose();

            adapt = new  SQLiteDataAdapter("select [purchase_name] from [purchases]", connect);
            adapt.Fill(purchasestable);
            adapt.FillSchema(purchasestable, SchemaType.Mapped);
            adapt.Dispose();

            adapt = new  SQLiteDataAdapter("select  * from [wallet] limit  " + displaysalescount + "", connect);
            adapt.Fill(wallettable);
            //adapt.FillSchema(wallettable, SchemaType.Mapped);
            adapt.Dispose();

            //using ( SQLiteDataAdapter adapt = new  SQLiteDataAdapter("select * from login", MainForm.connect))
            //{
            //    adapt.Fill(accountstable);
            //    adapt.FillSchema(accountstable, SchemaType.Mapped);
            //}


            using (cmd = new SQLiteCommand("select max(invoice_number) from invoice",connect))
            {
                read = cmd.ExecuteReader();
                read.Read();
                if (!read.IsDBNull(0))
                    InvoiceNum.Text = read[0].ToString();
                else
                    InvoiceNum.Text = "";

                read.Close();
            }

          

            ComboItemName.DataSource = itemstable;
            ComboItemName.DisplayMember = "item_name";

            ComboItemName2.DataSource = itemstable;
            ComboItemName2.DisplayMember = "item_name";


            ComboProductName.DataSource = productstable;
            ComboProductName.DisplayMember = "product_name";

            ComboPurchaseName.DataSource = purchasestable;
            ComboPurchaseName.DisplayMember = "purchase_name";

            

            DataGridSales.DataSource = salestable;
            DataGridSales.Columns[0].HeaderText = "رقم الفاتورة";
            DataGridSales.Columns[1].HeaderText = "اسم المنتج";
            DataGridSales.Columns[2].HeaderText = "الاجمالي";
            DataGridSales.Columns[3].HeaderText = "تاريخ الاصدار";
            DataGridSales.Columns[4].HeaderText = "اسم المستخدم";


            DataGridWallet.DataSource = wallettable;

            DataGridWallet.Columns[0].HeaderText = "رقم العملية";
            DataGridWallet.Columns[1].HeaderText = "اسم المستخدم";
            DataGridWallet.Columns[2].HeaderText = "المبلغ";
            DataGridWallet.Columns[3].HeaderText = "نوع العملية";
            DataGridWallet.Columns[4].HeaderText = "تاريخ العملية";

            invoice_number = InvoiceNum.Text;

            Sales sale = new Sales(connect);
            pagesnumber = (sale.GetAllSalesCount() / displaysalescount);
            LabelPages.Text = pagesnumber + "/" + "1";

            Wallet w = new Wallet(connect);
            walletpagenumber = (w.GetAllWalletCount() / displaysalescount);
            LabelWalletPageNumber.Text = walletpagenumber + "/" + "1";



            ShowTotalPriceByDate();
            ShowAllTotalPrice();
            ShowAllProfits();
            ShowProfitsByDate();



        }
        int displaysalescount = 500;
        int pagesnumber = 0;
        int walletpagenumber = 0;
        public static SQLiteConnection connect = new SQLiteConnection(@"Data Source=database\posdata.db;Version=3;");
        public static SQLiteTransaction trans;
         SQLiteDataAdapter adapt;
         SQLiteDataAdapter saleadapt;
        DataTable itemstable = new DataTable();
        DataTable productstable = new DataTable();
        DataTable salestable = new DataTable();
        DataTable purchasestable = new DataTable();
        DataTable wallettable = new DataTable();
      

        public static DataTable order = new DataTable();
        public static DataTable accountstable = new DataTable();
        public static Label Label_FullTotal = new Label();
        LoginForm login = new LoginForm();


        private void MainForm_Load(object sender, EventArgs e)
        {
            
            this.Hide();

            if(connect.State == ConnectionState.Closed)
            connect.Open();

            trans = connect.BeginTransaction();
            this.Icon = Icon.ExtractAssociatedIcon(AppDomain.CurrentDomain.FriendlyName);


            

            LoadAndSetUpTables();
            LoadRestaurantInfo();
            try
            {
                order.Columns.Add("اسم المنتج");
                order.Columns.Add("الكمية");
                order.Columns.Add("سعر الواحدة");
                order.Columns.Add("الاجمالي الفرعي");

                DataColumn[] columns = { order.Columns[0] };
                order.PrimaryKey = columns;

                DataGridOrder.DataSource = order;
           



            MainMenu_Panel.Height = this.Height;
            this.AutoScroll = false;

            LabelTitle.Location = new Point((Width - LabelTitle.Width) / 2 - 50, LabelTitle.Top);
            PictureBoxLogo.Location = new Point(MainMenu_Panel.Width, 5);

            Panel_Menu1.Height = this.Height - 250;
            Panel_Menu1.Width = this.Width - 830;

            Panel_ItemsList.Height = Panel_Menu1.Height;
            Panel_ItemsList.Location = new Point(156, 90);
            DataGridOrder.Size = new Size(450, Panel_ItemsList.Height);
            DataGridSales.Size = new Size(850, 312);
            Panel_Menu2.Size = new Size(850, 565);
            
                PutAllItemsAndProducts(Panel_ItemsList, Panel_Menu1);

            SetupPanels(Panel_Menu1);
            Panel_Menu1.Location = new Point(340, 90);
            DataGridOrder.Location = new Point(Panel_Menu1.Width + 345, 90);

            Label_FullTotal.RightToLeft = RightToLeft.Yes;
            LabelTotal.Location = new Point(DataGridOrder.Left, DataGridOrder.Top - 50);
            Label_FullTotal.Location = new Point(DataGridOrder.Left + 155, DataGridOrder.Top - 50);
            Label_FullTotal.Text = "0";
                Label_FullTotal.BackColor = Color.Transparent;
                Label_FullTotal.ForeColor = Color.White;
            Label_FullTotal.Font = LabelTotal.Font;
                Label_FullTotal.AutoSize = true;
                Label_FullTotal.Show();
                Label_FullTotal.Cursor = Cursors.Hand;

                Label_FullTotal.Click += delegate
                {

                    editform.TextView.Text = double.Parse(Label_FullTotal.Text).ToString();
                    editform.number = double.Parse(Label_FullTotal.Text);
                    editform.ShowDialog();

                    if(editform.OkPressed)
                    Label_FullTotal.Text = string.Format("{0:N0}",editform.number);

                    editform.OkPressed = false;

                };

                LabelTotal.Show();
              
            this.Controls.Add(Label_FullTotal);
                

            Btn_AddInvoice.Location = new Point(DataGridOrder.Left, DataGridOrder.Height + 100);
            Btn_PrintInvoice.Location = new Point(DataGridOrder.Left + 150, DataGridOrder.Height + 100);
            Btn_ClearRows.Location = new Point(DataGridOrder.Left + 300, DataGridOrder.Height + 100);
            LabelInvoiceNumber.Location = new Point(DataGridOrder.Left, DataGridOrder.Height + 160);
            InvoiceNum.Location = new Point(DataGridOrder.Left + 140, DataGridOrder.Height + 160);
            Btn_Logout.Location = new Point(this.Width - 150, 10);
            Btn_GetLastInvoice.Location = new Point(DataGridOrder.Left + 300, DataGridOrder.Height + 160);

            Panel_ItemsList.Show();
            DataGridOrder.Show();
            Label_FullTotal.Show();
            LabelTotal.Show();
            LabelInvoiceNumber.Show();
            InvoiceNum.Show();
            Btn_AddInvoice.Show();
            Btn_PrintInvoice.Show();
            Btn_ClearRows.Show();
            Btn_GetLastInvoice.Show();


                openFileDialog1.FileName = "";
            this.Show();

                this.Enabled = false;
                login.ShowDialog();

                SelectUserName.DataSource = accountstable;
                SelectUserName.DisplayMember = "username";
            }
            catch (Exception error) { }
        }

      





        /// <summary>
        /// Setup Main Panel Location And Make it Show On Load And hide The others
        /// </summary>
        /// <param name="panel"></param>
        private void SetupPanels(BunifuPanel panel)
        {
            foreach (Control p in this.Controls)
            {
                if (p is BunifuPanel && p.Name != "MainMenu_Panel")
                    p.Hide();
            }

            DataGridOrder.Hide();
            Label_FullTotal.Hide();
            LabelTotal.Hide();
            panel.Show();

            Label_FullTotal.Hide();
            LabelTotal.Hide();
            LabelInvoiceNumber.Hide();
            InvoiceNum.Hide();
            Btn_AddInvoice.Hide();
            Btn_PrintInvoice.Hide();
            Btn_ClearRows.Hide();
            Btn_GetLastInvoice.Hide();

            panel.Location = new Point((this.Width - panel.Width) / 2 + 70, (this.Height - panel.Height) / 2);

        }


        private void Btn_Menu2_Click(object sender, EventArgs e)
        {
            SetupPanels(Panel_Menu2);
            

        }

        private void Btn_Menu1_Click(object sender, EventArgs e)
        {
            SetupPanels(Panel_Menu1);
            Panel_Menu1.Location = new Point(340, 90);
            Panel_ItemsList.Show();
            DataGridOrder.Show();
            Label_FullTotal.Show();
            LabelTotal.Show();
            LabelInvoiceNumber.Show();
            InvoiceNum.Show();
            Btn_AddInvoice.Show();
            Btn_PrintInvoice.Show();
            Btn_ClearRows.Show();
            Btn_GetLastInvoice.Show();


        }

        private void Btn_Menu3_Click(object sender, EventArgs e)
        {
            SetupPanels(Panel_Menu3);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Trial.Enabled = false;
            LoginTimer.Enabled = false;

            trans.Commit();

            if (connect.State == ConnectionState.Open)
                connect.Close();


            Application.ExitThread();
        }
        AmountForm amountform = new AmountForm();
        SQLiteDataReader read;
        SQLiteCommand cmd;
        /// <summary>
        /// Load all Items in List And Products
        /// </summary>
        /// <param name="ItemsPanel"></param>
        /// <param name="ProductsPanel"></param>
        public void PutAllItemsAndProducts(BunifuPanel ItemsPanel, BunifuPanel ProductsPanel)
        {

            try
            {
                ItemsPanel.Controls.Clear();
                using (cmd = new SQLiteCommand("select * from items", connect))
                {
                    read = cmd.ExecuteReader();
                    int top = 0;
                    using (MainForm f = new MainForm())
                    {
                        while (read.Read())
                        {

                            BunifuButton button = new BunifuButton();
                            button.Size = new Size(160, 83);
                            button.Top = top;


                            if (!read.IsDBNull(1))
                            {

                                byte[] byt = (byte[])read[1];

                                MemoryStream stream = new MemoryStream(byt);
                                button.IdleIconRightImage = Image.FromStream(stream);

                                button.IconRightAlign = ContentAlignment.TopCenter;
                                button.TextAlign = ContentAlignment.BottomCenter;

                            }
                            button.Text = read[0].ToString();

                            button.Click += delegate (object sender, EventArgs e)
                            {
                                //Delete All Old Items Button


                                ProductsPanel.Controls.Clear();



                                using (cmd = new SQLiteCommand("select * from products where item_name = '" + button.Text + "'", connect))
                                {
                                    SQLiteDataReader read2 = cmd.ExecuteReader();
                                    int wid = 40;
                                    int top2 = 0;


                                    while (read2.Read())
                                    {
                                        BunifuButton button2 = new BunifuButton();

                                        button2.Size = new Size(240, 83);
                                        button2.Left = wid;
                                        button2.Top = top2;

                                        string productname = read2[0].ToString();
                                        double productprice = double.Parse(read2[1].ToString());

                                        if (!read2.IsDBNull(2))
                                        {
                                            byte[] byt = (byte[])read2[2];
                                            MemoryStream stream = new MemoryStream(byt);
                                            button2.IdleIconRightImage = Image.FromStream(stream);

                                            button2.IconRightAlign = ContentAlignment.TopCenter;
                                            button2.TextAlign = ContentAlignment.BottomCenter;
                                        }
                                        button2.Text = productname + " = " + string.Format("{0:N0}", productprice);

                                       
                                            button2.AllowAnimations = false;
                                            button2.Cursor = Cursors.Hand;
                                            button2.IconPadding = 15;
                                            button2.IdleFillColor = Btn_Menu1.IdleFillColor;
                                            button2.IdleBorderColor = Btn_Menu1.IdleFillColor;
                                            button2.onHoverState = Btn_Menu1.onHoverState;
                                            button2.OnPressedState = Btn_Menu1.OnPressedState;
                                            button2.ColorContrastOnHover = Btn_Menu1.ColorContrastOnHover;
                                            button2.ColorContrastOnHover = Btn_Menu1.ColorContrastOnClick;
                                            button2.Font = new Font("Arial", 11, FontStyle.Bold);
                                            button2.Height = 100;
                                        button2.IdleBorderRadius = 10;
                                            ProductsPanel.Controls.Add(button2);
                                            wid += button2.Width + 30;

                                            if (wid >= 500)
                                            {
                                                top2 += button2.Height + 5;
                                                wid = 40;
                                            }

                                            button2.Click += delegate (object sender2, EventArgs e2)
                                        {
                                            amountform.Text = productname;
                                            amountform.name = productname;
                                            amountform.price = productprice;
                                            amountform.ShowDialog();
                                        };
                                       
                                    }
                                    read2.Close();
                                }
                            };


                            button.AllowAnimations = false;
                            button.Cursor = Cursors.Hand;
                            button.Left += 1;
                            button.IconPadding = 15;
                            button.Font = new Font("Arial", 13, FontStyle.Bold);
                            button.IdleFillColor = Btn_Menu1.IdleFillColor;
                            button.IdleBorderColor = Btn_Menu1.IdleFillColor;
                            button.IdleBorderRadius = 35;
                            button.onHoverState = Btn_Menu1.onHoverState;
                            button.OnPressedState = Btn_Menu1.OnPressedState;
                            button.ColorContrastOnHover = Btn_Menu1.ColorContrastOnHover;
                            button.ColorContrastOnHover = Btn_Menu1.ColorContrastOnClick;
                            ItemsPanel.Controls.Add(button);
                            top += button.Height + 10;

                            LoadItemsProgress.Value += 10;

                        }
                        read.Close();
                    }
                }
                LoadItemsProgress.Value = 100;
                LoadItemsProgress.Value = 0;
            }
            catch (Exception error) { MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        string imagepath = "";
        private void ItemPhoto_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Images (*.jpg,*.png,*.jpeg)|*.jpg;*.png;*.jpeg";
           
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                imagepath = openFileDialog1.FileName;
                ItemPhoto.Image = Image.FromFile(imagepath);
            }
        }

        private void Btn_AddItem_Click(object sender, EventArgs e)
        {

            if (ComboItemName.Text.Trim() != "")
            {
                Items item = new Items(connect);
                item.item_name = ComboItemName.Text;
                item.item_image = imagepath;

                if (CheckBoxWithoutPhoto1.Checked)
                    item.item_image = "";

                if (!item.CheckItemExist())
                {
                    item.Insert();

                    object[] data = { ComboItemName.Text };
                    itemstable.Rows.Add(data);

                    MessageBox.Show("تمت الاضافة بنجاح", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);

                }
                else
                    MessageBox.Show("عفوا هذا الصنف موجود بالفعل", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);

            }

            /*
            int low = 0;
            int high = itemstable.Rows.Count - 1;
            while (high >= low)
            {


                int mid = low + (high - low) / 2;

                string text = itemstable.Rows[mid][0].ToString();

                if (text[0] > ComboItemName.Text[0])
                {
                    high = mid - 1;
                }

                if (text[0] < ComboItemName.Text[0])
                {
                    low = mid + 1;
                }

                if (ComboItemName.Text == text)
                {
                    byte[] byt = (byte[])itemstable.Rows[mid][1];
                    MemoryStream stream = new MemoryStream(byt);
                    ItemPhoto.Image = Image.FromStream(stream);
                    break;
                }

            }*/
        }

        private void Btn_EditItem_Click(object sender, EventArgs e)
        {
            /*

            DialogResult edit = MessageBox.Show("هل انت متأكد من انك تريد تعديل الصنف؟", "تحقق", MessageBoxButtons.YesNo, MessageBoxIcon.Question, 0, MessageBoxOptions.RtlReading);
            if (edit == DialogResult.Yes)
            {

                Items item = new Items(connect);
                item.item_name = ComboItemName.Text;
                item.item_image = imagepath;

                if (CheckBoxWithoutPhoto1.Checked)
                {
                    item.item_image = "";
                    ItemPhoto.Image = ItemPhoto.InitialImage;
                }

                if (item.CheckItemExist())
                {
                    item.Update();
                    MessageBox.Show("تم التعديل بنجاح", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);

                }
                else
                    MessageBox.Show("عفوا هذا الصنف غير موجود", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);


            }
            */

        }

        private void ComboItemName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                using (cmd = new SQLiteCommand("select * from items where item_name = '" + ComboItemName.Text + "'", connect))
                {
                    using (read = cmd.ExecuteReader())
                    {
                        read.Read();

                        if (read.HasRows)
                        {
                            if (!read.IsDBNull(1))
                            {
                                byte[] byt = (byte[])read[1];
                                MemoryStream stream = new MemoryStream(byt);
                                ItemPhoto.Image = Image.FromStream(stream);
                                stream.Dispose();
                            }
                            else
                                ItemPhoto.Image = ItemPhoto.InitialImage;

                        }

                    }
                }
            }
            catch(Exception error)
            {
                MessageBox.Show(error.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
            }

        }

        private void Btn_DeleteItem_Click(object sender, EventArgs e)
        {
            DialogResult delete = MessageBox.Show("هل انت متأكد حذف هذا الصنف؟", "تحقق", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 0, MessageBoxOptions.RtlReading);
            if (delete == DialogResult.Yes)
            {

                Items item = new Items(connect);
                item.item_name = ComboItemName.Text;
                item.item_image = imagepath;

                if (item.CheckItemExist())
                {
                    item.Delete();

                    for (int x = 0; x < itemstable.Rows.Count; x++)
                    {
                        if (itemstable.Rows[x][0].ToString() == item.item_name)
                        {
                            itemstable.Rows.RemoveAt(x);
                            break;
                        }

                    }


                    MessageBox.Show("تم الحذف بنجاح", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);
                    imagepath = "";
                }

                else
                    MessageBox.Show("عفوا هذا الصنف غير موجود", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);



            }

        }

        private void Btn_Menu4_Click(object sender, EventArgs e)
        {
            SetupPanels(Panel_Menu4);
        }

        private void ComboProductName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                using (cmd = new SQLiteCommand("select * from products where product_name = '" + ComboProductName.Text + "'", connect))
                {
                    using (read = cmd.ExecuteReader())
                    {
                        read.Read();

                        if (read.HasRows)
                        {
                            if (!read.IsDBNull(2))
                            {
                                byte[] byt = (byte[])read[2];
                                MemoryStream stream = new MemoryStream(byt);
                                ProductPhoto.Image = Image.FromStream(stream);
                                stream.Dispose();
                            }
                            else
                                ProductPhoto.Image = ProductPhoto.InitialImage;

                            TextProductPrice.Text = read[1].ToString();
                            ComboItemName2.Text = read[3].ToString();

                        }

                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
            }

        }

        private void Btn_AddProduct_Click(object sender, EventArgs e)
        {
            if (ComboItemName2.Text.Trim() != "" && ComboProductName.Text.Trim() != "" && TextProductPrice.Text.Trim() != "")
            {
                if (Regex.IsMatch(TextProductPrice.Text, @"^\d*\.?\d*$"))
                {
                    Products product = new Products(connect);
                    product.product_name = ComboProductName.Text;
                    product.item_name = ComboItemName2.Text;
                    product.product_image = imagepath;
                    product.product_price = TextProductPrice.Text;

                    if (CheckBoxWithoutPhoto2.Checked)
                        product.product_image = "";

                    if (!product.CheckItemExist())
                    {
                        product.Insert();

                        object[] data = { ComboProductName.Text };
                        productstable.Rows.Add(data);

                        MessageBox.Show("تمت الاضافة بنجاح", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);

                    }
                    else
                        MessageBox.Show("عفوا هذا المنتج موجود بالفعل !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);


                }
                else
                    MessageBox.Show("الرجاء ادخال ارقام فقط في خانة السعر !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);

            }
            else
                MessageBox.Show("الرجاء تعبئة جميع الخانات !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);

        }

        private void ProductPhoto_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Images (*.jpg,*.png,*.jpeg)|*.jpg;*.png;*.jpeg";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                imagepath = openFileDialog1.FileName;
                ProductPhoto.Image = Image.FromFile(imagepath);
            }
        }

        private void Btn_EditProduct_Click(object sender, EventArgs e)
        {
            DialogResult edit = MessageBox.Show("هل انت متأكد من انك تريد تعديل المنتج؟", "تحقق", MessageBoxButtons.YesNo, MessageBoxIcon.Question, 0, MessageBoxOptions.RtlReading);
            if (edit == DialogResult.Yes)
            {
                if (ComboItemName2.Text.Trim() != "" && ComboProductName.Text.Trim() != "" && TextProductPrice.Text.Trim() != "")
                {
                    if (Regex.IsMatch(TextProductPrice.Text, @"^\d*\.?\d*$"))
                    {
                        Products product = new Products(connect);
                        product.product_name = ComboProductName.Text;
                        product.item_name = ComboItemName2.Text;
                        product.product_image = imagepath;
                        product.product_price = TextProductPrice.Text;

                        if (CheckBoxWithoutPhoto2.Checked)
                        {
                            product.product_image = "";
                            ProductPhoto.Image = ProductPhoto.InitialImage;

                        }

                        if (product.CheckItemExist())
                        {
                            product.Update();

                            MessageBox.Show("تم التعديل", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);

                        }
                        else
                            MessageBox.Show("عفوا هذا المنتج غير موجود !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);


                    }
                    else
                        MessageBox.Show("الرجاء ادخال ارقام فقط في خانة السعر !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);

                }
                else
                    MessageBox.Show("الرجاء تعبئة جميع الخانات !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
            }
        }

        private void Btn_DeleteProduct_Click(object sender, EventArgs e)
        {
            if (ComboProductName.Text.Trim() != "")
            {
                DialogResult delete = MessageBox.Show("هل انت متأكد من حذف المنتج؟", "تحقق", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 0, MessageBoxOptions.RtlReading);
                if (delete == DialogResult.Yes)
                {

                    Products product = new Products(connect);
                    product.product_name = ComboProductName.Text;

                    if (product.CheckItemExist())
                    {
                        product.Delete();

                        for (int x = 0; x < productstable.Rows.Count; x++)
                        {
                            if (productstable.Rows[x][0].ToString() == product.product_name)
                            {
                                productstable.Rows.RemoveAt(x);
                                break;
                            }

                        }

                        MessageBox.Show("تم الحذف بنجاح", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);
                        imagepath = "";
                    }
                    else
                        MessageBox.Show("عفوا هذا المنتج غير موجود !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);

                }
            }
            else
                MessageBox.Show("الرجاء كتابة اسم المنتج !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            object[] data = { "gg", "ok", "g" };
            order.Rows.Add(data);
        }

        public static string invoice_number = "";
        private void Btn_AddInvoice_Click(object sender, EventArgs e)
        {
            if (DataGridOrder.Rows.Count <= 0)
            {
                MessageBox.Show("الرجاء اضافة منتج اولا !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
                return;
            }

            Invoice inv = new Invoice(connect);
            Sales sale = new Sales(connect);

            if (!inv.HasRows())
            {
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;
                int day = DateTime.Now.Day;
                string invoice_num = year + "" + month + "" + day;
                inv.invoice_number = int.Parse(invoice_num);
            }
            else
                inv.invoice_number = inv.GetLastInvoiceNumber() + 1;

            //double price = 0;
            inv.extract_date = DateTime.Now.ToString("dd/MM/yyyy h:mm:ss tt");
            for (int x = 0; x < DataGridOrder.Rows.Count; x++)
            {
                inv.product_name = DataGridOrder.Rows[x].Cells[0].Value.ToString();
                inv.product_amount = int.Parse(DataGridOrder.Rows[x].Cells[1].Value.ToString());
                inv.product_price = double.Parse(DataGridOrder.Rows[x].Cells[2].Value.ToString());
                //price += double.Parse(DataGridOrder.Rows[x].Cells[3].Value.ToString());

                if (x == DataGridOrder.Rows.Count - 1)
                    inv.total_price = double.Parse(Label_FullTotal.Text);

                inv.Insert();

                sale.sold_products +=  inv.product_name+",";


            }
            sale.total_price = inv.total_price;
            sale.invoice_number = inv.invoice_number;
            sale.extract_date = inv.extract_date;
            sale.user_name = LoginForm.username;
            sale.Insert();
            //object[] data = { sale.invoice_number , sale.sold_products, sale.total_price , sale.extract_date };
            //salestable.Rows.Add(data);
            pagesnumber = sale.GetAllSalesCount() / displaysalescount;

            InvoiceNum.Text = inv.invoice_number.ToString();
            MessageBox.Show("تمت الاضافة بنجاح", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);
            order.Rows.Clear();
            Label_FullTotal.Text = "0";
            amountform.fulltotal = 0;

            invoice_number = InvoiceNum.Text;

            LabelPages.Text = pagesnumber + "/" + (currentpage);

            ShowAllTotalPrice();
            ShowTotalPriceByDate();
            ShowAllProfits();
            ShowProfitsByDate();


            amountform.fulltotal = 0;
        }
        public static string invoice_extract_date = "";
        InvoiceReport invoice_report = new InvoiceReport();
        private void Btn_PrintInvoice_Click(object sender, EventArgs e)
        {
            try
            {

                if (InvoiceNum.Text.Trim() != "")
                    {
                        adapt = new SQLiteDataAdapter("select * from invoice where invoice_number = @invnum", connect);
                        adapt.SelectCommand.Parameters.AddWithValue("@invnum", InvoiceNum.Text);
                        read = adapt.SelectCommand.ExecuteReader();


                        read.Read();

                        if (read.HasRows)
                        {
                            if (!read.IsDBNull(6))
                                invoice_extract_date = read[6].ToString();

                        }
                        else
                            invoice_extract_date = "";

                        read.Close();

                    }
                    else
                    {
                        adapt = new SQLiteDataAdapter("select * from invoice where invoice_number = 0", connect);
                        invoice_extract_date = "";
                    }

                    adapt.Fill(invoice_report.posdataDataSet.invoice);


                    invoice_report.ShowDialog();

                invoice_report.posdataDataSet.Clear();


            }
            catch (Exception error) { MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }

        }
        int rowindex;
        int columnindex;
        EditForm editform = new EditForm();
        private void DataGridOrder_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (DataGridOrder.CurrentCell.ColumnIndex != 1)
            //    return;

            if (DataGridOrder.Rows.Count <= 0)
                return;

            rowindex = DataGridOrder.CurrentCell.RowIndex;
            columnindex = DataGridOrder.CurrentCell.ColumnIndex;

            string name = DataGridOrder.Rows[rowindex].Cells[0].Value.ToString();
            double price = double.Parse(DataGridOrder.Rows[rowindex].Cells[2].Value.ToString());
            double amount = double.Parse(DataGridOrder.Rows[rowindex].Cells[1].Value.ToString());

            if (DataGridOrder.CurrentCell.ColumnIndex == 1)
            {
                editform.number = double.Parse(DataGridOrder.Rows[rowindex].Cells[1].Value.ToString());


                editform.TextView.Text = DataGridOrder.CurrentCell.Value.ToString();

                editform.ShowDialog();

                if (editform.OkPressed)
                {
                    //order.Rows.RemoveAt(rowindex);

                    //object[] data = { name, edit.number, price, (edit.number * price) };
                    //order.Rows.Add(data);

                    order.Rows.Find(name)[1] = editform.number;
                    order.Rows.Find(name)[3] = (editform.number * price);

                    CountFullTotal();
                }
              
            }

            if (DataGridOrder.CurrentCell.ColumnIndex == 2)
            {
                editform.number = double.Parse(DataGridOrder.Rows[rowindex].Cells[2].Value.ToString());

                editform.TextView.Text = DataGridOrder.CurrentCell.Value.ToString();

                editform.ShowDialog();

                if (editform.OkPressed)
                {
                    //order.Rows.RemoveAt(rowindex);

                    //object[] data = { name, amount, edit.number, (edit.number * amount) };
                    //order.Rows.Add(data);

                    order.Rows.Find(name)[2] = editform.number;
                    order.Rows.Find(name)[3] = (editform.number * amount);

                    CountFullTotal();
                }
            }
            editform.OkPressed = false;
        }
        void CountFullTotal()
        {
            Label_FullTotal.Text = "";
            double total = 0;
            for (int x = 0; x < order.Rows.Count; x++)
            {
                total += double.Parse(order.Rows[x][3].ToString());
            }
            Label_FullTotal.Text = string.Format("{0:N0}", total);
            amountform.fulltotal = total;
        }

        private void حذفToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DataGridOrder.CurrentRow == null)
                return;

            int index = DataGridOrder.CurrentCell.RowIndex;
            order.Rows.RemoveAt(index);
            CountFullTotal();
        }


        private void Btn_ClearRows_Click(object sender, EventArgs e)
        {
            order.Rows.Clear();
            Label_FullTotal.Text = "0";
            amountform.fulltotal = 0;
        }



        public static double full_total_price = 0;
        public static double full_total_profits = 0;

        public static double daily_total_price = 0;
        public static double daily_total_profits = 0;

        public static double monthly_total_price = 0;
        public static double monthly_total_profits = 0;

        public static double yearly_total_price = 0;
        public static double yearly_total_profits = 0;


        private void ShowTotalPriceByDate()
        {
            Sales sale = new Sales(connect);
            Wallet w = new Wallet(connect);

            ////////////////////////Daily Sales/////////////////////


            double total = double.Parse(sale.GetFullTotalByDate(DatePicker1.Value.ToString("dd/MM/yyyy")));
            double wallet = w.GetAllAmountByDate(DatePicker1.Value.ToString("dd/MM/yyyy"));

            //if(total > 0)
            //total = total /*+ wallet*/;

            if (total <= 0)
                total = 0;

            daily_total_price = total;


            if (total.ToString().Trim() != "")
                LabelTotalToday.Text = string.Format("{0:N0}", total);
            else
                LabelTotalToday.Text = "لايوجد مبيعات";

            ///////////////////////Monthy Sales////////////////////////

            total = double.Parse(sale.GetFullTotalByDate(DatePicker1.Value.ToString("MM/yyyy")));
            wallet = w.GetAllAmountByDate(DatePicker1.Value.ToString("MM/yyyy"));

            //if(total > 0)
            //total = total /*+ wallet*/;

            if (total <= 0)
                total = 0;

            monthly_total_price = total;

            if (total.ToString().Trim() != "")
                LabelTotalMonthly.Text = string.Format("{0:N0}", total);
            else
                LabelTotalMonthly.Text = "لايوجد مبيعات";

            ///////////////////////Yearly Sales////////////////////////

            total = double.Parse(sale.GetFullTotalByDate(DatePicker1.Value.ToString("yyyy")));
            wallet = w.GetAllAmountByDate(DatePicker1.Value.ToString("yyyy"));

            //if(total > 0)
            //total = total /*+ wallet*/;

            if (total <= 0)
                total = 0;

            yearly_total_price = total;

            if (total.ToString().Trim() != "")
                LabelTotalYearly.Text = string.Format("{0:N0}", total);
            else
                LabelTotalYearly.Text = "لايوجد مبيعات";

        }
        public static string date = "";
        SalesReport sales_report = new SalesReport();

        private void Btn_SalesReport_Click(object sender, EventArgs e)
        {
            //Sales sale = new Sales(connect);
            //Wallet wallet = new Wallet(connect);
            date = DatePicker1.Value.ToString("dd/MM/yyy");
            //string price = sale.GetFullTotalByDate(date);
            //double w = wallet.GetAllAmountByDate(date);
            //price = (double.Parse(price) + w).ToString();

            //if (price.Trim() == "")
            //    price = "0";

            //double profits = sale.GetProfitsByDate(date);
            //profits = profits + w;

            //if (profits <= 0)
            //    profits = 0;

            //if (profits.ToString().Trim() == "")
            //    profits = 0;

            //string total = string.Format("{0:N0}", double.Parse(price));

            
                adapt = new  SQLiteDataAdapter("select * from sales  where extract_date like '%" + date + "%'",connect);
                adapt.Fill(sales_report.posdataDataSet.sales);
                ReportParameter[] parm = new ReportParameter[13];
                parm[0] = new ReportParameter("res_name", shop_name);
                parm[1] = new ReportParameter("address", shop_address);
                parm[2] = new ReportParameter("phone", shop_phone);
                parm[3] = new ReportParameter("phone2", shop_phone2);
                parm[4] = new ReportParameter("total_price", full_total_price.ToString());
                parm[5] = new ReportParameter("date", date);
                parm[6] = new ReportParameter("total_profits", full_total_profits.ToString());
                parm[7] = new ReportParameter("daily_total_price", daily_total_price.ToString());
                parm[8] = new ReportParameter("daily_total_profits", daily_total_profits.ToString());
                parm[9] = new ReportParameter("monthly_total_price", monthly_total_price.ToString());
                parm[10] = new ReportParameter("monthly_total_profits", monthly_total_profits.ToString());
                parm[11] = new ReportParameter("yearly_total_price", yearly_total_price.ToString());
                parm[12] = new ReportParameter("yearly_total_profits", yearly_total_profits.ToString());

                sales_report.reportViewer1.LocalReport.SetParameters(parm);


            
                sales_report.ShowDialog();
            sales_report.posdataDataSet.Clear();
          

        }

        private void Btn_Menu5_Click(object sender, EventArgs e)
        {
            SetupPanels(Panel_Menu5);
        }

        private void Btn_SaveResInfo_Click(object sender, EventArgs e)
        {
            if (CheckBoxNoLogo.Checked)
            {
                logopath = "";
                Logo.Image = Logo.InitialImage;
                PictureBoxLogo.Image = PictureBoxLogo.InitialImage;
            }

            ShopInfo info = new ShopInfo(connect);
            info.shop_name = text_ResName.Text;
            info.shop_address = text_ResAddress.Text;
            info.shop_tellphone = text_ResPhone.Text;
            info.shop_tellphone2 = text_ResPhone2.Text;
            info.logo_path = logopath;

            

            info.Save();

      

            MessageBox.Show("تم الحفظ بنجاح", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);

            LoadRestaurantInfo();
        }
        string logopath = "";
        private void Btn_Logo_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Images (*.jpg,*.png,*.jpeg)|*.jpg;*.png;*.jpeg";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                logopath = openFileDialog1.FileName;
                
                Logo.Image = Image.FromFile(logopath);
            }
        }

        private void Btn_Menu6_Click(object sender, EventArgs e)
        {
            SetupPanels(Panel_Menu6);
        }

        private void Btn_DeleteAllInvoiceAndSales_Click(object sender, EventArgs e)
        {
            DialogResult delete = MessageBox.Show("هل انت متأكد حذف جميع الفواتير والمبيعات والخزينة؟", "تحقق", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 0, MessageBoxOptions.RtlReading);
            if (delete == DialogResult.Yes)
            {
                Settings setting = new Settings(connect);
                setting.DeleteInvoicesAndSales();
                setting.DeleteWallet();
                MessageBox.Show("تم الحذف بنجاح", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);
                ShowAllTotalPrice();
                ShowTotalPriceByDate();
                ShowAllProfits();
                ShowProfitsByDate();
            }
        }

        private void Btn_DeleteAllItems_Click(object sender, EventArgs e)
        {
            DialogResult delete = MessageBox.Show("هل انت متأكد حذف جميع الاصناف؟", "تحقق", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 0, MessageBoxOptions.RtlReading);
            if (delete == DialogResult.Yes)
            {
                Settings setting = new Settings(connect);
                setting.DeleteItems();

                while (itemstable.Rows.Count > 0)
                    itemstable.Rows.RemoveAt(0);



                MessageBox.Show("تم الحذف بنجاح", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);
            }
        }

        private void Btn_DeleteAllProducts_Click(object sender, EventArgs e)
        {
            DialogResult delete = MessageBox.Show("هل انت متأكد حذف جميع المنتجات؟", "تحقق", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 0, MessageBoxOptions.RtlReading);
            if (delete == DialogResult.Yes)
            {
                Settings setting = new Settings(connect);
                setting.DeleteProducts();
                while (productstable.Rows.Count > 0)
                    productstable.Rows.RemoveAt(0);

                MessageBox.Show("تم الحذف بنجاح", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);

            }
        }

        private void Btn_DeleteAllInfo_Click(object sender, EventArgs e)
        {
            DialogResult delete = MessageBox.Show("هل انت متأكد حذف جميع معلومات المطعم؟", "تحقق", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 0, MessageBoxOptions.RtlReading);
            if (delete == DialogResult.Yes)
            {
                Settings setting = new Settings(connect);
                setting.DeleteRestaurantInfo();
                MessageBox.Show("تم الحذف بنجاح الرجاء اعادة تشغيل البرنامج", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);
                this.Close();

            }
        }

        private void Btn_DeleteAllAndReset_Click(object sender, EventArgs e)
        {
            DialogResult delete1 = MessageBox.Show("تنويه:هذا الامر يحذف جميع البيانات وارجاع البرنامج للحالة الافتراضية ", "تحقق", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 0, MessageBoxOptions.RtlReading);
            if (delete1 == DialogResult.Yes)
            {
                DialogResult delete2 = MessageBox.Show("هل انت متأكد حذف جميع البيانات؟", "تحقق", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 0, MessageBoxOptions.RtlReading);

                if (delete2 == DialogResult.Yes)
                {
                    Settings setting = new Settings(connect);
                    setting.DeleteAllDataAndReset();

                    //while (productstable.Rows.Count > 0)
                    //    productstable.Rows.RemoveAt(0);


                    MessageBox.Show("تم الحذف بنجاح الرجاء اعادة تشغيل البرنامج", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);
                    this.Close();
                }
            }
        }

        private void Btn_Menu7_Click(object sender, EventArgs e)
        {
            SetupPanels(Panel_Menu7);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
           
        }



        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.facebook.com/KING231MONTSER");
        }



        private void text_ResPhone_TextChanged(object sender, EventArgs e)
        {

        }



        private void text_ResAddress_TextChanged(object sender, EventArgs e)
        {

        }



        private void text_ResName_TextChanged(object sender, EventArgs e)
        {

        }



        private void TextProductPrice_TextChanged(object sender, EventArgs e)
        {

        }

        private void InvoiceNum_TextChanged(object sender, EventArgs e)
        {
            invoice_number = InvoiceNum.Text;
        }

        private void CheckBoxShowPassword_CheckedChanged(object sender, BunifuCheckBox.CheckedChangedEventArgs e)
        {
            if (CheckBoxShowPassword.Checked == true)
            {
                textPassword.UseSystemPasswordChar = false;
                textPassword.PasswordChar = '\0';
            }
            else
                textPassword.UseSystemPasswordChar = true;
        }

        private void Btn_EditAccount_Click(object sender, EventArgs e)
        {
            Login log = new Login(connect);
            log.username = textUserName.Text;
            log.password = textPassword.Text;
            if (textUserName.Text.Trim() != "" && textPassword.Text.Trim() != "")
            {

                DialogResult check = MessageBox.Show("هل انت متأكد من تعديل هذا المستخدم؟", "تحقق", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 0, MessageBoxOptions.RtlReading);
                if (check == DialogResult.Yes)
                {
                    if (log.IsAdmin(LoginForm.username))
                    {
                        if (!log.UserNameExist(log.username) || LoginForm.username == log.username)
                        {

                            log.Update(SelectUserName.Text);
                            MessageBox.Show("تم التعديل بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);

                            accountstable.Rows.Find(SelectUserName.Text)[0] = log.username;
                            LoginForm.username = log.username;
                            accountstable.Rows.Find(SelectUserName.Text)[1] = log.password;
                            LoginForm.password = log.password;
                        }
                        else
                            MessageBox.Show("عفوا هذا الاسم موجود بالفعل !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
                    }

                    else
                    {
                        if (!log.UserNameExist(log.username) || LoginForm.username == log.username)
                        {
                            log.Update(LoginForm.username);
                            MessageBox.Show("تم التعديل بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);

                            accountstable.Rows.Find(LoginForm.username)[0] = log.username;
                            LoginForm.username = log.username;
                            accountstable.Rows.Find(LoginForm.username)[1] = log.password;
                            LoginForm.password = log.password;
                        }
                        else
                            MessageBox.Show("عفوا هذا الاسم موجود بالفعل !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);

                    }
                }

            }
            else
                MessageBox.Show("الرجاء تعبة جميع الحقول !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);




        }
        private void Btn_Logout_Click(object sender, EventArgs e)
        {
           

            this.Enabled = false;
            login.logged = false;

            login.ShowDialog();
          

        }

        private void ShowAllTotalPrice()
        {

            Sales sale = new Sales(connect);
            Wallet w = new Wallet(connect);
            
            double total = double.Parse(sale.GetAllTotal());
            double wallet = w.GetAllAmount();

            //total = total + wallet */;

            if (total <= 0)
                total = 0;

            full_total_price = total;

            if (total.ToString().Trim() != "")
            {
                LabelAllTotal.Text = string.Format("{0:N0}", total);
            }
            else
                LabelAllTotal.Text = "لايوجد مبيعات";


            total = total + wallet;

            if (total <= 0)
                total = 0;

            if (total.ToString().Trim() != "")
            {
                LabelWalletTotal.Text = string.Format("{0:N0}", total);
            }

        }

        private void ShowAllProfits()
        {
            Wallet w = new Wallet(connect);
            Sales sale = new Sales(connect);
            double profit = sale.GetAllProfits();
            double wallet = w.GetAllAmount();

            //profit = profit + wallet;

            if (profit <= 0)
                profit = 0;

            full_total_profits = profit;


            if (profit.ToString().Trim() != "")
                LabelAllProfits.Text = string.Format("{0:N0}", profit);
            else
                LabelAllProfits.Text = "لايوجد مبيعات";

        }

        private void ShowProfitsByDate()
        {
            Wallet w = new Wallet(connect);
            Sales sale = new Sales(connect);

            ///////////////////////Daily Sales////////////////////////

            double profit = sale.GetProfitsByDate(DatePicker1.Value.ToString("dd/MM/yyyy"));
            double wallet = w.GetAllAmountByDate(DatePicker1.Value.ToString("dd/MM/yyyy")); 

            //if(profit > 0)
            //profit = profit + wallet;

            if (profit <= 0)
                profit = 0;

            daily_total_profits = profit;


            if (profit.ToString().Trim() != "")
                LabelAllTodayProfit.Text = string.Format("{0:N0}", profit);
            else
                LabelAllTodayProfit.Text = "لايوجد مبيعات";


            ///////////////////////Monthy Sales////////////////////////

            profit = sale.GetMonthlyProfits(DatePicker1.Value.ToString("MM/yyyy"));
            wallet = w.GetAllAmountByDate(DatePicker1.Value.ToString("MM/yyyy"));

            //if(total > 0)
            //total = total /*+ wallet*/;

            if (profit <= 0)
                profit = 0;

            monthly_total_profits = profit;

            if (profit.ToString().Trim() != "")
               LabelTotalProfitsMonthly.Text = string.Format("{0:N0}", profit);
            else
                LabelTotalProfitsMonthly.Text = "لايوجد مبيعات";

            ///////////////////////Yearly Sales////////////////////////

            profit = sale.GetProfitsByDate(DatePicker1.Value.ToString("yyyy"));
            wallet = w.GetAllAmountByDate(DatePicker1.Value.ToString("yyyy"));

            //if(total > 0)
            //total = total /*+ wallet*/;

            if (profit <= 0)
                profit = 0;

            yearly_total_profits = profit;

            if (profit.ToString().Trim() != "")
               LabelTotalProfitsYearly.Text = string.Format("{0:N0}", profit);
            else
                LabelTotalProfitsYearly.Text = "لايوجد مبيعات";
        }

        private void Btn_SalesFullReport_Click(object sender, EventArgs e)
        {
            /*
            Sales sale = new Sales(connect);
            Wallet wallet = new Wallet(connect);
            date = DateTime.Now.ToString("dd/MM/yyy");
            string price = sale.GetAllTotal();
            double w = wallet.GetAllAmount();
            price = (double.Parse(price) + w).ToString();

            if (price.Trim() == "")
                price = "0";

            double profits = sale.GetAllProfits();
            
            profits = profits + w;

            if (profits <= 0)
                profits = 0;

            if (profits.ToString().Trim() == "")
                profits = 0;

            string total = string.Format("{0:N0}", double.Parse(price));

            using (SalesReport report = new SalesReport())
            {
                adapt = new  SQLiteDataAdapter("select * from sales  where extract_date like '%" + date + "%'",connect);
                adapt.Fill(report.posdataDataSet.sales);
                ReportParameter[] parm = new ReportParameter[7];
                parm[0] = new ReportParameter("res_name", shop_name);
                parm[1] = new ReportParameter("address", shop_address);
                parm[2] = new ReportParameter("phone", shop_phone);
                parm[3] = new ReportParameter("phone2", shop_phone2);
                parm[4] = new ReportParameter("total_price", total.ToString());
                parm[5] = new ReportParameter("date", date);
                parm[6] = new ReportParameter("total_profits", profits.ToString());

                report.reportViewer1.LocalReport.SetParameters(parm);
                report.ShowDialog();
                
            }
            */

        }

        private void Btn_GetLastInvoice_Click(object sender, EventArgs e)
        {
            Invoice inv = new Invoice(connect);
            InvoiceNum.Text = inv.GetLastInvoiceNumber().ToString();
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                if (WindowState != FormWindowState.Maximized)
                {
                    WindowState = FormWindowState.Maximized;
                }
            }
        }

        private void InvoiceNum_Click(object sender, EventArgs e)
        {
            editform.TextView.Text = InvoiceNum.Text;
            editform.ShowDialog();


            if (editform.OkPressed)
            {
                InvoiceNum.Text = editform.TextView.Text;
            }
            editform.OkPressed = false;


        }
        int currentpage = 1;
        private void Btn_NextPage_Click(object sender, EventArgs e)
        {
            //if (!backgroundWorker1.IsBusy)
            //    backgroundWorker1.RunWorkerAsync();
            if (currentpage >= pagesnumber)
                return;

            int invnum = 0;
            if (salestable.Rows.Count > 0)
            {
                invnum = int.Parse(salestable.Rows[salestable.Rows.Count - 1][0].ToString());

                salestable.Rows.Clear();
                using (adapt = new  SQLiteDataAdapter("select   * from sales where invoice_number > @invnum limit " + displaysalescount + "", connect))
                {
                    adapt.SelectCommand.Parameters.AddWithValue("@invnum", invnum);
                    adapt.Fill(salestable);
                }

            }

            if (currentpage < pagesnumber)
                LabelPages.Text = pagesnumber + "/" + (++currentpage);
        }

        private void Btn_LastPage_Click(object sender, EventArgs e)
        {


            Sales sale = new Sales(connect);

            int invnum = (sale.GetLastInvoiceNumber() - displaysalescount);

            salestable.Rows.Clear();
            using (adapt = new  SQLiteDataAdapter("select * from sales where invoice_number > @invnum limit " + displaysalescount + "", connect))
            {
                adapt.SelectCommand.Parameters.AddWithValue("@invnum", invnum);
                adapt.Fill(salestable);
            }
            currentpage = pagesnumber;
            LabelPages.Text = pagesnumber + "/" + (currentpage);

        }

        private void Btn_FirstPage_Click(object sender, EventArgs e)
        {
            salestable.Rows.Clear();
            using (adapt = new  SQLiteDataAdapter("select   * from sales limit " + displaysalescount + "", connect))
            {
                adapt.Fill(salestable);
            }
            currentpage = 1;
            LabelPages.Text = pagesnumber + "/" + (currentpage);

        }

        private void Btn_PrevPage_Click(object sender, EventArgs e)
        {
            int invnum = 0;
            if (salestable.Rows.Count > 0)
            {
                invnum = int.Parse(salestable.Rows[0][0].ToString());
            }

            salestable.Rows.Clear();
            using (adapt = new  SQLiteDataAdapter("select  * from sales where invoice_number >= @invnum limit " + displaysalescount + "", connect))
            {
                adapt.SelectCommand.Parameters.AddWithValue("@invnum", invnum - displaysalescount);
                adapt.Fill(salestable);
            }

            if (currentpage > 1)
                LabelPages.Text = pagesnumber + "/" + (--currentpage);

        }

        private void DatePicker1_ValueChanged(object sender, EventArgs e)
        {
            ShowTotalPriceByDate();
            ShowAllTotalPrice();
            ShowAllProfits();
            ShowProfitsByDate();
        }

        private void نسخToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(DataGridSales.CurrentCell.Value.ToString());
        }

        private void Btn_SearchByDate_Click(object sender, EventArgs e)
        {
            salestable.Rows.Clear();
            using (adapt = new  SQLiteDataAdapter("select * from sales where  extract_date like '%" + DatePicker1.Value.ToString("dd/MM/yyyy") + "%'", connect))
            {
                //adapt.SelectCommand.Parameters.AddWithValue("@date",DatePicker1.Value.ToShortDateString());
                adapt.Fill(salestable);
            }
            currentpage = 1;
            LabelPages.Text = pagesnumber + "/" + (currentpage);
        }


        private void Trial_Tick(object sender, EventArgs e)
        {

            if (!Program.IsActive)
            {
                if (File.Exists(Program.trialfile))
                {
                    // Trial Time File Found

                    int count = int.Parse(File.ReadAllText(Program.trialfile));
                    if (count >= Program.trialtime)
                    {
                        ///Trial Has Ended And Stop The Program 
                        ///
                        Application.ExitThread();

                    }
                    else
                    {
                        //Keep Counting Trial Time...
                        count += 1;
                        File.WriteAllText(Program.trialfile, count.ToString());
                    }
                }
                else
                {
                    // Trial Time File Not Found
                    File.WriteAllText(Program.trialfile, "1");
                }
            }
        }

        private void Btn_AddAccount_Click(object sender, EventArgs e)
        {

            Login login = new Login(connect);
            login.username = textUserName.Text;
            login.password = textPassword.Text;
            login.permission = "employe";
            if (textUserName.Text.Trim() != "" && textPassword.Text.Trim() != "")
            {
                if (!login.UserNameExist(login.username))
                {
                    login.Register();

                    object[] row = {login.username,login.password,login.permission};
                    accountstable.Rows.Add(row);

                    MessageBox.Show("تم التسجيل بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);
                }
                else
                    MessageBox.Show("هذا الاسم موجود بالفعل !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);

            }
            else
                MessageBox.Show("الرجاء تعبة جميع الحقول !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);

        }

        private void Btn_DeleteAccount_Click(object sender, EventArgs e)
        {
            Login login = new Login(connect);
            login.username = SelectUserName.Text;

                if(login.UserNameExist(SelectUserName.Text))
                {
                    if (!login.IsAdmin(login.username))
                    {
                        DialogResult delete = MessageBox.Show("هل انت متأكد من حذف هذا المستخدم؟", "تحقق", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 0, MessageBoxOptions.RtlReading);
                        if (delete == DialogResult.Yes)
                        {
                            login.Delete();


                            accountstable.Rows.Find(login.username).Delete();


                            MessageBox.Show("تم حذف المستخدم بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);
                        }
                    }
                    else
                        MessageBox.Show("عفوا لايمكنك مسح المدير !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);

                }
                else
                    MessageBox.Show("هذا الاسم غير موجود", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);



        }

        private void SelectUserName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (login.IsAdmin)
                {


                    using (cmd = new SQLiteCommand("select * from login where username = @user",connect))
                    {
                        cmd.Parameters.AddWithValue("@user", SelectUserName.Text);
                        read = cmd.ExecuteReader();
                        read.Read();

                        if (read.HasRows)
                        {
                            textUserName.Text = read[0].ToString();
                            textPassword.Text = read[1].ToString();
                        }

                        LoginForm.username = textUserName.Text;
                        LoginForm.password = textPassword.Text;
                        read.Close();
                    }

                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
            }

        }

        private void LoginTimer_Tick(object sender, EventArgs e)
        {
            if (login.logged)
                this.Enabled = true;
            else
                this.Enabled = false;

            
            if (!login.IsAdmin)
            {
                foreach (Control c in Panel_Menu6.Controls)
                {
                    if (c is Bunifu.UI.WinForms.BunifuButton.BunifuButton)
                    {
                        c.Hide();
                    }
                }
                Btn_EditAccount.Text = "حفظ";
                Btn_EditAccount.Show();
                SelectUserName.Hide();
                LabelSelectUser.Hide();
            }
            else
            {

                //SelectUserName.DataSource = accountstable;
                //SelectUserName.DisplayMember = "username";

                foreach (Control c in Panel_Menu6.Controls)
                {
                    if (c is Bunifu.UI.WinForms.BunifuButton.BunifuButton)
                        c.Show();
                }

                Btn_EditAccount.Text = "تعديل";
                SelectUserName.Show();
                LabelSelectUser.Show();

            }
        }

        private void Btn_LoadItems_Click(object sender, EventArgs e)
        {
         if(!ItemsLoadWorker.IsBusy)
                ItemsLoadWorker.RunWorkerAsync();

            
        }

        private void ItemsLoadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Invoke((MethodInvoker)delegate {
                PutAllItemsAndProducts(Panel_ItemsList, Panel_Menu1);
                });



        }

        private void ItemsLoadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("تمت اعادة تحميل الاصناف بنجاح", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);
        }

        private void Btn_Color_Click(object sender, EventArgs e)
        {
            
            
        }

        private void Btn_Menu8_Click(object sender, EventArgs e)
        {
            SetupPanels(Panel_Menu8);
        }

        private void Btn_AddPurchase_Click(object sender, EventArgs e)
        {

            Purchases p = new Purchases(connect);
            p.purchase_name = ComboPurchaseName.Text;

            if (ComboPurchaseName.Text.Trim() != "" && textPurchaseAmount.Text.Trim() != "" && textPurchasePrice.Text.Trim() != "")
            {
                if (!p.PurchaseNameExist())
                {
                    if (Regex.IsMatch(textPurchasePrice.Text, @"^\d*\.?\d*$"))
                    {
                        p.purchase_amount = textPurchaseAmount.Text;
                        p.purchase_price = double.Parse(textPurchasePrice.Text);
                        p.purchase_date = PurchaseDatePicker.Value.ToShortDateString();

                        p.AddData();
                        purchasestable.Rows.Add(new object[] {p.purchase_name});
                        MessageBox.Show("تمت الاضافة بنجاح", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);

                        ShowAllProfits();
                        ShowProfitsByDate();
                    }
                    else
                        MessageBox.Show("الرجاء ادخال ارقام فقط في خانة السعر", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);

                }
                else
                    MessageBox.Show("هذا المنتج (المشترى) موجود مسبقا", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
            }
            else
                MessageBox.Show("الرجاء تعبئة جميع الخانات", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);

        }

        private void Btn_EditPurchase_Click(object sender, EventArgs e)
        {
            Purchases p = new Purchases(connect);
            p.purchase_name = ComboPurchaseName.Text;

            if (ComboPurchaseName.Text.Trim() != "" && textPurchaseAmount.Text.Trim() != "" && textPurchasePrice.Text.Trim() != "")
            {
                DialogResult delete = MessageBox.Show("هل انت متأكد من تعديل هذا المنتج (المشترى)؟", "تحقق", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 0, MessageBoxOptions.RtlReading);
                if (delete == DialogResult.Yes)
                {
                    if (p.PurchaseNameExist())
                    {
                        if (Regex.IsMatch(textPurchasePrice.Text, @"^\d*\.?\d*$"))
                        {
                            p.purchase_amount = textPurchaseAmount.Text;
                            p.purchase_price = double.Parse(textPurchasePrice.Text);
                            p.purchase_date = PurchaseDatePicker.Value.ToShortDateString();

                            p.EditByName();
                            MessageBox.Show("تم التعديل بنجاح", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);

                            ShowAllProfits();
                            ShowProfitsByDate();
                        }
                        else
                            MessageBox.Show("الرجاء ادخال ارقام فقط في خانة السعر", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);

                    }
                    else
                        MessageBox.Show("هذا المنتج (المشترى) غير موجود", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
                }

                }
            else
                MessageBox.Show("الرجاء تعبئة جميع الخانات", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
        }

        private void ComboPurchaseName_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (cmd = new SQLiteCommand("select * from purchases where [purchase_name] = @name",connect))
            {
                cmd.Parameters.AddWithValue("@name",ComboPurchaseName.Text);
                read = cmd.ExecuteReader();
                read.Read();
                if(read.HasRows)
                {
                    textPurchaseAmount.Text = read[1].ToString();
                    textPurchasePrice.Text = read[2].ToString();
                    PurchaseDatePicker.Value = DateTime.Parse(read[3].ToString());
                }
            }
        }

        private void Btn_DeletePurchase_Click(object sender, EventArgs e)
        {
            Purchases p = new Purchases(connect);
            p.purchase_name = ComboPurchaseName.Text;

                DialogResult delete = MessageBox.Show("هل انت متأكد من حذف هذا المنتج (المشترى)؟", "تحقق", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 0, MessageBoxOptions.RtlReading);
                if (delete == DialogResult.Yes)
                {
                    if (p.PurchaseNameExist())
                    {
                            p.DeleteByName();
                            purchasestable.Rows.Find(p.purchase_name).Delete();
                            MessageBox.Show("تم الحذف بنجاح", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);

                    ShowAllProfits();
                    ShowProfitsByDate();
                }
                    else
                        MessageBox.Show("هذا المنتج (المشترى) غير موجود", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
                }

           

        }

        private void Btn_Menu9_Click(object sender, EventArgs e)
        {
            SetupPanels(Panel_Menu9);
        }

        private void Btn_Withdraw_Click(object sender, EventArgs e)
        {
            if (textCash.Text.Trim() != "")
            {
                if (Regex.IsMatch(textCash.Text, @"^\d*\.?\d*$"))
                {
                    DialogResult check = MessageBox.Show("هل انت متأكد من سحب هذا المبلغ ؟", "تحقق", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 0, MessageBoxOptions.RtlReading);
                    if (check == DialogResult.Yes)
                    {
                        Sales s = new Sales(connect);
                        Wallet w = new Wallet(connect);
                        double st = double.Parse(s.GetAllTotal());
                        double sw = w.GetAllAmount();
                        if (double.Parse(textCash.Text) <= (st + sw) )
                        {
                            w.user_name = LoginForm.username;
                            w.cash = -double.Parse(textCash.Text);
                            w.process_date = DateTime.Now.ToString();
                            w.process_type = "سحب";
                            w.Add();


                            ShowTotalPriceByDate();
                            ShowAllTotalPrice();
                            ShowAllProfits();
                            ShowProfitsByDate();

                            MessageBox.Show("تم السحب بنجاح", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);
                            textCash.ResetText();
                        }
                        else
                        MessageBox.Show("عفوا هذا المبلغ اكبر من المبلغ الموجود في الخزينة ! !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);

                    }
                }
                else
                    MessageBox.Show("الرجاء ادخال ارقام فقط !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);

            }
            else
                MessageBox.Show("الرجاء ادخال المبلغ !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);



        }

        private void Btn_Deposit_Click(object sender, EventArgs e)
        {
          
                if (textCash.Text.Trim() != "")
                {
                    if (Regex.IsMatch(textCash.Text, @"^\d*\.?\d*$"))
                    {
                        DialogResult check = MessageBox.Show("هل انت متأكد من ايداع هذا المبلغ ؟", "تحقق", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 0, MessageBoxOptions.RtlReading);
                        if (check == DialogResult.Yes)
                        {
                            Wallet w = new Wallet(connect);
                            w.user_name = LoginForm.username;
                            w.cash = double.Parse(textCash.Text);
                            w.process_date = DateTime.Now.ToString();
                            w.process_type = "ايداع";
                            w.Add();

                        ShowTotalPriceByDate();
                            ShowAllTotalPrice();
                            ShowAllProfits();
                            ShowProfitsByDate();

                            MessageBox.Show("تم الايداع بنجاح", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);
                            textCash.ResetText();
                        }
                    }
                    else
                        MessageBox.Show("الرجاء ادخال ارقام فقط !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);

                }
                else
                    MessageBox.Show("الرجاء ادخال المبلغ !", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error, 0, MessageBoxOptions.RtlReading);
            
        }
        int walletcurrentpage = 1;
        private void Btn_WalletFirstPage_Click(object sender, EventArgs e)
        {
            wallettable.Rows.Clear();
            using (adapt = new  SQLiteDataAdapter("select * from [wallet] limit " + displaysalescount + "", connect))
            {
                adapt.Fill(wallettable);
                
            }
            walletcurrentpage = 1;
            LabelWalletPageNumber.Text = walletpagenumber + "/" + (walletcurrentpage);
        }

        private void Btn_WalletNextPage_Click(object sender, EventArgs e)
        {
            if (walletcurrentpage >= walletpagenumber)
                return;

            int id = 0;
            if (wallettable.Rows.Count > 0)
            {
                id = int.Parse(wallettable.Rows[wallettable.Rows.Count - 1][0].ToString());

                wallettable.Rows.Clear();
                using (adapt = new  SQLiteDataAdapter("select * from wallet where id > @id limit " + displaysalescount + "", connect))
                {
                    adapt.SelectCommand.Parameters.AddWithValue("@id", id);
                    adapt.Fill(wallettable);
                }

            }

            if (walletcurrentpage < walletpagenumber)
                LabelWalletPageNumber.Text = walletpagenumber + "/" + (++walletcurrentpage);
        }

        private void Btn_WalletPrevPage_Click(object sender, EventArgs e)
        {
            int id = 0;
            if (wallettable.Rows.Count > 0)
            {
                id = int.Parse(wallettable.Rows[0][0].ToString());
            }

            wallettable.Rows.Clear();
            using (adapt = new  SQLiteDataAdapter("select  * from wallet where id >= @id limit " + displaysalescount + "", connect))
            {
                adapt.SelectCommand.Parameters.AddWithValue("@id", id - displaysalescount);
                adapt.Fill(wallettable);
            }

            if (walletcurrentpage > 1)
                LabelWalletPageNumber.Text = walletpagenumber + "/" + (--walletcurrentpage);
        }

        private void Btn_WalletLastPage_Click(object sender, EventArgs e)
        {
            Wallet w = new Wallet(connect);

            int id = (w.GetAllWalletCount() - displaysalescount);

            wallettable.Rows.Clear();
            using (adapt = new  SQLiteDataAdapter("select * from wallet where id > @id limit " + displaysalescount + "", connect))
            {
                adapt.SelectCommand.Parameters.AddWithValue("@id", id);
                adapt.Fill(wallettable);
            }
           walletcurrentpage = walletpagenumber;
            LabelWalletPageNumber.Text = walletpagenumber + "/" + (walletcurrentpage);
        }

        private void Btn_DeleteAllPurchases_Click(object sender, EventArgs e)
        {
            DialogResult delete = MessageBox.Show("هل انت متأكد حذف جميع المشتريات؟", "تحقق", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, 0, MessageBoxOptions.RtlReading);
            if (delete == DialogResult.Yes)
            {
                Settings s = new Settings(connect);
                s.DeletePurchases();
                purchasestable.Rows.Clear();
        
                MessageBox.Show("تم الحذف بنجاح", "نجحت العملية", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, 0, MessageBoxOptions.RtlReading);

                ShowTotalPriceByDate();
                ShowAllTotalPrice();
                ShowAllProfits();
                ShowProfitsByDate();
            }
        }

        private void SaveData_Tick(object sender, EventArgs e)
        {
            //trans.Commit();
            //trans = connect.BeginTransaction();


        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void LabelTotal_Click(object sender, EventArgs e)
        {

        }

        private void CLearMemory_Tick(object sender, EventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
