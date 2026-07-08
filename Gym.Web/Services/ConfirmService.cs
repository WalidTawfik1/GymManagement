using System;
using System.Threading.Tasks;

namespace Gym.Web.Services
{
    public class ConfirmService
    {
        public Func<string, string, Task<bool>>? OnShow { get; set; }

        public async Task<bool> ShowConfirmAsync(string message, string title = "تأكيد")
        {
            if (OnShow != null)
            {
                return await OnShow.Invoke(message, title);
            }
            return false;
        }
    }
}
