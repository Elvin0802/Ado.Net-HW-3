using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Task3.Views;

public partial class MainWindow : Window
{
	public SqlConnection? Conn { get; set; }
	public SqlDataAdapter? Adapter { get; set; }
	public DataSet? DataSet { get; set; }
	public string? ConnStr { get; set; }

	public MainWindow()
	{
		InitializeComponent();

		ConnStr = App.Configuration!.GetConnectionString("DefaultConnection")!;

		AuthorsNameCBox.ItemsSource = GetAuthorsName();

		AuthorsNameCBox.Items.Refresh();
	}

	private List<string>? GetAuthorsName()
	{
		List<string>? Authors = new();

		DataSet = new();

		string? query = $@"
							SELECT [FirstName], [LastName]
							FROM Authors
		";

		Conn = new(ConnStr);

		Adapter = new(query, Conn);

		Adapter.Fill(DataSet, "authors");

		foreach (DataRow row in DataSet.Tables["authors"]!.Rows)
		{
			Authors.Add($"{row["FirstName"]} | {row["LastName"]}");
		}

		return Authors;
	}

	private void NewAuthorSelected(object sender, SelectionChangedEventArgs e)
	{
		DataSet = new();

		string? fn = $"{AuthorsNameCBox.SelectedValue.ToString()!.Split('|')[0]}",
					ln = $"{AuthorsNameCBox.SelectedValue.ToString()!.Split('|')[1]}";

		string? query = $@"
								SELECT [Name], [Pages], [YearPress], [Comment], [Quantity]   
								FROM Authors INNER JOIN Books
								ON Authors.Id = Books.Id_Author
								WHERE Authors.[FirstName] IN(N'{fn}') OR Authors.[LastName] IN(N'{ln}')
		";

		Conn = new(ConnStr);

		Adapter = new(query, Conn);

		Adapter.Fill(DataSet, "books");

		BooksDG.ItemsSource = DataSet.Tables["books"]!.DefaultView;
		BooksDG.Items.Refresh();
	}
}