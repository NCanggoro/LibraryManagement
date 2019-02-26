using FluentAssertions;
using LibraryData;
using LibraryData.Models;
using LibraryServices;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    [TestFixture]
    public class CheckoutServiceShould
    {
       private static IEnumerable<Checkout> GetCheckouts()
        {
            return new List<Checkout>
            {
                new Checkout
                {
                    Id = 1111,
                    Since = new DateTime(2011, 12, 23),
                    Until = new DateTime(2012, 02, 22),
                    LibraryCard = new LibraryCard
                    {
                        Id = -1,
                        Created = new DateTime(2001, 02, 02),
                        Fees = 111
                    }
                },
                new Checkout
                {
                    Id = 1234,
                    Since = new DateTime(2011, 02, 23),
                    Until = new DateTime(2012, 02, 22),
                    LibraryCard = new LibraryCard
                    {
                        Id = 2,
                        Created = new DateTime(2004, 02, 02),
                        Fees = 51
                    }
                },
            };
        }
       
      [Test]
      public void Add_New_Checkout()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase("Add_writes_to_database")
                .Options;

            using(var context = new LibraryContext(options))
            {
                var service = new CheckoutService(context);

                service.Add(new Checkout
                {
                    Id = -13
                });

                Assert.AreEqual(-13, context.Checkouts.Single().Id);
            }
        }

        [Test]

        public void Get_All_Checkouts()
        {
            var temp = GetCheckouts().AsQueryable();

            var mockSet = new Mock<DbSet<Checkout>>();
            mockSet.As<IQueryable<Checkout>>().Setup(b => b.Provider).Returns(temp.Provider);
            mockSet.As<IQueryable<Checkout>>().Setup(b => b.Expression).Returns(temp.Expression);
            mockSet.As<IQueryable<Checkout>>().Setup(b => b.ElementType).Returns(temp.ElementType);
            mockSet.As<IQueryable<Checkout>>().Setup(b => b.GetEnumerator()).Returns(temp.GetEnumerator());

            var mockContext = new Mock<LibraryContext>();
            mockContext.Setup(c => c.Checkouts).Returns(mockSet.Object);

            var sut = new CheckoutService(mockContext.Object);
            var queryResult = sut.GetAll().ToList();

            queryResult.Should().AllBeOfType(typeof(Checkout));
            queryResult.Should().HaveCount(2);
            //queryResult.Should().Contain(a => a.LibraryCard.Id == 2);
        }

        [Test]
        public void Add_New_Checkout_Calls_Context_Save()
        {
            var mockSet = new Mock<DbSet<Checkout>>();
            var mockContext = new Mock<LibraryContext>();

            mockContext.Setup(c => c.Checkouts).Returns(mockSet.Object);
            var service = new CheckoutService(mockContext.Object);

            service.Add(new Checkout());

            mockContext.Verify(s => s.Add(It.IsAny<Checkout>()), Times.Once());
            mockContext.Verify(c => c.SaveChanges(), Times.Once());
        }

        
    }
}