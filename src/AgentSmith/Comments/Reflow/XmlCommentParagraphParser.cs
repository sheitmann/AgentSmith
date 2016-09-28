using System;
using System.Collections.Generic;

namespace AgentSmith.Comments.Reflow
{
    public class XmlCommentParagraphParser
    {
        private readonly XmlCommentReflowableBlockLexer  _xmlBlockLexer;
        
        public XmlCommentParagraphParser(XmlCommentReflowableBlockLexer xmlBlockLexer)
        {
            _xmlBlockLexer = xmlBlockLexer;
        }

        public IEnumerable<Paragraph> Parse()
        {
            Paragraph paragraph = new Paragraph();
            ParagraphLine previousLine = null;
            foreach (ParagraphLine line in readLines())
            {                              
                
                ParagraphLine trimmedLine = line.Trim();
                ParagraphLine previousTrimmedLine = previousLine == null ? null : previousLine.Trim();

                //empty line is paragraph
                if (trimmedLine.Items.Count == 0)
                {
                    if (paragraph.Lines.Count > 0)
                        yield return paragraph;
                    paragraph = new Paragraph();
                    paragraph.Add(line);
                }
                //xml element on start of line starts new paragraph
                else if (trimmedLine.Items.Count > 0 && (trimmedLine.Items[0].ItemType == ItemType.XmlElement ||
                    trimmedLine.Items[0].ItemType == ItemType.NonReflowableBlock))
                {
                    if (paragraph.Lines.Count > 0)
                        yield return paragraph;
                    paragraph = new Paragraph();
                    paragraph.Add(line); // trimmedLine
                }
                //Anythyng after empty line starts new paragraph.
                else if (previousLine != null && previousTrimmedLine.Items.Count == 0)
                {
                    if (paragraph.Lines.Count > 0)
                        yield return paragraph;
                    paragraph = new Paragraph();
                    paragraph.Add(line);
                }
                //anything after xml element on own line starts new paragraph.
                else if (previousTrimmedLine != null && previousTrimmedLine.Items.Count == 1 && previousTrimmedLine.Items[0].ItemType == ItemType.XmlElement)
                {
                    if (paragraph.Lines.Count > 0)
                        yield return paragraph;
                    paragraph = new Paragraph();
                    paragraph.Add(line);
                }
                else
                {
                    paragraph.Add(line);
                }
                
                previousLine = line;
            }

            if (paragraph.Lines.Count > 0)
                yield return paragraph;
        }

        IEnumerable<ParagraphLine> readLines()
        {
            // We're processing "blocks" out of the lexer. Blocks are either:
            // - xml tags - these will always come in as a single block
            // - slabs of text - these will come one per line (but they may have a tag breaking up two slabs on the one line).
            //
            // We need to break them into what we think the lines should be.

            // Start reading blocks from the lexer
            ParagraphLine paragraphLine = new ParagraphLine();            
            foreach (string block in _xmlBlockLexer)
            {
                // Create a new item to put this block into.
                ParagraphLineItem item = new ParagraphLineItem();

                // If the block is a code or "c" block then we treat the block as a single, non-reflowable chunk.
                if (block.StartsWith("<code") || block.StartsWith("<c") || block.StartsWith("<see") || block.StartsWith("<paramref") || block.StartsWith("<typeparamref"))
                {
                    item.Text = block;
                    item.ItemType = ItemType.NonReflowableBlock;
                    paragraphLine.AddItem(item);                    
                }
                // If the block is some other xml tag then treat it as a line (we may compress these again later).
                else if (block.StartsWith("<"))
                {
                    /*
                    // Yield the previous line
                    yield return paragraphLine;

                    // Create a new line
                    paragraphLine = new ParagraphLine();
                    */
                    // And add the new tag into it
                    item.Text = block;
                    item.ItemType = ItemType.XmlElement;
                    paragraphLine.AddItem(item);

                    /*
                    // Yield that
                    yield return paragraphLine;

                    // Create a new line
                    paragraphLine = new ParagraphLine();
                     */
                }
                // Otherwise it must be a text block so deal with that.
                else
                {
                    // Split the text block into lines
                    string[] lines = block.Replace("\r", "").Split('\n');

                    // Process each line
                    for (int i=0; i<lines.Length; i++)
                    {
                        // Create an item for this line
                        string line = lines[i];
                        item.Text = line;
                        item.ItemType = line.Trim().Length == 0 ? ItemType.XmlSpace : ItemType.Text;
                        paragraphLine.AddItem(item);

                        // If we're not the last line then yield this line (last line may contain other sections).
                        if (i != lines.Length - 1)
                        {
                            yield return paragraphLine;
                            item = new ParagraphLineItem();
                            paragraphLine = new ParagraphLine();
                        }
                    }
                }
            }

            if (paragraphLine.Items.Count > 0)
            {
                yield return paragraphLine;
            }
        }
    }
}
