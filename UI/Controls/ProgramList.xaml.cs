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


namespace iRobotGUI.Controls
{
    /// <summary>
    /// Interaction logic for ProgramList.xaml
    /// </summary>
    public partial class ProgramList : UserControl
    {
        #region data

        private HLProgram program;

        private int selectedInstructionIndex;
        private Instruction selectedInstruction;

        bool canInitiateDrag;
        bool showDragAdorner;
        double dragAdornerOpacity = 0.7;
        int indexToSelect;
        Point ptMouseDown;
        DragAdorner dragAdorner;
        Instruction ItemUnderDragCursor;

        #endregion

        #region constructor

        public ProgramList()
        {
            InitializeComponent();
            this.canInitiateDrag = false;
            this.showDragAdorner = true;
            this.indexToSelect = -1;
        }

        #endregion

        private void ShowSongDialog(Instruction ins)
        {
            SongWindow dlg = new SongWindow();
            dlg.Owner = Window.GetWindow(this);
            dlg.Ins = ins;
            dlg.ShowDialog();
        }

        private void ShowLedDialog(Instruction ins)
        {
            LedWindow dlg = new LedWindow();
            dlg.Owner = Window.GetWindow(this);
            dlg.Ins = ins;
            dlg.ShowDialog();
            UpdateContent();
        }

        #region HLProgram

        public HLProgram Program
        {
            get
            {
                return program;
            }
            set
            {
                program = value;
                UpdateContent();
            }
        }

        #endregion

        #region event handling methods

        void listBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ClickCount == 2)
            {
                switch (selectedInstruction.opcode)
                {
                    case Instruction.FORWARD:
                        break;
                    case Instruction.LEFT:
                        break;
                    case Instruction.LED:
                        ShowLedDialog(selectedInstruction);
                        break;
                    case Instruction.SONG_DEF:
                        ShowSongDialog(selectedInstruction);
                        break;
                }
            }
            else
            {
                int index = this.IndexUnderDragCursor;
                this.canInitiateDrag = index > -1;

                if (this.canInitiateDrag)
                {
                    this.ptMouseDown = MouseUtilities.GetMousePosition(ListboxProgram);
                    this.indexToSelect = index;
                }
                else
                {
                    this.ptMouseDown = new Point(-10000, -10000);
                    this.indexToSelect = -1;
                }
                if (ListboxProgram.SelectedIndex != this.indexToSelect)
                    ListboxProgram.SelectedIndex = this.indexToSelect;

                if (ListboxProgram.SelectedItem == null)
                    return;

                ListBoxItem itemToDrag = this.GetListBoxItem(selectedInstructionIndex);
                if (itemToDrag == null)
                    return;

                AdornerLayer adornerLayer = this.ShowDragAdornerResolved ? this.InitializeAdornerLayer(itemToDrag) : null;

                this.PerformDragOperation();

                this.FinishDragOperation(itemToDrag, adornerLayer);
            }

        }

        void listBox_DragOver(object sender, DragEventArgs e)
        {
           if (this.ShowDragAdornerResolved)
                this.UpdateDragAdornerLocation();
            
            // Update the item which is known to be currently under the drag cursor.
            int index = this.IndexUnderDragCursor;
            this.ItemUnderDragCursor = index < 0 ? null : ListboxProgram.Items[index] as Instruction;
        }

        void listBox_DragLeave(object sender, DragEventArgs e)
        {
            if (!this.IsMouseOver(ListboxProgram))
            {
                if (this.ItemUnderDragCursor != null)
                    this.ItemUnderDragCursor = null;

                if (this.dragAdorner != null)
                    this.dragAdorner.Visibility = Visibility.Collapsed;
            }
        }

        void listBox_DragEnter(object sender, DragEventArgs e)
        {
            if (this.dragAdorner != null && this.dragAdorner.Visibility != Visibility.Visible)
            {
                this.UpdateDragAdornerLocation();
                this.dragAdorner.Visibility = Visibility.Visible;
            }
        }

        void listBox_Drop(object sender, DragEventArgs e)
        {
            /// drag inside program list
            if (e.Data.GetDataPresent("draghere"))
            {
                Instruction data = e.Data.GetData("draghere") as Instruction;
                if (data == null)
                    return;

                int oldIndex = ListboxProgram.Items.IndexOf(data);
                int newIndex = this.IndexUnderDragCursor;

                if (newIndex < 0)
                {
                    if (ListboxProgram.Items.Count == 0)
                        newIndex = 0;
                    else if (oldIndex < 0)
                        newIndex = ListboxProgram.Items.Count;
                    else
                        return;
                }

                if (oldIndex == newIndex)
                    return;

                ListboxProgram.Items.Remove(data);
                ListboxProgram.Items.Insert(newIndex, data);

                e.Effects = DragDropEffects.Move;

                program.Rearrange(data, newIndex);
            }
            /// drag from instructin panel to program list
            else
            {
                string op = (string)e.Data.GetData(DataFormats.StringFormat);
                Instruction newIns = null;
                switch (op)
                {
                    case Instruction.FORWARD:
                        newIns = new Instruction(Instruction.FORWARD + " 500,3");
                        break;
                    case Instruction.LEFT:
                        newIns = new Instruction(Instruction.LEFT + " 90");
                        break;
                    case Instruction.LED:
                        newIns = new Instruction(Instruction.LED + " 10,128,128");
                        break;
                    case Instruction.SONG_DEF:
                        newIns = new Instruction(Instruction.SONG_DEF + " 0");
                        break;
                }
                if (newIns != null)
                {
                    program.Add(newIns);
                }

                UpdateContent();
                ListboxProgram.SelectedItem = newIns;
            }
        }

        private void ListboxProgram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListboxProgram.SelectedItem != null)
            {
                selectedInstructionIndex = ListboxProgram.SelectedIndex;
                selectedInstruction = ListboxProgram.SelectedItem as Instruction;
            }
        }

        #endregion

        int IndexUnderDragCursor
        {
            get
            {
                int index = -1;
                for (int i = 0; i < ListboxProgram.Items.Count; ++i)
                {
                    ListBoxItem item = this.GetListBoxItem(i);
                    if (this.IsMouseOver(item))
                    {
                        index = i;
                        break;
                    }
                }
                return index;
            }
        }

        ListBoxItem GetListBoxItem(int index)
        {
            if (ListboxProgram.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;

            return ListboxProgram.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
        }

        bool IsMouseOver(Visual target)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            Point mousePos = MouseUtilities.GetMousePosition(target);
            return bounds.Contains(mousePos);
        }

        void PerformDragOperation()
        {
            Instruction selectedItem = ListboxProgram.SelectedItem as Instruction;
            DragDropEffects allowedEffects = DragDropEffects.Move | DragDropEffects.Move | DragDropEffects.Link;
            DataObject dragData = new DataObject("draghere", selectedItem);
            DragDrop.DoDragDrop(ListboxProgram, dragData, allowedEffects);
            ListboxProgram.SelectedItem = selectedItem;
        }

        void FinishDragOperation(ListBoxItem draggedItem, AdornerLayer adornerLayer)
        {
            if (this.ItemUnderDragCursor != null)
                this.ItemUnderDragCursor = null;

            if (adornerLayer != null)
            {
                adornerLayer.Remove(this.dragAdorner);
                this.dragAdorner = null;
            }
        }

        bool ShowDragAdornerResolved
        {
            get { return this.showDragAdorner && this.dragAdornerOpacity > 0.0; }
        }

        AdornerLayer InitializeAdornerLayer(ListBoxItem itemToDrag)
        {
            VisualBrush brush = new VisualBrush(itemToDrag);

            this.dragAdorner = new DragAdorner(ListboxProgram, itemToDrag.RenderSize, brush);

            // Set the drag adorner's opacity.		
            this.dragAdorner.Opacity = this.dragAdornerOpacity;

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(ListboxProgram);
            layer.Add(dragAdorner);

            this.ptMouseDown = MouseUtilities.GetMousePosition(ListboxProgram);

            return layer;
        }

        void UpdateDragAdornerLocation()
        {
            if (this.dragAdorner != null)
            {
                Point ptCursor = MouseUtilities.GetMousePosition(ListboxProgram);

                double left = ptCursor.X - this.ptMouseDown.X;

                ListBoxItem itemBeingDragged = this.GetListBoxItem(this.indexToSelect);
                Point itemLoc = itemBeingDragged.TranslatePoint(new Point(0, 0), ListboxProgram);
                double top = itemLoc.Y + ptCursor.Y - this.ptMouseDown.Y;

                this.dragAdorner.SetOffsets(left, top);
            }
        }

        public void UpdateContent()
        {
            ListboxProgram.Items.Clear();
            foreach (Instruction i in program.GetInstructionList())
            {
                ListboxProgram.Items.Add(i);
            }
        }
    }

}
