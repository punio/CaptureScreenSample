using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace CaptureScreenSample
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog();
			dialog.DefaultExt = "jpg";
			dialog.Filter = "JPEG File|*.jpg|All Files|*";
			if (!(dialog.ShowDialog() ?? false)) return;

			var width = (int)CaptureTarget.ActualWidth;
			var height = (int)CaptureTarget.ActualHeight;

			// UIElement to RenderTargetBitmap
			var renderTargetBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
			var visual = new DrawingVisual();
			using (var context = visual.RenderOpen())
			{
				var brush = new VisualBrush(CaptureTarget);
				context.DrawRectangle(brush, null, new Rect(0, 0, width, height));
			}
			renderTargetBitmap.Render(visual);

			var encoder = new JpegBitmapEncoder();

			// Add exif data
			var metadata = new BitmapMetadata("jpg");
			metadata.ApplicationName = Assembly.GetExecutingAssembly().GetName().Name;
			metadata.Comment = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			metadata.DateTaken = DateTime.UtcNow.ToString(DateTimeFormatInfo.InvariantInfo);
			//Custom metadata
			//metadata.SetQuery("", "");

			encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap, null, metadata, null));

			using (var stream = dialog.OpenFile())
			{
				encoder.Save(stream);
			}
		}
	}
}
