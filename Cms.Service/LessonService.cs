using Gms.Entity;
using Gms.Entity.Enum;
using Gms.Entity.Search;
using Gms.IRepository;
using Gms.IService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Service
{
    public class LessonService:BaseService<Lesson>, ILessonService
    {
        private readonly ILessonRepository lessonRepository;

        public LessonService(ILessonRepository lessonRepository)
        {
            base.Repository = lessonRepository;
            this.lessonRepository = lessonRepository;
        }
        /// <summary>
        /// 分页获取课程
        /// </summary>
        /// <param name="lessonSearch"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        public  IQueryable<Lesson> LoadPagesEntities(LessonSearch lessonSearch, bool isDeleted)
        {
            var temp=  lessonRepository.LoadEntities(l => l.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal));
            if(!string.IsNullOrEmpty(lessonSearch.LessonName))
            {
                temp = temp.Where(l => l.LessonName.Contains(lessonSearch.LessonName));
            }
            if(lessonSearch.Status==0|| lessonSearch.Status ==2|| lessonSearch.Status == 1)
            {
                temp = temp.Where(l => l.Status== lessonSearch.Status);
            }
            lessonSearch.TotalCount=temp.Count();
            int skip = (lessonSearch.PageIndex - 1) * lessonSearch.PageSize;
            int take = lessonSearch.PageSize;

            temp = lessonSearch.Order
                ? temp.OrderBy(a => a.Id).Include(l => l.Couch).Include(l => l.VipInfos).Skip(skip).Take(take)
                : temp.OrderByDescending(a => a.Id).Include(l => l.Couch).Include(l => l.VipInfos).Skip(skip).Take(take);
            return temp;
        }
    }
}
