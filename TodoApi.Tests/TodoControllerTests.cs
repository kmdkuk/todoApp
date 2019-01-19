using System.Threading.Tasks;
using System.Linq;
using todoApp.Controllers;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TodoApi.Models;
using System;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace todoApi.Tests
{
    public class TodoControllerTests
    {
        private readonly ITestOutputHelper _output;
        private List<TodoItem> itemlist;
        private TodoController controller;
        private TodoContext context;
        public TodoControllerTests(ITestOutputHelper output)
        {
            _output = output;

            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: "Tests_db")
                .Options;

            itemlist = new List<TodoItem>()
            {
                new TodoItem{ Name = "Test1", IsComplete = false},
                new TodoItem{ Name = "Test2", IsComplete = false},
                new TodoItem{ Name = "Test3", IsComplete = false}
            };

            context = new TodoContext(options);
            context.Database.EnsureDeleted();
            itemlist.ForEach(item => 
            {
                context.TodoItems.Add(item);
                context.SaveChanges();
            });

            itemlist = context.TodoItems.ToList();
            controller = new TodoController(new TodoContext(options));

        }

        [Fact(DisplayName = "GET api/todo/{id} 正常系")]
        public async Task OkGetTodoItemTest()
        {
            var targetId = itemlist[0].Id;
            var result = await controller.GetTodoItem(targetId);

            Assert.IsType<ActionResult<TodoItem>>(result);
            Assert.Equal(targetId, result.Value.Id);
        }

        [Fact(DisplayName = "GET api/todo 正常系")]
        public async Task OkGetTodoItemsTest()
        {
            var result = await controller.GetTodoItems();

            Assert.IsType<ActionResult<IEnumerable<TodoItem>>>(result);
        }

        [Fact(DisplayName = "POST api/todo 正常系")]
        public async Task OkPostTodoItemTest()
        {
            var item = new TodoItem { Name = "new item", IsComplete = false };
            var result = await controller.PostTodoItem(item);

            await PrintData();

            Assert.IsType<ActionResult<TodoItem>>(result);
            Assert.Equal(4, context.TodoItems.Count());
        }

        [Fact(DisplayName = "PUT api/todo/{id} 正常系")]
        public async Task OkPutTodoItemTest()
        {
            var targetId = context.TodoItems.ToList()[0].Id;
            var item = new TodoItem { Id = targetId, Name = "updateditem", IsComplete = false };
            _output.WriteLine("target: {0}, update item:{1} {2} {3}", targetId, item.Id, item.Name, item.IsComplete);
            var result = await controller.PutTodoItem(targetId, item);

            await PrintData();

            Assert.IsType<NoContentResult>(result);
            Assert.Equal(item.Name, context.TodoItems.Where(x => x.Id == targetId).FirstOrDefault().Name);
            Assert.Equal(item.IsComplete, context.TodoItems.Where(x => x.Id == targetId).FirstOrDefault().IsComplete);
        }

        [Fact(DisplayName = "DELETE api/todo/{id} 正常系")]
        public async Task OkDeleteTodoItemTest()
        {
            var targetId = itemlist[0].Id;
            var item = itemlist[0];

            var result = await controller.DeleteTodoItem(targetId);

            await PrintData();

            Assert.Equal(2, context.TodoItems.Count());
            Assert.DoesNotContain<TodoItem>(item, await context.TodoItems.ToListAsync());
        }

        private async Task PrintData()
        {
            (await context.TodoItems.ToListAsync()).ForEach(i =>
            {
                _output.WriteLine("{0} {1} {2}", i.Id, i.Name, i.IsComplete);
            });
        }
    }
}
