using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SportsStore.WebUI.HtmlHelpers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class ProductTests
    {
        [TestMethod]
        public void Can_Paginate()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "p2"},
                new Product {ProductID = 3, Name = "p3"},
                new Product {ProductID = 4, Name = "p4"},
                new Product {ProductID = 5, Name = "p5"}
            }.AsQueryable());

            ProductController controller = new ProductController(mock.Object);

            controller.PageSize = 3;

            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "p4");
            Assert.AreEqual(prodArray[1].Name, "p5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            HtmlHelper myHelper = null;

            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            Func<int, string> pageUrlDelegate = i => "Page" + i;

            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            Assert.AreEqual(result.ToString(), @"<a href=""Page1"">1</a>"
                + @"<a class=""selected"" href=""Page2"">2</a>"
                + @"<a href=""Page3"">3</a>");
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductID = 1, Name = "p1" },
                new Product { ProductID = 2, Name = "p2" },
                new Product { ProductID = 3, Name = "p3" },
                new Product { ProductID = 4, Name = "p4" },
                new Product { ProductID = 5, Name = "p5" }
            }.AsQueryable());

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductID = 1, Name = "p1", Category = "Cat1" },
                new Product { ProductID = 2, Name = "p2", Category = "Cat2" },
                new Product { ProductID = 3, Name = "p3", Category = "Cat1" },
                new Product { ProductID = 4, Name = "p4", Category = "Cat2" },
                new Product { ProductID = 5, Name = "p5", Category = "Cat3" }
            }.AsQueryable());

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            Product[] result = ((ProductsListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();

            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "p2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "p4" && result[1].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductID = 1, Name = "p1", Category = "Apples" },
                new Product { ProductID = 2, Name = "p2", Category = "Apples" },
                new Product { ProductID = 3, Name = "p3", Category = "Plums" },
                new Product { ProductID = 4, Name = "p4", Category = "Oranges" }
            }.AsQueryable());

            NavController target = new NavController(mock.Object);

            String[] results = ((IEnumerable<String>)target.Menu().Model).ToArray();

            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product { ProductID = 1, Name = "p1", Category = "Cat1" },
                new Product { ProductID = 2, Name = "p2", Category = "Cat2" },
                new Product { ProductID = 3, Name = "p3", Category = "Cat1" },
                new Product { ProductID = 4, Name = "p4", Category = "Cat2" },
                new Product { ProductID = 5, Name = "p5", Category = "Cat3" }
            }.AsQueryable());

            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;

            int res1 = ((ProductsListViewModel)target.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductsListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductsListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;
            int res4 = ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;

            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(res4, 5);
        }
    }
}
