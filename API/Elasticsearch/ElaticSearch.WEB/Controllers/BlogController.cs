using ElaticSearch.WEB.Models;
using ElaticSearch.WEB.Services;
using ElaticSearch.WEB.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ElaticSearch.WEB.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogService _blogService;

        public BlogController(BlogService blogService)
        {
            _blogService = blogService;
        }

        public async Task<IActionResult> Search()
        {
            return View(await _blogService.SearchAsync(string.Empty));
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchText)
        {
            var blogList = await _blogService.SearchAsync(searchText);

            return View(blogList);
        }

        public IActionResult Save()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Save(BlogCreateViewModel blogCreateViewModel)
        {
            var isSuccess = await _blogService.SaveAsync(blogCreateViewModel);
            if (!isSuccess)
            {
                TempData["result"] = "kayıt başarısız";
                return RedirectToAction(nameof(BlogController.Save));
            }

            TempData["result"] = "kayıt başarılı";
            return RedirectToAction(nameof(BlogController.Save));            
        }
    }
}
