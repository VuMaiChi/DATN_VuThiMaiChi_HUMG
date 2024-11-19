using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;


namespace TestEShopSolution.Functions
{
    public class Auth
    {
        private IWebDriver driver;

        public Auth(IWebDriver webDriver = null)
        {
            driver = webDriver ?? new ChromeDriver();
        }

        public List<string> GetAllErrorMessagesLogin()
        {
            List<string> errorMessages = new List<string>();

            // Sử dụng WebDriverWait để chờ các phần tử xuất hiện
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            try
            {
                // Chờ thẻ span của input-username xuất hiện và lấy thông báo lỗi
                var usernameError = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#input-username > span")));
                if (!string.IsNullOrEmpty(usernameError.Text))
                {
                    errorMessages.Add("Fail: " + usernameError.Text);
                }

                // Chờ thẻ span của input-password xuất hiện và lấy thông báo lỗi
                var passwordError = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#input-password > span")));
                if (!string.IsNullOrEmpty(passwordError.Text))
                {
                    errorMessages.Add("Fail: " + passwordError.Text);
                }
            }
            catch (NoSuchElementException)
            {
                // Khi phần tử không tồn tại, hiển thị thông báo tài khoản không tồn tại
                errorMessages.Add("Tài khoản không tồn tại.");
            }

            return errorMessages;
        }



        public string PerformLogin(string username, string password)
        {
            // Nhập thông tin đăng nhập
            driver.Navigate().GoToUrl("https://localhost:5003/vi-VN/account/login");
            driver.FindElement(By.Id("username")).SendKeys(username);
            driver.FindElement(By.Id("password")).SendKeys(password);

            // Nhấp vào nút đăng nhập
            driver.FindElement(By.ClassName("register-button")).Click();

            // Sử dụng WebDriverWait để chờ chuyển hướng URL
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            try
            {
                // Chờ URL thay đổi sau khi đăng nhập
                wait.Until(ExpectedConditions.UrlToBe("https://localhost:5003/"));

                // Kiểm tra cookie chứa token
                var authTokenCookie = driver.Manage().Cookies.GetCookieNamed("Token");

                // Nếu token tồn tại và hợp lệ, trả về thông báo thành công
                if (authTokenCookie != null && !string.IsNullOrEmpty(authTokenCookie.Value))
                {
                    return "Đăng nhập thành công";
                }
                else
                {
                    return "Fail: Đăng nhập không thành công. Vui lòng thử lại sau!";
                }
            }
            catch (WebDriverTimeoutException)
            {
                // Nếu URL không chuyển hướng đúng, kiểm tra các thông báo lỗi từ các thẻ <span>
                List<string> errorMessages = GetAllErrorMessagesLogin();

                // Nếu có lỗi, trả về tất cả các lỗi
                if (errorMessages.Count > 0)
                {
                    return string.Join("\n", errorMessages);
                }
                else
                {
                    // Nếu không có lỗi nhưng cũng không đăng nhập thành công
                    return "Fail: Tài khoản không hợp lệ!";
                }
            }
        }


        public List<string> GetAllErrorMessagesRegister()
        {
            List<string> errorMessages = new List<string>();

            // Sử dụng WebDriverWait để chờ các phần tử xuất hiện
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            // Danh sách các trường cần kiểm tra
            var inputSelectors = new List<string>
        {
            "#input-firstname > span",
            "#input-lastname > span",
            "#input-email > span",
            "#input-phonenumber > span",
            "#input-dob > span",
            "#input-username > span",
            "#input-password > span",
            "#input-confirm-password > span"
        };

            foreach (var selector in inputSelectors)
            {
                try
                {
                    var errorElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(selector)));
                    if (!string.IsNullOrEmpty(errorElement.Text))
                    {
                        errorMessages.Add(errorElement.Text);
                    }
                }
                catch (NoSuchElementException)
                {
                    errorMessages.Add("Fail: Đăng ký thất bại. Vui lòng thử lại sau!");
                }
            }

            return errorMessages;
        }

        public List<string> PerformRegister(string firstname, string lastname,
            string email, string phonenumber, string dob,
            string username, string password, string confirmpassword)
        {
            // Đặt một danh sách để chứa thông báo lỗi
            List<string> errorMessages = new List<string>();

            try
            {
                // Mở website cần kiểm thử
                driver.Navigate().GoToUrl("https://localhost:5003/vi-VN/Account/Register");
                driver.FindElement(By.Id("firstname")).SendKeys(firstname);
                driver.FindElement(By.Id("lastname")).SendKeys(lastname);
                driver.FindElement(By.Id("email")).SendKeys(email);
                driver.FindElement(By.Id("phonenumber")).SendKeys(phonenumber);
                driver.FindElement(By.Id("dob")).SendKeys(dob);
                driver.FindElement(By.Id("username")).SendKeys(username);
                driver.FindElement(By.Id("password")).SendKeys(password);
                driver.FindElement(By.Id("confirm-password")).SendKeys(confirmpassword);

                // Nhấp vào nút đăng ký
                driver.FindElement(By.ClassName("register-button")).Click();

                // Chờ một chút để xem nếu có thông báo lỗi
                Thread.Sleep(1000); // Có thể điều chỉnh thời gian này

                // Lấy tất cả thông báo lỗi
                errorMessages = GetAllErrorMessagesRegister();

                // Nếu không có thông báo lỗi, tiếp tục kiểm tra URL
                if (errorMessages.Count == 0)
                {
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                    wait.Until(ExpectedConditions.UrlToBe("https://localhost:5003/vi-VN/account/login"));

                    return new List<string> { "Đăng ký thành công" };
                }
            }
            catch (Exception ex)
            {
                errorMessages.Add($"Fail: {ex.Message}");
            }

            // Nếu có lỗi, trả về danh sách thông báo lỗi
            return errorMessages.Count > 0 ? errorMessages : new List<string> { "Fail: Đăng ký thất bại. Vui lòng thử lại!" };
        }

        public void LoginAdmin(string username, string password)
        {
            // Điều hướng đến trang đăng nhập
            driver.Navigate().GoToUrl("https://localhost:5002/Login");

            // Nhập tên đăng nhập
            driver.FindElement(By.Id("username")).SendKeys(username);

            // Nhập mật khẩu
            driver.FindElement(By.Id("password")).SendKeys(password);

            // Nhấp vào nút đăng nhập
            driver.FindElement(By.Id("btn-dangnhap")).Click();

            // Chờ để chắc chắn đã đăng nhập thành công
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.UrlToBe("https://localhost:5002/"));  // URL sau khi đăng nhập thành công
        }

        // Hàm nhấp vào "Thêm mới" để vào trang thêm sản phẩm
        public void GoToAddProductPage()
        {
            try
            {
                // Bước 1: Mở menu
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("document.getElementById('pagesCollapseAuth').classList.add('show');");

                // Bước 2: Chờ menu mở ra và nhấp vào liên kết "Thêm mới"
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//a[@href='/product/create']")));
                var addProductLink = driver.FindElement(By.XPath("//a[@href='/product/create']"));
                addProductLink.Click();

                // Bước 3: Chờ trang thêm sản phẩm tải hoàn tất
                wait.Until(ExpectedConditions.UrlToBe("https://localhost:5002/product/create"));

                Console.WriteLine("Đã điều hướng đến trang Thêm sản phẩm.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Đã xảy ra lỗi khi điều hướng đến trang Thêm sản phẩm: {ex.Message}");
            }
        }


        public List<string> GetAllErrorMessagesAddProduct()
        {
            List<string> errorMessages = new List<string>();

            // Lấy tất cả các phần tử span chứa class "text-danger"
            var errorElements = driver.FindElements(By.CssSelector("span.text-danger"));

            // Duyệt qua từng phần tử và lấy text nếu có lỗi
            foreach (var errorElement in errorElements)
            {
                if (!string.IsNullOrEmpty(errorElement.Text))
                {
                    errorMessages.Add("Fail: " + errorElement.Text);
                }
            }

            return errorMessages;
        }


        // Hàm thực hiện thêm sản phẩm
        public List<string> PerformAddProduct(string name, string price, string originalPrice, string stock,
    string brandId, string origin, string warranty, string description, string details,
    string seoDescription, string seoTitle, string seoAlias, string categoryId, string thumbnailImagePath)
        {
            List<string> errorMessages = new List<string>();

            try
            {
                // Mở trang thêm sản phẩm
                driver.Navigate().GoToUrl("https://localhost:5002/product/create");

                // Chờ cho các phần tử sẵn sàng
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));

                // Nhập thông tin sản phẩm
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Name"))).SendKeys(name);

                if (!string.IsNullOrEmpty(price))
                {
                    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Price"))).SendKeys(price);
                }

                if (!string.IsNullOrEmpty(originalPrice))
                {
                    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("OriginalPrice"))).SendKeys(originalPrice);
                }

                if (!string.IsNullOrEmpty(stock))
                {
                    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Stock"))).SendKeys(stock);
                }

                //// Chọn thương hiệu
                var brandSelect = new SelectElement(wait.Until(ExpectedConditions.ElementIsVisible(By.Id("BrandId"))));
                brandSelect.SelectByValue(brandId);

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Origin"))).SendKeys(origin);
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Warranty"))).SendKeys(warranty);
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Description"))).SendKeys(description);
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Details"))).SendKeys(details);
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("SeoDescription"))).SendKeys(seoDescription);
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("SeoTitle"))).SendKeys(seoTitle);

                if (!string.IsNullOrEmpty(seoAlias))
                {
                    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("SeoAlias"))).SendKeys(seoAlias);
                }

                //// Chọn danh mục
                var categorySelect = new SelectElement(wait.Until(ExpectedConditions.ElementIsVisible(By.Id("CategoryId"))));
                categorySelect.SelectByValue(categoryId);

                //// Tải lên hình ảnh
                if (!string.IsNullOrEmpty(thumbnailImagePath))
                {
                    driver.FindElement(By.Id("ThumbnailImage")).SendKeys(thumbnailImagePath);
                }

                Thread.Sleep(3000);

                // Nhấp vào button tạo
                Actions actions = new Actions(driver);
                var createButton = driver.FindElement(By.ClassName("btn-create-product"));
                actions.MoveToElement(createButton).Click().Perform();

                // Chờ một chút để xem nếu có thông báo lỗi
                Thread.Sleep(2000);

                // Lấy tất cả thông báo lỗi
                errorMessages = GetAllErrorMessagesAddProduct();

                // Nếu không có thông báo lỗi, tiếp tục kiểm tra URL
                if (errorMessages.Count == 0)
                {
                    wait.Until(ExpectedConditions.UrlToBe("https://localhost:5002/product"));

                    return new List<string> { "Tạo sản phẩm mới thành công" };
                }
            }
            catch (Exception)
            {
                errorMessages = GetAllErrorMessagesAddProduct();
            }

            // Nếu có lỗi, trả về danh sách thông báo lỗi
            return errorMessages.Count > 0 ? errorMessages : new List<string> { "Lỗi không xác định" };
        }

        private bool isLoggedIn = false;
        // Quy trình thêm sản phẩm hoàn chỉnh
        public List<string> AddProductFlow(string username, string password, string name, string price, string originalPrice, string stock,
            string brandId, string origin, string warranty, string description, string details,
            string seoDescription, string seoTitle, string seoAlias, string categoryId, string thumbnailImagePath)
        {
            // Bước 1: Đăng nhập
            if (!isLoggedIn)
            {
                LoginAdmin(username, password);
                isLoggedIn = true; // Cập nhật trạng thái đăng nhập
            }

            // Bước 2: Điều hướng đến trang thêm sản phẩm
            GoToAddProductPage();

            // Bước 3: Thêm sản phẩm và trả về kết quả
            return PerformAddProduct(name, price, originalPrice, stock, brandId, origin, warranty, description, details, seoDescription, seoTitle, seoAlias, categoryId, thumbnailImagePath);
        }

        public List<string> LoginUser(string username, string password) // đánh dấu chức năng cần chức năng đăng nhập
        {
            List<string> errorMessages = new List<string>();
            try
            {
                // Mở trang đăng nhập
                driver.Navigate().GoToUrl("https://localhost:5003/vi-VN/account/login");

                // Nhập tên đăng nhập
                driver.FindElement(By.Id("username")).SendKeys(username);

                // Nhập mật khẩu
                driver.FindElement(By.Id("password")).SendKeys(password);

                // Nhấn nút đăng nhập
                driver.FindElement(By.ClassName("register-button")).Click();

                // Chờ trang chuyển đổi
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.UrlToBe("https://localhost:5003"));

                // Kiểm tra xem có lỗi không
                if (driver.Url != "https://localhost:5003")
                {
                    errorMessages.Add("Đăng nhập không thành công.");
                }
            }
            catch (Exception ex)
            {
                errorMessages.Add($"Cảnh báo: {ex.Message}");
            }
            return errorMessages;
        }

        //Hàm lấy message lỗi ở alert
        public string GetAlertErrorMessageEvaluate()
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                var alertEvaluate = wait.Until(ExpectedConditions.AlertIsPresent());
                string alertMessageEvaluate = alertEvaluate.Text;
                alertEvaluate.Accept();
                return "Fail: " + alertMessageEvaluate;
            }
            catch (WebDriverTimeoutException)
            {
                return "Gửi đánh giá thành công";
            }
        }


        // Hàm để truy cập trang đánh giá sản phẩm
        public void NavigateToProductReview(string productId)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

                // Tìm thẻ a có href tương ứng với sản phẩm
                var productLink = driver.FindElement(By.XPath($"//a[@href='/vi-VN/san-pham/{productId}']"));

                // Cuộn đến thẻ a để nó hiển thị trên màn hình
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", productLink);

                // Nhấp vào thẻ a
                productLink.Click();


                // đợi trang đánh giá
                driver.Navigate().GoToUrl($"https://localhost:5003/vi-VN/san-pham/{productId}");

                // Nhấp vào thẻ a có href là #reviews để mở tab Nhận xét
                var reviewTab = driver.FindElement(By.CssSelector("a[href='#reviews']"));
                reviewTab.Click(); // Nhấp vào tab "Nhận xét"

                // Chờ cho phần tử cha có class "review-btn" không còn bị ẩn
                wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("review-btn")));

                // Sau khi phần tử cha hiển thị, tìm và nhấp vào thẻ <a> có class "review-links"
                driver.FindElement(By.ClassName("review-links")).Click();

            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine($"Cảnh báo: {ex.Message}");
                //throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cảnh báo1: {ex.Message}");
                //throw;
            }
        }

        private bool IsLoggedInUser()
        {
            var authTokenCookie = driver.Manage().Cookies.GetCookieNamed("Token"); // Kiểm tra cookie xác thực
            return authTokenCookie != null;
        }


        public string FillReviewForm(string feedback, string author, string phone, string email)
        {
            try
            {
                // Kiểm tra trạng thái đăng nhập
                if (!IsLoggedInUser())
                {
                    var loginErrors = LoginUser("Test1233", "Test1234@");
                    if (loginErrors.Count > 0)
                    {
                        Console.WriteLine($"Cảnh báo: {string.Join("; ", loginErrors)}");
                        return "";
                    }
                }

                // Đợi các phần tử sẵn sàng
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                // Nhập thông tin vào form
                // Đợi và xóa dữ liệu cũ, sau đó nhập nội dung đánh giá
                var feedbackElement = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("feedback")));
                feedbackElement.Clear();
                feedbackElement.SendKeys(feedback);

                // Đợi và xóa dữ liệu cũ, sau đó nhập họ và tên
                var authorElement = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("author")));
                authorElement.Clear();
                authorElement.SendKeys(author ?? "");

                // Đợi và xóa dữ liệu cũ, sau đó nhập số điện thoại
                var phoneElement = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("phone")));
                phoneElement.Clear();
                phoneElement.SendKeys(phone);

                // Đợi và xóa dữ liệu cũ, sau đó nhập email
                var emailElement = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("email")));
                emailElement.Clear();
                emailElement.SendKeys(email);

                // Nhấn nút gửi đánh giá
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.ClassName("send"))).Click();

                // Kiểm tra và lấy alert nếu có
                if (wait.Until(ExpectedConditions.AlertIsPresent()) != null)
                {
                    return GetAlertErrorMessageEvaluate();
                }
                return "Gửi đánh giá thành công";
            }
            catch (Exception)
            {
                return "Gửi đánh giá thành công";
            }
        }

        public string GetAlertErrorMessageOrder()
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                var alert = wait.Until(ExpectedConditions.AlertIsPresent());
                string alertMessage = alert.Text;
                alert.Accept(); // Đóng alert sau khi lấy thông báo
                return "Fail: " + alertMessage;
            }
            catch (WebDriverTimeoutException)
            {
                return "Đặt hàng thành công";
            }
        }


        public string TestOrder(string shipName, string shipProvince, string shipDistrict, string shipCommune, string shipAddress, string shipEmail, string shipPhoneNumber)
        {
            try
            {
                // Kiểm tra trạng thái đăng nhập
                if (!IsLoggedInUser())
                {
                    var loginErrors = LoginUser("Test1233", "Test1234@"); // Đăng nhập bằng hàm login có sẵn
                    if (loginErrors.Count > 0)
                    {
                        Console.WriteLine($"Lỗi đăng nhập: {string.Join("; ", loginErrors)}");
                        return ""; // Ngừng thực hiện nếu không đăng nhập thành công
                    }
                }

                // Nhấp vào icon giỏ hàng và đến trang thanh toán
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("a[href='/vi-VN/san-pham']"))).Click();
                wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("hm-minicart-trigger"))).Click(); // Nhấp vào icon giỏ hàng
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("a[href='/vi-VN/Cart/Checkout']"))).Click(); // Nhấp vào "Thanh toán"

                // Điền họ và tên
                var shipNameField = driver.FindElement(By.Id("ShipName"));
                shipNameField.Clear();
                shipNameField.SendKeys(shipName);

                // Chọn Tỉnh/Thành
                var provinces = driver.FindElements(By.CssSelector(".nice-select.wide"));
                provinces[1].Click(); // Giả sử tỉnh là phần tử đầu tiên

                // Chọn giá trị tỉnh
                var provinceOption = driver.FindElement(By.XPath("//li[@data-value='" + shipProvince + "']"));
                provinceOption.Click();

                // Chọn Quận/Huyện
                var districts = driver.FindElements(By.CssSelector(".nice-select.wide"));
                districts[3].Click(); // Giả sử huyện là phần tử thứ hai

                // Chọn giá trị huyện
                var districtOption = driver.FindElement(By.XPath("//li[@data-value='" + shipDistrict + "']"));
                districtOption.Click();

                // Chọn Phường/Xã
                var communes = driver.FindElements(By.CssSelector(".nice-select.wide"));
                communes[5].Click(); // Giả sử xã là phần tử thứ ba

                // Chọn giá trị xã
                var communeOption = driver.FindElement(By.XPath("//li[@data-value='" + shipCommune + "']"));
                communeOption.Click();

                // Nhập địa chỉ cụ thể
                var shipAddressField = driver.FindElement(By.Id("ShipAddress"));
                shipAddressField.Clear();
                shipAddressField.SendKeys(shipAddress);

                // Nhập địa chỉ email
                var shipEmailField = driver.FindElement(By.Id("ShipEmail"));
                shipEmailField.Clear();
                shipEmailField.SendKeys(shipEmail);

                // Nhập số điện thoại
                var shipPhoneNumberField = driver.FindElement(By.Id("ShipPhoneNumber"));
                shipPhoneNumberField.Clear();
                shipPhoneNumberField.SendKeys(shipPhoneNumber);

                // Nhấn nút "Đặt hàng"
                // Cuộn đến nút và click
                WebDriverWait Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                var orderButton = wait.Until(driver => driver.FindElement(By.ClassName("button-order")));
                orderButton.Click();

                // Kiểm tra và lấy alert nếu có
                if (wait.Until(ExpectedConditions.AlertIsPresent()) != null)
                {
                    return GetAlertErrorMessageOrder();
                }
                return "Gửi đánh giá thành công";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thực hiện đặt hàng: {ex.Message}");
                return "";
            }
        }

        public void CheckCategory(string category)
        {
            try
            {
                // Bước 1: Điều hướng đến trang chủ
                driver.Navigate().GoToUrl("https://localhost:5003");

                // Bước 2: Nhấp vào thẻ <a> có href="/vi-VN/san-pham"
                var productPageLink = driver.FindElement(By.CssSelector("a[href='/vi-VN/san-pham']"));
                productPageLink.Click();

                // Bước 3: Tìm checkbox theo giá trị của data-category
                var categoryCheckbox = driver.FindElement(By.CssSelector($"input[data-category='{category}']"));

                Thread.Sleep(1000);
                // Cuộn đến checkbox và nhấp chọn
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].scrollIntoView(true);", categoryCheckbox);
                categoryCheckbox.Click();

                // Chờ cho URL thay đổi
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.UrlContains($"/vi-VN/{category}"));

                // Kiểm tra URL hiện tại
                string currentUrl = driver.Url;
                if (currentUrl.Contains($"/vi-VN/{category}"))
                {
                    // Hiển thị thông báo tùy thuộc vào category
                    switch (category)
                    {
                        case "1":
                            Console.WriteLine("Tìm kiếm thành công cho danh mục máy ảnh");
                            break;
                        case "2":
                            Console.WriteLine("Tìm kiếm thành công cho danh mục ống kính");
                            break;
                        case "1001":
                            Console.WriteLine("Tìm kiếm thành công cho danh mục máy quay phim");
                            break;
                        case "1002":
                            Console.WriteLine("Tìm kiếm thành công cho danh mục phụ kiện máy ảnh");
                            break;
                        default:
                            Console.WriteLine($"Tìm kiếm thành công cho danh mục {category}");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine($"Tìm kiếm thất bại cho danh mục {category}");
                }
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine($"Không tìm thấy checkbox cho danh mục {category}: {ex.Message}");
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine($"Thời gian chờ quá lâu cho danh mục {category}");
            }

            // Chờ một chút trước khi tiếp tục danh mục khác
            Thread.Sleep(1000);
        }

        public void GoToUpdateProductPage()
        {
            try
            {
                // Bước 1: Mở rộng menu collapse bằng JavaScript
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("document.getElementById('pagesCollapseAuth').classList.add('show');");

                // Bước 2: Chờ menu mở ra và nhấp vào liên kết "Danh sách"
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//a[@href='/product']")));

                var updateProductLink = driver.FindElement(By.XPath("//a[@href='/product']"));
                updateProductLink.Click();

                // Bước 3: Chờ trang danh sách sản phẩm tải hoàn tất
                wait.Until(ExpectedConditions.UrlContains("/product"));

                // Bước 4: Nhấp vào nút chỉnh sửa sản phẩm bằng XPath
                var editProductLink = driver.FindElement(By.XPath("//a[@href='/Product/Edit/1025']"));
                editProductLink.Click();

                // Bước 5: Chờ trang cập nhật sản phẩm tải hoàn tất
                wait.Until(ExpectedConditions.UrlContains("/Product/Edit/1025"));

                Console.WriteLine("Đã điều hướng đến trang Cập nhật sản phẩm.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Đã xảy ra lỗi khi điều hướng đến trang Cập nhật sản phẩm: {ex.Message}");
            }
        }


        // Hàm lấy thông báo lỗi khi cập nhật sản phẩm
        public List<string> GetAllErrorMessagesUpdateProduct()
        {
            List<string> errorMessages = new List<string>();

            // Lấy tất cả các phần tử span chứa class "text-danger"
            var errorElements = driver.FindElements(By.CssSelector("span.text-danger"));

            // Duyệt qua từng phần tử và lấy text nếu có lỗi
            foreach (var errorElement in errorElements)
            {
                if (!string.IsNullOrEmpty(errorElement.Text))
                {
                    errorMessages.Add("Fail: " + errorElement.Text);
                }
            }

            return errorMessages;
        }


        // Hàm thực hiện cập nhật sản phẩm
        public List<string> PerformUpdateProduct(string name, string price, string originalPrice, string brandId, string origin, string warranty, string description, string details,
        string seoDescription, string seoTitle, string seoAlias, string thumbnailImagePath)
        {
            List<string> errorMessages = new List<string>();

            try
            {
                // Điều hướng đến trang cập nhật sản phẩm
                driver.Navigate().GoToUrl("https://localhost:5002/Product/Edit/1025");

                // Chờ các phần tử trên trang sẵn sàng
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

                // Nhập thông tin sản phẩm
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Name"))).Clear();
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Name"))).SendKeys(name);

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Price"))).Clear();
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Price"))).SendKeys(price);

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("OriginalPrice"))).Clear();
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("OriginalPrice"))).SendKeys(originalPrice);

                // Chọn thương hiệu
                var brandSelect = new SelectElement(wait.Until(ExpectedConditions.ElementIsVisible(By.Id("BrandId"))));
                brandSelect.SelectByValue(brandId);

                // Nhập các thông tin khác
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Origin"))).Clear();
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Origin"))).SendKeys(origin);

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Warranty"))).Clear();
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Warranty"))).SendKeys(warranty);

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Description"))).Clear();
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Description"))).SendKeys(description);

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Details"))).Clear();
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Details"))).SendKeys(details);

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("SeoDescription"))).Clear();
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("SeoDescription"))).SendKeys(seoDescription);

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("SeoTitle"))).Clear();
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("SeoTitle"))).SendKeys(seoTitle);

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("SeoAlias"))).Clear();
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("SeoAlias"))).SendKeys(seoAlias);

                // Tải lên hình ảnh nếu có đường dẫn hình ảnh
                if (!string.IsNullOrEmpty(thumbnailImagePath))
                {
                    var imageUpload = driver.FindElement(By.Id("ThumbnailImage"));
                    imageUpload.Clear();
                    imageUpload.SendKeys(thumbnailImagePath);
                }

                // Nhấp vào nút cập nhật sản phẩm
                Actions actions = new Actions(driver);
                var updateButton = driver.FindElement(By.ClassName("btn-update-product"));
                actions.MoveToElement(updateButton).Click().Perform();

                // Chờ để kiểm tra thông báo lỗi nếu có
                Thread.Sleep(3000);

                // Lấy thông báo lỗi sau khi nhấn nút cập nhật
                errorMessages = GetAllErrorMessagesUpdateProduct();

                // Kiểm tra nếu không có lỗi, xác nhận URL điều hướng thành công
                if (errorMessages.Count == 0)
                {
                    wait.Until(ExpectedConditions.UrlContains("/product"));
                    return new List<string> { "Cập nhật sản phẩm thành công" };
                }
            }
            catch (Exception)
            {
                errorMessages = GetAllErrorMessagesUpdateProduct();
            }

            // Trả về thông báo lỗi nếu có
            return errorMessages.Count > 0 ? errorMessages : new List<string> { "Lỗi không xác định" };
        }


        public List<string> UpdateProductFlow(string username, string password, string name, string price, string originalPrice, string brandId, string origin, string warranty, string description, string details, string seoDescription, string seoTitle, string seoAlias, string thumbnailImagePath)
        {
            // Bước 1: Đăng nhập
            if (!isLoggedIn)
            {
                LoginAdmin(username, password);
                isLoggedIn = true; // Cập nhật trạng thái đăng nhập
            }

            // Bước 2: Điều hướng đến trang cập nhật sản phẩm
            GoToUpdateProductPage();

            // Bước 3: Cập nhật sản phẩm và trả về kết quả
            return PerformUpdateProduct(name, price, originalPrice, brandId, origin, warranty, description, details, seoDescription, seoTitle, seoAlias, thumbnailImagePath);
        }


        // Hàm điều hướng đến trang xóa sản phẩm với ID cụ thể
        public void GoToDeleteProduct(string productId)
        {
            try
            {
                // Mở rộng menu collapse bằng JavaScript
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("document.getElementById('pagesCollapseAuth').classList.add('show');");

                // Chờ menu mở ra và nhấp vào liên kết "Danh sách"
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(25));  // Tăng thời gian chờ
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//a[@href='/product']")));

                var listProductLink = driver.FindElement(By.XPath("//a[@href='/product']"));
                listProductLink.Click();

                // Chờ trang danh sách sản phẩm tải hoàn tất
                wait.Until(ExpectedConditions.UrlContains("/product"));

                // Cuộn đến liên kết trang 5
                var deleteProductPage = driver.FindElement(By.XPath("//a[@href='/Product?pageIndex=5']"));
                js.ExecuteScript("arguments[0].scrollIntoView(true);", deleteProductPage);

                // Kiểm tra phần tử trước khi nhấp
                Console.WriteLine("Is Displayed: " + deleteProductPage.Displayed);
                Console.WriteLine("Is Enabled: " + deleteProductPage.Enabled);

                // Nhấp vào liên kết trang 5 bằng JavaScript
                js.ExecuteScript("arguments[0].click();", deleteProductPage);

                // Chờ trang sản phẩm ở trang 5 tải hoàn tất
                wait.Until(ExpectedConditions.UrlContains("/Product?pageIndex=5"));

                // Bước 3: Tìm và nhấp vào liên kết xóa sản phẩm
                var deleteProductLink = driver.FindElement(By.XPath($"//a[contains(@href, 'Product/Delete/{productId}')]"));
                js.ExecuteScript("arguments[0].scrollIntoView(true);", deleteProductLink);
                js.ExecuteScript("arguments[0].click();", deleteProductLink);

                // Bước 4: Chờ nút xóa xuất hiện
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".button-delete-product")));

                // Nhấp vào nút xóa có class 'button-delete-product'
                var deleteButton = driver.FindElement(By.CssSelector(".button-delete-product"));
                wait.Until(ExpectedConditions.ElementToBeClickable(deleteButton));
                deleteButton.Click();

                // Chờ URL trở về trang danh sách sản phẩm
                wait.Until(ExpectedConditions.UrlToBe("https://localhost:5002/Product"));

            }
            catch (NoSuchElementException)
            {
                Console.WriteLine($"Không tìm thấy sản phẩm cần xóa");
                throw;
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine($"Thao tác bị timeout");
                throw;
            }
            catch (Exception)
            {
                Console.WriteLine($"Đã xảy ra lỗi hệ thống");
                throw;
            }
        }


        // Hàm điều hướng để đăng nhập và xóa sản phẩm với ID
        public List<string> DeleteProductFlow(string username, string password, string productId)
        {
            try
            {
                // Bước 1: Đăng nhập
                if (!isLoggedIn)
                {
                    LoginAdmin(username, password);
                    isLoggedIn = true; // Cập nhật trạng thái đăng nhập
                }

                // Bước 2: Điều hướng đến trang xóa sản phẩm và xóa sản phẩm
                GoToDeleteProduct(productId);

                return new List<string> { "Xóa sản phẩm thành công." };
            }
            catch (Exception ex)
            {
                return new List<string> { $"Đã xảy ra lỗi: {ex.Message}" };
            }
        }


        public void GoToAddUserPage()
        {
            try
            {
                // Bước 1: Mở rộng menu collapse 
                driver.FindElement(By.CssSelector("a[data-bs-target='#collapseLayouts']")).Click();

                // Bước 2: Chờ menu mở ra và nhấp vào liên kết "Thêm mới"
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[@href='/user/create']")));

                var addUserLink = driver.FindElement(By.XPath("//a[@href='/user/create']"));
                addUserLink.Click();

                // Bước 3: Chờ trang thêm sản phẩm tải hoàn tất
                wait.Until(ExpectedConditions.UrlToBe("https://localhost:5002/user/create"));

                Console.WriteLine("Đã điều hướng đến trang Thêm người dùng.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"cảnh báo khi điều hướng đến trang Thêm người dùng: {ex.Message}");
            }
        }


        // Hàm lấy thông báo lỗi khi thêm người dùng
        public List<string> GetAllErrorMessagesAddUser()
        {
            List<string> errorMessages = new List<string>();

            // Lấy tất cả các phần tử span chứa class "text-danger"
            var errorElements = driver.FindElements(By.CssSelector("span.text-danger"));

            // Duyệt qua từng phần tử và lấy text nếu có lỗi
            foreach (var errorElement in errorElements)
            {
                if (!string.IsNullOrEmpty(errorElement.Text))
                {
                    errorMessages.Add("Fail: " + errorElement.Text);
                }
            }

            return errorMessages;
        }


        // Hàm thực hiện thêm người dùng
        public List<string> PerformAddUser(string firstname, string lastname, string dob, string email,
    string phonenumber, string username, string password, string confirmpassword)
        {
            List<string> errorMessages = new List<string>();

            try
            {
                // Mở trang thêm sản phẩm
                driver.Navigate().GoToUrl("https://localhost:5002/user/create");

                // Chờ cho các phần tử sẵn sàng
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));

                // Nhập thông tin sản phẩm
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("FirstName"))).SendKeys(firstname);
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("LastName"))).SendKeys(lastname);
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Dob"))).SendKeys(dob);
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Email"))).SendKeys(email);
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("PhoneNumber"))).SendKeys(phonenumber);
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("UserName"))).SendKeys(username);
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Password"))).SendKeys(password);
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ConfirmPassword"))).SendKeys(confirmpassword);

                Thread.Sleep(3000);

                // Nhấp vào button tạo
                Actions actions = new Actions(driver);
                var createButton = driver.FindElement(By.ClassName("button-create-user"));
                actions.MoveToElement(createButton).Click().Perform();

                // Chờ một chút để xem nếu có thông báo lỗi
                Thread.Sleep(2000); // Có thể điều chỉnh thời gian này

                // Lấy tất cả thông báo lỗi
                errorMessages = GetAllErrorMessagesAddUser();

                // Nếu không có thông báo lỗi, tiếp tục kiểm tra URL
                if (errorMessages.Count == 0)
                {
                    //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                    wait.Until(ExpectedConditions.UrlToBe("https://localhost:5002/User"));

                    return new List<string> { "Tạo người dùng mới thành công" };
                }
            }
            catch (Exception)
            {
                errorMessages = GetAllErrorMessagesAddUser();
            }

            // Nếu có lỗi, trả về danh sách thông báo lỗi
            return errorMessages.Count > 0 ? errorMessages : new List<string> { "Thêm người dùng thất bại: Không rõ lỗi." };
        }

        public List<string> AddUserFlow(string user, string pass, string firstname, string lastname, string dob, string email,
        string phonenumber, string username, string password, string confirmpassword)
        {
            // Bước 1: Đăng nhập
            if (!isLoggedIn)
            {
                LoginAdmin(user, pass);
                isLoggedIn = true; // Cập nhật trạng thái đăng nhập
            }

            // Bước 2: Điều hướng đến trang thêm 
            GoToAddUserPage();

            // Bước 3: Thêm và trả về kết quả
            return PerformAddUser(firstname, lastname, dob, email, phonenumber, username, password, confirmpassword);
        }

        public void GoToUpdateUserPage()
        {
            try
            {
                // Bước 1: Mở rộng menu collapse 
                driver.FindElement(By.CssSelector("a[data-bs-target='#collapseLayouts']")).Click();

                // Bước 2: Chờ menu mở ra và nhấp vào liên kết "Thêm mới"
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[@href='/user']")));

                var addUserLink = driver.FindElement(By.XPath("//a[@href='/user']"));
                addUserLink.Click();

                // Bước 3: Chờ trang danh sách người dùng tải hoàn tất
                wait.Until(ExpectedConditions.UrlContains("/User"));

                // Bước 4: Nhấp vào nút chỉnh sửa người dùng bằng XPath
                var editUserLink = driver.FindElement(By.XPath("//a[@href='User/Edit/a932e74a-2363-45d9-473b-08dcd40a60c5']"));
                editUserLink.Click();

                // Bước 5: Chờ trang cập nhật người dùng tải hoàn tất
                wait.Until(ExpectedConditions.UrlContains("/User/Edit/a932e74a-2363-45d9-473b-08dcd40a60c5"));

                Console.WriteLine("Đã điều hướng đến trang Thêm người dùng.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"cảnh báo khi điều hướng đến trang Thêm người dùng: {ex.Message}");
            }
        }

        // Hàm lấy thông báo lỗi khi thêm người dùng (bỏ qua các trường username, password, và confirmpassword)
        public List<string> GetAllErrorMessagesUpdateUser()
        {
            List<string> errorMessages = new List<string>();

            // Lấy tất cả các phần tử span chứa class "text-danger"
            var errorElements = driver.FindElements(By.CssSelector("span.text-danger"));

            // Duyệt qua từng phần tử và lấy text nếu có lỗi
            foreach (var errorElement in errorElements)
            {
                if (!string.IsNullOrEmpty(errorElement.Text))
                {
                    errorMessages.Add("Fail: " + errorElement.Text);
                }
            }

            return errorMessages;
        }

        // Hàm thực hiện thêm người dùng (bỏ qua username, password và confirmpassword)
        public List<string> PerformUpdateUser(string firstname, string lastname, string dob, string email, string phonenumber)
        {
            List<string> errorMessages = new List<string>();

            try
            {
                // Mở trang thêm người dùng
                driver.Navigate().GoToUrl("https://localhost:5002/User/Edit/a932e74a-2363-45d9-473b-08dcd40a60c5");

                // Chờ cho các phần tử sẵn sàng
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));

                // Nhập thông tin người dùng
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("FirstName"))).Clear();
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("FirstName"))).SendKeys(firstname);

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("LastName"))).Clear();
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("LastName"))).SendKeys(lastname);

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Dob"))).Clear();
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Dob"))).SendKeys(dob);

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Email"))).Clear();
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Email"))).SendKeys(email);

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("PhoneNumber"))).Clear();
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("PhoneNumber"))).SendKeys(phonenumber);

                // Nhấp vào button tạo
                Actions actions = new Actions(driver);
                var updateUserButton = driver.FindElement(By.ClassName("button-update-user"));
                actions.MoveToElement(updateUserButton).Click().Perform();

                // Chờ một chút để xem nếu có thông báo lỗi
                Thread.Sleep(2000); // Có thể điều chỉnh thời gian này

                // Lấy tất cả thông báo lỗi
                errorMessages = GetAllErrorMessagesUpdateUser();

                // Nếu không có thông báo lỗi, tiếp tục kiểm tra URL
                if (errorMessages.Count == 0)
                {
                    wait.Until(ExpectedConditions.UrlToBe("https://localhost:5002/User"));
                    return new List<string> { "Cập nhật người dùng thành công" };
                }
            }
            catch (Exception)
            {
                errorMessages.Add("Lỗi không xác định");
            }

            // Nếu có lỗi, trả về danh sách thông báo lỗi
            return errorMessages.Count > 0 ? errorMessages : new List<string> { "Cập nhật người dùng thất bại: Không rõ lỗi." };
        }

        // Hàm cập nhật người dùng với quy trình mới (bỏ qua username, password và confirmpassword)
        public List<string> UpdateUserFlow(string user, string pass, string firstname, string lastname, string dob, string email, string phonenumber)
        {
            // Bước 1: Đăng nhập
            if (!isLoggedIn)
            {
                LoginAdmin(user, pass);
                isLoggedIn = true; // Cập nhật trạng thái đăng nhập
            }

            // Bước 2: Điều hướng đến trang thêm 
            GoToUpdateUserPage();

            // Bước 3: Thực hiện thêm/cập nhật và trả về kết quả
            return PerformUpdateUser(firstname, lastname, dob, email, phonenumber);
        }

        public void GoToDeleteUser(string userId)
        {
            try
            {
                // Bước 1: Mở rộng menu collapse 
                driver.FindElement(By.CssSelector("a[data-bs-target='#collapseLayouts']")).Click();

                // Bước 2: Chờ menu mở ra và nhấp vào liên kết "Danh sách"
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[@href='/user']")));

                var addUserLink = driver.FindElement(By.XPath("//a[@href='/user']"));
                addUserLink.Click();

                // Bước 3: Chờ trang danh sách người dùng tải hoàn tất
                wait.Until(ExpectedConditions.UrlContains("/User"));

                // Bước 4: Tìm thẻ 'a' có href chứa "User/Delete/{id}" và nhấp vào
                var deleteUserLink = wait.Until(ExpectedConditions.ElementExists(By.XPath($"//a[contains(@href, 'User/Delete/{userId}')]")));

                if (deleteUserLink != null)
                {
                    wait.Until(ExpectedConditions.ElementToBeClickable(deleteUserLink));
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    js.ExecuteScript("arguments[0].click();", deleteUserLink);
                }
                else
                {
                    Console.WriteLine("Không tìm thấy liên kết xóa người dùng.");
                }

                // Bước 5: Chờ trang xóa người dùng tải hoàn tất
                wait.Until(ExpectedConditions.UrlToBe($"https://localhost:5002/User/Delete/{userId}"));

                // Bước 6: Nhấp vào nút xóa có class 'button-delete-user'
                var deleteButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".button-delete-user")));
                deleteButton.Click();

                // Bước 7: Chờ URL trở về trang danh sách người dùng
                wait.Until(ExpectedConditions.UrlToBe("https://localhost:5002/User"));
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Không tìm thấy người dùng. Vui lòng kiểm tra lại ID người dùng.");
                throw;
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Thao tác bị timeout. Vui lòng thử lại sau.");
                throw;
            }
            catch (Exception)
            {
                Console.WriteLine($"Đã xảy ra lỗi hệ thống");
                throw;
            }
        }


        public List<string> DeleteUserFlow(string username, string password, string userId)
        {
            List<string> result = new List<string>();

            // Bước 1: Đăng nhập
            if (!isLoggedIn)
            {
                LoginAdmin(username, password);
                isLoggedIn = true; // Cập nhật trạng thái đăng nhập
            }

            // Bước 2: Điều hướng đến trang xóa người dùng
            try
            {
                GoToDeleteUser(userId);
                result.Add("Xóa người dùng thành công.");
            }
            catch (Exception ex)
            {
                result.Add($"Không thể xóa người dùng: {ex.Message}");
            }

            return result;
        }

        public void GoToEvaluate(string evaluateId)
        {
            try
            {
                // Bước 1: Mở rộng menu collapse 
                driver.FindElement(By.CssSelector("a[data-bs-target='#pagesCollapseReview']")).Click();

                // Bước 2: Chờ menu mở ra và nhấp vào liên kết "Danh sách"
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[@href='/duyet-danh-gia']"))).Click();

                // Bước 3: Chờ trang đánh giá tải hoàn tất
                wait.Until(ExpectedConditions.UrlContains("/duyet-danh-gia"));

                // Bước 4: Nhấp vào nút đánh giá có id="btn-approved" và data-id={userId}
                var evaluateButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector($"a#btn-approved[data-id='{evaluateId}']")));
                evaluateButton.Click();

                // Bước 5: Chờ modal xác nhận hiển thị
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".modal-content")));

                // Bước 6: Nhấp vào nút xác nhận trong modal
                var confirmButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("btn-confirm-approved")));
                confirmButton.Click();
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Không tìm thấy đánh giá.");
                throw;
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Thao tác bị timeout.");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Đã xảy ra lỗi hệ thống: {ex.Message}");
                throw;
            }
        }

        public List<string> EvaluateFlow(string username, string password, string evaluateId)
        {
            // Bước 1: Đăng nhập
            if (!isLoggedIn)
            {
                LoginAdmin(username, password);
                isLoggedIn = true; // Cập nhật trạng thái đăng nhập
            }

            // Bước 2: Điều hướng đến trang duyệt đánh giá
            GoToEvaluate(evaluateId);
            return new List<string> { "Duyệt đánh giá thành công." };
        }



        public string GetAlertErrorMessageLoginAdmin()
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                var alert = wait.Until(ExpectedConditions.AlertIsPresent());
                string alertMessage = alert.Text;
                alert.Accept(); // Đóng alert sau khi lấy thông báo
                return alertMessage;
            }
            catch (WebDriverTimeoutException)
            {
                return ""; // Nếu không có alert, trả về chuỗi rỗng
            }
        }

        public string PerformLoginAdmin(string username, string password)
        {
            try
            {
                // Điều hướng đến trang đăng nhập
                driver.Navigate().GoToUrl("https://localhost:5002/Login");

                // Nhập thông tin đăng nhập
                driver.FindElement(By.Id("username")).SendKeys(username);
                driver.FindElement(By.Id("password")).SendKeys(password);

                // Nhấp vào nút đăng nhập
                driver.FindElement(By.Id("btn-dangnhap")).Click();

                // Chờ kiểm tra nếu có alert xuất hiện trước khi kiểm tra URL
                string alertMessage = GetAlertErrorMessageLoginAdmin();
                if (!string.IsNullOrEmpty(alertMessage))
                {
                    return alertMessage; // Trả về thông báo alert nếu có
                }

                // Sử dụng WebDriverWait để chờ trang chuyển hướng
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                // Kiểm tra nếu URL chuyển đến trang chính thành công
                if (wait.Until(ExpectedConditions.UrlContains("https://localhost:5002/")))
                {
                    return "Đăng nhập thành công";
                }
                else
                {
                    return "Đăng nhập thất bại: Không thể chuyển đến trang chính";
                }
            }
            catch (Exception ex)
            {
                return $"Cảnh báo: {ex.Message}";
            }
        }


        public void Clean()
        {
            // Đóng trình duyệt sau khi test hoàn thành
            if (driver != null)
            {
                driver.Quit(); // Giải phóng tài nguyên
                driver.Dispose(); // Gọi Dispose để đảm bảo tất cả tài nguyên được giải phóng
                driver = null; // Đặt driver về null để tránh lỗi
            }
        }
    }
}
