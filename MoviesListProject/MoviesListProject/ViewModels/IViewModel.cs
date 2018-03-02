using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesListProject.Helpers
{
    public interface IViewModel
    {
        ICommandEx[] Commands { get; }

        Task Init();
        Task Activate();
        Task Deactivate();
        /// <summary>
        /// Returns true to cancel back button.
        /// </summary>
        /// <returns></returns>
        Task<bool> Exit();

    }
}
