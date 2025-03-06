using TaskManagement.API.Data.Entities.Tables;

namespace TaskManagement.Tests.DynamicData
{
    public static class DynamicDataRepository
    {
        #region Tasks Data

        /// <summary>
        /// DataRowTasks
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> DataRowTasks()
        {
            yield return new object[] {
                new List<TbTask>()
                {
                    new TbTask { Id = 1,
                        Completed = false,
                        CreatedAt = DateTime.Now,
                        Description = string.Empty,
                        DueDate = DateTime.Now.AddDays(2),
                        InProgress = true,
                        Pending = true,
                        Priority = 1,
                        Status = "Pending",
                        Title = string.Empty,
                        UpdatedAt = DateTime.Now  },
                    new TbTask { Id = 2,
                        Completed = false,
                        CreatedAt = DateTime.Now,
                        Description = string.Empty,
                        DueDate = DateTime.Now.AddDays(3),
                        InProgress = false,
                        Pending = true,
                        Priority = 3,
                        Status = "Pending",
                        Title = string.Empty,
                        UpdatedAt = DateTime.Now  }
                }
            };
        }

        #endregion
    }
}
