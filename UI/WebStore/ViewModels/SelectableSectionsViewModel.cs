using System.Collections.Generic;
using WebStore.Domain.ViewModels;

namespace WebStore.ViewModels
{
    public class SelectableSectionsViewModel
    {
        /// <summary>
        /// подготовленный список секций
        /// </summary>
        public IEnumerable<SectionViewModel> Sections { get; set; }

        /// <summary>
        /// текущая выбранная секция
        /// </summary>
        public int? SectionId { get; set; }

        /// <summary>
        /// текущая родительская секция
        /// </summary>
        public int? ParentSectionId { get; set; }
    }
}
