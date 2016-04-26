//using System;
//using JetBrains.ReSharper.Psi;
//using JetBrains.Text;
//using JetBrains.Util;
//using NUnit.Framework;
//using AgentSmith.Comments.Reflow;

//namespace AgentSmith.Test.Comments.Reflow
//{
//	[TestFixture]
//	public class XmlCommentReflowerTest
//	{
//		[Test]
//		public void TestReflow()
//		{
//			string unreflownBlock = @"///<summary>
//    This block needs to be reflown.
//    Here goes some crap. Here goes some <c> some code</c> that should not
//    be reflown. <code> 
//blocks shall go
//as they are
//</code> Paragraphs start from  new line.
//
//    For example this is new paragraph.
//   <list>
//        <ul>Xml elements on start of
//            line start new paragraph.
//        </ul>
//    </list>
//</summary>";

//			string reflownBlock = @"<summary>
//    This block needs to be reflown. Here goes some
//    crap. Here goes some <c> some code</c> that
//    should not be reflown. <code> 
//blocks shall go
//as they are
//</code> Paragraphs start from  new line.
//
//    For example this is new paragraph.
//   <list>
//        <ul>Xml elements on start of line start
//        new paragraph.
//        </ul>
//    </list>
//</summary>";

//			doTest(unreflownBlock, reflownBlock, 50);
//		}

//		[Test]
//		public void TestSimple()
//		{
//			string unreflownBlock = @"///<summary>
//word
//</summary>";

//			string reflownBlock = @"<summary>
//word
//</summary>";

//			doTest(unreflownBlock, reflownBlock, 50);
//		}

//		[Test]
//		public void TestParagraphs()
//		{
//			string unreflownBlock = @"/// <exception cref=""ApplicationException"">Server returned unknown error.</exception>
// <exception cref=""ArgumentException"">Some text shall go here.</exception>";

//			string reflownBlock = @" <exception cref=""ApplicationException"">
// Server returned unknown error.
// </exception>
// <exception cref=""ArgumentException"">
// Some text shall go here.</exception>";


//			doTest(unreflownBlock, reflownBlock, 40);
//		}

//		[Test]
//		public void TestEmbeddedTags()
//		{
//			string unreflownBlock = @"/// <remarks>calling <see cref=""CreateResource(string)""/> or.</remarks>
// <exception cref=""NotFoundException"">This folder doesn't exist </exception>";

//			string reflownBlock = @" <remarks>calling 
// <see cref=""CreateResource(string)""/>
// or.</remarks>
// <exception cref=""NotFoundException"">
// This folder doesn't exist </exception>";

//			doTest(unreflownBlock, reflownBlock, 40);
//		}

//		[Test]
//		public void TestNoExtraSpace()
//		{
//			string unreflownBlock =@"/// <exception cref=""NotFoundException"">The 
// requested folder doesn't exist on the 
// server.</exception>";
//			string reflownBlock = @" <exception cref=""NotFoundException"">The
// requested folder doesn't exist on the 
// server.</exception>";
            
//			doTest(unreflownBlock, reflownBlock, 40);
//		}
        
//		[Test]
//		public void TestNoExtraLine()
//		{
//			string unreflownBlock = @"/// <returns>Item corresponding to 
//  requested path.</returns>
//  <exception cref=""UnauthorizedException"">";
//			string reflownBlock = @" <returns>Item corresponding to 
// requested path.</returns>
//  <exception cref=""UnauthorizedException"">";

//			doTest(unreflownBlock, reflownBlock, 40);
//		}

//		[Test]
//		public void TestNewLineAfterBr()
//		{
//			string unreflownBlock = @"/// <returns>Item corresponding <br/>to 
//  requested path.</returns>";
  
//			string reflownBlock = @" <returns>Item corresponding <br/>
// to  requested path.</returns>";

//			doTest(unreflownBlock, reflownBlock, 40);
//		}

//		private void doTest(string unreflownBlock, string reflownBlock, int n)
//		{
//			StringBuffer buffer = new StringBuffer(unreflownBlock);            
//			DocCommentNode docCommentNode = new DocCommentNode(new MyNodeType(), buffer, new TreeOffset(0), new TreeOffset(unreflownBlock.Length));
//			DocCommentBlockNode blockNode = new DocCommentBlockNode(docCommentNode);
//			XmlCommentReflower reflower = new XmlCommentReflower();
//			string result = reflower.Reflow(blockNode, n);

//			Assert.AreEqual(reflownBlock, result);
//		}
//	}
//}
