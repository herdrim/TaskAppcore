using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskAppCore.Models
{
    public interface ITaskRepository
    {
        IEnumerable<Models.Task> Tasks { get; }

        void SaveChanges();

        void CreateTask(Models.Task task);

        void DeleteTask(Models.Task task);

        void Update(Models.Task task);
    }
}
