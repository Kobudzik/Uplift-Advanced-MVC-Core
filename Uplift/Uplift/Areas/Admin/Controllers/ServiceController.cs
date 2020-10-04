using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Uplift.DataAccess.Data.Repository.IRepository;
using Uplift.Models;
using Uplift.Models.ViewModels;

namespace Uplift.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ServiceController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            ServiceViewModel serviceVM = new ServiceViewModel()
            {
                Service = new Service(),
                CategoryList = _unitOfWork.Category.GetCategoryListForDropDown(),
                FrequencyList = _unitOfWork.Frequency.GetFrequencyListForDropDown()
            };

            if (id != null)
            {
                serviceVM.Service = _unitOfWork.Service.Get(id.GetValueOrDefault());
            }

            return View(serviceVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ServiceViewModel ServiceVM)
        {
            if (ModelState.IsValid) 
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                if (ServiceVM.Service.Id == 0) //ADDING NEW SERVICE
                {
                    string fileName = Guid.NewGuid().ToString();
                    var newFileUploadLocation = Path.Combine(webRootPath, @"images\services");
                    var extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(
                        Path.Combine(newFileUploadLocation, fileName + extension), //creates full path
                        FileMode.Create)
                    )
                    {
                        files[0].CopyTo(fileStream); //path passed to ctor
                    }

                    ServiceVM.Service.ImageUrl = @"\images\services\" + fileName + extension;
                    _unitOfWork.Service.Add(ServiceVM.Service);
                }
                else //EDIT SERVICE
                {
                    var serviceFromDb = _unitOfWork.Service.Get(ServiceVM.Service.Id);
                    if (files.Count > 0)    //CHANGE IMAGE IF passed
                    {
                        string changedFileName = Guid.NewGuid().ToString();
                        var changedFileUploadLocation = Path.Combine(webRootPath, @"images\services");
                        var changedFileExtension = Path.GetExtension(files[0].FileName);

                        var oldImagePath = Path.Combine(webRootPath, serviceFromDb.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath); //remove existing file
                        }

                        using (var fileStream = new FileStream(
                            Path.Combine(changedFileUploadLocation, changedFileName + changedFileExtension), //creates full path
                            FileMode.Create)
                        )
                        {
                            files[0].CopyTo(fileStream);
                        }

                        ServiceVM.Service.ImageUrl = @"\images\services" + changedFileName + changedFileExtension;
                    }
                    else //edit but without changing image
                    {
                        ServiceVM.Service.ImageUrl = serviceFromDb.ImageUrl;
                    }
                    _unitOfWork.Service.Update(ServiceVM.Service);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            //if state invalid
            else
            {
                ServiceVM.CategoryList = _unitOfWork.Category.GetCategoryListForDropDown();
                ServiceVM.FrequencyList = _unitOfWork.Frequency.GetFrequencyListForDropDown();
                return View(ServiceVM);
            }
        }



        #region Api Calls

        public IActionResult GetAll()
        {
            return Json(new {data = _unitOfWork.Service.GetAll(includeProperties: "Category,Frequency") });  //will add .include to eager load to populate Category and Frequency foreign tables
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var serviceFromDb = _unitOfWork.Service.Get(id);
            string webRootPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, serviceFromDb.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath); //remove existing file
            }

            if (serviceFromDb == null)
            {
                return Json(new {success = false, message = "Error while deleting."});
            }
            _unitOfWork.Service.Remove(serviceFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleted successfully." });



        }
        #endregion
    }
}
