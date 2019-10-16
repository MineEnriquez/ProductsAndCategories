using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProductsAndCategories.Models;
using Microsoft.EntityFrameworkCore;    //MMGC: For entity handling
using Microsoft.AspNetCore.Identity;    //MMGC:  For password hashing.
using Microsoft.AspNetCore.Http;

namespace ProductsAndCategories.Controllers
{
    public class HomeController : Controller
    {
        private ProductsAndCategoriesContext dbContext;
        public HomeController(ProductsAndCategoriesContext context) { dbContext = context; }

        public IActionResult Index()
        {
            return View();
        }

        //-----------------
        [HttpPost("Register")]
        public IActionResult Register(User _user)
        {
            // Check initial ModelState
            if (ModelState.IsValid)
            {
                // If a User exists with provided email
                if (dbContext.Users.Any(u => u.Email == _user.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");
                    return Redirect("/");
                    // You may consider returning to the View at this point
                }
                // Initializing a PasswordHasher object, providing our User class as its
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                _user.Password = Hasher.HashPassword(_user, _user.Password);

                dbContext.Add(_user);
                dbContext.SaveChanges();

                return Redirect("Login");
            }
            else
            {
                // Oh no!  We need to return a ViewResponse to preserve the ModelState, and the errors it now contains!
                return View("Index");
            }
    }

        [Route("Login")]
        [HttpGet]
        public IActionResult CompleteRegistration()
        {
            return View("Login");
        }

        [Route("Login")]
        [HttpPost]
        public IActionResult Login(LoginUser userSubmission)
        {
            HttpContext.Session.Clear();
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);

                // If no user exists with provided email
                if (userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }

                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();

                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);

                // result can be compared to 0 for failure
                if (result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Password", "Invalid Email/Password");
                    //Clean up the session's user Id:
                    return View("Login");

                }

                if(HttpContext.Session.GetInt32("UserId")==null){
                    HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                }
                //return View("Success");
                return Redirect("Products");
            }
            else
            {
                // Oh no!  We need to return a ViewResponse to preserve the ModelState, and the errors it now contains!
                return View("Login");
            }
        }

        public void CleanUpUserId()
        {    
                    HttpContext.Session.Clear();
        }

        [Route("Success")]
        [HttpGet]
        public IActionResult Success()
        {
            if(HttpContext.Session.GetInt32("UserId")==null){
                return Redirect("/");
            }
            return Redirect("Login");
        }
        //--------------

        [Route("Products")]
        [HttpGet]
        public IActionResult GetProducts()
        {
            // ViewBag.Products = dbContext.Products.ToList();
            Console.WriteLine(" Passing at Products - HttpGet ------");
            ProductsPageModel prod = new ProductsPageModel();
            prod.Products =  dbContext.Products.ToList();

            return View("Products", prod);
        }


        [Route("Products")]
        [HttpPost]
        public IActionResult AddProduct(ProductsPageModel prod)
        {
            //in this list we add the new product, then redirect to the products page again
            if(ModelState.IsValid)
            {
                dbContext.Products.Add(prod.OneProduct);
                dbContext.SaveChanges();
                return Redirect("Products");    
            }
            else
            {
                prod.Products =  dbContext.Products.ToList();
                Console.WriteLine(" Passing at Products - Error in model state ------");
                return View("Products", prod);
            }
        }
        [Route("Categories")]
        [HttpGet]
        public IActionResult GetCategories()
        {
            // ViewBag.Categories = dbContext.Categories.ToList();
            Console.WriteLine(" Passing at Categories - HttpGet ------");
            CategoriesPageModel cate = new CategoriesPageModel();
            cate.Categories =  dbContext.Categories.ToList();

            return View("Categories", cate);
        }


        [Route("Categories")]
        [HttpPost]
        public IActionResult AddCategory(CategoriesPageModel cate)
        {
            //in this list we add the new Category, then redirect to the Categories page again
            if(ModelState.IsValid)
            {
                dbContext.Categories.Add(cate.OneCategory);
                dbContext.SaveChanges();
                return Redirect("Categories");    
            }
            else
            {
                cate.Categories =  dbContext.Categories.ToList();
                Console.WriteLine(" Passing at Categories - Error in model state ------");
                return View("Categories", cate);
            }
        }


        [Route("Products/{_prodId}")]
        [HttpGet]
        public IActionResult DisplayProduct(int _prodId)
        {
            Product myProduct = dbContext.Products
            .Include(u=> u.ListOfAssociations)
            .ThenInclude(s => s.AssociatedCategory)
            .FirstOrDefault(p => p.ProductId== _prodId);

            var prodCategories = myProduct.ListOfAssociations.Select(a => a.AssociatedCategory);
            foreach( var x in prodCategories)
            {
                Console.WriteLine(x);
            }

            List<int> associated = myProduct.ListOfAssociations.Select(a=>a.CategoryId).ToList();
            List<Category> cateList = dbContext.Categories.Where(c => !associated.Contains(c.CategoryId)).ToList();

            ViewBag.Categories = cateList;
            return View("DisplayProduct", myProduct);
        }

        [Route("AddAssociation")]
        [HttpPost]
        public IActionResult AddAnAssocitationToTheProduct(int AnId, int ProductId)
        {
            Association entry = new Association();
            entry.CategoryId = AnId;
            entry.ProductId = ProductId;

            dbContext.Associations.Add(entry);
            dbContext.SaveChanges();
            string redirectTo = "Products/" + ProductId;
            return Redirect(redirectTo);

        }


        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
