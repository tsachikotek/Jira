using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;
using JiraHelper;
using System.Reflection;
using System.IO;

namespace WordHelper
{
    public class wordCreate
    {
        public Word._Application oWord;
        public Word._Document oDoc;
        public object oMissing = System.Reflection.Missing.Value;
        public object oEndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark */

        public void New()
        {
            //Start Word and create a new document.

            oWord = new Word.Application();
            oWord.Visible = false;
            oDoc = oWord.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);

            insertNewParagraph(oDoc, "OPENNING PAGE", "Openning");
            oDoc.Words.Last.InsertBreak(Word.WdBreakType.wdPageBreak);

            insertNewParagraph(oDoc, "TABLE OF CONTENT", "TOC");
            insertTOC(oWord, oDoc);
            oDoc.Words.Last.InsertBreak(Word.WdBreakType.wdPageBreak);
        }

        public void Open(string template)
        {
            //Start Word and create a new document.

            if (File.Exists(template))
            {                
                oWord = new Word.Application();
                oWord.Visible = false;
                oDoc = oWord.Documents.Open(template, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            }
            else {
                New();
            }
        }

        public void Save(string filename)
        {
            oDoc.SaveAs(filename, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            
            oDoc.SaveAs2(@"C:\Temp\TestDocumentWith1Paragraph.docx");

            oDoc.Close(ref oMissing, ref oMissing, ref oMissing);

            oWord.Quit();
        }

        public void AddIssue(Issue jiraIssue)
        {           
            addJiraIssue(oDoc, jiraIssue);

            //insertChart(oWord, oDoc);
            //addTable(oWord, oDoc);
            //insertText(oDoc, "END DOCUMENT!!!");


            updateTOC(oWord, oDoc);
        }

        public void create(JiraIssues jiraIssues)
        {

            New();            

            //insertNewParagraph(oDoc, "Jira Issues:", "Content");
            int x = 0;
            int total = jiraIssues.issues.Count();
            foreach (var issue in jiraIssues.issues)
            {   
                addJiraIssue(oDoc, issue);

                //insertChart(oWord, oDoc);
                //addTable(oWord, oDoc);
                //insertText(oDoc, "END DOCUMENT!!!");
            }

            updateTOC(oWord, oDoc);
            //insertNewParagraph(oDoc, "Jira Issues:", null);
            //insertTOC(oWord, oDoc);
            //Close this form.
            //this.Close();
        }

        private static void insertNewParagraph(Word._Document oDoc, string title, string bookmark)
        {
            object oMissing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark */

            //Insert a paragraph at the end of the document.
            Word.Paragraph oPara;
            object oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara.Range.Text = title;
            oPara.Range.Font.Bold = 1;
            if (bookmark != null & bookmark != string.Empty) oDoc.Bookmarks.Add(bookmark);
            oPara.Format.SpaceAfter = 6;
            oPara.Range.InsertParagraphAfter();
        }

        private static void insertText(Word._Document oDoc, string text)
        {
            object oMissing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark */

            Word.Range wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;

            //Add text after the chart.
            wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            wrdRng.InsertParagraphAfter();
            wrdRng.InsertAfter(text);


        }

        private static void insertChart(Word._Application oWord, Word._Document oDoc)
        {
            object oMissing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark */

            Word.Range wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;

            //Insert a chart.
            Word.InlineShape oShape;
            object oClassType = "MSGraph.Chart.8";
            wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oShape = wrdRng.InlineShapes.AddOLEObject(ref oClassType, ref oMissing,
            ref oMissing, ref oMissing, ref oMissing,
            ref oMissing, ref oMissing, ref oMissing);

            //Demonstrate use of late bound oChart and oChartApp objects to
            //manipulate the chart object with MSGraph.
            object oChart;
            object oChartApp;
            oChart = oShape.OLEFormat.Object;
            oChartApp = oChart.GetType().InvokeMember("Application",
            BindingFlags.GetProperty, null, oChart, null);

            //Change the chart type to Line.
            object[] Parameters = new Object[1];
            Parameters[0] = 4; //xlLine = 4
            oChart.GetType().InvokeMember("ChartType", BindingFlags.SetProperty,
            null, oChart, Parameters);

            //Update the chart image and quit MSGraph.
            oChartApp.GetType().InvokeMember("Update",
            BindingFlags.InvokeMethod, null, oChartApp, null);
            oChartApp.GetType().InvokeMember("Quit",
            BindingFlags.InvokeMethod, null, oChartApp, null);
            //... If desired, you can proceed from here using the Microsoft Graph 
            //Object model on the oChart and oChartApp objects to make additional
            //changes to the chart.

            //Set the width of the chart.
            oShape.Width = oWord.InchesToPoints(6.25f);
            oShape.Height = oWord.InchesToPoints(3.57f);

        }

        private static void addTable(Word._Application oWord, Word._Document oDoc)
        {
            object oMissing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark */

            //Insert a 3 x 5 table, fill it with data, and make the first row
            //bold and italic.
            Word.Table oTable;
            Word.Range wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oTable = oDoc.Tables.Add(wrdRng, 3, 5, ref oMissing, ref oMissing);
            oTable.Range.ParagraphFormat.SpaceAfter = 6;
            int r, c;
            string strText;
            for (r = 1; r <= 3; r++)
                for (c = 1; c <= 5; c++)
                {
                    strText = "r" + r + "c" + c;
                    oTable.Cell(r, c).Range.Text = strText;
                }
            oTable.Rows[1].Range.Font.Bold = 1;
            oTable.Rows[1].Range.Font.Italic = 1;

            //Add some text after the table.
            Word.Paragraph oPara4;
            object oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara4 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara4.Range.InsertParagraphBefore();
            oPara4.Range.Text = "And here's another table:";
            oPara4.Format.SpaceAfter = 24;
            oPara4.Range.InsertParagraphAfter();

            //Insert a 5 x 2 table, fill it with data, and change the column widths.
            wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oTable = oDoc.Tables.Add(wrdRng, 5, 2, ref oMissing, ref oMissing);
            oTable.Range.ParagraphFormat.SpaceAfter = 6;
            for (r = 1; r <= 5; r++)
                for (c = 1; c <= 2; c++)
                {
                    strText = "r" + r + "c" + c;
                    oTable.Cell(r, c).Range.Text = strText;
                }
            oTable.Columns[1].Width = oWord.InchesToPoints(2); //Change width of columns 1 & 2
            oTable.Columns[2].Width = oWord.InchesToPoints(3);

            //Keep inserting text. When you get to 7 inches from top of the
            //document, insert a hard page break.
            object oPos;
            double dPos = oWord.InchesToPoints(7);
            oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range.InsertParagraphAfter();
            do
            {
                wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                wrdRng.ParagraphFormat.SpaceAfter = 6;
                wrdRng.InsertAfter("A line of text");
                wrdRng.InsertParagraphAfter();
                oPos = wrdRng.get_Information
                                       (Word.WdInformation.wdVerticalPositionRelativeToPage);
            }
            while (dPos >= Convert.ToDouble(oPos));
            object oCollapseEnd = Word.WdCollapseDirection.wdCollapseEnd;
            object oPageBreak = Word.WdBreakType.wdPageBreak;
            wrdRng.Collapse(ref oCollapseEnd);
            wrdRng.InsertBreak(ref oPageBreak);
            wrdRng.Collapse(ref oCollapseEnd);
            wrdRng.InsertAfter("We're now on page 2. Here's my chart:");
            wrdRng.InsertParagraphAfter();
        }

        private static void addJiraIssue(Word._Document oDoc, Issue issue)
        {
            object oEndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark */

            Word.Paragraph oPara2;
            object oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara2 = oDoc.Content.Paragraphs.Add(ref oRng);
            object styleHeading1 = "Heading 1";
            oPara2.Range.Text = issue.key;
            oPara2.Range.set_Style(ref styleHeading1);
            oPara2.Format.SpaceAfter = 6;
            oPara2.Range.InsertParagraphAfter();

            //Insert another paragraph.
            Word.Paragraph oPara3;
            oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara3 = oDoc.Content.Paragraphs.Add(ref oRng);

            Fields fields = issue.fields; // new List<string>();

            foreach (PropertyInfo propertyInfo in issue.fields.GetType().GetProperties())
            {
                // do stuff here
                oPara3.Range.Text = propertyInfo.Name + ": " + propertyInfo.GetValue(issue.fields);
                oPara3.Range.Font.Bold = 0;
                //oPara3.Format.SpaceAfter = 24;
                oPara3.Range.InsertParagraphAfter();
            }

            oPara3.Format.SpaceAfter = 24;
            //oPara3.Range.Text = issue.fields.description;
            //oPara3.Range.Font.Bold = 0;
            //oPara3.Format.SpaceAfter = 24;
            //oPara3.Range.InsertParagraphAfter();
        }

        private static void insertTOC(Word._Application oWord, Word._Document oDoc)
        {
            Object oClassType = "Word.Document.8";
            Object oTrue = true;
            Object oFalse = false;
            Object oMissing = System.Reflection.Missing.Value;

            object oEndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark */
            Word.Range rngTOC = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;

            //Add the TOC to the document

            //Object oBookmarkTOC = "TOC";
            // SETTING THE RANGE AT THE BOOKMARK  
            //Word.Range rngTOC = oDoc.Bookmarks.get_Item(ref oBookmarkTOC).Range;
            // SELECTING THE SET RANGE  
            rngTOC.Select();
            // INCLUDING THE TABLE OF CONTENTS  
            Object oUpperHeadingLevel = "1";
            Object oLowerHeadingLevel = "3";
            Object oTOCTableID = "TableOfContents";
            oDoc.TablesOfContents.Add(rngTOC, ref oTrue, ref oUpperHeadingLevel,
                ref oLowerHeadingLevel, ref oMissing, ref oTOCTableID, ref oTrue,
                ref oTrue, ref oMissing, ref oTrue, ref oTrue, ref oTrue);

            //UPDATING THE TABLE OF CONTENTS  
            oDoc.TablesOfContents[1].Update();
            //UPDATING THE TABLE OF CONTENTS  
            oDoc.TablesOfContents[1].UpdatePageNumbers();


        }

        private static void updateTOC(Word._Application oWord, Word._Document oDoc)
        {
            //UPDATING THE TABLE OF CONTENTS  
            oDoc.TablesOfContents[1].Update();
            //UPDATING THE TABLE OF CONTENTS  
            oDoc.TablesOfContents[1].UpdatePageNumbers();


        }

        static void create()
        {
            object oMissing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark */

            //Start Word and create a new document.
            Word._Application oWord;
            Word._Document oDoc;
            oWord = new Word.Application();
            oWord.Visible = true;
            oDoc = oWord.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);

            //Insert a paragraph at the beginning of the document.
            Word.Paragraph oPara1;
            oPara1 = oDoc.Content.Paragraphs.Add(ref oMissing);
            oPara1.Range.Text = "Heading 1";
            oPara1.Range.Font.Bold = 1;
            oPara1.Format.SpaceAfter = 24;    //24 pt spacing after paragraph.
            oPara1.Range.InsertParagraphAfter();

            //Insert a paragraph at the end of the document.
            Word.Paragraph oPara2;
            object oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara2 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara2.Range.Text = "Heading 2";
            oPara2.Format.SpaceAfter = 6;
            oPara2.Range.InsertParagraphAfter();

            //Insert another paragraph.
            Word.Paragraph oPara3;
            oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara3 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara3.Range.Text = "This is a sentence of normal text. Now here is a table:";
            oPara3.Range.Font.Bold = 0;
            oPara3.Format.SpaceAfter = 24;
            oPara3.Range.InsertParagraphAfter();

            //Insert a 3 x 5 table, fill it with data, and make the first row
            //bold and italic.
            Word.Table oTable;
            Word.Range wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oTable = oDoc.Tables.Add(wrdRng, 3, 5, ref oMissing, ref oMissing);
            oTable.Range.ParagraphFormat.SpaceAfter = 6;
            int r, c;
            string strText;
            for (r = 1; r <= 3; r++)
                for (c = 1; c <= 5; c++)
                {
                    strText = "r" + r + "c" + c;
                    oTable.Cell(r, c).Range.Text = strText;
                }
            oTable.Rows[1].Range.Font.Bold = 1;
            oTable.Rows[1].Range.Font.Italic = 1;

            //Add some text after the table.
            Word.Paragraph oPara4;
            oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara4 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara4.Range.InsertParagraphBefore();
            oPara4.Range.Text = "And here's another table:";
            oPara4.Format.SpaceAfter = 24;
            oPara4.Range.InsertParagraphAfter();

            //Insert a 5 x 2 table, fill it with data, and change the column widths.
            wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oTable = oDoc.Tables.Add(wrdRng, 5, 2, ref oMissing, ref oMissing);
            oTable.Range.ParagraphFormat.SpaceAfter = 6;
            for (r = 1; r <= 5; r++)
                for (c = 1; c <= 2; c++)
                {
                    strText = "r" + r + "c" + c;
                    oTable.Cell(r, c).Range.Text = strText;
                }
            oTable.Columns[1].Width = oWord.InchesToPoints(2); //Change width of columns 1 & 2
            oTable.Columns[2].Width = oWord.InchesToPoints(3);

            //Keep inserting text. When you get to 7 inches from top of the
            //document, insert a hard page break.
            object oPos;
            double dPos = oWord.InchesToPoints(7);
            oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range.InsertParagraphAfter();
            do
            {
                wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                wrdRng.ParagraphFormat.SpaceAfter = 6;
                wrdRng.InsertAfter("A line of text");
                wrdRng.InsertParagraphAfter();
                oPos = wrdRng.get_Information
                                       (Word.WdInformation.wdVerticalPositionRelativeToPage);
            }
            while (dPos >= Convert.ToDouble(oPos));
            object oCollapseEnd = Word.WdCollapseDirection.wdCollapseEnd;
            object oPageBreak = Word.WdBreakType.wdPageBreak;
            wrdRng.Collapse(ref oCollapseEnd);
            wrdRng.InsertBreak(ref oPageBreak);
            wrdRng.Collapse(ref oCollapseEnd);
            wrdRng.InsertAfter("We're now on page 2. Here's my chart:");
            wrdRng.InsertParagraphAfter();

            //Insert a chart.
            Word.InlineShape oShape;
            object oClassType = "MSGraph.Chart.8";
            wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oShape = wrdRng.InlineShapes.AddOLEObject(ref oClassType, ref oMissing,
            ref oMissing, ref oMissing, ref oMissing,
            ref oMissing, ref oMissing, ref oMissing);

            //Demonstrate use of late bound oChart and oChartApp objects to
            //manipulate the chart object with MSGraph.
            object oChart;
            object oChartApp;
            oChart = oShape.OLEFormat.Object;
            oChartApp = oChart.GetType().InvokeMember("Application",
            BindingFlags.GetProperty, null, oChart, null);

            //Change the chart type to Line.
            object[] Parameters = new Object[1];
            Parameters[0] = 4; //xlLine = 4
            oChart.GetType().InvokeMember("ChartType", BindingFlags.SetProperty,
            null, oChart, Parameters);

            //Update the chart image and quit MSGraph.
            oChartApp.GetType().InvokeMember("Update",
            BindingFlags.InvokeMethod, null, oChartApp, null);
            oChartApp.GetType().InvokeMember("Quit",
            BindingFlags.InvokeMethod, null, oChartApp, null);
            //... If desired, you can proceed from here using the Microsoft Graph 
            //Object model on the oChart and oChartApp objects to make additional
            //changes to the chart.

            //Set the width of the chart.
            oShape.Width = oWord.InchesToPoints(6.25f);
            oShape.Height = oWord.InchesToPoints(3.57f);

            //Add text after the chart.
            wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            wrdRng.InsertParagraphAfter();
            wrdRng.InsertAfter("THE END.");

            //Close this form.
            //this.Close();
        }
    }
}
