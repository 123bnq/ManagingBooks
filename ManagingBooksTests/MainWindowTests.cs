using Microsoft.VisualStudio.TestTools.UnitTesting;
using ManagingBooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagingBooks.Tests
{
    [TestClass()]
    public class MainWindowTests
    {
        [TestMethod()]
        public void SearchBookTest()
        {
            MainWindow mainWindow = new MainWindow(9);
            mainWindow.SearchBook();
            Assert.Fail();
        }
    }
}