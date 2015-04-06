using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using iRobotGUI.Util;


namespace iRobotGUI.Controls
{
	/// <summary>
	/// Interaction logic for ProgramList.xaml
	/// </summary>
	public partial class ProgramList : UserControl
	{
		#region data

		private ListViewDragDropManager<Image> dragMgr;

		private ProgramViewModel pvm;

		#endregion // data

		#region constructor

		/// <summary>
		/// Initializes a new Instance of ProgramList
		/// </summary>
		public ProgramList()
		{
			InitializeComponent();

			RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
			this.Loaded += ListView1_Loaded;

			pvm = new ProgramViewModel(new HLProgram());
		}

		#endregion // constructor

		#region HLProgram

		/// <summary>
		/// Gets/sets the program so that the program can be updated when some function is dropped
		/// </summary>
		public HLProgram Program
		{
			get
			{
				return pvm.GetHLProgram();
			}
			set
			{
				pvm = new ProgramViewModel(value);
				UpdateContent();
			}
		}

		#endregion // HLProgram

		#region ListView1_Loaded

		/// <summary>
		/// Load event handler
		/// </summary>
		void ListView1_Loaded(object sender, RoutedEventArgs e)
		{
			this.dragMgr = new ListViewDragDropManager<Image>(ListviewProgram);
			ListviewProgram.PreviewMouseLeftButtonDown += NewlistView_PreviewMouseLeftButtonDown;
			ListviewProgram.Drop -= dragMgr.listView_Drop;
			ListviewProgram.Drop += NewlistView_Drop;
		}

		#endregion // ListView1_Loaded

		#region event handler

		#region NewlistView_PreviewMouseLeftButtonDown

		/// <summary>
		/// Open the dialog when an item is double clicked
		/// </summary>
		void NewlistView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				// The index of item in PVM
				int index = ListviewProgram.SelectedIndex;
				if (index < 0)
					return;

				PopUpWindow(index);
			}

		}

		#endregion // NewlistView_PreviewMouseLeftButtonDown

		#region listView_PreviewMouseRightButtonDown

		/// <summary>
		///  delete item when right button is clicked
		/// </summary>
		void ListviewProgram_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
			{
				int index = ListviewProgram.SelectedIndex;
				if (index < 0)
					return;

				// Just remove the pointer. We don't care the source in HLProgram.
				pvm.Remove(index);

				UpdateContent();
			}
		}


		#endregion // listView_PreviewMouseRightButtonDown

		#region NewlistView_Drop

		/// <summary>
		/// handler for drop event
		/// </summary>
		void NewlistView_Drop(object sender, DragEventArgs e)
		{
			int newIndex = this.dragMgr.IndexUnderDragCursor;

			// drag inside program list
			if (this.dragMgr.IsDragInProgress)
			{
				Image data = e.Data.GetData(typeof(Image)) as Image;

				if (data == null)
					return;

				int oldIndex = this.ListviewProgram.Items.IndexOf(data);

				if (newIndex < 0)
					return;

				if (oldIndex == newIndex)
					return;

				int item = pvm[oldIndex];
				pvm.Remove(item);
				pvm.Insert(newIndex, item);

				UpdateContent();
				e.Effects = DragDropEffects.Move;
			}
			// drag from instruction panel to program list
			else
			{
				if (newIndex < 0)
					newIndex = pvm.Count;

				string op = (string)e.Data.GetData(DataFormats.StringFormat);

				Instruction newIns = Instruction.CreatFromOpcode(op);

				if (newIns != null)
				{
					if (op == Instruction.IF || op == Instruction.LOOP)
					{
						// Add HLProgram for IF and LOOP.
						pvm.InsertSubProgram(newIndex, HLProgram.GetIfLoopBlock(newIns));
					}
					else
					{
						// Add single Instruction.
						pvm.InsertInstruction(newIndex, newIns);
					}
				}

				UpdateContent();
				if (Properties.Settings.Default.PopupWindowForNewIns)
					PopUpWindow(newIndex);
				ListviewProgram.SelectedItem = newIns;
			}
		}

		#endregion // NewlistView_Drop

		#endregion // event handler

		#region UpdateContent

		/// <summary>
		/// Update content in ProgramList accordint to pvm
		/// </summary>
		private void UpdateContent()
		{
			ListviewProgram.Items.Clear();

			for (int i = 0; i < pvm.Count; i++)
			{
				Instruction ins = pvm.GetInstruction(i);
				Image im = GetImageFromInstruction(ins);
				if (im != null)
					ListviewProgram.Items.Add(GetImageFromInstruction(ins));
			}

		}

		#endregion

		#region GetImageFromInstruction

		/// <summary>
		/// Get the corresponding image from a specific instruction
		/// </summary>
		/// <param name="ins"> The Instruction </param>
		private Image GetImageFromInstruction(Instruction ins)
		{
			string picPath = "/iRobotGUI;component/pic/";
			Image im = new Image();
			BitmapImage bi = new BitmapImage();

			bi.BeginInit();
			string picName = InstructionPicture.GetPictureName(ins);
			bi.UriSource = new Uri(picPath + picName, UriKind.Relative);

			if (bi.UriSource == null) return null;
			bi.EndInit();
			im.Stretch = Stretch.Fill;
			im.Source = bi;
			im.Width = 50;
			im.Height = 50;
			return im;
		}

		#endregion

		/// <summary>
		/// pop up the parameter window
		/// </summary>
		/// <param name="index">index in pvm</param>
		private void PopUpWindow(int index)
		{
			// The Ins under modification
			Instruction selectedIns = pvm.GetInstruction(index);

			if (selectedIns.opcode == Instruction.IF || selectedIns.opcode == Instruction.LOOP)
			{
				HLProgram subProgram = pvm.GetSubProgram(index);

				// invoke the dialog
				HLProgram result = DialogInvoker.ShowDialog(subProgram, Window.GetWindow(this));

				pvm.UpdateSubProgram(index, result);
			}
			else
			{
				Instruction result = DialogInvoker.ShowDialog(selectedIns, Window.GetWindow(this));
				pvm.UpdateInstruction(index, result);
			}

			UpdateContent();
		}

	}
}