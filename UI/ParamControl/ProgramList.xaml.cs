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

		public delegate void ProgramListEventHandler();
		public delegate void SelectedInstructionChangedEventHandler(string insString);

		public event ProgramListEventHandler ProgramChanged;
		public event ProgramListEventHandler ClipboardChanged;
		public event SelectedInstructionChangedEventHandler SelectedInstructionChanged;

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
				listViewProgram.SelectedIndex = newIndex;

				UpdateContent();
				e.Effects = DragDropEffects.Move;
			}
			// drag from instruction panel to program list
			else
			{
				if (newIndex < 0)
					newIndex = pvm.Count;

				string op = (string)e.Data.GetData(DataFormats.StringFormat);

				InsertNewInstruction(newIndex, op);				
			}
		}

		/// <summary>
		/// Insert a new instruction at specified index and update the content.
		/// </summary>	
		/// <param name="index">The zero-base index at which the new instruction should be inserted.</param>
		/// <param name="opCode">The opcode of the instruction.</param>
		public void InsertNewInstruction(int index, string opCode)
		{
			Instruction newIns = Instruction.CreatDefaultFromOpcode(opCode);

			if (newIns != null)
			{
				if (opCode == Instruction.IF || opCode == Instruction.LOOP)
				{
					// Add HLProgram for IF and LOOP.
					pvm.InsertSubProgram(index, HLProgram.GetDefaultIfLoopBlock(newIns));
				}
				else
				{
					// Add single Instruction.
					pvm.InsertInstruction(index, newIns);
				}
				UpdateContent();
				listViewProgram.SelectedIndex = index;

				// To ensure that the selected item is visible.
				listViewProgram.ScrollIntoView(listViewProgram.SelectedItem);

				if (Properties.Settings.Default.PopupWindowForNewIns)
					ShowParamWindow(index);
			}
		}

		/// <summary>
		/// Add a new instruction to the end of the list.
		/// </summary>	
		/// <param name="opCode">The opcode of the instruction.</param>
		public void AddNewInstruction(string opCode)
		{
			int index = pvm.Count;
			InsertNewInstruction(index, opCode);
		}

		#endregion // NewlistView_Drop

		#endregion // event handler

		#region private operations

		/// <summary>
		/// Update content in ProgramList according to pvm
		/// </summary>
		private void UpdateContent()
		{
			// Save the selected index and restore it when refreshing finishes.
			int selectedIndex = listViewProgram.SelectedIndex;

			listViewProgram.Items.Clear();

			for (int i = 0; i < pvm.Count; i++)
			{
				Instruction ins = pvm.GetInstruction(i);
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

			// Fire ProgramChanged event.
			if (ProgramChanged != null)
			{
				ProgramChanged();
			}
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

		#endregion // private operations

		#region Edit

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

		private void CopyExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			CopySelection();
		}

		private void CutExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			CopySelection();
			RemoveSelection();
		}

		private void PasteExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			int newIndex = listViewProgram.SelectedIndex;
			if (newIndex < 0)
				newIndex = pvm.Count;

			string subProgramString = Clipboard.GetText();
			HLProgram subProgram = new HLProgram(subProgramString);
			pvm.InsertSubProgram(newIndex, subProgram);

			UpdateContent();
		}

		private void CutCopyCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = listViewProgram.SelectedIndex >= 0;
		}

		private void PasteCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Clipboard.ContainsText();
		}

		private void CopySelection()
		{
			int index = listViewProgram.SelectedIndex;
			if (index < 0)
				return;

			Clipboard.Clear();
			Clipboard.SetText(pvm.GetSubProgram(index).ToString());

			if (ClipboardChanged != null)
			{
				ClipboardChanged();
			}
		}

		private void RemoveSelection()
		{
			int index = listViewProgram.SelectedIndex;
			if (index < 0)
				return;

			// Just remove the pointer. We don't care the source in HLProgram.
			pvm.RemoveAt(index);

			UpdateContent();
		}

		#endregion // file operations

		private void listViewProgram_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (SelectedInstructionChanged != null)
			{
				if (listViewProgram.SelectedIndex != -1)
					SelectedInstructionChanged(pvm.GetSubProgram(listViewProgram.SelectedIndex).ToString());
			}
		}



	}
}