using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Library.Models.DTO;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace XUnitTestProject1.Controllers
{
    public class UsersInitTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;


        public UsersInitTests()
        {
            _server = ServerFactory.GetServerInstance();
            _client = _server.CreateClient();


            using (var scope = _server.Host.Services.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();

                _db.User.Add(new User
                {
                    IdUser = 1,
                    Email = "jd@pja.edu.pl",
                    Name = "Daniel",
                    Surname = "Jabłoński",
                    Login = "jd",
                    Password = "ASNDKWQOJRJOP!JO@JOP"
                });

                _db.User.Add(new User
                {
                    IdUser = 2,
                    Email = "jd2@pja.edu.pl",
                    Name = "Daniel2",
                    Surname = "Jabłoński2",
                    Login = "jd2",
                    Password = "ASNDKWQOJRJOP!JO@JOP2"
                });

                _db.Book.Add(new Book
                {
                    IdBook = 1,
                    Title = "HelloWorld",
                    PublishYear = "2020"
                });

                _db.BookBorrow.Add(new BookBorrow
                {
                    IdBookBorrow = 1,
                    IdUser = 1,
                    IdBook = 1,
                    BorrowDate = DateTime.Now,
                    ReturnDate = DateTime.Now.AddDays(1),
                    Comments = "comment here"

                });

                _db.SaveChanges();

            }
        }


        [Fact]
        public async Task GetUsersById_200Ok()
        {
            var httpResponse = await _client.GetAsync($"{_client.BaseAddress.AbsoluteUri}api/users/1");

            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(content);
            Assert.True(user.IdUser == 1);
        }

        [Fact]
        public async Task GetUsers_200Ok()
        {
            var httpResponse = await _client.GetAsync($"{_client.BaseAddress.AbsoluteUri}api/users");

            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<IEnumerable<User>>(content);
            Assert.True(users.Count() == 2);
            Assert.True(users.ElementAt(1).Login == "jd2");  //httpResponse.StatusCode==HttpStatusCode.NotFound

        }

        [Fact]
        public async Task PutBookBorrows_200Ok()
        {
            var bookBorrow = new UpdateBookBorrowDto
            {
                IdBookBorrow = 1,
                IdBook = 1,
                IdUser = 1,
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now.AddDays(2),
                Comments = "comment here"
            };
            string json = JsonConvert.SerializeObject(bookBorrow);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var httpResponse = await _client.PutAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows/1", byteContent);
            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            var bookBorrowl = JsonConvert.DeserializeObject<IEnumerable<BookBorrow>>(content);
            Assert.True(bookBorrowl.ElementAt(0).IdBookBorrow == 1);

        }

        [Fact]
        public async Task PostBookBorrows_200Ok()
        {
            var bookBorrow = new BookBorrowDto
            {
                IdBook = 1,
                IdUser = 1,
                Comment = "comment here"
            };
            string json = JsonConvert.SerializeObject(bookBorrow);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var httpResponse = await _client.PostAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows/", byteContent);
            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            var bookBorrowl = JsonConvert.DeserializeObject<IEnumerable<BookBorrow>>(content);
            Assert.True(bookBorrowl.ElementAt(0).IdBookBorrow == 1);
        }

        //[Fact]
        //public async Task GetBookBorrows_200Ok()
        //{
        //    ///AAA
        //    ///Arrange-Act-Assert
        //    ///
        //    ///Arrange-Act
        //    var httpResponse = await _client.GetAsync($"{_client.BaseAddress.AbsoluteUri}api/users");

        //    httpResponse.EnsureSuccessStatusCode();
        //    var content = await httpResponse.Content.ReadAsStringAsync();
        //    var users = JsonConvert.DeserializeObject<IEnumerable<User>>(content);
        //    // using (var scope = _server.Host.Services.CreateScope())
        //    // {
        //    //     var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
        //    //     Assert.True(_db.User.Any());
        //    // }

        //    Assert.True(users.Count() == 1);
        //    Assert.True(users.ElementAt(0).Login == "jd");  //httpResponse.StatusCode==HttpStatusCode.NotFound

        //}

        ///!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!PRZETESTOWAC BookRorrows(2 testy), Users(2 testy)
        ///GIT(link) albo Projekt, do konca poniedzialku, testy 200Ok i 404 lub 404

    }
}
