using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;
using ProductAPI.Persistance;

namespace ProductAPI.Data
{
    public class ForbiddenWordRepository : IForbiddenWordRepository
    {
        private readonly AppDbContext _context;

        public ForbiddenWordRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsForbiddenWordAsync(string word)
        {
            return await _context.ForbiddenWords
                .AnyAsync(fw => fw.Word.ToLower() == word.ToLower());
        }

        public async Task<IEnumerable<ForbiddenWord>> GetAllForbiddenWordsAsync()
        {
            return await _context.ForbiddenWords.ToListAsync();
        }
    }

}
