using Excel;
using RWProject.Database;
using RWProject.Enum;
using RWProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace RWProject.Controllers
{
    public class ProductController : Controller
    {
        RWModel Model = new RWModel();
        public ActionResult Index()
        {            
            return View();
        }

        public JsonResult GetProducts()
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
                Actions = "<a data-id='" + s.Id + "' class='deleteProduct'><i class='fa fa-edit' aria-hidden='true'></i></a> <a data-id='" + s.Id + "' class='deleteProduct'><i class='fa fa-trash' aria-hidden='true'></i></a>"
            }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase upload)
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
                    foreach(DataRow row in result.Tables[0].Rows)
                    {
                        var categoryName = row[(int)ProductImportEnum.Category].ToString();
                        var category = Model.Category.FirstOrDefault(x => x.Name == categoryName) ?? new Category() {Name= categoryName };
                        if (category.Id == 0)
                        {
                            Model.Category.Add(category);
                            Model.SaveChanges();
                        }
                        var subcategoryName = row[(int)ProductImportEnum.Subcategory].ToString();
                        var subcategory = Model.Category.FirstOrDefault(x => x.Name == subcategoryName) ?? new Category() { Name = subcategoryName, Parent=category };
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
            return View();
        }


    }
}