using Excel;
using RWProject.Database;
using RWProject.Enum;
using RWProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace RWProject.Controllers
{
    public class ProductController : Controller
    {
        public ProductController()
        {
        }
        public ActionResult Index()
        {            
            return View();
        }

        public JsonResult GetProducts()
        {
            using (var Model = new RWModel())
            {
                var query = Model.Product.Select(s => new ProductViewModel
                {
                    Id = s.Id,
                    Category = s.Category != null ? s.Category.Parent != null ? s.Category.Parent.Name : s.Category.Name : "",
                    Subcategory = s.Category != null ? s.Category.Parent == null ? "" : s.Category.Name : "",
                    Size = s.Size,
                    Code = s.Code,
                    Colour = s.Colour,
                    Description = s.Description,
                    Model = s.Model,
                    Product = s.Name,
                    Price = s.Price,
                    Actions = "<a href='/Product/CreateOrUpdate/"+s.Id+"' class='editProduct'><i class='fa fa-edit' aria-hidden='true'></i></a> <a data-id='" + s.Id + "' class='deleteProduct'><i class='fa fa-trash' aria-hidden='true'></i></a>"
                }).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult CreateOrUpdate(long Id)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");
            Product product;
            var model = new ProductViewModel();
            using (var Model = new RWModel())
            {
                product = Model.Product.Find(Id) ?? new Product();
                model.Id = product.Id;
                model.Code = product.Code;
                model.Model = product.Model;
                model.Product = product.Name;
                model.Price = product.Price;
                model.Size = product.Size;
                model.Colour = product.Colour;
                model.Description = product.Description;
                model.Categories = Model.Category.Where(x => x.Parent == null).Select(y => new CategoryViewModel() { Id = y.Id, Category = y.Name }).ToList();
                model.CategoryId = product.Category != null ? product.Category.Parent != null ? product.Category.Parent.Id : product.Category.Id : 0;
                model.Subcategories = product.Category != null && product.Category.Parent != null ? Model.Category.Where(x => x.Parent != null && x.Parent.Id == product.Category.Parent.Id).Select(y => new CategoryViewModel() { Id = y.Id, Category = y.Name }).ToList() : new List<CategoryViewModel>();
                model.SubcategoryId = product.Category != null ? product.Category.Parent == null ? 0 : product.Category.Id : 0;
            }  
            return View("CreateOrUpdate",model);
        }

        [HttpPost]
        public ActionResult CreateOrUpdate(ProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");
                Product product;
                using (var Model = new RWModel())
                {
                    product = Model.Product.Find(viewModel.Id) ?? new Product();
                    product.Id = viewModel.Id;
                    product.Code = viewModel.Code;
                    product.Model = viewModel.Model;
                    product.Name = viewModel.Product;
                    product.Price = viewModel.Price;
                    product.Size = viewModel.Size;
                    product.Colour = viewModel.Colour;
                    product.Description = viewModel.Description;
                    var category = viewModel.SubcategoryId != 0 ? Model.Category.Find(viewModel.SubcategoryId) : viewModel.CategoryId != 0 ? Model.Category.Find(viewModel.Category) : null;
                    product.Category = category;
                    if (product.Id == 0)
                    {
                        Model.Product.Add(product);
                    }
                    Model.SaveChanges();
                }
                return Redirect("/Product/Index");
            }
            return Redirect("/Product/CreateOrUpdate/"+viewModel.Id);
        }
        [HttpPost]
        public JsonResult DeleteProduct(long Id)
        {
            using (var Model = new RWModel())
            {
                var product = Model.Product.Find(Id);
                Model.Product.Remove(product);
                Model.SaveChanges();    
                return Json(new {ok=true }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSubcategories(long Id)
        {
            using (var Model = new RWModel())
            {
                var query = Model.Category.Where(x=>x.Parent!=null && x.Parent.Id==Id).Select(s => new CategoryViewModel
                {
                    Category = s.Name,
                    Id = s.Id
                }).ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            using (var Model = new RWModel())
            {
                if (ModelState.IsValid)
                {

                    if (upload != null && upload.ContentLength > 0)
                    {
                        // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                        // to get started. This is how we avoid dependencies on ACE or Interop:
                        Stream stream = upload.InputStream;

                        // We return the interface, so that
                        IExcelDataReader reader = null;


                        if (upload.FileName.EndsWith(".xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        else if (upload.FileName.EndsWith(".xlsx"))
                        {
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        else
                        {
                            ModelState.AddModelError("File", "This file format is not supported");
                            return View();
                        }

                        reader.IsFirstRowAsColumnNames = true;

                        DataSet result = reader.AsDataSet();
                        reader.Close();
                        foreach (DataRow row in result.Tables[0].Rows)
                        {
                            var categoryName = row[(int)ProductImportEnum.Category].ToString();
                            var category = Model.Category.FirstOrDefault(x => x.Name == categoryName) ?? new Category() { Name = categoryName };
                            if (category.Id == 0)
                            {
                                Model.Category.Add(category);
                                Model.SaveChanges();
                            }
                            var subcategoryName = row[(int)ProductImportEnum.Subcategory].ToString();
                            var subcategory = Model.Category.FirstOrDefault(x => x.Name == subcategoryName) ?? new Category() { Name = subcategoryName, Parent = category };
                            if (subcategory.Id == 0)
                            {
                                Model.Category.Add(subcategory);
                                Model.SaveChanges();
                            }
                            var productCode = row[(int)ProductImportEnum.Code].ToString();
                            var product = Model.Product.FirstOrDefault(x => x.Code == productCode) ?? new Product();
                            product.Name = row[(int)ProductImportEnum.Name].ToString();
                            product.Description = row[(int)ProductImportEnum.Description].ToString();
                            product.Code = productCode;
                            product.Colour = row[(int)ProductImportEnum.Colour].ToString();
                            product.Model = row[(int)ProductImportEnum.Model].ToString();
                            product.Price = Convert.ToDecimal(row[(int)ProductImportEnum.Price].ToString());
                            product.Size = row[(int)ProductImportEnum.Size].ToString();
                            product.Category = subcategory;
                            if (product.Id == 0)
                            {
                                Model.Product.Add(product);
                                Model.SaveChanges();
                            }


                        }
                        return View(result.Tables[0]);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Please Upload Your file");
                    }
                }
            }
            return View();
        }

        
    }
}