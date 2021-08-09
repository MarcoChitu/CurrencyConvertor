using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CurrencyConvertor.Class;
using Newtonsoft.Json;

namespace CurrencyConvertor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Root val = new Root();

        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();

        private int CurrencyId = 0;
        private double FromAmount = 0;
        private double ToAmount = 0;

      

        public MainWindow()
        {
            InitializeComponent();
            ClearControls();
            GetValue();
        }

        private async void GetValue()
        {
            val = await GetData<Root>("https://openexchangerates.org/api/latest.json?app_id=050184166f30401ea8ab37dbbc41f1af");
            BindCurrency();
        }

        protected static async Task<Root> GetData<T>(string url)
        {
            var myRoot = new Root();
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(1);
                    HttpResponseMessage response = await client.GetAsync(url);
                    if(response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var ResponceString = await response.Content.ReadAsStringAsync();
                        var ResponceObject = JsonConvert.DeserializeObject<Root>(ResponceString);

                        MessageBox.Show("TimeStamp: " + ResponceObject.license, "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                        return ResponceObject;
                    }
                    return myRoot;
                }
            }
            catch
            {
                return myRoot;
            }
        }

        public void mycon()
        {
            String Conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            con = new SqlConnection(Conn);
            con.Open(); //Connection Open
        }

        private void BindCurrency()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Text");
            dt.Columns.Add("Value");

            dt.Rows.Add("--SELECT--", 0);
            dt.Rows.Add("INR", val.rates.INR);
            dt.Rows.Add("JPY", val.rates.JPY);
            dt.Rows.Add("USD", val.rates.USD);
            dt.Rows.Add("NZD", val.rates.NZD);
            dt.Rows.Add("EUR", val.rates.EUR);
            dt.Rows.Add("CAD", val.rates.CAD);
            dt.Rows.Add("ISK", val.rates.ISK);
            dt.Rows.Add("PHP", val.rates.PHP);
            dt.Rows.Add("DKK", val.rates.DKK);
            dt.Rows.Add("RON", val.rates.RON);

            mycon();
            /*DataTable dt = new DataTable()*/;
            cmd = new SqlCommand("select Id, CurrencyName from Currency_Master", con);
            cmd.CommandType = CommandType.Text;

            da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            DataRow newRow = dt.NewRow();
            newRow["Id"] = 0;
            newRow["CurrencyName"] = "--SELECT--";
            dt.Rows.InsertAt(newRow, 0);

            if (dt != null && dt.Rows.Count > 0)
            {
                //Assign the datatable data to from currency combobox using ItemSource property.
                cmbFromCurrency.ItemsSource = dt.DefaultView;
                cmbToCurrency.ItemsSource = dt.DefaultView;
            }
            con.Close();

            cmbFromCurrency.DisplayMemberPath = "CurrencyName";

            cmbFromCurrency.SelectedValuePath = "Id";

            cmbFromCurrency.SelectedValue = 0;

            cmbFromCurrency.DisplayMemberPath = "CurrencyName";
            cmbFromCurrency.SelectedValuePath = "Id";
            cmbFromCurrency.SelectedIndex = 0;

            cmbToCurrency.DisplayMemberPath = "CurrencyName";
            cmbToCurrency.SelectedValuePath = "Id";
            cmbToCurrency.SelectedIndex = 0;
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            double ConvertedValue;

            //Check amount textbox is Null or Blank
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCurrency.Focus();
                return;
            }
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbFromCurrency.Focus();
                return;
            }
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbToCurrency.Focus();
                return;
            }

            if(cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                ConvertedValue = double.Parse(txtCurrency.Text);
                //N3 is used to place 000 after the dot (.)
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else
            {
                ConvertedValue = (double.Parse(cmbToCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text)) / double.Parse(cmbFromCurrency.SelectedValue.ToString());
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if (cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void tbMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //private void btnSave_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (txtAmount.Text == null || txtAmount.Text.Trim() == "")
        //        {
        //            MessageBox.Show("Please enter amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        //            txtAmount.Focus();
        //            return;
        //        }
        //        else if (txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "")
        //        {
        //            MessageBox.Show("Please enter currency name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        //            txtCurrencyName.Focus();
        //            return;
        //        }
        //        else
        //        {   //Edit time and set that record Id in CurrencyId variable.
        //            if (CurrencyId > 0)
        //            {
        //                if (MessageBox.Show("Are you sure you want to update ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        //                {
        //                    mycon();
        //                    DataTable dt = new DataTable();

        //                    //Update Query Record update using Id
        //                    cmd = new SqlCommand("UPDATE Currency_Master SET Amount = @Amount, CurrencyName = @CurrencyName WHERE Id = @Id", con);
        //                    cmd.CommandType = CommandType.Text;
        //                    cmd.Parameters.AddWithValue("@Id", CurrencyId);
        //                    cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
        //                    cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
        //                    cmd.ExecuteNonQuery();
        //                    con.Close();

        //                    MessageBox.Show("Data updated successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        //                }
        //            }
        //            else
        //            {
        //                if (MessageBox.Show("Are you sure you want to save ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        //                {
        //                    mycon();
        //                    //Insert query to Save data in the table
        //                    cmd = new SqlCommand("INSERT INTO Currency_Master(Amount, CurrencyName) VALUES(@Amount, @CurrencyName)", con);
        //                    cmd.CommandType = CommandType.Text;
        //                    cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
        //                    cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
        //                    cmd.ExecuteNonQuery();
        //                    con.Close();

        //                    MessageBox.Show("Data saved successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        //                }
        //            }
        //            ClearMaster();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //private void ClearMaster()
        //{
        //    try
        //    {
        //        txtAmount.Text = string.Empty;
        //        txtCurrencyName.Text = string.Empty;
        //        btnSave.Content = "Save";
        //        GetData();
        //        CurrencyId = 0;
        //        BindCurrency();
        //        txtAmount.Focus();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //public void GetData()
        //{
        //    mycon();
        //    DataTable dt = new DataTable();
        //    cmd = new SqlCommand("SELECT * FROM Currency_Master", con);
        //    cmd.CommandType = CommandType.Text;
        //    da = new SqlDataAdapter(cmd);
        //    da.Fill(dt);

        //    if (dt != null && dt.Rows.Count > 0)
        //        dgvCurrency.ItemsSource = dt.DefaultView;
        //    else
        //        dgvCurrency.ItemsSource = null;

        //    con.Close();
        //}

        //private void btnCancel_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        ClearMaster();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //private void dgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        //{
        //    try
        //    {
        //        //Create object for DataGrid
        //        DataGrid grd = (DataGrid)sender;

        //        //Create an object for DataRowView
        //        DataRowView row_selected = grd.CurrentItem as DataRowView;

        //        if (row_selected != null)
        //        {
        //            if (dgvCurrency.Items.Count > 0)
        //            {
        //                if (grd.SelectedCells.Count > 0)
        //                {
        //                    CurrencyId = Int32.Parse(row_selected["Id"].ToString());

        //                    if (grd.SelectedCells[0].Column.DisplayIndex == 0)
        //                    {
        //                        txtAmount.Text = row_selected["Amount"].ToString();

        //                        txtCurrencyName.Text = row_selected["CurrencyName"].ToString();
        //                        btnSave.Content = "Update";     //Change save button text Save to Update
        //                    }

        //                    if (grd.SelectedCells[0].Column.DisplayIndex == 1)
        //                    {
        //                        if (MessageBox.Show("Are you sure you want to delete ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        //                        {
        //                            mycon();
        //                            DataTable dt = new DataTable();

        //                            cmd = new SqlCommand("DELETE FROM Currency_Master WHERE Id = @Id", con);
        //                            cmd.CommandType = CommandType.Text;

        //                            //CurrencyId set in @Id parameter and send it in delete statement
        //                            cmd.Parameters.AddWithValue("@Id", CurrencyId);
        //                            cmd.ExecuteNonQuery();
        //                            con.Close();

        //                            MessageBox.Show("Data deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        //                            ClearMaster();
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //private void tbMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //}

        //private void cmbToCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    try
        //    {
        //        if (cmbToCurrency.SelectedValue != null && int.Parse(cmbToCurrency.SelectedValue.ToString()) != 0 && cmbToCurrency.SelectedIndex != 0)
        //        {

        //            int CurrencyToId = int.Parse(cmbToCurrency.SelectedValue.ToString());

        //            mycon();

        //            DataTable dt = new DataTable();

        //            cmd = new SqlCommand("SELECT Amount FROM Currency_Master WHERE Id = @CurrencyToId", con);
        //            cmd.CommandType = CommandType.Text;

        //            if (CurrencyToId != null && CurrencyToId != 0)

        //                cmd.Parameters.AddWithValue("@CurrencyToId", CurrencyToId);

        //            da = new SqlDataAdapter(cmd);


        //            da.Fill(dt);

        //            if (dt != null && dt.Rows.Count > 0)

        //                ToAmount = double.Parse(dt.Rows[0]["Amount"].ToString());
        //            con.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //private void cmbFromCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    try
        //    {
        //        if (cmbFromCurrency.SelectedValue != null && int.Parse(cmbFromCurrency.SelectedValue.ToString()) != 0 && cmbFromCurrency.SelectedIndex != 0)
        //        {
        //            int CurrencyFromId = int.Parse(cmbFromCurrency.SelectedValue.ToString());

        //            mycon();
        //            DataTable dt = new DataTable();

        //            //Select query to get amount from database using id
        //            cmd = new SqlCommand("SELECT Amount FROM Currency_Master WHERE Id = @CurrencyFromId", con);
        //            cmd.CommandType = CommandType.Text;

        //            if (CurrencyFromId != null && CurrencyFromId != 0)
        //                cmd.Parameters.AddWithValue("@CurrencyFromId", CurrencyFromId);

        //            da = new SqlDataAdapter(cmd);

        //            da.Fill(dt);

        //            if (dt != null && dt.Rows.Count > 0)
        //                FromAmount = double.Parse(dt.Rows[0]["Amount"].ToString());

        //            con.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //private void cmbFromCurrency_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Tab || e.SystemKey == Key.Enter)
        //    {
        //        cmbFromCurrency_SelectionChanged(sender, null);
        //    }
        //}

        ////cmbToCurrency preview key down event
        //private void cmbToCurrency_PreviewKeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Tab || e.SystemKey == Key.Enter)
        //    {
        //        cmbToCurrency_SelectionChanged(sender, null);
        //    }
        //}

    }
}
