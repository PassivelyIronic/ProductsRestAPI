using ProductAPI.Models;

namespace ProductAPI.Data
{
    public interface IForbiddenWordRepository
    {
        Task<bool> IsForbiddenWordAsync(string word);
        Task<IEnumerable<ForbiddenWord>> GetAllForbiddenWordsAsync();
        //jeszcze dodawanie itp.
    }

}
