using ElaticSearch.WEB.Models;
using ElaticSearch.WEB.Repositories;
using ElaticSearch.WEB.ViewModels;

namespace ElaticSearch.WEB.Services
{
    public class BlogService
    {
        private readonly BlogRepository _blogRepository;

        public BlogService(BlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<bool> SaveAsync(BlogCreateViewModel model)
        {
            var newBlog = new Blog
            {
                Title = model.Title,
                Content = model.Content,
                UserId = Guid.NewGuid(),
                Tags = model.Tags.Split(",")
            };

            var isCreated =  await _blogRepository.SaveAsync(newBlog);
            return isCreated != null;
        }

        public Task<List<Blog>> SearchAsync(string searchText)
        {
            return _blogRepository.SearchAsync(searchText);
        }
    }
}
