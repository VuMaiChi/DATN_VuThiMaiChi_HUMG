using ClosedXML.Excel;
using NUnit.Framework;
using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using TestEShopSolution.Functions;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;

namespace WebUITests
{

    [TestFixture] // Đánh dấu class là một bộ test
    public class Tests
    {
        private Auth auth;

        [SetUp]
        public void Setup()
        {
            auth = new Auth();
        }

        [Test]
        public void TestLogin()
        {
            // Đường dẫn đến file Excel
            string filePath = @"C:\Users\Admin\Desktop\DATN\TestEShopSolution\data_test_login.xlsx";

            // Kiểm tra file có tồn tại không
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File Excel không tồn tại.");
                return;
            }

            // Mở file Excel
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1); // Chọn worksheet đầu tiên
                var rows = worksheet.RowsUsed(); // Lấy các dòng có dữ liệu

                foreach (var row in rows.Skip(1)) // Bỏ qua dòng tiêu đề
                {
                    // Đọc giá trị từ cột Username và Password
                    string username = row.Cell(1).GetValue<string>();
                    string password = row.Cell(2).GetValue<string>();

                    // Gọi hàm PerformLogin và lấy kết quả
                    string result = auth.PerformLogin(username, password);

                    // Ghi kết quả vào cột "result"
                    row.Cell(3).Value = result;
                }

                // Lưu lại file Excel với kết quả
                workbook.SaveAs(filePath);
            }
        }

        [Test]
        public void TestRegister()
        {
            // Đường dẫn đến file Excel
            string filePath = @"C:\Users\Admin\Desktop\DATN\TestEShopSolution\data_test_register.xlsx";

            // Kiểm tra file có tồn tại không
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File Excel không tồn tại.");
                return;
            }

            // Mở file Excel
            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1); // Chọn worksheet đầu tiên
                var rows = worksheet.RowsUsed(); // Lấy các dòng có dữ liệu

                foreach (var row in rows.Skip(1)) // Bỏ qua dòng tiêu đề
                {
                    // Đọc giá trị từ cột
                    string firstname = row.Cell(1).GetValue<string>();
                    string lastname = row.Cell(2).GetValue<string>();
                    string email = row.Cell(3).GetValue<string>();
                    string phonenumber = row.Cell(4).GetValue<string>();
                    string birthday = row.Cell(5).GetValue<string>();
                    string username = row.Cell(6).GetValue<string>();
                    string password = row.Cell(7).GetValue<string>();
                    string passwordconfirm = row.Cell(8).GetValue<string>();

                    // Gọi hàm PerformRegister và lấy danh sách kết quả
                    List<string> results = auth.PerformRegister(firstname, lastname, email, phonenumber, birthday, username, password, passwordconfirm);

                    // Nối tất cả thông báo thành một chuỗi
                    string resultString = string.Join("; ", results);

                    // Ghi kết quả vào cột "result"
                    row.Cell(9).Value = resultString;
                }

                // Lưu lại file Excel với kết quả
                workbook.SaveAs(filePath);
            }
        }

        
        [Test]
        public void TestAddProduct()
        {
            // Đường dẫn đến file Excel
            string filePath = @"C:\Users\Admin\Desktop\DATN\TestEShopSolution\data_test_create_product.xlsx";

            // Kiểm tra file có tồn tại không
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File Excel không tồn tại.");
                return;
            }

            try
            {
                // Mở file Excel
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1); // Chọn worksheet đầu tiên
                    var rows = worksheet.RowsUsed(); // Lấy các dòng có dữ liệu

                    foreach (var row in rows.Skip(1)) // Bỏ qua dòng tiêu đề
                    {
                        // Đọc giá trị từ các cột
                        string name = row.Cell(1).GetValue<string>();
                        string price = row.Cell(2).GetValue<string>();
                        string originalPrice = row.Cell(3).GetValue<string>();
                        string stock = row.Cell(4).GetValue<string>();
                        string brandId = row.Cell(5).GetValue<string>();
                        string origin = row.Cell(6).GetValue<string>();
                        string warranty = row.Cell(7).GetValue<string>();
                        string description = row.Cell(8).GetValue<string>();
                        string details = row.Cell(9).GetValue<string>();
                        string seoDescription = row.Cell(10).GetValue<string>();
                        string seoTitle = row.Cell(11).GetValue<string>();
                        string seoAlias = row.Cell(12).GetValue<string>();
                        string categoryId = row.Cell(13).GetValue<string>();
                        string thumbnailImagePath = row.Cell(14).GetValue<string>(); // Đường dẫn đến hình ảnh

                        // Gọi hàm AddProductFlow và lấy danh sách kết quả
                        List<string> results = auth.AddProductFlow("admin", "Admin123@", name, price, originalPrice, stock, brandId, origin,
                            warranty, description, details, seoDescription, seoTitle, seoAlias, categoryId, thumbnailImagePath);

                        // Nối tất cả thông báo thành một chuỗi
                        string resultString = string.Join("; ", results);

                        // Ghi kết quả vào cột "result"
                        row.Cell(15).Value = resultString; // Giả sử cột 15 là cột kết quả
                    }

                    // Lưu lại file Excel với kết quả
                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Đã xảy ra lỗi khi xử lý file Excel: {ex.Message}");
            }
        }

        [Test]
        public void TestEvaluateProduct()
        {
            string filePath = @"C:\Users\Admin\Desktop\DATN\TestEShopSolution\data_test_feedback.xlsx";

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File Excel không tồn tại.");
                return;
            }

            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RowsUsed().Skip(1);

                    foreach (var row in rows)
                    {
                        string feedback = row.Cell(1).GetValue<string>();
                        string author = row.Cell(2).GetValue<string>();
                        string phone = row.Cell(3).GetValue<string>();
                        string email = row.Cell(4).GetValue<string>();
                        string productId = "3228";

                        // Điều hướng đến sản phẩm và điền form đánh giá
                        auth.NavigateToProductReview(productId);
                        string result = auth.FillReviewForm(feedback, author, phone, email);

                        // Ghi kết quả vào cột thứ 5
                        row.Cell(5).Value = result;
                    }

                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Đã xảy ra lỗi khi xử lý file Excel: {ex.Message}");
            }
        }

        [Test]
        public void TestOrderFromExcel()
        {
            // Đường dẫn đến file Excel
            string filePath = @"C:\Users\Admin\Desktop\DATN\TestEShopSolution\data_test_order.xlsx";

            // Kiểm tra file có tồn tại không
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File Excel không tồn tại.");
                return;
            }

            try
            {
                // Mở file Excel
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1); // Chọn worksheet đầu tiên
                    var rows = worksheet.RowsUsed().Skip(1); // Lấy các dòng có dữ liệu, bỏ qua dòng tiêu đề

                    foreach (var row in rows)
                    {
                        // Đọc dữ liệu từ các cột trong file Excel
                        string shipName = row.Cell(1).GetValue<string>();
                        string shipProvince = row.Cell(2).GetValue<string>();
                        string shipDistrict = row.Cell(3).GetValue<string>();
                        string shipCommune = row.Cell(4).GetValue<string>();
                        string shipAddress = row.Cell(5).GetValue<string>();
                        string shipEmail = row.Cell(6).GetValue<string>();
                        string shipPhoneNumber = row.Cell(7).GetValue<string>();

                        // Thực hiện đặt hàng với các thông tin từ Excel và lấy thông báo lỗi
                        string result = auth.TestOrder(shipName, shipProvince, shipDistrict, shipCommune, shipAddress, shipEmail, shipPhoneNumber);

                        // Ghi kết quả vào cột thứ 8
                        row.Cell(8).Value = result;

                        // Lưu kết quả sau mỗi dòng để đảm bảo dữ liệu không bị mất nếu có lỗi
                        workbook.SaveAs(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Đã xảy ra lỗi khi xử lý file Excel: {ex.Message}");
            }
        }


        [Test]
        public void SearchByCategory()
        {
            // Danh sách các danh mục (data-category)
            var categories = new List<string> { "1", "2", "1001", "1002" };

            foreach (var category in categories)
            {
                auth.CheckCategory(category);
            }
        }

        [Test]
        public void TestUpdateProduct()
        {
            // Đường dẫn đến file Excel
            string filePath = @"C:\Users\Admin\Desktop\DATN\TestEShopSolution\data_test_update_product.xlsx";

            // Kiểm tra file có tồn tại không
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File Excel không tồn tại.");
                return;
            }

            try
            {
                // Mở file Excel
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1); // Chọn worksheet đầu tiên
                    var rows = worksheet.RowsUsed(); // Lấy các dòng có dữ liệu

                    foreach (var row in rows.Skip(1)) // Bỏ qua dòng tiêu đề
                    {
                        // Đọc giá trị từ các cột
                        string name = row.Cell(1).GetValue<string>();
                        string price = row.Cell(2).GetValue<string>();
                        string originalPrice = row.Cell(3).GetValue<string>();
                        string brandId = row.Cell(4).GetValue<string>();
                        string origin = row.Cell(5).GetValue<string>();
                        string warranty = row.Cell(6).GetValue<string>();
                        string description = row.Cell(7).GetValue<string>();
                        string details = row.Cell(8).GetValue<string>();
                        string seoDescription = row.Cell(9).GetValue<string>();
                        string seoTitle = row.Cell(10).GetValue<string>();
                        string seoAlias = row.Cell(11).GetValue<string>();
                        string thumbnailImagePath = row.Cell(12).GetValue<string>(); // Đường dẫn đến hình ảnh

                        // Gọi hàm AddProductFlow và lấy danh sách kết quả
                        List<string> results = auth.UpdateProductFlow("admin", "Admin123@", name, price, originalPrice, brandId, origin,
                            warranty, description, details, seoDescription, seoTitle, seoAlias, thumbnailImagePath);

                        // Nối tất cả thông báo thành một chuỗi
                        string resultString = string.Join("; ", results);

                        // Ghi kết quả vào cột "result"
                        row.Cell(13).Value = resultString; // Giả sử cột 13 là cột kết quả
                    }

                    // Lưu lại file Excel với kết quả
                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Đã xảy ra lỗi khi xử lý file Excel: {ex.Message}");
            }
        }


        [Test]
        public void TestAddUser()
        {
            // Đường dẫn đến file Excel
            string filePath = @"C:\Users\Admin\Desktop\DATN\TestEShopSolution\data_test_create_user.xlsx";

            // Kiểm tra file có tồn tại không
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File Excel không tồn tại.");
                return;
            }

            try
            {
                // Mở file Excel
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1); // Chọn worksheet đầu tiên
                    var rows = worksheet.RowsUsed(); // Lấy các dòng có dữ liệu

                    foreach (var row in rows.Skip(1)) // Bỏ qua dòng tiêu đề
                    {
                        // Đọc giá trị từ các cột
                        string firstname = row.Cell(1).GetValue<string>();
                        string lastname = row.Cell(2).GetValue<string>();
                        string dob = row.Cell(3).GetValue<string>();
                        string email = row.Cell(4).GetValue<string>();
                        string phonenumber = row.Cell(5).GetValue<string>();
                        string username = row.Cell(6).GetValue<string>();
                        string password = row.Cell(7).GetValue<string>();
                        string confirmpassword = row.Cell(8).GetValue<string>();

                        // Gọi hàm AddUserFlow và lấy danh sách kết quả
                        List<string> results = auth.AddUserFlow("admin", "Admin123@", firstname, lastname, dob, email, phonenumber, username,
                            password, confirmpassword);

                        // Nối tất cả thông báo thành một chuỗi
                        string resultString = string.Join("; ", results);

                        // Ghi kết quả vào cột "result"
                        row.Cell(9).Value = resultString; // Giả sử cột 15 là cột kết quả
                    }

                    // Lưu lại file Excel với kết quả
                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Đã xảy ra lỗi khi xử lý file Excel: {ex.Message}");
            }
        }

        [Test]
        public void TestUpdateUser()
        {
            // Đường dẫn đến file Excel
            string filePath = @"C:\Users\Admin\Desktop\DATN\TestEShopSolution\data_test_update_user.xlsx";

            // Kiểm tra file có tồn tại không
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File Excel không tồn tại.");
                return;
            }

            try
            {
                // Mở file Excel
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1); // Chọn worksheet đầu tiên
                    var rows = worksheet.RowsUsed(); // Lấy các dòng có dữ liệu

                    foreach (var row in rows.Skip(1)) // Bỏ qua dòng tiêu đề
                    {
                        // Đọc giá trị từ các cột
                        string firstname = row.Cell(1).GetValue<string>();
                        string lastname = row.Cell(2).GetValue<string>();
                        string dob = row.Cell(3).GetValue<string>();
                        string email = row.Cell(4).GetValue<string>();
                        string phonenumber = row.Cell(5).GetValue<string>();

                        // Gọi hàm AddUserFlow và lấy danh sách kết quả
                        List<string> results = auth.UpdateUserFlow("admin", "Admin123@", firstname, lastname, dob, email, phonenumber);

                        // Nối tất cả thông báo thành một chuỗi
                        string resultString = string.Join("; ", results);

                        // Ghi kết quả vào cột "result"
                        row.Cell(6).Value = resultString; // Giả sử cột 15 là cột kết quả
                    }

                    // Lưu lại file Excel với kết quả
                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Đã xảy ra lỗi khi xử lý file Excel: {ex.Message}");
            }
        }

        [Test]
        public void TestLoginAdmin()
        {
            // Đường dẫn đến file Excel
            string filePath = @"C:\Users\Admin\Desktop\DATN\TestEShopSolution\data_test_login_admin.xlsx";

            // Kiểm tra xem file Excel có tồn tại hay không
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File Excel không tồn tại.");
                return;
            }

            try
            {
                // Mở file Excel
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1); // Chọn worksheet đầu tiên
                    var rows = worksheet.RowsUsed(); // Lấy tất cả các dòng có dữ liệu

                    foreach (var row in rows.Skip(1)) // Bỏ qua dòng tiêu đề
                    {
                        // Đọc dữ liệu từ các cột
                        string username = row.Cell(1).GetValue<string>();
                        string password = row.Cell(2).GetValue<string>();

                        // Gọi hàm PerformLoginAdmin và nhận kết quả
                        string result = auth.PerformLoginAdmin(username, password);

                        // Ghi kết quả vào cột thứ 3 (cột kết quả)
                        row.Cell(3).Value = result;
                    }

                    // Lưu lại file Excel sau khi ghi kết quả
                    workbook.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Đã xảy ra lỗi khi xử lý file Excel: {ex.Message}");
            }
        }

        [Test]
        public void TestDeleteProduct()
        {
            // Gọi hàm DeleteProductFlow để kiểm tra việc xóa sản phẩm
            List<string> result = auth.DeleteProductFlow("admin", "Admin123@", "3232");

            // Duyệt qua các message trả về và in ra kết quả
            foreach (var message in result)
            {
                Console.WriteLine(message);
            }
        }

        [Test]
        public void TestDeleteUser()
        {
            // Gọi hàm DeleteUserFlow để kiểm tra việc xóa sản phẩm
            List<string> result = auth.DeleteUserFlow("admin", "Admin123@", "0bc44612-94e3-4f4f-05f7-08dce093afa6");

            // Duyệt qua các message trả về và in ra kết quả
            foreach (var message in result)
            {
                Console.WriteLine(message);
            }
        }

        [Test]
        public void TestEvaluate()
        {
            // Gọi hàm EvaluateFlow để duyệt đánh giá sản phẩm
            List<string> result = auth.EvaluateFlow("admin", "Admin123@", "36");

            // Duyệt qua các message trả về và in ra kết quả
            foreach (var message in result)
            {
                Console.WriteLine(message);
            }
        }


        [TearDown]
        public void TearDown()
        {
            auth.Clean();
        }
    }
}
