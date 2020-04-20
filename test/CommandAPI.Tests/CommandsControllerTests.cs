using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using CommandAPI.Controllers;
using CommandAPI.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CommandAPI.Tests
{
    public class CommandsControllerTests : IDisposable
    {
        DbContextOptionsBuilder<CommandContext> optionsBuilder;
        CommandContext dbContext;
        CommandsController controller;

        public CommandsControllerTests()
        {
            optionsBuilder = new DbContextOptionsBuilder<CommandContext>();
            optionsBuilder.UseInMemoryDatabase("UnitTestInMemDB");
            dbContext = new CommandContext(optionsBuilder.Options);
            // Controller
            controller = new CommandsController(dbContext);
        }
        public void Dispose()
        {
            optionsBuilder = null;
            foreach (var cmd in dbContext.CommandItems)
            {
                dbContext.CommandItems.Remove(cmd);
            }
            dbContext.SaveChanges();
            dbContext.Dispose();
            controller = null;
        }
        
        [Fact]
        public void GetCommandItems_ReturnsZeroItems_WhenDBIsEmpty()
        {
            //Given
        
            //When
            var result = controller.GetCommandItems();
            //Then
            Assert.Empty(result.Value);
        }
        [Fact]
        public void GetCommandItems_ReturnsOneItem_WhenDBHasOneObject()
        {
            //Given
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Some platform",
                CommandLine = "Some command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();
            //When
            var result = controller.GetCommandItems();
            //Then
            Assert.Single(result.Value);
        }

        [Fact]
        public void GetCommandItems_ReturnNItems_WhenDBHasNObjects()
        {
            //Arrange
            var command1 = new Command
            {
                HowTo = "Do Something",
                Platform = "Some platform",
                CommandLine = "Some command"
            };
            var command2 = new Command
            {
                HowTo = "Do Something 2",
                Platform = "Some platform 2",
                CommandLine = "Some command 2"
            };  
            dbContext.CommandItems.Add(command1);
            dbContext.CommandItems.Add(command2);
            dbContext.SaveChanges();

            //Act
            var result = controller.GetCommandItems();
            //Assert
            Assert.Equal(2, result.Value.Count());          
        }

        [Fact]
        public void GetCommandItems_ReturnsTheCorrectType()
        {
            //Arrange

            //Act
            var result = controller.GetCommandItems();
            //Assert
            Assert.IsType<ActionResult<IEnumerable<Command>>>(result);
        }
        [Fact]
        public void GetCommandItem_ReturnsNullResult_WhenInvalidID()
        {
            //Arrange
            //DB should be empty, any ID will be invalid

            //Act
            var result = controller.GetCommandItem(0);
            //Assert
            Assert.Null(result.Value);
        }
        [Fact]
        public void GetCommandItem_Returns404NotFound_WhenInvalidID()
        {
            //Arrange
            //DB should be empty, any ID will be invalid

            //Act
            var result = controller.GetCommandItem(0);
            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        [Fact]
        public void GetCommandItem_ReturnsTheCorrectType()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something 2",
                Platform = "Some platform 2",
                CommandLine = "Some command 2"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            var cmdId = command.Id;
            //Act
            var result = controller.GetCommandItem(cmdId);
            //Assert
            Assert.IsType<ActionResult<Command>>(result);
        }
        [Fact]
        public void GetCommandItem_ReturnsTheCorrectResource()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something 2",
                Platform = "Some platform 2",
                CommandLine = "Some command 2"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            var cmdId = command.Id;
            //Act
            var result = controller.GetCommandItem(cmdId);
            //Assert
            Assert.Equal(cmdId, result.Value.Id);
        }
        [Fact]
        public void PostCommandItem_ObjectCountIncrement_WhenValidObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something 2",
                Platform = "Some platform 2",
                CommandLine = "Some command 2"
            };
            var oldCount = dbContext.CommandItems.Count();
            //Act
            var result = controller.PostCommandItem(command);
            //Assert
            Assert.Equal(oldCount+1, dbContext.CommandItems.Count());
        }
        [Fact]
        public void PostCommandItem_Returns201Created_WhenValidObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something 2",
                Platform = "Some platform 2",
                CommandLine = "Some command 2"
            };
            //Act
            var result = controller.PostCommandItem(command);
            //Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }
        [Fact]
        public void PutCommandItem_AttributeUpdated_WhenValidObject()
        {   
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something 2",
                Platform = "Some platform 2",
                CommandLine = "Some command 2"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            var cmdId = command.Id;
            command.HowTo = "UPDATED";

            //Act
            controller.PutCommandItem(cmdId, command);
            var result = dbContext.CommandItems.Find(cmdId);
            //Assert
            Assert.Equal(command.HowTo, result.HowTo);
        }
        [Fact]
        public void PutCommandItem_Returns204_WhenValidObject()
        {   
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something 2",
                Platform = "Some platform 2",
                CommandLine = "Some command 2"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            var cmdId = command.Id;
            command.HowTo = "UPDATED";

            //Act
            var result = controller.PutCommandItem(cmdId, command);
            //Assert
            Assert.IsType<NoContentResult>(result);
        }
        [Fact]
        public void PutCommandItem_Returns404_WhenInValidObject()
        {   
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something 2",
                Platform = "Some platform 2",
                CommandLine = "Some command 2"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            var cmdId = command.Id+1;
            command.HowTo = "UPDATED";

            //Act
            var result = controller.PutCommandItem(cmdId, command);
            //Assert
            Assert.IsType<BadRequestResult>(result);
        }
        [Fact]
        public void PutCommandItem_AttributeUnchanged_WhenInvalidObject()
        {   
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something 2",
                Platform = "Some platform 2",
                CommandLine = "Some command 2"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            var command2 = new Command
            {
                Id = command.Id,
                HowTo = "UPDATED",
                Platform = "UPDATED",
                CommandLine = "UPDATED"
            };
            //Act
            controller.PutCommandItem(command.Id + 1, command2);
            var result = dbContext.CommandItems.Find(command.Id);
            //Assert
            Assert.Equal(command.HowTo, result.HowTo);
        }
        [Fact] 
        public void DeleteCommandItem_ObjectsDecrement_WhenValidObjectID() 
        { 
            //Arrange 
            var command = new Command 
                { 
                    HowTo = "Do Somethting", 
                    Platform = "Some Platform", 
                    CommandLine = "Some Command" 
                }; 
            dbContext.CommandItems.Add(command); 
            dbContext.SaveChanges(); 
            var cmdId = command.Id; 
            var objCount = dbContext.CommandItems.Count(); 
            //Act 
            controller.DeleteCommandItem(cmdId); 
            //Assert 
            Assert.Equal(objCount-1, dbContext.CommandItems.Count()); 
        }

        [Fact] 
        public void DeleteCommandItem_Returns200OK_WhenValidObjectID() 
        { 
            //Arrange 
            var command = new Command 
            { 
                HowTo = "Do Somethting", 
                Platform = "Some Platform", 
                CommandLine = "Some Command" 
            }; 
            dbContext.CommandItems.Add(command); 
            dbContext.SaveChanges(); 
            var cmdId = command.Id; 
            //Act 
            var result = controller.DeleteCommandItem(cmdId); 
            //Assert 
            Assert.Null(result.Result); 
        }

        [Fact] 
        public void DeleteCommandItem_Returns404NotFound_WhenValidObjectID() 
        { 
            //Arrange 
            
            //Act 
            var result = controller.DeleteCommandItem(-1); 
            //Assert 
            Assert.IsType<NotFoundResult>(result.Result); 
        }

        [Fact] 
        public void DeleteCommandItem_ObjectCountNotDecremented_WhenInvalidObjectID() 
        { 
            //Arrange 
            var command = new Command 
            { 
                HowTo = "Do Somethting", 
                Platform = "Some Platform", 
                CommandLine = "Some Command" 
            }; 
            dbContext.CommandItems.Add(command); 
            dbContext.SaveChanges(); 
            var cmdId = command.Id; 
            var objCount = dbContext.CommandItems.Count(); 
            //Act 
            var result = controller.DeleteCommandItem(cmdId+1); 
            //Assert 
            Assert.Equal(objCount, dbContext.CommandItems.Count()); 
        }  
              

    }
}