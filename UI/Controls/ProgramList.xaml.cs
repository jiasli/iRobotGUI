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

	public class DisplayItem
	{
		//private Image im;
		private Uri imageuri;
		private string description;

		/// <summary>
		/// constructor
		/// </summary>
		public DisplayItem(Uri imageuri, string description)
		{
			this.imageuri = imageuri;
			this.description = description;
		}

		/// <summary>
		/// Image Uri
		/// </summary>
		public Uri ImageUri
		{
			get
			{
				return imageuri;
			}
			set
			{
				imageuri = value;
			}
		}

		/// <summary>
		/// Text Description
		/// </summary>
		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

	}

	/// <summary>
	/// Interaction logic for ProgramList.xaml
	/// </summary>
	public partial class ProgramList : UserControl
	{
		#region data

		private ListViewDragDropManager<DisplayItem> dragMgr;

		private ProgramViewModel pvm;

		#endregion // data

		#region constructor

		/// <summary>
		/// Initializes a new Instance of ProgramList
		/// </summary>
		public ProgramList()
		{
			InitializeComponent();

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
			this.dragMgr = new ListViewDragDropManager<DisplayItem>(listViewProgram);
			listViewProgram.PreviewMouseLeftButtonDown += listView_PreviewMouseLeftButtonDown;
			listViewProgram.Drop -= dragMgr.listView_Drop;
			listViewProgram.Drop += listView_Drop;
		}

		#endregion // ListView1_Loaded

		#region event handler

		#region NewlistView_PreviewMouseLeftButtonDown

		/// <summary>
		/// Open the dialog when an item is double clicked
		/// </summary>
		void listView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				// The index of item in PVM
				int index = listViewProgram.SelectedIndex;
				if (index < 0)
					return;

				ShowParamWindow(index);
			}
		}

		#endregion // NewlistView_PreviewMouseLeftButtonDown


		#region NewlistView_Drop

		/// <summary>
		/// handler for drop event
		/// </summary>
		void listView_Drop(object sender, DragEventArgs e)
		{
			int newIndex = this.dragMgr.IndexUnderDragCursor;

			// drag inside program list
			if (this.dragMgr.IsDragInProgress)
			{
				DisplayItem data = e.Data.GetData(typeof(DisplayItem)) as DisplayItem;

				if (data == null)
					return;

				int oldIndex = this.listViewProgram.Items.IndexOf(data);

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

				Instruction newIns = Instruction.CreatDefaultFromOpcode(op);

				if (newIns != null)
				{
					if (op == Instruction.IF || op == Instruction.LOOP)
					{
						// Add HLProgram for IF and LOOP.
						pvm.InsertSubProgram(newIndex, HLProgram.GetDefaultIfLoopBlock(newIns));
					}
					else
					{
						// Add single Instruction.
						pvm.InsertInstruction(newIndex, newIns);
					}
				}

				UpdateContent();
				listViewProgram.SelectedIndex = newIndex;
				if (Properties.Settings.Default.PopupWindowForNewIns)
					ShowParamWindow(newIndex);
			}
		}

		#endregion // NewlistView_Drop

		#endregion // event handler

		#region private operations

		/// <summary>
		/// Update content in ProgramList accordint to pvm
		/// </summary>
		private void UpdateContent()
		{
			// Save the selected index and restore it when refreshing finishes.
			int selectedIndex = listViewProgram.SelectedIndex;

			listViewProgram.Items.Clear();

			for (int i = 0; i < pvm.Count; i++)
			{
				Instruction ins = pvm.GetInstruction(pvm[i]);
				//Image icon = GetImageFromInstruction(ins);
				//DisplayItem itemToDisplay = new DisplayItem(icon, TextDescriber.GetTextDescription(ins));
				Uri path = GetPathFromInstruction(ins);
				DisplayItem itemToDisplay = new DisplayItem(path, TextDescription.GetTextDescription(ins));
				if (itemToDisplay != null)
				{
					listViewProgram.Items.Add(itemToDisplay);
				}
			}
			listViewProgram.SelectedIndex = selectedIndex;
		}

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
			string picName = PictureDiscription.GetPictureName(ins);
			bi.UriSource = new Uri(picPath + picName, UriKind.Relative);
			if (bi.UriSource == null) return null;
			bi.EndInit();

			im.Stretch = Stretch.Fill;
			im.Source = bi;
			im.Width = 45;
			im.Height = 45;
			return im;
		}

		/// <summary>
		/// Get the image uri given a specified instruction
		/// </summary>
		private Uri GetPathFromInstruction(Instruction ins)
		{
			string picPath = "/iRobotGUI;component/pic/";
			string picName = PictureDiscription.GetPictureName(ins);
			return new Uri(picPath + picName, UriKind.Relative);
		}

		/// <summary>
		/// pop up the parameter window
		/// </summary>
		/// <param name="index">index in pvm</param>
		private void ShowParamWindow(int index)
		{
			// The Ins under modification
			Instruction selectedIns = pvm.GetInstruction(pvm[index]);

			if (selectedIns.opcode == Instruction.IF || selectedIns.opcode == Instruction.LOOP)
			{
				HLProgram subProgram = pvm.GetSubProgram(pvm[index]);

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

		#endregion // private operations

		#region file operations

		private void ListCopyExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			CopySelection();
		}

		private void ListCutExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			CopySelection();
			RemoveSelection();
		}

		private void ListPasteExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			int newIndex = listViewProgram.SelectedIndex;
			if (newIndex < 0)
				newIndex = pvm.Count;

			int pvmvalue = Int32.Parse(Clipboard.GetText());
			Instruction ins = pvm.GetInstruction(pvmvalue);

			if (ins.opcode == Instruction.IF || ins.opcode == Instruction.LOOP)
			{
				pvm.InsertSubProgram(newIndex, pvm.GetSubProgram(pvmvalue));
			}
			else
			{
				String insstring = ins.ToString();
				Instruction newins = new Instruction(insstring);
				pvm.InsertInstruction(newIndex, newins);
			}

			UpdateContent();
		}

		private void ListCutCopyCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = listViewProgram.SelectedIndex >= 0;
		}

		private void ListPasteCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Clipboard.ContainsText();
		}

		private void CopySelection()
		{
			int index = listViewProgram.SelectedIndex;
			if (index < 0)
				return;

			Clipboard.Clear();
			Clipboard.SetText(pvm[index].ToString());
		}

		private void RemoveSelection()
		{
			int index = listViewProgram.SelectedIndex;
			if (index < 0)
				return;

			// Just remove the pointer. We don't care the source in HLProgram.
			pvm.Remove(pvm[index]);

			UpdateContent();
		}

		#endregion // file operations

		private void DeleteCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if (listViewProgram.SelectedIndex != -1)
			{
				e.CanExecute = true;
			}
		}

		private void DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			RemoveSelection();
		}

	}
}