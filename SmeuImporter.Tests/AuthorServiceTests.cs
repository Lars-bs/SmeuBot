using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SmeuImporter.Domain;
using SmeuImporter.Services;
using SmeuImporter.Services.Implementation;

namespace SmeuImporter.Tests
{
    public class AuthorServiceTests
    {
        private const string Author = "Lars";
        private const string Author2 = "Justin";
        private const string Author3 = "Luus";
        private const string Author4 = "Wietse";
        private const ulong Id = 1324424;
        private readonly IFileOperationService fileOperationService = Substitute.For<IFileOperationService>();
        private IAuthorService authorService;
        private readonly IUserInteractionService userInteractionService = Substitute.For<IUserInteractionService>();
        private User user;
        private User user2;
        private List<User> users;
        
        [SetUp]
        public void Setup()
        {
            authorService = new AuthorService(fileOperationService, userInteractionService);
            user = SmeuTestDataFactory.NewUser(3,new List<string>{Author, Author2});
            user2 = SmeuTestDataFactory.NewUser(4,new List<string>{Author3});
            users = new List<User>
            {
                user,
                user2
            };
        }
        
        [Test]
        public void ResolveAuthor_WithMultipleAuthorsKnown_ExpectCorrectIdFound()
        {
            //Arrange
            fileOperationService.GetUserMap().Returns(users);
            var chatEntry = SmeuTestDataFactory.NewChatEntry(Author2);
            
            //Act
            var result = authorService.ResolveAuthorId(chatEntry);

            //Assert
            result.Should().Be(user.Id);
        }

        [Test]
        public void ResolveAuthor_WithoutKnownAuthor_ExpectUserAskedForUsername()
        {
            //Arrange
            fileOperationService.GetUserMap().Returns(users);
            userInteractionService.AskForAuthor(Author4, users).Returns(user.Id);
            var chatEntry = SmeuTestDataFactory.NewChatEntry(Author4);
            
            //Act
            var result = authorService.ResolveAuthorId(chatEntry);

            //Assert
            result.Should().Be(user.Id);
        }
        
        [Test]
        public void ResolveAuthor_WithoutKnownAuthor_ExpectAuthor4AddedToAuthors()
        {
            //Arrange
            fileOperationService.GetUserMap().Returns(users);
            userInteractionService.AskForAuthor(Author4, users).Returns(user.Id);
            var chatEntry = SmeuTestDataFactory.NewChatEntry(Author4);
            
            //Act
            var result = authorService.ResolveAuthorId(chatEntry);

            //Assert
            fileOperationService.Received(1)
                .SetUserMap(Arg.Is<List<User>>(userMap => userMap.SelectMany(row => row.Names).Contains(Author4)));
        }
    }
}