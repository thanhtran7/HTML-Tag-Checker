using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
/* Author: Thanh Tran
 * December 31, 2017
 * Description: A form that allow the user to upload a .html file to check for common html tags to ensure that they are closed. A status will appear displaying whether or not
 * the .html have balanced tags. This is done using the stack method, NOT THE PRETTIEST CODE. Exemption of certain tags found at line 282.
 */
namespace lab4b
{
    public partial class Form1 : Form
    {
      
        private FileStream input; // maintains the connection to the file
        private StreamReader fileReader; // reads data from text file
        string fileName; // holds file name 
        string resultName; // holds result name

        /// <summary>
        /// Generic stack class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class Stack<T>
        { 
            private int top; // location of the top element
            private T[] elements; //array that stores 

            public Stack() : this(50) // default stack size - CHANGE IF NEED BIGGER
            {
            }
            public Stack(int stackSize)
            {
                if (stackSize <= 0) // validate stackSize
                {
                    throw new ArgumentException("Stack size must be positive.");
                }

                elements = new T[stackSize]; // create stackSize elements
                top = -1; // stack initially empty
            }

            /// <summary>
            /// push element into the stack
            /// </summary>
            /// <param name="pushValue">values for element</param>
            public void Push(T pushValue)
            {
                if (top == elements.Length - 1) // stack is full
                {
                    throw new FullStackException(
                       $"Stack is full, cannot push {pushValue}");
                }

                ++top; // increment top
                elements[top] = pushValue; // place pushValue on stack
            }

            /// <summary>
            /// Remove the element and set top to null
            /// </summary>
            /// <returns></returns>
            public T Pop()
            {

                if (top == -1) // stack is empty
                {
                    throw new EmptyStackException("Stack is empty, cannot pop");
                }
                // change the top element to null when popping it
                elements[top] = default(T);
                --top; // decrement top
                return elements[top + 1]; // return top value
            }

            /// <summary>
            /// Used to view element that's at top
            /// </summary>
            /// <returns></returns>
            public T Peek()
            {
                return elements[top];
            }
            
            /// <summary>
            /// check if stack is empty or not
            /// </summary>
            /// <returns></returns>
            public Boolean IsEmpty()
            {
               if (elements[0] == null)
                {
                    return true;
                }
               else
                return false;
            }
        }

        public Form1()
        {
            InitializeComponent();
            
        }
        /// <summary>
        /// When file is clicked, it allow the user to upload an .html file from their computer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // create dialog box enabling user to open file
            DialogResult result;
            // clear textBox if there is anything
            TextBox.Clear();

            using (OpenFileDialog fileChooser = new OpenFileDialog())
            {
                // filter to only allow .html files to be opened
                fileChooser.Filter = "HTML Files (.html)|*.html"; // allow only .html files
                fileChooser.FilterIndex = 1;
                result = fileChooser.ShowDialog();
                fileName = fileChooser.FileName;

                // set file name to be displayed
                resultName = Path.GetFileName(fileName);
            }

            // exit event handler if user clicked Cancel
            if (result == DialogResult.OK)
            {
                // show error if user specified invalid file
                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("Invalid File Name", "Error",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    try
                    {
                        // create FileStream to obtain read access to file
                        FileStream input = new FileStream(
                           fileName, FileMode.Open, FileAccess.Read);

                        // set file from where data is read
                        fileReader = new StreamReader(input);
                        
                        //set file load status to show what file is loaded
                        fileLoadStatus.Text = String.Format("Loaded: {0}", resultName);

                        processToolStripMenuItem.Enabled = true;

                    }
                    catch (IOException)
                    {
                        MessageBox.Show("Error reading from file",
                           "File Error", MessageBoxButtons.OK,
                           MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void processToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        /// <summary>
        /// When Process, check tags is pressed, go through the html file check for tags. Use stack to keep track.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkTagsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // initalize stack
            Stack<string> stackHtml = new Stack<string>();
            int lineNumb = 1; //line number to display which line the tags are found on
            string spaces = "";
            int spaceNum = 0;


            // read until file is at a complete stop
            while (fileReader.Peek() >= 0)
            {
                // triggers to keep track of opening and closing
                Boolean match = false;
                Boolean first = false;
                Boolean second = false;
                Boolean third = false;
                Boolean spaceTrigger = false; // for spaces in tags

                String s = fileReader.ReadLine().ToLower(); // set s to line currently being read
                String temp = ""; // temp to hold and append char into
                // loop through the string s
                foreach (var c in s)
                {
                    if (temp.Substring(0).Equals(">"))
                    {
                        temp = "";
                    }
                    // first trigger if string has <
                    if (c == '<')
                    {
                        first = true;
                    }

                    // second trigger if string has / for closing. To elminate url being identified with /, use a space trigger
                    if (c == '/' && spaceTrigger == false)
                    {
                        second = true;
                    }

                    // last trigger which is >, run if statement depending on which var triggered
                    if (c == '>')
                    {
                        third = true;
                        temp = temp + c; // apend to temp

                        // once done, push opening tags to stack and print to textbox
                        // reset variables and temp
                        if (first == true && third == true && second == false)
                        {
                            stackHtml.Push(temp);
                            TextBox.AppendText($"{spaces}{lineNumb} - Found opening tag: {temp}");
                            spaces += "   ";
                            TextBox.AppendText("\n");
                            temp = "";
                        }

                        // if all triggers are true which mean they are closing tag, remove the / from temp then pop the element from stack
                        if (first == true && third == true && second == true && stackHtml.Peek().Equals(temp.Remove(1, 1)))
                        {
                            stackHtml.Pop();
                            spaces = spaces.Substring(0, spaces.Length - 3);
                            TextBox.AppendText($"{spaces}{lineNumb} - Found closing tag: {temp}");
                            TextBox.AppendText("\n");
                            temp = "";
                        }
                        
                        
                        // reset everything
                        first = false;
                        second = false;
                        third = false;
                        spaceTrigger = false;

                    }

                    // remove spaces within tag to keep it short
                    if (first == true && c == ' ')
                    {
                        spaceTrigger = true;
                    }

                    // append to temp variable
                    if (first == true && third == false && spaceTrigger == false)
                    {
                        temp = temp + c;
                    }

                    // ignore tags that don't have closing
                    if (temp.Equals("<img") || temp.Equals("<hr") || temp.Equals("<br") || temp.Equals("<!") || temp.Equals("<meta") || temp.Equals("<link") 
                        || temp.Equals("<iframe") || temp.Equals("<input") || temp.Equals("<area") || temp.Equals("<base") || temp.Equals("<col") || temp.Equals("<embed")
                        || temp.Equals("<keygen") || temp.Equals("<param") || temp.Equals("<source") || temp.Equals("<track") || temp.Equals("<wbr"))
                    {
                            first = false;
                            second = false;
                            third = false;
                            spaceTrigger = false;
                            temp = "";
                            continue;
                    }

                }
                lineNumb++;

            }

            // check if stack is empty, if it is that mean html is good
            if (stackHtml.IsEmpty() == true)
            {
                fileLoadStatus.Text = String.Format(resultName + " have balanced tags");
            }
            else
            {
                fileLoadStatus.Text = String.Format(resultName + " does not have balanced tags");
                spaces = spaces.Substring(0, spaces.Length - 3);
                TextBox.AppendText(spaces + stackHtml.Peek() + " tag located above in this position, please scroll up and look for line number to address the issue");
            }

        }
    }
}
